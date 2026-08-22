using Microsoft.Xna.Framework;
using Stellamod.Tiles;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.TilesNew.IceTiles
{
    //Wall Version
    public class SmallIceyStoneBlock : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<SmallIceyStone>();
        }
    }

    public class SmallIceyStone : DecorativeWall
    {
        public override void SetStaticDefaults()
        {

            base.SetStaticDefaults();
            StructureColor = Color.Gray;
            //If you need other static defaults it go here
        }
    }
}
