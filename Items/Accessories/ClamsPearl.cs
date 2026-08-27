
using Stellamod.Content.CommonMaterials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
    public class ClamsPearl : ModItem
    {

        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            Lighting.AddLight(player.Center, Color.LightBlue.ToVector3() * 1.75f * Main.essScale);
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<Mushroom, BlankAccessory>();
        }
    }
}


