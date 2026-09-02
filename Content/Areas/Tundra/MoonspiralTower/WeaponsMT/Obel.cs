using Stellamod.Common.MagicCauldron;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Dusts;
using Stellamod.Content.GunSwapping;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.MoonspiralTower.WeaponsMT
{
    public class Obel : MiniGun
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 40;
            RightHand = true;


            SoundStyle soundStyle = new("Stellamod/Assets/Sounds/GunShootNew10");
            soundStyle.PitchVariance = 0.5f;
            Item.UseSound = soundStyle;

            //Higher is faster
            AttackSpeed = 19;

            //Offset it so it doesn't hold gun by weird spot
            HolsterOffset = new Vector2(15, -6);

            //Recoil
            RecoilDistance = 4;
        }

        public override void Fire(Player player, Vector2 position, Vector2 velocity, int damage, float knockback)
        {
            base.Fire(player, position, velocity, damage, knockback);
            float spread = 0.4f;
            for (int k = 0; k < 7; k++)
            {
                Vector2 newDirection = velocity.RotatedByRandom(spread);
                Dust.NewDustPerfect(position, ModContent.DustType<GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.AliceBlue, Main.rand.NextFloat(0.2f, 0.5f));
            }
            Dust.NewDustPerfect(position, ModContent.DustType<GlowDust>(), new Vector2(0, 0), 125, Color.LightGoldenrodYellow, 1);

            Vector2 vel = velocity * 16;
            vel = vel.RotatedByRandom(MathHelper.PiOver4 / 15);
            if (Main.myPlayer == player.whoAmI)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), position, vel,
                    ModContent.ProjectileType<EnergyBall>(), damage, knockback, player.whoAmI);
            }

            SoundStyle soundStyle = new("Stellamod/Assets/Sounds/GunShootNew10");
            soundStyle.PitchVariance = 0.5f;
            SoundEngine.PlaySound(soundStyle, position);
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<PearlescentScrap, BlankGun>();
        }

    }

    public class EnergyBall : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 32;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.timeLeft = 180;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer % 8 == 0)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowDust>(), newColor: Color.DarkGoldenrod);
                Dust dust = Main.dust[dustIndex];
                dust.velocity = Vector2.Zero;
            }
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.2f);
        }


        private void DrawEnergyBall(ref Color lightColor)
        {
            //Draw Code for the orb
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 centerPos = Projectile.Center - Main.screenPosition;
            GlowCircleShader shader = GlowCircleShader.Instance;

            //How quickly it lerps between the colors
            shader.Speed = 10f;

            //This effects the distribution of colors
            shader.BasePower = 1f;

            //Radius of the circle
            shader.Size = 0.12f;


            //Colors
            Color startInner = Color.Lerp(Color.SkyBlue, Color.BlueViolet, VectorHelper.Osc(0f, 1f, speed: 5f)); ;
            Color startGlow = Color.Lerp(Color.CadetBlue, Color.CadetBlue, VectorHelper.Osc(0f, 1f, speed: 3f));
            Color startOuterGlow = Color.Lerp(Color.Black, Color.Black, VectorHelper.Osc(0f, 1f, speed: 3f));

            shader.InnerColor = startInner;
            shader.GlowColor = startGlow;
            shader.OuterGlowColor = startOuterGlow;

            //Idk i just included this to see how it would look
            //Don't go above 0.5;
            shader.Pixelation = 0.0055f;
            shader.Apply();

            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Restart(blendState: BlendState.Additive, effect: shader.Effect);
            for (int i = 0; i < 3; i++)
            {
                spriteBatch.Draw(texture, centerPos, null, Color.White, Projectile.rotation, texture.Size() / 2f, 1f, SpriteEffects.None, 0);
            }

            spriteBatch.RestartDefaults();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawEnergyBall(ref lightColor);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            FXUtil.GlowCircleBoom(Projectile.Center,
              innerColor: Color.White,
              glowColor: Color.Blue,
              outerGlowColor: Color.Red, duration: 25f, baseSize: 0.06f);
            for (int i = 0; i < 4; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.AliceBlue, 1f).noGravity = true;
            }
        }
    }
}