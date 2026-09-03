using Terraria;

namespace Stellamod.Core.Particles
{
    public static class ParticleUtils
    {
        public static bool OnScreen(Vector2 pos) => pos.X > -16 && pos.X < Main.screenWidth + 16 && pos.Y > -16 && pos.Y < Main.screenHeight + 16;

    }
}
