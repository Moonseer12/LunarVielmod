using Stellamod.Core.DecorativeTileSystem;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Desert.TilesCL
{
    //Wall Version
    public class FuneralBedItem : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<FuneralBed>());
        }
    }

    public class FuneralBed : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            StructureColor = Color.White;
            Origin = DrawOrigin.BottomUp;

            //If you need other static defaults it go here
        }
    }
}