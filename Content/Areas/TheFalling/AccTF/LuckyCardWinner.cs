using Stellamod.Common.IgnitersNPowders;
using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.TheFalling.AccTF;

public class LuckyCardWinner : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToAccessory();
    }
    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);
        player.GetModPlayer<IgniterPlayer>().lucky = true;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<GhastlySpirit, BlankAccessory>();
    }
}
