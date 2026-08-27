using Stellamod.Core.DecorativeTileSystem;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.MoonspiralTower.TilesMT
{
    //Wall Version
    public class GiantCrystal1Item : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<GiantCrystal1>());
        }
    }

    public class GiantCrystal1 : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            StructureColor = Color.Gray;
            Origin = DrawOrigin.BottomUp;

            //If you need other static defaults it go here
        }
    }


    public class GiantCrystal2Item : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<GiantCrystal2>());
        }
    }

    public class GiantCrystal2 : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            StructureColor = Color.Gray;
            Origin = DrawOrigin.BottomUp;

            //If you need other static defaults it go here
        }
    }

    public class MedCrystal1Item : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<MedCrystal1>());
        }
    }

    public class MedCrystal1 : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            StructureColor = Color.Gray;
            Origin = DrawOrigin.BottomUp;

            //If you need other static defaults it go here
        }
    }

    public class MedCrystal2Item : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<MedCrystal2>());
        }
    }

    public class MedCrystal2 : DecorativeWall
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
