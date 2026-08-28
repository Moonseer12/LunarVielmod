using Stellamod.Common.IgnitersNPowders;
using Stellamod.Common.MagicCauldron;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Palettes;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital.WeaponsRC;

public class AlcaricPowder : BasePowder
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        //Percent increase, 1 is +100% damage
        DamageModifier = 5f;
        ExplosionType = ModContent.ProjectileType<AlcaBoom>();

        ExplosionScreenshakeAmt = 2f;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<AlcaricMush, BlankBag>();
    }
}

public class AlcaBoom : BaseIgniterExplosion
{
    public override int FrameCount => 10;
    public override void SetDefaults()
    {
        FrameSpeed = 0.5f;
        base.SetDefaults();
 
    }

    public override void Start()
    {
        base.Start();
    
        var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.Purple, Color.Black, Color.White, 15, baseSize: 0.24f);



        SoundStyle glowSound;
        switch (Main.rand.Next(2))
        {
            default:
            case 0:
                glowSound = new SoundStyle("Stellamod/Assets/Sounds/Magic/AutomationCast1");
                break;
            case 1:
                glowSound = new SoundStyle("Stellamod/Assets/Sounds/Magic/AutomationCast2");
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
        PalettizerShader shader = PalettizerShader.Use(PaletteAssets.ROYALCAPITAL);
        SpritebatchParams @params = SpritebatchParams.InWorldAndZoomed() with { effect = shader, sortMode = SpriteSortMode.Immediate };
        using (SpritebatchStarter.Begin(spriteBatch, @params))
        {
            SpritebatchDrawer drawer = SpritebatchDrawer.FromProjectile(Projectile);
            drawer.color = Color.White;
            drawer.color.A = 0;
            drawer.scale *= 1f;
            spriteBatch.Draw(drawer);
        }
    }
}