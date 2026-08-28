using Stellamod.Common.IgnitersNPowders;
using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Terraria;

namespace Stellamod.Content.Areas.Illuria.WeaponsIL;

public class SiegfriedsCard : BaseIgniterCard
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 60;
    }

    public override int GetPowderSlotCount()
    {
        return 5;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<IllurineScale, BlankCard>();
    }
}