using Stellamod.Common.IgnitersNPowders;
using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.WeaponsAB;

public class AbyssalPowder : BasePowder
{
    public override void SetDefaults()
    {
        base.SetDefaults();

        DamageModifier = 1.65f;
        ExplosionType = ModContent.ProjectileType<VoidKaboom>();

        SoundStyle explosionSoundStyle = new($"Stellamod/Assets/Sounds/ExplosionBurstBomb");
        explosionSoundStyle.PitchVariance = 0.15f;
        ExplosionSound = explosionSoundStyle;
        ExplosionScreenshakeAmt = 4;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<ConvulgingMater, BlankBag>();
    }
}

    public class VoidKaboom : BaseIgniterExplosion
    {
        public override int FrameCount => 30;
        public override void Start()
        {
            base.Start();
            if (Main.myPlayer == Projectile.owner)
            {
                EffectsHelper.SimpleExplosionCircle(Projectile, Color.Blue, 48);
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            if (Main.rand.NextBool(3))
            {
                target.AddBuff(ModContent.BuffType<AbyssalFlame>(), 120);
            }
        }
    }