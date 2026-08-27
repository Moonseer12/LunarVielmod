using Stellamod.Common.DashSystem;
using Stellamod.Content.CommonMaterials;
using Stellamod.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WaterSide.AccWS;

public class WaterproofHeadphones : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToAccessory();
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<DashPlayer>().dashRestoreChance += 25;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<MusicalHarmonise, BlankAccessory>();
    }
}
