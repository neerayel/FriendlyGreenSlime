
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FriendlyGreenSlime.Content.Items.Weapons
{
    internal class ThrowableEvilGreenSlime : ModItem
	{
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Clear();
            tooltips.Add(new TooltipLine(Mod, "ItemName", "Throwable Evil Green Slime"));
            tooltips[0].OverrideColor = Color.LightGreen;

            tooltips.Add(new TooltipLine(Mod, "Tooltip0", "Ah-Oh sticky"));

            base.ModifyTooltips(tooltips);
        }

        public override void SetDefaults()
		{
			Item.noMelee = true;
			Item.damage = 1;
			Item.DamageType = DamageClass.Throwing;
			Item.knockBack = 5;
			Item.value = 1;
			Item.rare = ItemRarityID.Blue;

			Item.maxStack = 999;
			Item.consumable = true;

			Item.shoot = ModContent.ProjectileType<Content.Projectile.ThrowableSlimeEnemy>();
			Item.shootSpeed = 8;

			Item.useTime = 20;
			Item.useAnimation = 20;

			Item.useStyle = ItemUseStyleID.Swing;

			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(1);
			recipe.AddIngredient(ItemID.Gel, 3);
            recipe.AddIngredient(ItemID.FallenStar, 1);
            recipe.AddIngredient(ItemID.CopperCoin, 10);
			recipe.Register();
		}
	}
}