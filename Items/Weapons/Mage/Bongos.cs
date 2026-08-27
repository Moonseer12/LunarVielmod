
using Stellamod.Content.CommonMaterials;
using Stellamod.Projectiles.IgniterExplosions;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Mage
{
    public class Bongos : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.mana = 42;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Guitar;
            Item.noMelee = true;
            Item.knockBack = 0f;
            Item.DamageType = DamageClass.Magic;
            Item.value = 200;
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/bongo");
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BongoBoom>();
            Item.autoReuse = true;
            Item.crit = 22;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI, 0f, 0f);
            return false;

        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankStaff>(), material: ModContent.ItemType<AlcadizScrap>());
        }
    }
}
