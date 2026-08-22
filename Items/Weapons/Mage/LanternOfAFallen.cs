using Microsoft.Xna.Framework;
using Stellamod.Content.CommonMaterials;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.Magic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Mage
{
    public class LanternOfAFallen : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 80;
            Item.mana = 30;
            Item.width = 29;
            Item.height = 31;
            Item.useTime = 50;
            Item.useAnimation = 50;
            Item.useStyle = ItemUseStyleID.RaiseLamp;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.DamageType = DamageClass.Magic;
            Item.value = 15000;
            Item.scale = 0.5f;
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.DD2_DarkMageSummonSkeleton;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<LanternOfTheFallenFly>();
            Item.shootSpeed = 7f;
            Item.autoReuse = true;
            Item.crit = 22;

        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            velocity.Y = -7;
            for (int i = 0; i < Main.rand.Next(2, 6); i++)
            {
                Projectile.NewProjectile(source, position, velocity.RotatedByRandom(MathHelper.PiOver4), type, damage, knockback, player.whoAmI);
            }
            return false;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankStaff>(), material: ModContent.ItemType<MarshScrap>());
        }
    }
}









