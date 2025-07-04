
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace FriendlyGreenSlime.Content.Projectile
{
    internal class TransparentProjectile : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.PinkLaser);

			Projectile.width = 32;
			Projectile.height = 22;

			Projectile.friendly = true;

			Projectile.timeLeft = 100;
			Projectile.extraUpdates = 1;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Projectile.Kill();
			return false;
		}

	}
}
