using Stellamod.Common.IgnitersNPowders;
using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.WeaponsIL;

public class FrozenPowder : BasePowder
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        //Percent increase, 1 is +100% damage
        DamageModifier = 2f;
        ExplosionType = ModContent.ProjectileType<FrozenBoom>();

        SoundStyle explosionSoundStyle = new($"Stellamod/Assets/Sounds/Green");
        explosionSoundStyle.PitchVariance = 0.15f;
        ExplosionSound = explosionSoundStyle;
        ExplosionScreenshakeAmt = 3;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<IllurineScale, BlankBag>();
    }
}
public class FrozenBoom : BaseIgniterExplosion
{
    public override int FrameCount => 6;
    public override void SetDefaults()
    {
        FrameSpeed = 0.25f;
        base.SetDefaults();
        DrawScale = 1f;
    }

    public override void Start()
    {
        base.Start();
        FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.SkyBlue, Color.DarkBlue, duration: 24);
        SoundStyle glowSound;
        switch (Main.rand.Next(2))
        {
            default:
            case 0:
                glowSound = new SoundStyle("Stellamod/Assets/Sounds/Illuria/IceImpact1");
                break;
            case 1:
                glowSound = new SoundStyle("Stellamod/Assets/Sounds/Illuria/IceImpact2");
                break;
        }
        glowSound.Volume = 0.4f;
        glowSound.PitchVariance = 0.6f;
        SoundEngine.PlaySound(glowSound, Projectile.position);
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
        if (Main.rand.NextBool(3))
        {
            target.AddBuff(BuffID.Frostburn2, 120);
        }
    }
}