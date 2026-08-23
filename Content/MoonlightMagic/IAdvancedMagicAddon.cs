using Terraria;

namespace Stellamod.Content.MoonlightMagic
{
    public interface IAdvancedMagicAddon
    {
        public AdvancedMagicProjectile MagicProj { get; set; }
        public Projectile Projectile => MagicProj.Projectile;
    }
}
