using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Fable.AccFB;

public class FireEmblemPlayer : ModPlayer
{
    public bool hasFireEmblem;
    public int fireEmblemCooldown;
    public override void ResetEffects()
    {
        hasFireEmblem = false;
    }

    public override void PostUpdateEquips()
    {
        if (fireEmblemCooldown > 0)
            fireEmblemCooldown--;
    }


    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (hasFireEmblem && fireEmblemCooldown <= 0)
        {
            if (Main.rand.NextBool(6))
            {
                switch (Main.rand.Next(0, 4))
                {
                    case 0:
                        target.AddBuff(BuffID.OnFire3, 120);
                        break;
                    case 1:
                        target.AddBuff(BuffID.ShadowFlame, 120);
                        break;
                    case 2:
                        target.AddBuff(BuffID.CursedInferno, 120);
                        break;
                    case 3:
                        target.AddBuff(BuffID.Daybreak, 60);
                        break;
                }
            }


            if (hit.Crit && Main.rand.NextBool(2))
            {
                ShakeScreenPosition.Shake = 10;
                SoundStyle soundStyle = new($"Stellamod/Assets/Sounds/Kaboom");
                soundStyle.PitchVariance = 0.15f;
                SoundEngine.PlaySound(soundStyle, target.position);
                Projectile.NewProjectile(Player.GetSource_FromThis(), target.Center, Vector2.Zero,
                    ModContent.ProjectileType<FireBoom>(), damageDone / 2, hit.Knockback, Player.whoAmI);
            }

            fireEmblemCooldown = 120;
        }
    }
}

    public class FireBoom : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("FrostShotIN");
            Main.projFrames[Projectile.type] = 15;
        }

        public override void SetDefaults()
        {
            Projectile.localNPCHitCooldown = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.friendly = true;
            Projectile.width = 512;
            Projectile.height = 512;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;
        }

        public override void AI()
        {

            Vector3 RGB = new(0.89f, 2.53f, 2.55f);
            // The multiplication here wasn't doing anything
            Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            switch (Main.rand.Next(0, 4))
            {
                case 0:
                    target.AddBuff(BuffID.OnFire3, 120);
                    break;
                case 1:
                    target.AddBuff(BuffID.ShadowFlame, 120);
                    break;
                case 2:
                    target.AddBuff(BuffID.CursedInferno, 120);
                    break;
                case 3:
                    target.AddBuff(BuffID.Daybreak, 60);
                    break;
            }
        }

        public override bool PreAI()
        {
            if (++Projectile.frameCounter >= 2)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 15)
                {
                    Projectile.frame = 0;
                }
            }
            return true;


        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 50f);
        }


    }

public class FireEmblem : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToAccessory();
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<FireEmblemPlayer>().hasFireEmblem = true;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<AlcadizScrap, BlankAccessory>();
    }
}
