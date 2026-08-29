using Stellamod.Common.ArmorRework;
using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Jungle.AccJN;

public class ScriptofTheSun : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToAccessory();
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);
        player.GetStats().mainSummonDamage += 0.30f;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<RadiantNectar, BlankAccessory>();
    }
}
