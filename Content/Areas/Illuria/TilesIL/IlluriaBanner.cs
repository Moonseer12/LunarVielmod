using Stellamod.Core.DecorativeTileSystem;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.TilesIL
{
    public class IlluriaBannerItem : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<IlluriaBanner>());
        }
    }
    public class IlluriaBanner : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Origin = DrawOrigin.TopDown;

            StructureColor = BackgroundColor;
        }
    }


    public class IlluriaWallsItem : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<IlluriaWalls>());
        }
    }
    public class IlluriaWalls : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
   
            StructureColor = BackgroundColor;
        }
    }
}
