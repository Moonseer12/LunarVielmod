using Stellamod.Core.DecorativeTileSystem;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Snow.TilesSN
{
    //Wall Version
    public class IceyAlterBlock : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<IceyAlter>();
        }
    }

    public class IceyAlter : DecorativeWall
    {
        public override void SetStaticDefaults()
        {

            base.SetStaticDefaults();
            StructureColor = Color.Gray;
            //If you need other static defaults it go here
        }
    }
}
