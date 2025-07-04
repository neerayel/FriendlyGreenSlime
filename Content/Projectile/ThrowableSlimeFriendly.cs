using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using FriendlyGreenSlime.Content.NPCs;

namespace FriendlyGreenSlime.Content.Projectile
{
	public class ThrowableSlimeFriendly : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.CloneDefaults(3);

			Projectile.width = 10;
			Projectile.height = 10;

			Projectile.aiStyle = 1;
			AIType = ProjectileID.Shuriken;

			Projectile.friendly = true;

			Projectile.timeLeft = 600;
			Projectile.extraUpdates = 1;
		}

		public override bool OnTileCollide(Vector2 oldveloctiy)
		{
			NPC.NewNPC(Terraria.Entity.GetSource_None(), (int)Projectile.position.X, (int)Projectile.position.Y, ModContent.NPCType<FriendlySlime>());

			SoundEngine.PlaySound(SoundID.Dig, Projectile.position);

			Projectile.Kill();

			return false;
		}
	}
}