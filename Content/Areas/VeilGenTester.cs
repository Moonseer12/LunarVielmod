using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas;

public class VeilGenTester : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.useTime = 1;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useAnimation = 1;
    }

    public override bool? UseItem(Player player)
    {
        return true;
    }

}
