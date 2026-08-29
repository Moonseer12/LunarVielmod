using Stellamod.Common.IgnitersNPowders;
using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Ishtar.WeaponsIS;

public class EreshkinPowder : BasePowder
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        //Percent increase, 1 is +100% damage
        DamageModifier = 2f;
        ExplosionType = ModContent.ProjectileType<IshBoom>();

        SoundStyle explosionSoundStyle = new("Stellamod/Assets/Sounds/ExplosionGaseous");
        explosionSoundStyle.PitchVariance = 0.15f;
        ExplosionSound = explosionSoundStyle;
        ExplosionScreenshakeAmt = 2;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<EreshkinCandle, BlankBag>();
    }
}

    public class IshBoom : BaseIgniterExplosion
    {
        public override int FrameCount => 61;

        public override bool BlackIsTransparency => false;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 128;
            Projectile.height = 128;
        }

        public override void Start()
        {
            base.Start();
            if (Main.myPlayer == Projectile.owner)
            {
                EffectsHelper.SimpleExplosionCircle(Projectile, Color.White, 128);
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            if (Main.rand.NextBool(3))
            {
                target.AddBuff(BuffID.Confused, 120);
            }
        }
    }