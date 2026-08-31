using Stellamod.Content.Areas.Underground.TilesUG;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Underground
{
    public class GlisteningPearl : ModItem
    {
        public override void SetDefaults()
        {
            Item.maxStack = Item.CommonMaxStack;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient<GlisteningOre>(15).Register();
        }
    }
}