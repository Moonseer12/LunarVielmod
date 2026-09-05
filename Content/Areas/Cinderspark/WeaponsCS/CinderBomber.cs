using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Dusts;
using Stellamod.Core.Bases;
using Stellamod.Content.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.WeaponsCS
{
    public class CinderBomber : BaseJugglerItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToCombatTool(0.01f, 0.04f, 1);
            Item.damage = 8;
            Item.DamageType = DamageClass.Ranged;
            Item.noUseGraphic = true;
            Item.useTime = 80;
            Item.useAnimation = 80;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<CinderBomberProj>();
            Item.shootSpeed = 28;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<Cinderscrap, BlankJuggler>();
        }
    }

    public class CinderBomberProj : BaseJugglerProjectile
    {
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (Juggler.combo >= 5)
            {
                FXUtil.ShakeCamera(target.Center, 1024, 4);
                SoundStyle fireBomb = new("Stellamod/Assets/Sounds/StormDragon_Bomb");
                fireBomb.PitchVariance = 0.3f;
                fireBomb.Volume = 0.5f;
                SoundEngine.PlaySound(fireBomb, target.Center);

                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero,
                    ModContent.ProjectileType<CinderBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

                for (int i = 0; i < 16; i++)
                {
                    DustParticleSpawnParams spawnParams = new DustParticleSpawnParams
                    {
                        innerColor = Color.OrangeRed,
                        outerColor = Color.Red,
                        scaleRange = new Vector2(0.3f, 1f)
                    };
                    DustParticle.Spawn(target.Center, Main.rand.NextVector2Circular(32, 32), spawnParams);
                }

                for (int i = 0; i < 8; i++)
                {
                    Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.OrangeRed, 1f).noGravity = true;
                }

                var boom  = FXUtil.GlowCircleBoom(target.Center,
                    innerColor: Color.White,
                    glowColor: Color.OrangeRed,
                    outerGlowColor: Color.Red, duration: 25, baseSize: 0.2f);
                boom.Scale *= 0.6f;
                for (int i = 0; i < 16; i++)
                {
                    Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                    var d = Dust.NewDustPerfect(target.Center, DustID.Torch, speed * 4, Scale: 1f);
                    d.noGravity = true;
                }
            }
        }
    }

    public class CinderBoom : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("FrostShotIN");
            Main.projFrames[Projectile.type] = 10;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = 196;
            Projectile.height = 196;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 20;
            Projectile.scale = 1f;
        }

        public override void AI()
        {

            Vector3 RGB = new(0.89f, 2.53f, 2.55f);
            // The multiplication here wasn't doing anything
            Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);

        }

        public override bool PreAI()
        {
            Projectile.tileCollide = false;
            if (++Projectile.frameCounter >= 2)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 10)
                {
                    Projectile.frame = 0;
                }
            }
            return true;


        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 120);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 50f);
        }
    }
}