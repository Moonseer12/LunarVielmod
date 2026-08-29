using Stellamod.Common.IgnitersNPowders;
using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WondrousDarkspace.WeaponsWD
{
    public class HypnoFlamePowder : BasePowder
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            //Percent increase, 1 is +100% damage
            DamageModifier = 5;
            ExplosionType = ModContent.ProjectileType<KaBoomShade>();

            SoundStyle explosionSoundStyle = new($"Stellamod/Assets/Sounds/ExplosionBurstBomb");
            explosionSoundStyle.PitchVariance = 0.15f;
            ExplosionSound = explosionSoundStyle;
            ExplosionScreenshakeAmt = 2f;
        }


        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<HypnotizedSoul, BlankBag>();
        }
    }
    
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