using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.WeaponsCS
{
    public class Vhel : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 2910;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 10;

            Item.shootSpeed = 15;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Generic;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 16f;
            Item.useAmmo = AmmoID.Arrow;
            Item.UseSound = SoundID.Item5;
            Item.useAnimation = 21;
            Item.useTime = 7; // one third of useAnimation
            Item.reuseDelay = 35;
            Item.noMelee = true;
        }



        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {




            return false;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2f, 0f);
        }
    }
}
