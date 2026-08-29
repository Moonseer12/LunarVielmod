using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.GunSwapping
{
    public class RavestBlast : MiniGun
    {
        public override void SetDefaults()
        {
            base.SetDefaults();

            //Setting this to width and height of the texture cause idk
            Item.damage = 202;
            Item.width = 56;
            Item.height = 30;
            LeftHand = true;
            RightHand = true;

            SoundStyle soundStyle = new("Stellamod/Assets/Sounds/GunRaving");
            soundStyle.PitchVariance = 0.5f;
            Item.UseSound = soundStyle;


            //Higher is faster
            AttackSpeed = 39;
            ShootCount = 3;

            //Offset it so it doesn't hold gun by weird spot
            HolsterOffset = new Vector2(0, -6);
        }

        public override void Fire(Player player, Vector2 position, Vector2 velocity, int damage, float knockback)
        {
            base.Fire(player, position, velocity, damage, knockback);
            if (player.PickAmmo(Item, out int projToShoot, out float speed, out int newDamage, out float knockBack, out int usedAmmoItemId))
            {
                float spread = 0.4f;
                for (int k = 0; k < 7; k++)
                {
                    Vector2 newDirection = velocity.RotatedByRandom(spread);
                    Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.Red, Main.rand.NextFloat(0.2f, 0.5f));
                }
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.DarkRed, 1);
                if (Main.myPlayer == player.whoAmI)
                {
                    Projectile.NewProjectile(player.GetSource_FromThis(), position, velocity * 8, ModContent.ProjectileType<RavestblastProj>(), damage, knockback, player.whoAmI);
                }
                SoundStyle soundStyle = new("Stellamod/Assets/Sounds/GunRaving");
                soundStyle.PitchVariance = 0.5f;
                SoundEngine.PlaySound(soundStyle, position);
            }
        }
    }
    
    public class RavestblastProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 1;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            Main.projFrames[Projectile.type] = 16;
        }


        public override void SetDefaults()
        {
            Projectile.width = 448;
            Projectile.height = 225;
            Projectile.penetrate = -1;
            Projectile.knockBack = 12.9f;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            Projectile.timeLeft = 255;
            AIType = ProjectileID.Bullet;
            Projectile.scale = 0.1f;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            DrawOriginOffsetY = 0;
        }

        public override bool PreAI()
        {

            Projectile.tileCollide = false;
            if (++_frameTick >= 2)
            {
                _frameTick = 0;
                if (++_frameCounter >= 16)
                {
                    _frameCounter = 0;
                }
            }
            return true;

        }
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 50f);
        }


        public override void AI()
        {

            Projectile.scale *= 1.02f;
            Projectile.ai[1]++;
            Projectile.velocity *= 1.02f;
            if (Projectile.ai[1] == 1)
            {



                for (int j = 0; j < 10; j++)
                {
                    Vector2 vector2 = Vector2.UnitX * -Projectile.width / 2f;
                    vector2 += -Vector2.UnitY.RotatedBy(j * 3.141591734f / 6f, default) * new Vector2(8f, 16f);
                    vector2 = vector2.RotatedBy(Projectile.rotation - 1.57079637f, default);
                    int num8 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.CoralTorch, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                    Main.dust[num8].scale = 1.3f;
                    Main.dust[num8].noGravity = true;
                    Main.dust[num8].position = Projectile.Center + vector2;
                    Main.dust[num8].velocity = Projectile.velocity * 0.1f;
                    Main.dust[num8].noLight = true;
                    Main.dust[num8].velocity = Vector2.Normalize(Projectile.Center - Projectile.velocity * 3f - Main.dust[num8].position) * 1.25f;
                }

            }

            if (Projectile.ai[1] > 1)
            {
                Projectile.alpha++;
            }




        }
        private int _frameCounter;
        private int _frameTick;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            float width = 448;
            float height = 225;
            Vector2 origin = new(width / 2, height / 2);
            int frameSpeed = 2;
            int frameCount = 16;
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Draw(texture, drawPosition,
                texture.AnimationFrame(ref _frameCounter, ref _frameTick, frameSpeed, frameCount, false),
                (Color)GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Lighting.AddLight(Projectile.Center, Color.Gold.ToVector3() * 1.75f * Main.essScale);
            if (Main.rand.NextBool(5))
            {
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.CoralTorch, 0f, 0f, 150, Color.White, 1f);
                Main.dust[dustnumber].velocity *= 0.3f;
            }
        }
    }
}