using Stellamod.Common.IgnitersNPowders;
using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Terraria;

namespace Stellamod.Content.Areas.RoyalCapital.WeaponsRC;

public class FenixCard : BaseIgniterCard
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 225;
    }
    public override int GetPowderSlotCount()
    {
        return 6;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<AlcaricMush, BlankCard>();
    }
}