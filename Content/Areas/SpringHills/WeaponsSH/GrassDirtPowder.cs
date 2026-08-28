using Stellamod.Common.IgnitersNPowders;
using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.WeaponsSH;

public class GrassDirtPowder : BasePowder
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        DamageModifier = 1f;
        ExplosionType = ModContent.ProjectileType<GrassExSps>();

        SoundStyle explosionSoundStyle = SoundID.DD2_ExplosiveTrapExplode;
        explosionSoundStyle.PitchVariance = 0.15f;
        ExplosionSound = explosionSoundStyle;
        ExplosionScreenshakeAmt = 1.5f;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<Ivythorn, BlankBag>();
    }
}

    public class GrassExSps : BaseIgniterExplosion
    {
        public override int FrameCount => 30;

        public override void Start()
        {
            base.Start();
            if (Main.myPlayer == Projectile.owner)
            {
                EffectsHelper.SimpleExplosionCircle(Projectile, Color.Green);
            }
        }
    }