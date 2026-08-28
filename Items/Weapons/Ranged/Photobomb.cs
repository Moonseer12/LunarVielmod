using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Stellamod.Projectiles.Paint;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged
{

    public class Photobomb : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 9;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 32;
            Item.height = 25;
            Item.useTime = 98;
            Item.useAnimation = 98;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 10;
            Item.rare = ItemRarityID.LightPurple;
            Item.autoReuse = false;
            Item.shootSpeed = 30f;
            Item.shoot = ModContent.ProjectileType<PhotobombProj>();
            Item.scale = 0.8f;
            Item.noMelee = true; // The projectile will do the damage and not the item
            Item.value = Item.buyPrice(silver: 7);
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.noMelee = true;

        }


        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<KaleidoscopicInk, BlankGun>();
        }
    }
}