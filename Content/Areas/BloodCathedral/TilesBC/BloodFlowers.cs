using Stellamod.Core.DecorativeTileSystem;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.BloodCathedral.TilesBC
{
    //Wall Version
    public class BloodFlower1Item : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<BloodFlower1>());
        }
    }

    public class BloodFlower1 : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            StructureColor = Color.Gray;
            Origin = DrawOrigin.BottomUp;

            //If you need other static defaults it go here
        }
    }


    public class BloodFlower2Item : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<BloodFlower2>());
        }
    }

    public class BloodFlower2 : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            StructureColor = Color.Gray;
            Origin = DrawOrigin.BottomUp;

            //If you need other static defaults it go here
        }
    }

    public class BloodFlower3Item : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<BloodFlower3>());
        }
    }

    public class BloodFlower3 : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            StructureColor = Color.Gray;
            Origin = DrawOrigin.BottomUp;

            //If you need other static defaults it go here
        }
    }

    public class BloodFlower4Item : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<BloodFlower4>());
        }
    }

    public class BloodFlower4 : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            StructureColor = Color.Gray;
            Origin = DrawOrigin.BottomUp;

            //If you need other static defaults it go here
        }
    }

    public class BloodFlower5Item : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<BloodFlower5>());
        }
    }

    public class BloodFlower5 : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            StructureColor = Color.Gray;
            Origin = DrawOrigin.BottomUp;

            //If you need other static defaults it go here
        }
    }
}
