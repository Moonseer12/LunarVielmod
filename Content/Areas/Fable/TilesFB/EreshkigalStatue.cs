using Stellamod.Core.DecorativeTileSystem;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Fable.TilesFB
{
    //Wall Version
    public class EreshkigalStatueItem : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<EreshkigalStatue>();
        }
    }

    public class EreshkigalStatue : DecorativeWall
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