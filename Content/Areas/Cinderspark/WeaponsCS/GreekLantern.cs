using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.WeaponsCS
{
    public class GreekLantern : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 30;
            Item.mana = 2;
            Item.useTime = 8;
            Item.useAnimation = 8;
            Item.useStyle = ItemUseStyleID.RaiseLamp;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.scale = 0.5f;
            Item.DamageType = DamageClass.Magic;
            Item.UseSound = SoundID.DD2_BookStaffCast;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.MolotovFire;
            Item.shootSpeed = 8f;
            Item.crit = 12;
        }
    }
}