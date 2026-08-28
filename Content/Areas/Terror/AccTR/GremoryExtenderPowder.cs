using Stellamod.Common.IgnitersNPowders;
using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Terror.AccTR;

public class GremoryExtenderPowder : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToAccessory();
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<IgniterPlayer>().extenderBonus += 0.25f;
        player.GetModPlayer<IgniterPlayer>().hasLifesteal = true;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<TerrorFragments, BlankAccessory>();
    }
}