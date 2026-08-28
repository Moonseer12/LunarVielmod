using Stellamod.Core.Bases;
using Terraria;
using Terraria.ID;

namespace Stellamod.Projectiles
{
    public class KaBoomShade : BaseIgniterExplosion
    {
        public override int FrameCount => 33;

        public override bool BlackIsTransparency => false;

        public override void Start()
        {
            base.Start();
            if (Main.myPlayer == Projectile.owner)
            {
                EffectsHelper.SimpleExplosionCircle(Projectile, Color.Purple);
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            if (Main.rand.NextBool(3))
            {
                target.AddBuff(BuffID.ShadowFlame, 120);
            }
        }
    }
}