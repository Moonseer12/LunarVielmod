
using Terraria;

namespace Stellamod.Content.MoonlightMagic.Movements
{
    public class CircularMovement : BaseMovement
    {
        // public float maxHomingDetectDistance = 512;
        public override void AI()
        {
            Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(9));
        }
    }
}
