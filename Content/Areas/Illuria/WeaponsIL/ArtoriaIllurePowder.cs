using Stellamod.Common.IgnitersNPowders;
using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.WeaponsIL;

public class ArtoriaIllurePowder : BasePowder
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        //Percent increase, 1 is +100% damage
        DamageModifier = 2f;
        ExplosionType = ModContent.ProjectileType<IlluredBoom>();

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

    public class IlluredBoom : BaseIgniterExplosion
    {
        public override int FrameCount => 32;

        public override void Start()
        {
            base.Start();
            if (Main.myPlayer == Projectile.owner)
            {
                EffectsHelper.SimpleExplosionCircle(Projectile, Color.LightSkyBlue, 80);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            Vector2 upwardVelocity = -Vector2.UnitY * Projectile.knockBack * 8.5f;
            upwardVelocity *= target.knockBackResist;
            target.velocity += upwardVelocity;
        }
    }