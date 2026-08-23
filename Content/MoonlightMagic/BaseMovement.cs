using Terraria;

namespace Stellamod.Content.MoonlightMagic
{
    public abstract class BaseMovement : IAdvancedMagicAddon
    {
        public AdvancedMagicProjectile MagicProj { get; set; }
        public Projectile Projectile => MagicProj.Projectile;
        public abstract void AI();
    }
}
