using Stellamod.Common.DashSystem;
using Stellamod.Content.Areas.PunkerTown.BossesPT.Gothivia;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.AccPT;

public class MagmaPendant : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToAccessory();
    }
    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);
        player.GetModPlayer<GothiviaPlayer>().maxStacks++;
        player.GetModPlayer<DashPlayer>().doubleStaminaCost = true;
    }
}
