using Stellamod.Common.IgnitersNPowders;
using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Terraria;

namespace Stellamod.Content.Areas.Underground.WeaponsUG;

public class RingedCard : BaseIgniterCard
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 9;
    }

    public override int GetPowderSlotCount()
    {
        return 3;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<MinersGold, BlankCard>();
    }
}