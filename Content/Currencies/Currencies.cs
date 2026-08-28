using Stellamod.Content.Areas.Underground.TilesUG;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Currencies;

public class RuinMedal : ModItem
{
    public override void SetDefaults()
    {
        Item.maxStack = Item.CommonMaxStack;
        Item.value = Item.buyPrice(0, 20, 0, 0);
    }
}
    
public class Ereshstyl : ModItem
{
    public override void SetDefaults()
    {
        Item.maxStack = Item.CommonMaxStack;
        Item.value = Item.buyPrice(0, 20, 0, 0);
    }
}

public class NoHitCrystal : ModItem
{
    public override void SetDefaults()
    {
        Item.maxStack = Item.CommonMaxStack;
        Item.value = Item.buyPrice(0, 20, 0, 0);
    }
 }

public class DragonShard : ModItem
{
    public override void SetDefaults()
    {
        Item.rare = ItemRarityID.Orange;
        Item.maxStack = Item.CommonMaxStack;
    }

    public override void AddRecipes()
    {
        CreateRecipe().AddIngredient<Dragonpiece>(50).Register();
    }
}