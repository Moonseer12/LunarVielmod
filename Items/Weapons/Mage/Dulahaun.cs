
using Stellamod.Projectiles.IgniterExplosions;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Mage
{
    public class Dulahaun : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 375;
            Item.mana = 30;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 44;
            Item.useAnimation = 44;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 0f;
            Item.DamageType = DamageClass.Melee;
            Item.value = 200;
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/OverGrowth_TP2");
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Dulahan>();
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.crit = 2;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI, 0f, 0f);
            return false;

        }
    }
}
