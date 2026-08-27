using Stellamod.Core.DecorativeTileSystem;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Desert.TilesCL
{
    public class DesertCatacombPillar11Item : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<DesertCatacombPillar1>());
        }
    }

    public class DesertCatacombPillar1 : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
    }

    public class DesertCatacombPillar2Item : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<DesertCatacombPillar2>());
        }
    }

    public class DesertCatacombPillar2 : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
    }
}
