using Stellamod.Common.IgnitersNPowders;
using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Jungle.WeaponsJN;

public class RadiantPowder : BasePowder
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        //Percent increase, 1 is +100% damage
        DamageModifier = 2f;
        ExplosionType = ModContent.ProjectileType<RadiantBoom>();


        ExplosionScreenshakeAmt = 2f;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<RadiantNectar, BlankBag>();
    }
}

public class RadiantBoom : BaseIgniterExplosion
{
    public override int FrameCount => 7;
    public override void SetDefaults()
    {
        FrameSpeed = 0.25f;
        base.SetDefaults();

        DrawScale = 1.5f;
    }

    public override void Start()
    {
        base.Start();
        SoundStyle glowSound;
        switch (Main.rand.Next(3))
        {
            default:
            case 0:
                glowSound = new SoundStyle("Stellamod/Assets/Sounds/GoldenSlice1");
                break;
            case 1:
                glowSound = new SoundStyle("Stellamod/Assets/Sounds/GoldenSlice2");
                break;
            case 2:
                glowSound = new SoundStyle("Stellamod/Assets/Sounds/GoldenSlice3");
                break;
        }
        glowSound.Volume = 0.4f;
        glowSound.PitchVariance = 0.6f;
        SoundEngine.PlaySound(glowSound, Projectile.position);

        var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.Gold, Color.DarkGoldenrod, 15, baseSize: 0.24f);
        fx.Scale *= 2f;
        for(float f =0;f < 4; f++)
        {
            var dp = DustParticle.Spawn(Projectile.Center, Main.rand.NextVector2Circular(24, 24));
            dp.outerColor = Color.Gold;
            dp.dampening = 0.1f;
            dp.Scale *= 0.5f;
            dp.noTileCollide = true;
            dp.dampening = 0.05f;
            dp.gravity = 0;
        }
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
    }
}