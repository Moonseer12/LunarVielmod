using Stellamod.Common.IgnitersNPowders;
using Stellamod.Common.MagicCauldron;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Palettes;
using Stellamod.Content.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.MothlightManor.WeaponsMM;

public class WinglessPowder : BasePowder
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        //Percent increase, 1 is +100% damage
        DamageModifier = 2.5f;
        ExplosionType = ModContent.ProjectileType<WinglessBoom>();

        ExplosionScreenshakeAmt = 2f;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<MothlightWing, BlankBag>();
    }
}

public class WinglessBoom : BaseIgniterExplosion
{
    public override int FrameCount => 18;
    public override void SetDefaults()
    {
        FrameSpeed = 0.5f;
        base.SetDefaults();
        DrawScale = 2.5f;
    
    }

    public override void Start()
    {
        base.Start();
        PixelPrimitiveCircleFactory.CreateInGoldBoom(Projectile.Center);
        var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.LightGoldenrodYellow, Color.Gold, 15, baseSize: 0.24f);
 
        for (float f = 0; f < 4; f++)
        {
            var dp = DustParticle.Spawn(Projectile.Center, Main.rand.NextVector2Circular(24, 24));
            dp.outerColor = Color.Gold;
            dp.dampening = 0.1f;
            dp.Scale *= 0.5f;
            dp.noTileCollide = true;
            dp.dampening = 0.05f;
            dp.gravity = 0;
        }

        SoundStyle glowSound;
        switch (Main.rand.Next(3))
        {
            default:
            case 0:
                glowSound = new SoundStyle("Stellamod/Assets/Sounds/GW1");
                break;
            case 1:
                glowSound = new SoundStyle("Stellamod/Assets/Sounds/GW2");
                break;
            case 2:
                glowSound = new SoundStyle("Stellamod/Assets/Sounds/GW3");
                break;
        }
        glowSound.Volume = 0.6f;
        glowSound.PitchVariance = 0.6f;
        SoundEngine.PlaySound(glowSound, Projectile.position);
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
    }
    protected override void DrawPixelExplosion(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        PalettizerShader shader = PalettizerShader.Use(PaletteAssets.PERFECT);
        SpritebatchParams @params = SpritebatchParams.InWorldAndZoomed() with { effect = shader, sortMode = SpriteSortMode.Immediate };
        using (SpritebatchStarter.Begin(spriteBatch, @params))
        {
            SpritebatchDrawer drawer = SpritebatchDrawer.FromProjectile(Projectile);
            drawer.color = Color.White;
            drawer.color.A = 0;
            drawer.scale *= 1.5f;
            spriteBatch.Draw(drawer);
        }
    }
}