using Stellamod.Content.CommonMaterials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
    public class ShadeCharm : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (!Main.dayTime)
            {
                Lighting.AddLight(player.Center, Color.MediumPurple.ToVector3() * 1.75f * Main.essScale);
                player.manaCost -= 0.2f;
                player.manaRegen += 2;
            }
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<HypnotizedSoul, BlankAccessory>();
        }
    }
}