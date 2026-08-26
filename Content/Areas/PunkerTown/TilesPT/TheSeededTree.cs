using Stellamod.Core.DecorativeTileSystem;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.TilesPT
{
    public class TheSeededTreeItem : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<TheSeededTree>();
        }
    }

    public class TheSeededTree : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            StructureColor = Color.Gray;
            Origin = DrawOrigin.BottomUp;
        }
    }
}