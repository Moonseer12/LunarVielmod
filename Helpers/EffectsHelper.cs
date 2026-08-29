using Terraria;

namespace Stellamod.Helpers
{
    public static class EffectsHelper
    {
        public struct Nothin
        {

        }
        public static Nothin SimpleExplosionCircle(Projectile baseProjectile, Color explosionColor, float endRadius = 64)
        {
            PixelPrimitiveCircleFactory.CreateGenericBoom(baseProjectile.Center, explosionColor * 0.5f, explosionColor * 0.5f, 30, endRadius);
            return new Nothin();
        }
    }
}