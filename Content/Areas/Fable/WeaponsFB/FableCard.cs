using Stellamod.Common.IgnitersNPowders;
using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Terraria;

namespace Stellamod.Content.Areas.Fable.WeaponsFB;

public class FableCard : BaseIgniterCard
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 10;

    }
    public override int GetPowderSlotCount()
    {
        return 2;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<AlcadizScrap, BlankCard>();
    }
}