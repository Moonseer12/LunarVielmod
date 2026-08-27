using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.WallBackgroundSystem
{
    public class MaskingWallBlock : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<MaskingWall>());
        }
    }
}
