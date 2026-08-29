using Stellamod.Common.IgnitersNPowders;
using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Snow.WeaponsSN;

public class FrostedPowder : BasePowder
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        //Percent increase, 1 is +100% damage
        DamageModifier = 1f;
        ExplosionType = ModContent.ProjectileType<FrostbiteProj>();

        SoundStyle explosionSoundStyle = new($"Stellamod/Assets/Sounds/Frosty");
        explosionSoundStyle.PitchVariance = 0.15f;
        ExplosionSound = explosionSoundStyle;
        ExplosionScreenshakeAmt = 2;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<WinterbornShard, BlankBag>();
    }
}

    public class FrostbiteProj : BaseIgniterExplosion
    {
        public override int FrameCount => 30;
        public override bool BlackIsTransparency => false;

        public override void SetDefaults()
        {
            base.SetDefaults();
            DrawScale = 1f;
        }

        public override void AI()
        {
            base.AI();
            DrawScale *= 0.98f;
        }

        public override void Start()
        {
            base.Start();
            if (Main.myPlayer == Projectile.owner)
            {
                EffectsHelper.SimpleExplosionCircle(Projectile, Color.LightCyan, 70);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (Main.rand.NextBool(3))
            {
                target.AddBuff(BuffID.Frostburn, 120);
            }
        }
    }