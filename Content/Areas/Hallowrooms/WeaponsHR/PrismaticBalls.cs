using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Hallowrooms.WeaponsHR
{
    public class PrismaticBalls : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 23;
            Item.mana = 2;
            Item.useTime = 8;
            Item.useAnimation = 8;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.DamageType = DamageClass.Magic;
            Item.UseSound = SoundID.DD2_BookStaffCast;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.RainbowRodBullet;
            Item.shootSpeed = 7f;
            Item.autoReuse = true;
            Item.crit = 12;
            Item.noUseGraphic = true;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<KaleidoscopicInk, BlankOrb>();
        }
    }
}