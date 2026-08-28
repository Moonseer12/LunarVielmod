using Stellamod.Common.IgnitersNPowders;
using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Terror.WeaponsTR;

public class BloodPowder : BasePowder
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        //Percent increase, 1 is +100% damage
        DamageModifier = 1.35f;
        ExplosionType = ModContent.ProjectileType<KaBoomKaev>();


        SoundStyle explosionSoundStyle = new($"Stellamod/Assets/Sounds/Suckler");
        explosionSoundStyle.PitchVariance = 0.15f;
        ExplosionSound = explosionSoundStyle;
        ExplosionScreenshakeAmt = 2;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<TerrorFragments, BlankBag>();
    }
}

    public class KaBoomKaev : BaseIgniterExplosion
    {
        public override int FrameCount => 8;
        public override void SetExplosionDefaults()
        {
            base.SetExplosionDefaults();
            FrameSpeed = 0.5f;
        }

        public override void Start()
        {
            base.Start();
            if (Main.myPlayer == Projectile.owner)
            {
                EffectsHelper.SimpleExplosionCircle(Projectile, Color.Red, endRadius: 70);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (Main.rand.NextBool(3))
            {
                //Life steal for % of the damage
                float healFactor = damageDone * 0.08f;
                int healthToHeal = (int)healFactor;
                healthToHeal = Math.Clamp(healthToHeal, 1, 20);
                Player owner = Main.player[Projectile.owner];
                owner.Heal(healthToHeal);
            }
        }
    }