using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.WeaponsPT
{
    public class MahoganyStorm : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 6;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4;
            Item.shootSpeed = 17;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Ranged;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 16f;
            Item.useAmmo = AmmoID.Arrow;
            Item.UseSound = SoundID.Item5;
            Item.useAnimation = 20;
            Item.useTime = 4; // one third of useAnimation
            Item.reuseDelay = 70;
            Item.consumeAmmoOnLastShotOnly = true;
            Item.noMelee = true;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2f, 0f);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {


            int numProjectiles = Main.rand.Next(1, 6);
            for (int p = 0; p < numProjectiles; p++)
            {
                // Rotate the velocity randomly by 30 degrees at max.
                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(30));
                newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                Projectile.NewProjectileDirect(source, position, newVelocity, type, damage, knockback, player.whoAmI);
            }


            return false;
        }


    }
}
