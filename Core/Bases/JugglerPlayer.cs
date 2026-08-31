using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.Bases
{
    public class JugglerPlayer : ModPlayer
    {
        public float DamageBonus;
        public int CatchCount;
        public bool SpecialAttack;
        public void ResetJuggle()
        {
            DamageBonus = 0f;
            CatchCount = 0;
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            bool isLegal = false;
            if (proj.ModProjectile is BaseJugglerProjectile)
            {
                isLegal = true;
            }

            if (!isLegal)
                return;

            modifiers.ScalingBonusDamage += DamageBonus;
        }
    }
}