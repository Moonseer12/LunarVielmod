
using Stellamod.Projectiles.Thrown;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Thrown
{
    public class YourFired : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 170;
            Item.width = 50;
            Item.height = 50;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 12;
            Item.value = Item.sellPrice(0, 1, 1, 29);
            Item.rare = ItemRarityID.Purple;
            Item.shootSpeed = 25;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.DamageType = DamageClass.Ranged;
            Item.shoot = ModContent.ProjectileType<YourFiredProj>();
            Item.shootSpeed = 20f;
            Item.useAnimation = 36;
            Item.useTime = 36;
            Item.consumable = false;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-3f, -2f);
        }
    }
}
