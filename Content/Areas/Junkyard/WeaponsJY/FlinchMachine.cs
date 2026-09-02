using Stellamod.Content.Dusts;
using Stellamod.Content.Gores;
using Stellamod.Core.Bases;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Junkyard.WeaponsJY
{
    public class FlinchMachine : BaseJugglerItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 252;
            Item.DamageType = DamageClass.Ranged;
            Item.noUseGraphic = true;
            Item.useTime = 120;
            Item.useAnimation = 120;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.crit = 16;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<FlinchMachineProj>();
            Item.shootSpeed = 24;
        }
    }

    public class FlinchMachineProj : BaseJugglerProjectile
    {
        private ref float HitCount => ref Projectile.ai[2];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);

            HitCount++;
            bool extremeHitEffect = false;
            if (Juggler.combo >= 5)
            {
                if (HitCount >= 3)
                {
                    HitCount = 0;
                    extremeHitEffect = true;
                }
            }

            float catchCount = Juggler.combo;
            float pitch = MathHelper.Clamp(catchCount * 0.05f, 0f, 1f);
            if (extremeHitEffect)
            {
                SoundStyle fanHit2 = SoundRegistry.FanHit2;
                fanHit2.PitchVariance = 0.1f;
                SoundEngine.PlaySound(fanHit2, Projectile.position);
                FXUtil.ShakeCamera(Projectile.position, 2048, 64);
                target.SimpleStrikeNPC(Projectile.damage * 5, hit.HitDirection, damageType: Projectile.DamageType);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero,
                    ModContent.ProjectileType<FlinchMachineExplosionProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

                FXUtil.GlowCircleBoom(target.Center,
                     innerColor: Color.White,
                     glowColor: Color.Black,
                     outerGlowColor: Color.Black, duration: 25, baseSize: 0.4f);

                for (int i = 0; i < 7; i++)
                {
                    Gore.NewGore(Projectile.GetSource_FromThis(), target.position, Projectile.velocity.RotatedByRandom(MathHelper.TwoPi), GoreHelper.Fan1);
                    Gore.NewGore(Projectile.GetSource_FromThis(), target.position, Projectile.velocity.RotatedByRandom(MathHelper.TwoPi), GoreHelper.Fan2);
                    Gore.NewGore(Projectile.GetSource_FromThis(), target.position, Projectile.velocity.RotatedByRandom(MathHelper.TwoPi), GoreHelper.Fan3);
                }

                for (int i = 0; i < 16; i++)
                {
                    //Get a random velocity
                    Vector2 velocity = Main.rand.NextVector2CircularEdge(16, 16);

                    //Get a random
                    float randScale = Main.rand.NextFloat(0.5f, 1.5f);
                }
            }
            else
            {
                SoundStyle fanHit = SoundRegistry.FanHit1;
                fanHit.Pitch = pitch;
                fanHit.PitchVariance = 0.1f;
                fanHit.Volume = 0.85f;
                SoundEngine.PlaySound(fanHit, Projectile.position);

                for (int i = 0; i < 1; i++)
                {
                    Gore.NewGore(Projectile.GetSource_FromThis(), target.position, Projectile.velocity.RotatedByRandom(MathHelper.TwoPi), GoreHelper.Fan1);
                    Gore.NewGore(Projectile.GetSource_FromThis(), target.position, Projectile.velocity.RotatedByRandom(MathHelper.TwoPi), GoreHelper.Fan2);
                    Gore.NewGore(Projectile.GetSource_FromThis(), target.position, Projectile.velocity.RotatedByRandom(MathHelper.TwoPi), GoreHelper.Fan3);
                }

                for (int i = 0; i < 14; i++)
                {
                    Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.LightGray, 1f).noGravity = true;
                }

                for (int i = 0; i < 2; i++)
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Electric);
                }
            }
        }

    }

    public class FlinchMachineExplosionProj : ModProjectile
    {
        public override string Texture => TextureRegistry.ZuiEffect;
        private ref float Timer => ref Projectile.ai[0];
        public static float LifeTime => 60;
        public override void SetDefaults()
        {
            Projectile.width = 256;
            Projectile.height = 256;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.light = 0.78f;
            Projectile.timeLeft = (int)LifeTime;
        }

        public override void AI()
        {
            Timer++;
        }


        public override bool PreDraw(ref Color lightColor)
        {
            var texture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Skull");

            float progress = Timer / LifeTime;
            float easedProgress = Easing.OutExpo(progress);
            float alphaProgress = Easing.SpikeInOutCirc(progress);
            Main.spriteBatch.Draw(texture.Value, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(8, 8), null, new Color(255, 255, 255, 0) * alphaProgress, Projectile.rotation, new Vector2(24, 24), easedProgress * 3, SpriteEffects.None, 0f);
            return false;
        }
    }
}