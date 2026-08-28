using Stellamod.Common.IgnitersNPowders;
using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Terraria;

namespace Stellamod.Content.Areas.WondrousDarkspace.WeaponsWD;

public class MiracleCard : BaseIgniterCard
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 75;
    }

    public override int GetPowderSlotCount()
    {
        return 5;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<MiracleThread, BlankCard>();
    }
}