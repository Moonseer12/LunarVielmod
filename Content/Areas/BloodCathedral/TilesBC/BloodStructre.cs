using Stellamod.Core.DecorativeTileSystem;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.BloodCathedral.TilesBC
{
    //Wall Version
    public class BloodHallItem : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<BloodHall>();

        }
    }

    public class BloodHall : BehindDecorativeWall
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