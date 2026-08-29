using Stellamod.Common.IgnitersNPowders;
using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Fable.WeaponsFB;

public class FlamePowder : BasePowder
{
    public override void SetDefaults()
    {
        base.SetDefaults();

        DamageModifier = 1f;
        ExplosionType = ModContent.ProjectileType<KaBoom>();

        SoundStyle explosionSoundStyle = new($"Stellamod/Assets/Sounds/Kaboom");
        explosionSoundStyle.PitchVariance = 0.15f;
        ExplosionSound = explosionSoundStyle;
        ExplosionScreenshakeAmt = 2;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<AlcadizScrap, BlankBag>();
    }
}

    public class KaBoom : BaseIgniterExplosion
    {
        public override int FrameCount => 20;
        public override bool BlackIsTransparency => false;

        public override void Start()
        {
            base.Start();
            if (Main.myPlayer == Projectile.owner)
            {
                EffectsHelper.SimpleExplosionCircle(Projectile, Color.OrangeRed, 70);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (Main.rand.NextBool(3))
            {
                target.AddBuff(BuffID.OnFire, 120);
            }
        }
    }