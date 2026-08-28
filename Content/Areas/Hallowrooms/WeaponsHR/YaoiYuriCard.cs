using Stellamod.Common.IgnitersNPowders;
using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Terraria;

namespace Stellamod.Content.Areas.Hallowrooms.WeaponsHR;

public class YaoiYuriCard : BaseIgniterCard
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 50;
    }
    public override int GetPowderSlotCount()
    {
        return 4;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<KaleidoscopicInk, BlankCard>();
    }
}