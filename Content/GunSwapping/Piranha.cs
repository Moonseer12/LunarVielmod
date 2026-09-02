using Stellamod.Assets;
using Stellamod.Content.Dusts;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.GunSwapping
{
    public class Piranha : MiniGun
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 4;
            RightHand = true;


            SoundStyle soundStyle = new("Stellamod/Assets/Sounds/GunShootNew4");
            soundStyle.PitchVariance = 0.5f;
            Item.UseSound = soundStyle;

            //Higher is faster
            AttackSpeed = 24;
            ShootCount = 6;
            //Offset it so it doesn't hold gun by weird spot
            HolsterOffset = new Vector2(15, -6);

            //Recoil
            RecoilDistance = 3;
        }

        public override void Fire(Player player, Vector2 position, Vector2 velocity, int damage, float knockback)
        {
            base.Fire(player, position, velocity, damage, knockback);
            float spread = 0.4f;
            for (int k = 0; k < 7; k++)
            {
                Vector2 newDirection = velocity.RotatedByRandom(spread);
                Dust.NewDustPerfect(position, ModContent.DustType<GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.Red, Main.rand.NextFloat(0.2f, 0.5f));
            }
            Dust.NewDustPerfect(position, ModContent.DustType<GlowDust>(), new Vector2(0, 0), 125, Color.DarkRed, 1);
            for (int i = 0; i < Main.rand.Next(1, 3); i++)
            {
                Vector2 vel = velocity * 16;
                vel = vel.RotatedByRandom(MathHelper.PiOver4 / 15);
                if (Main.myPlayer == player.whoAmI)
                {
                    Projectile.NewProjectile(player.GetSource_FromThis(), position, vel,
                    ModContent.ProjectileType<PiranhaProj>(), damage, knockback, player.whoAmI);
                }
            }

            SoundStyle soundStyle = new("Stellamod/Assets/Sounds/GunShootNew4");
            soundStyle.PitchVariance = 0.5f;
            SoundEngine.PlaySound(soundStyle, position);
        }
    }

    public class PiranhaProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.height = 22;
            Projectile.width = 32;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 120;
        }

        public override void AI()
        {
            Projectile.velocity.X *= 1.01f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Visuals();
        }

        private void Visuals()
        {
            float radius = 1 / 6f;
            if (Main.rand.NextBool(12))
            {
                for (int i = 0; i < 2; i++)
                {
                    float speedX = Main.rand.NextFloat(-radius, radius);
                    float speedY = Main.rand.NextFloat(-radius, radius);
                    float scale = Main.rand.NextFloat(0.66f, 1f);
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.InfernoFork,
                        speedX, speedY, Scale: scale);
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 20);
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width / 2;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public static Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Tan, Color.Transparent, completionRatio);
        }

        //Visual Stuffs
        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawSimpleTrail(Projectile, WidthFunction, ColorFunction, TrailRegistry.CrystalTrail);
            return base.PreDraw(ref lightColor);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.OrangeRed, 1f).noGravity = true;
            }
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DarkGray, Main.rand.NextFloat(0.3f, 1f)).noGravity = true;
            }
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);
            FXUtil.ShakeCamera(Projectile.Center, 1024f, 16f);




            SoundStyle soundStyle = new("Stellamod/Assets/Sounds/ExplosionBurstBomb");
            soundStyle.PitchVariance = 0.5f;
            SoundEngine.PlaySound(soundStyle, Projectile.position);

            Vector2 velocity2 = Projectile.velocity;
            velocity2 = -velocity2;
            velocity2 = velocity2.RotatedByRandom(MathHelper.PiOver4 + MathHelper.PiOver2);
            velocity2 *= Main.rand.NextFloat(0.5f, 1f);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity2 * 0,
                ModContent.ProjectileType<PiranhaBoomMini>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);

        }
    }

    public class PiranhaBoomMini : ModProjectile
    {
        private int _frameCounter;
        private int _frameTick;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 30;
        }

        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.localNPCHitCooldown = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.friendly = true;
            Projectile.width = 96;
            Projectile.height = 96;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;
            Projectile.scale = 1f;
        }

        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        private float _sizeMultiplier;


        public override bool PreAI()
        {

            Timer++;
            if (Timer == 1)
            {
                //Randomize Size
                _sizeMultiplier = Main.rand.NextFloat(0.3f, 0.8f);
                //  Projectile.width = (int)((float)Projectile.width * _sizeMultiplier);
                //   Projectile.height = (int)((float)Projectile.height * _sizeMultiplier);
            }
            if (++_frameTick >= 1)
            {
                _frameTick = 0;
                if (++_frameCounter >= 30)
                {
                    _frameCounter = 0;
                }
            }
            return true;
        }


        public override void AI()
        {
            Vector3 RGB = new(0.89f, 2.53f, 2.55f);
            Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 120);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 50f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            float width = 214;
            float height = 214;
            Vector2 origin = new(width / 2, height / 2);
            int frameSpeed = 1;
            int frameCount = 30;
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Draw(texture, drawPosition,
                texture.AnimationFrame(ref _frameCounter, ref _frameTick, frameSpeed, frameCount, false),
                (Color)GetAlpha(lightColor), 0f, origin, _sizeMultiplier, SpriteEffects.None, 0f);
            return false;
        }
    }
}