
using Stellamod.Projectiles.Thrown;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Thrown
{
    public class AssassinsKnife : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 12;
            Item.width = 50;
            Item.height = 50;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 8;
            Item.value = Item.buyPrice(0, 5, 10, 0);
            Item.rare = ItemRarityID.Blue;
            Item.shootSpeed = 15;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.DamageType = DamageClass.Ranged;
            Item.shoot = ModContent.ProjectileType<AssassinsKnifeProg>();
            Item.shootSpeed = 20f;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.consumable = false;

        }


        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-3f, -2f);
        }
    }
}
