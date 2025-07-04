
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace FriendlyGreenSlime.Content.NPCs
{
    internal class FriendlySlime : ModNPC
    {
		private enum ActionState
		{
			Idle,
			Notice,
			Jump,
			Fall
		}

		private enum Frame
		{
			Idle,
			Notice,
			Falling
		}

		public ref float AI_State => ref NPC.ai[0];
		public ref float AI_Timer => ref NPC.ai[1];

		public override string Texture => $"Terraria/Images/NPC_{NPCID.BlueSlime}";

		public NPC Target { get; set; }
		public bool HasTarget { get; set; }

		public int IdleDirection { get; set; }
		public float IdleX { get; set; }
		public int IdleTimer { get; set; } = 0;

		public int ContactDamage { get; set; } = 20;
		public int HitKnockback { get; set; } = 20;

		public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = Main.npcFrameCount[NPCID.BlueSlime];

			NPCID.Sets.AttackType[Type] = 0;
		}


		public override void SetDefaults()
		{
			NPC.CloneDefaults(NPCID.GreenSlime);

			AnimationType = NPCID.BlueSlime;

			NPC.friendly = true;
			NPC.aiStyle = -1;

			NPC.lifeMax *= 2;
		}

        public override void OnSpawn(IEntitySource source)
        {
			Random rand = new Random();
			IdleDirection = rand.Next(-1, 1);
			if (IdleDirection == 0) IdleDirection = 1;
		}

        private int SpawnProjectile(Vector2 hitDirection)
		{
			return Terraria.Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, hitDirection, ModContent.ProjectileType<Content.Projectile.TransparentProjectile>(), ContactDamage, HitKnockback, Main.myPlayer);
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			return SpawnCondition.OverworldDaySlime.Chance * 0.1f;
		}

		public override void AI()
		{
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }

            Player owner = Main.player[Main.myPlayer];
			SearchForTargets(owner);

			switch (AI_State)
			{
				case (float)ActionState.Idle:
					IdleState();
					break;
				case (float)ActionState.Notice:
					if (NPC.life > 10) Notice();
					break;
				case (float)ActionState.Jump:
					if (NPC.velocity.Y == 0) Jump();
					break;
				case (float)ActionState.Fall:
					Fall();
					break;
			}

            Rectangle hitbox = NPC.Hitbox;
            if (NPC.life != NPC.lifeMax) StealLootInHitbox(hitbox, ItemID.Gel);
			if (HasTarget && Target.Hitbox.Intersects(hitbox)) SpawnProjectile(Target.DirectionFrom(NPC.position));
        }

		private void StealLootInHitbox(Rectangle hitbox, int lootType)
		{
            foreach (Item item in Main.item)
            {
                if (item.active && !item.beingGrabbed && item.type == lootType && hitbox.Intersects(item.Hitbox))
                {
                    item.active = false;
                    NetMessage.SendData(MessageID.SyncItem, number: item.whoAmI);

                    NPC.life = NPC.lifeMax;
                    EmoteBubble.NewBubble(0, new WorldUIAnchor(NPC), 90);
                }
            }
        }

        private void SearchForTargets(Player owner)
		{
			float distanceFromTarget = 1000f;
			Vector2 targetCenter = NPC.position;
			HasTarget = false;

			if (!HasTarget)
			{
				for (int i = 0; i < Main.maxNPCs; i++)
				{
					NPC npc = Main.npc[i];

					if (npc.CanBeChasedBy())
					{
						float between = Vector2.Distance(npc.Center, NPC.Center);
						bool closest = Vector2.Distance(NPC.Center, targetCenter) > between;
						bool inRange = between < distanceFromTarget;

						if ((closest && inRange) || !HasTarget)
						{
							distanceFromTarget = between;
							targetCenter = npc.Center;
							HasTarget = true;

							Target = npc;
						}
					}
				}
			}
		}

		public override bool? CanFallThroughPlatforms()
		{
			if (AI_State == (float)ActionState.Fall && HasTarget && Target.Top.Y > NPC.Bottom.Y)
			{
				return true;
			}

			return false;
		}

		private int Direction()
        {
			Vector2 direction = new Vector2(0, 0);
			if (HasTarget) direction = Target.Center - NPC.Center;

			if (direction.X > 0) return 1;
			else if (direction.X < 0) return -1;
			else return 0;
		}

		private void IdleState()
		{
			AI_Timer++;

			NPC.direction = Direction();

			if (HasTarget && Target.Distance(NPC.Center) < 1000f)
			{
				AI_State = (float)ActionState.Notice;
				AI_Timer = 0;
				IdleTimer = 0;
			}
            else
            {
				if (AI_Timer >= 60)
				{
					IdleTimer++;

					if (IdleTimer >= 15 && NPC.position.X == IdleX) 
					{
						IdleDirection *= -1;
						IdleTimer = 0;
					}
					else IdleX = NPC.position.X;

					NPC.direction = IdleDirection;
					AI_State = (float)ActionState.Jump;
                    AI_Timer = 0;
				}
            }
        }

		private void Notice()
		{
			AI_Timer++;

			if (Target.Distance(NPC.Center) < 1000f)
			{
				if (AI_Timer >= 50)
				{
					AI_State = (float)ActionState.Jump;
					AI_Timer = 0;
				}
			}
			else
			{
				NPC.direction = Direction();

				if (!HasTarget || Target.Distance(NPC.Center) > 1000f)
				{
					AI_State = (float)ActionState.Idle;
					AI_Timer = 0;
				}
			}
		}

		private void Jump()
		{
			AI_Timer++;

			Random rand = new Random();

			if (AI_Timer == 1)
			{
				NPC.velocity = new Vector2(NPC.direction * rand.Next(3, 5), rand.Next(4, 8) * -1);
			}

			AI_State = (float)ActionState.Fall;
		}

		private void Fall()
		{
			if (HasTarget && Target.Distance(NPC.Center) < 20f)
			{
				int projectile = SpawnProjectile(Target.DirectionFrom(NPC.position));
            }

            if (NPC.velocity.Y == 0)
            {
                NPC.velocity.X = 0;
                AI_State = (float)ActionState.Idle;
                AI_Timer = 0;
            }
        }
	}
}
