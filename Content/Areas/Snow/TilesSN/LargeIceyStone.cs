using Stellamod.Core.DecorativeTileSystem;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Snow.TilesSN
{
    //Wall Version
    public class LargeIceyStoneBlock : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<LargeIceyStone>();
        }
    }

    public class LargeIceyStone : DecorativeWall
    {
        public override void SetStaticDefaults()
        {

            base.SetStaticDefaults();
            StructureColor = Color.Gray;
            //If you need other static defaults it go here
        }
    }
}
