using Stellamod.Core.DecorativeTileSystem;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.TilesSH
{
    public class TargetBoard1Item : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<TargetBoard1>());
        }
    }

    public class TargetBoard1 : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            StructureColor = BackgroundColor;
        }
    }

    public class FrontTargetBoardItem : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<FrontTargetBoard>());
        }
    }

    public class FrontTargetBoard : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            StructureColor = BackgroundColor;
        }
    }

    public class RedRoseItem : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<RedRose>());
        }
    }

    public class RedRose : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            StructureColor = BackgroundColor;
        }
    }
}
