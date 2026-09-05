using Stellamod.Assets;
using Stellamod.Common.GunSystem;
using Stellamod.Common.MagicCauldron;
using Stellamod.Common.Players;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Content.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.WeaponsCS
{
    public class FlameBuster : BaseGun
    {
        private int _comboCounter;
        public override void SetDefaults()
        {
            base.SetDefaults();
            remainingAmmo = 29;
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 12;
            Item.useTime = 29;
            Item.useAnimation = 29;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6;
            Item.autoReuse = true;
            Item.shootSpeed = 15;
            Item.shoot = ProjectileID.Bullet;
            Item.useAmmo = AmmoID.Bullet;
            Item.noMelee = true;
        }
        public override void SetMagazine(ref GunReloadParams fireParams)
        {
            base.SetMagazine(ref fireParams);
            fireParams.maxAmmo = 29;
            fireParams.reloadWindow = 180;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(12, 0);
        }

        public override bool GunShot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            _comboCounter++;
            if (_comboCounter > 28)
            {
                //Reset
                Item.useTime = 29;
                Item.useAnimation = 29;
                _comboCounter = 0;
            }

            if (_comboCounter > 5)
            {
                Item.useTime--;
                Item.useAnimation--;
                float recoilStrength = 5;
                player.AddRecoil(-velocity.SafeNormalize(Vector2.Zero) * recoilStrength);
                FXUtil.ShakeCamera(player.Center, 1024, 8f);

                int numProjectiles = Main.rand.Next(2, 5);
                velocity *= 2.5f;
                type = ModContent.ProjectileType<CinderFlameball>();
                for (int p = 0; p < numProjectiles; p++)
                {
                    // Rotate the velocity randomly by 30 degrees at max.
                    Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));
                    newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                    Projectile.NewProjectileDirect(source, position, newVelocity, type, damage, knockback, player.whoAmI);
                }

                for (int p = 0; p < numProjectiles / 2; p++)
                {
                    // Rotate the velocity randomly by 30 degrees at max.
                    Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));
                    newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                    Projectile.NewProjectileDirect(source, position, newVelocity,
                        ProjectileID.WandOfSparkingSpark, damage, knockback, player.whoAmI);
                }

                //Dust Burst Towards Mouse
                int count = (int)(_comboCounter * 0.5f);
                for (int k = 0; k < count; k++)
                {
                    Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(7));
                    newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                    Dust.NewDust(position, 0, 0, DustID.Smoke, newVelocity.X * 0.5f, newVelocity.Y * 0.5f);
                }

                SoundStyle shootSound = AssetManager.GetSound("CinderBraker");
                shootSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(shootSound, position);
                return false;
            }
            else
            {
                int numProjectiles = Main.rand.Next(3, 6);
                for (int p = 0; p < numProjectiles; p++)
                {
                    // Rotate the velocity randomly by 30 degrees at max.
                    Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));
                    newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                    Projectile.NewProjectileDirect(source, position, newVelocity, type, damage, knockback, player.whoAmI);
                }

                SoundEngine.PlaySound(SoundID.Item38, position);
            }

            return base.GunShot(player, source, position, velocity, type, damage, knockback);
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<Cinderscrap, BlankGun>();
        }
    }

    public class CinderFlameball : ModProjectile
    {
        public override string Texture => TextureRegistry.EmptyTexture;
        
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.height = 16;
            Projectile.width = 16;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 120;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.33f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Visuals();
        }

        private void Visuals()
        {
            if (Main.rand.NextBool(12))
            {
                DustParticleSpawnParams spawnParams = new DustParticleSpawnParams
                {
                    innerColor = Color.OrangeRed,
                    outerColor = Color.DarkRed,
                    gravity = 0f
                };
                DustParticle.Spawn(Projectile.Center, Vector2.Zero, spawnParams);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 180);
        }

        private float GetTrailWidth(float completionRatio)
        {
            return MathHelper.SmoothStep(24, 0, completionRatio);
        }

        private Color GetTrailColor(float p)
        {
            Color trailColor = Color.Lerp(Color.White, Color.LightBlue, p);
            return trailColor;
        }

        private void DrawPixelFlameTrail(GraphicsDevice graphicsDevice)
        {
            RichLaserShader laserShader = RichLaserShader.Instance;
            laserShader.LaserColor = Color.Goldenrod;
            laserShader.InnerColor = Color.Red;
            laserShader.OuterColor = Color.DarkRed;
            TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, GetTrailColor, GetTrailWidth, laserShader, Projectile.Size * 0.5f);
        }

        private void DrawPixelFlameBall(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            Texture2D ballTexture = AssetManager.GlowMask.SimpleGlowCircle.Value;
            Vector2 drawOrigin = ballTexture.Size() * 0.5f;
            Vector2 drawCenter = Projectile.Center - screenPos;

            Color glowColor = Color.OrangeRed;
            glowColor.A = 0;
            spriteBatch.Draw(ballTexture, drawCenter, null, glowColor, 0, drawOrigin, Projectile.scale * 0.06f, SpriteEffects.None, 0);

            glowColor = Color.Goldenrod;
            glowColor.A = 0;
            spriteBatch.Draw(ballTexture, drawCenter, null, glowColor, 0, drawOrigin, Projectile.scale * 0.06f, SpriteEffects.None, 0);
        }
        //Visual Stuffs
        public override bool PreDraw(ref Color lightColor)
        {
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelFlameTrail);
            PixelationManager.QueueSpritebatchDrawAction(DrawPixelFlameBall);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (float n = 0; n < 2f; n++)
            {
                DustParticleSpawnParams spawnParams = new();
                spawnParams.innerColor = Color.OrangeRed;
                spawnParams.outerColor = Color.Red;
                spawnParams.scaleRange = new Vector2(0.1f, 1f);
                DustParticle.Spawn(Projectile.Center, -Projectile.oldVelocity.RotatedByRandom(1.5f) * Main.rand.NextFloat(0.5f, 1f), spawnParams);
            }

            SmokeParticle sp = Particle<SmokeParticle>.SpawnInAlphaLayer(Projectile.Center, -Vector2.UnitY, Color.White, Scale: 1f);
            sp.initialColor = Color.White * 0.14f;

            for (int i = 0; i < 8; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
                var d = Dust.NewDustPerfect(Projectile.Center, DustID.InfernoFork, speed);
                d.noGravity = true;
            }
        }
    }
}