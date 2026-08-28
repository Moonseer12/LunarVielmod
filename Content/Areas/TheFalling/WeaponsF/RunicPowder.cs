using Stellamod.Common.IgnitersNPowders;
using Stellamod.Common.MagicCauldron;
using Stellamod.Common.Shaders;
using Stellamod.Common.WeaponUpgrade.UI;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Palettes;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.TheFalling.WeaponsF;

public class RunicPowder : BasePowder
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        //Percent increase, 1 is +100% damage
        DamageModifier = 2.5f;
        ExplosionType = ModContent.ProjectileType<RunicBoom>();

        SoundStyle explosionSoundStyle = new($"Stellamod/Assets/Sounds/windpetal");
        explosionSoundStyle.PitchVariance = 0.15f;
        ExplosionSound = explosionSoundStyle;
        ExplosionScreenshakeAmt = 2f;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<GhastlySpirit, BlankBag>();
    }
}
public class RunicBoom : BaseIgniterExplosion
{
    private float _timer;
    private AnimationFramer _sunAnimationFrame;
    public override int FrameCount => 24;
    public override void SetDefaults()
    {
        base.SetDefaults();
        FrameSpeed = 0.5f;
    }

    public override void Start()
    {
        base.Start();
        var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.SkyBlue, Color.DarkBlue, 15, baseSize: 0.24f);
        fx.Scale *= 2f;
        for (float f = 0; f < 4; f++)
        {
            var dp = DustParticle.Spawn(Projectile.Center, Main.rand.NextVector2Circular(24, 24));
            dp.outerColor = Color.Blue;
            dp.dampening = 0.1f;
            dp.Scale *= 0.5f;
            dp.noTileCollide = true;
            dp.dampening = 0.05f;
            dp.gravity = 0;
        }
    }

    public override void AI()
    {
        base.AI();
        _timer++;
        _sunAnimationFrame.frameSpeed = 1;
        _sunAnimationFrame.maxFrame = 6 * 4;
        _sunAnimationFrame.UpdateTick();
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
    }
    protected override void DrawPixelExplosion(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        PalettizerShader shader = PalettizerShader.Use(PaletteAssets.MOONSPIRALTOWER);
        SpritebatchParams @params = SpritebatchParams.InWorldAndZoomed() with { effect = shader, sortMode = SpriteSortMode.Immediate };
        float a = EasingFunction.InOutSine(_timer / 40f);
        using (SpritebatchStarter.Begin(spriteBatch, @params))
        {
            SpritebatchDrawer drawer = SpritebatchDrawer.FromProjectile(Projectile);
            drawer.color = Color.White  * MathHelper.Lerp(1f, 0f, a);
            drawer.color.A = 0;// (byte)(MathHelper.Lerp(255, 0, 0.5f));

            Rectangle sunFrame = drawer.texture.GetFrame(_sunAnimationFrame.frame, 6, 4);
            drawer.sourceRect = sunFrame;
            drawer.CenterOrigin();
            spriteBatch.Draw(drawer);
        }
    }
}