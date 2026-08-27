using Stellamod.Core.DecorativeTileSystem;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.TilesSH
{
    public class WitchAcademyPosterItem : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<WitchAcademyPoster>());
        }
    }

    public class WitchAcademyPoster : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            Origin = DrawOrigin.Center;
            StructureColor = BackgroundColor;
        }
    }

    public class WitchAcademyBannerItem : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<WitchAcademyBanner>());
        }
    }

    public class WitchAcademyBanner : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            Origin = DrawOrigin.TopDown;
            StructureColor = BackgroundColor;
        }
    }

    public class WitchAcademyBookshelfItem : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<WitchAcademyBookshelf>());
        }
    }

    public class WitchAcademyBookshelf : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            Origin = DrawOrigin.BottomUp;
            StructureColor = BackgroundColor;
        }
    }
}
