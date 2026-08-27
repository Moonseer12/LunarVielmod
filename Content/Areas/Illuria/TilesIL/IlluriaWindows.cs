using Stellamod.Core.DecorativeTileSystem;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.TilesIL
{
    public class IlluriaWindows1Item : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<IlluriaWindows1>());
        }
    }

    public class IlluriaWindows1 : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            StructureColor = BackgroundColor;
        }
    }

    public class IlluriaWindows2Item : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<IlluriaWindows2>());
        }
    }

    public class IlluriaWindows2 : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            StructureColor = BackgroundColor;
        }
    }


    public class IlluriaWindows3Item : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<IlluriaWindows3>());
        }
    }

    public class IlluriaWindows3 : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            StructureColor = BackgroundColor;
        }
    }

    public class IlluriaWindows4Item : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<IlluriaWindows4>());
        }
    }

    public class IlluriaWindows4 : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            StructureColor = BackgroundColor;
        }
    }
}
