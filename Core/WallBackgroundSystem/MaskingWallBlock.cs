using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.WallBackgroundSystem
{
    public class MaskingWallBlock : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;
            Item.maxStack = Item.CommonMaxStack;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 7;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createWall = ModContent.WallType<MaskingWall>();
        }
    }
}
