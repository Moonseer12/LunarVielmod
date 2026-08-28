using Stellamod.Common.IgnitersNPowders;
using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Terraria;

namespace Stellamod.Content.Areas.Ishtar.WeaponsIS;

public class EreshkigalsCard : BaseIgniterCard
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 200;
    }
    public override int GetPowderSlotCount()
    {
        return 6;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<EreshkinCandle, BlankCard>();
    }
}