using Stellamod.Common.IgnitersNPowders;
using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Hallowrooms.WeaponsHR;

public class CrystalPowder : BasePowder
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        //Percent increase, 1 is +100% damage
        DamageModifier = 1.65f;
        ExplosionType = ModContent.ProjectileType<CrystalBloom>();


        SoundStyle explosionSoundStyle = new("Stellamod/Assets/Sounds/GhostExcalibur1");
        explosionSoundStyle.PitchVariance = 0.15f;
        ExplosionSound = explosionSoundStyle;
        ExplosionScreenshakeAmt = 2;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<KaleidoscopicInk, BlankBag>();
    }
}

    public class CrystalBloom : BaseIgniterExplosion
    {
        public override int FrameCount => 60;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.ArmorPenetration += 10;
        }

        public override void Start()
        {
            base.Start();
            if (Main.myPlayer == Projectile.owner)
            {
                EffectsHelper.SimpleExplosionCircle(Projectile, Color.Purple, endRadius: 70);
            }
        }
    }