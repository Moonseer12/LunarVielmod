using Stellamod.Core.DecorativeTileSystem;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.TilesPT
{

    public class PaintingSunsetItem : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<PaintingSunset>());
        }
    }
    public class PaintingComputerItem : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<PaintingComputer>());
        }
    }
    public class PaintingBeachItem : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<PaintingBeach>());
        }
    }
    public class PaintingSnowForestItem : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<PaintingSnowForest>());
        }
    }
    public class PaintingCityWallItem : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<PaintingCity>());
        }
    }

    public abstract class AbstractPaintingWall : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Origin = DrawOrigin.Center;
        }
    }

    public class PaintingSunset : AbstractPaintingWall { }
    public class PaintingComputer : AbstractPaintingWall { }
    public class PaintingSnowForest : AbstractPaintingWall { }
    public class PaintingBeach : AbstractPaintingWall { }
    public class PaintingCity : AbstractPaintingWall { }
}
