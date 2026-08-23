using Stellamod.Core.DecorativeTileSystem;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.TilesSH
{
    public class WindmillTopItem : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<WindmillTop>();
        }
    }

    public class WindmillTop : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            //Number of frames in the animation
            FrameCount = 60;
            HorizontalFrameCount = 5;
            VerticalFrameCount = 12;
            Origin = DrawOrigin.Center;

            //How fast the animation is
            FrameSpeed = 30;

            //Draw Scale
            DrawScale = 2;

            //If this is set to true, wall tiles will offset their animation so they're not all synced
            DesyncAnimations = false;
        }
    }
}
