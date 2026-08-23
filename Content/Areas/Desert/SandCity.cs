using Stellamod.Core.Foreground;
using Terraria;

namespace Stellamod.Content.Areas.Desert
{
    public class SandCity : ForegroundLayer
    {
        public override bool IsActive()
        {
            return Main.LocalPlayer.ZoneDesert && !Main.LocalPlayer.ZoneUndergroundDesert;
        }
        public override void SetLayering(ref float zLayer, ref Vector2 parallax)
        {
            base.SetLayering(ref zLayer, ref parallax);
            parallax.X = 1.2f;
            parallax.Y = 1;
        }
    }
}
