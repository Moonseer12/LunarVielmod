using Stellamod.Core.Bases;
using Terraria;
using Terraria.ID;

namespace Stellamod.Projectiles
{
    public class KaBoomTrick : BaseIgniterExplosion
    {
        public override int FrameCount => 20;
        public override bool BlackIsTransparency => false;

        public override void Start()
        {
            base.Start();
            if (Main.myPlayer == Projectile.owner)
            {
                var circle = EffectsHelper.SimpleExplosionCircle(Projectile, Color.Purple);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (Main.rand.NextBool(12))
            {
                target.AddBuff(BuffID.Confused, 120);
            }
        }
    }
}