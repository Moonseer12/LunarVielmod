using Stellamod.Common.IgnitersNPowders;
using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Desert.WeaponsCL;

public class AivanPowder : BasePowder
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        //Percent increase, 1 is +100% damage
        DamageModifier = 1.35f;
        ExplosionType = ModContent.ProjectileType<AivanKaboom>();

        SoundStyle explosionSoundStyle = SoundID.DD2_ExplosiveTrapExplode;
        explosionSoundStyle.PitchVariance = 0.15f;
        ExplosionSound = explosionSoundStyle;
        ExplosionScreenshakeAmt = 2;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<GintzlMetal, BlankBag>();
    }
}

    public class AivanKaboom : BaseIgniterExplosion
    {
        public override int FrameCount => 22;
        public override void Start()
        {
            base.Start();
            if (Main.myPlayer == Projectile.owner)
            {
                EffectsHelper.SimpleExplosionCircle(Projectile, Color.White, 64);
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            modifiers.Knockback += 4;
        }
    }