using Stellamod.Common.IgnitersNPowders;
using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Terraria;

namespace Stellamod.Content.Areas.Terror.WeaponsTR;

public class BloodyCard : BaseIgniterCard
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 11;
    }

    public override int GetPowderSlotCount()
    {
        return 3;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<TerrorFragments, BlankCard>();
    }
}