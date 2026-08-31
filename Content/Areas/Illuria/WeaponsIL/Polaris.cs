using Stellamod.Assets;
using Stellamod.Common;
using Stellamod.Common.Shaders;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Dusts;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.WeaponsIL
{
    public class Polaris : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 750;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 45;
            Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2;
            Item.autoReuse = false;
            Item.shootSpeed = 30f;
            Item.shoot = ModContent.ProjectileType<PolarisHold>();
            Item.scale = 0.8f;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.WhiteSmoke.ToVector3() * 0.55f * Main.essScale); // Makes this item glow when thrown out of inventory.
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            DrawHelper.DrawGlowInInventory(Item, spriteBatch, position, ColorFunctions.Niivin);
            return true;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            DrawHelper.DrawGlow2InWorld(Item, spriteBatch, ref rotation, ref scale, whoAmI);
            return true;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            //The below code makes this item hover up and down in the world
            //Don't forget to make the item have no gravity, otherwise there will be weird side effects
            float hoverSpeed = 5;
            float hoverRange = 0.2f;
            float y = VectorHelper.Osc(-hoverRange, hoverRange, hoverSpeed);
            Vector2 position = new(Item.position.X, Item.position.Y + y);
            Item.position = position;
        }
    }

    public class PolarisHold : ModProjectile
    {
        enum ActionState
        {
            Aim_And_Charge,
            Fire
        }

        private float Max_Charge_Time => 120;

        ActionState State
        {
            get
            {
                return (ActionState)Projectile.ai[0];
            }
            set
            {
                Projectile.ai[0] = (float)value;
            }
        }
        private ref float SwordRotation => ref Projectile.ai[1];
        float ChargeTimer;
        float FireTimer;
        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.aiStyle = 595;
            Projectile.DamageType = DamageClass.Ranged;

            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = int.MaxValue;
        }

        public override void AI()
        {
            switch (State)
            {
                case ActionState.Aim_And_Charge:
                    AimAndCharge();
                    break;
                case ActionState.Fire:
                    Fire();
                    break;
            }

        }



        private void ChargeVisuals(float timer, float maxTimer)
        {
            float progress = timer / maxTimer;
            if (progress >= 1f)
                return;
            float minParticleSpawnSpeed = 24;
            float maxParticleSpawnSpeed = 12;
            int particleSpawnSpeed = (int)MathHelper.Lerp(minParticleSpawnSpeed, maxParticleSpawnSpeed, progress);
            if (timer % particleSpawnSpeed == 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 pos = Projectile.Center + Main.rand.NextVector2CircularEdge(168, 168);
                    Vector2 vel = (Projectile.Center - pos).SafeNormalize(Vector2.Zero) * 5;
                    if (Main.rand.NextBool(4))
                    {
                        var d = Dust.NewDustPerfect(pos, ModContent.DustType<GlyphDust>(), vel, newColor: Color.White, Scale: 1.2f);
                        d.noGravity = true;
                    }
                    else
                    {
                        var particle = FXUtil.GlowStretch(pos, vel * 3f);
                        particle.InnerColor = Color.White;
                        particle.GlowColor = Color.LightCyan;
                        particle.OuterGlowColor = Color.Black;
                        particle.Duration = Main.rand.NextFloat(12, 25);
                        particle.BaseSize = Main.rand.NextFloat(0.09f, 0.18f);
                        particle.VectorScale *= 0.35f;
                    }

                }
            }
        }

        private void AimAndCharge()
        {
            //Aiming Code
            Player player = Main.player[Projectile.owner];
            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);
            if (Main.myPlayer == Projectile.owner)
            {
                player.ChangeDir(Projectile.direction);
                SwordRotation = (Main.MouseWorld - player.Center).ToRotation();
                Projectile.netUpdate = true;
            }

            Projectile.velocity = SwordRotation.ToRotationVector2();
            Projectile.spriteDirection = player.direction;
            if (Projectile.spriteDirection == 1)
                Projectile.rotation = Projectile.velocity.ToRotation();
            else
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;


            Projectile.Center = playerCenter + Projectile.velocity * 1f;// customization of the hitbox position

            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);

            //Charging Code
            if (ChargeTimer == Max_Charge_Time - 1)
            {
                //Complete Charge
                for (int i = 0; i < 16; i++)
                {
                    Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                    var d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), speed, Scale: 0.5f, newColor: Color.LightCyan);
                    d.noGravity = true;
                }

                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/StormDragon_WaveCharge");
                soundStyle.PitchVariance = 0.15f;
                SoundEngine.PlaySound(soundStyle, Projectile.position);
            }

            ChargeTimer++;
            if (ChargeTimer == 1)
            {
                SoundEngine.PlaySound(SoundRegistry.Niivi_LaserBlastReady, Projectile.position);
            }

            ChargeVisuals(ChargeTimer, Max_Charge_Time);


            ChargeTimer = MathHelper.Clamp(ChargeTimer, 0, Max_Charge_Time);
            if (!player.channel)
            {
                State = ActionState.Fire;
                Projectile.netUpdate = true;
            }
        }

        public override bool ShouldUpdatePosition()
        {
            //Make velocity not move it
            return false;
        }

        private void Fire()
        {
            //Stay on player
            Player player = Main.player[Projectile.owner];
            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);
            float swordRotation = 0f;
            if (Main.myPlayer == Projectile.owner)
            {
                player.ChangeDir(Projectile.direction);
                swordRotation = (Main.MouseWorld - player.Center).ToRotation();
            }

            Projectile.velocity = swordRotation.ToRotationVector2();
            Projectile.spriteDirection = player.direction;
            Projectile.Center = playerCenter + Projectile.velocity * 1f;

            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);

            FireTimer++;
            if (FireTimer == 1)
            {
                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/StormDragon_Wave");
                soundStyle.PitchVariance = 0.15f;
                SoundEngine.PlaySound(soundStyle, Projectile.position);

                Vector2 velocity = Projectile.velocity;
                //Funny Recoil
                float recoilStrength = 7;
                Vector2 targetVelocity = -velocity.SafeNormalize(Vector2.Zero) * recoilStrength;
                player.velocity = VectorHelper.VelocityUpTo(player.velocity, targetVelocity);

                //Funny Screenshake
                Main.LocalPlayer.GetModPlayer<ShakePlayer>().ShakeAtPosition(player.Center, 1024f, 32f);

                //Dust Burst Towards Mouse
                float chargeProgress = ChargeTimer / Max_Charge_Time;
                int count = (int)(48f * chargeProgress);
                for (int k = 0; k < count; k++)
                {
                    Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(15)) * 18;
                    newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                    Dust.NewDust(Projectile.Center, 0, 0, DustID.IceTorch, newVelocity.X, newVelocity.Y);
                }

                float multiplier = chargeProgress * 3;
                int damage = (int)(multiplier * (float)Projectile.damage);
                //Shoot the laser
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity,
                    ModContent.ProjectileType<PolarisLaserProj>(), damage, Projectile.knockBack, player.whoAmI, ai0: chargeProgress);

            }

            if (FireTimer >= 60)
            {
                Projectile.Kill();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Player player = Main.player[Projectile.owner];

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            origin.X = Projectile.spriteDirection == 1 ? sourceRectangle.Width - 90 : 90; // Customization of the sprite position

            Color drawColor = Projectile.GetAlpha(lightColor);
            Main.EntitySpriteDraw((Texture2D)TextureAssets.Projectile[Projectile.type], Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            string glowTexture = Texture + "_White";
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;
            Texture2D texture = ModContent.Request<Texture2D>(glowTexture).Value;
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            origin.X = Projectile.spriteDirection == 1 ? sourceRectangle.Width - 90 : 90; // Customization of the sprite position

            float chargeProgress = ChargeTimer / Max_Charge_Time;
            Color drawColor = Color.Lerp(Color.Transparent, Color.White, chargeProgress);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
        }
    }

    public class PolarisLaserProj : ModProjectile
    {
        public override string Texture => TextureRegistry.EmptyTexture;
        //Don't change the sample points, 3 is good enough
        private const int NumSamplePoints = 3;

        private const float MaxBeamLength = 2400f;

        public float BeamLength;
        public List<Vector2> BeamPoints;
        ref float Size => ref Projectile.ai[0];
        float Timer;
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 45;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 25;
            BeamPoints = new();
        }

        public override void AI()
        {
            float targetBeamLength = PerformBeamHitscan();
            BeamLength = targetBeamLength;
            Timer++;
            if (Timer == 1)
            {
                switch (Main.rand.Next(2))
                {
                    case 0:
                        SoundEngine.PlaySound(SoundRegistry.Niivi_LaserBlast1, Projectile.position);
                        break;
                    case 1:
                        SoundEngine.PlaySound(SoundRegistry.Niivi_LaserBlast2, Projectile.position);
                        break;
                }


                Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
                Vector2 explosionCenter = Projectile.Center + direction * BeamLength;
                Main.LocalPlayer.GetModPlayer<ShakePlayer>().ShakeAtPosition(explosionCenter, 1024f, 32f);
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), explosionCenter, Vector2.Zero, ModContent.ProjectileType<SiriusBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }


                ShakeScreenPosition.Shake = 3;
                for (float f = 0; f < 12; f++)
                {
                    Vector2 initialVelocity = -Vector2.UnitY;
                    initialVelocity *= 12;
                    initialVelocity = initialVelocity.RotatedByRandom(MathHelper.ToRadians(60));
                    initialVelocity *= Main.rand.NextFloat(0.5f, 1f);

                    DustParticle dustParticle = Particle<DustParticle>.Spawn(explosionCenter, initialVelocity, Color.White, Scale: Main.rand.NextFloat(1.3f, 2f));
                    dustParticle.innerColor = Color.SkyBlue;
                    dustParticle.outerColor = Color.Violet;
                }

                for(float f = 0; f < 12; f++)
                {
                    Vector2 initialVelocity = -Vector2.UnitY;
                    initialVelocity *= 4;
                    initialVelocity = initialVelocity.RotatedByRandom(MathHelper.ToRadians(60));
                    initialVelocity *= Main.rand.NextFloat(0.15f, 1f);

                    SmokeParticle smokeParticle = Particle<SmokeParticle>.SpawnInAlphaLayer(explosionCenter + initialVelocity,
                        initialVelocity, Color.White, Scale: Main.rand.NextFloat(1.3f, 3f));
                    smokeParticle.initialColor = Color.Lerp(Color.White, Color.Black, 0.4f);
                    smokeParticle.extraUpdates = Main.rand.Next(0, 1);
                    smokeParticle.fadeToColor = Color.Black;
                }

                float numZaps = 4;
                for(float f = 0; f < numZaps; f++)
                {
                    Vector2 initialVelocity = -Vector2.UnitY;
                    initialVelocity *= 4;
                    initialVelocity = initialVelocity.RotatedByRandom(MathHelper.ToRadians(60));
                    initialVelocity *= Main.rand.NextFloat(0.15f, 1f);
                    ZapParticle zapParticle = LegacyParticle.NewParticle<ZapParticle>(explosionCenter + initialVelocity, Main.rand.NextVector2Circular(1, 1), Color.White);
                }


                FXUtil.GlowCircleBoom(explosionCenter,
                    innerColor: Color.White,
                    glowColor: Color.Yellow,
                    outerGlowColor: Color.Blue, duration: 25, baseSize: 0.24f);

                for (float i = 0; i < 8; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(360);
                    rot += Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 offset = rot.ToRotationVector2() * 24;
                    var particle = FXUtil.GlowCircleDetailedBoom1(explosionCenter,
                        innerColor: Color.White,
                        glowColor: Color.Yellow,
                        outerGlowColor: Color.Blue,
                        baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                        duration: Main.rand.NextFloat(15, 25));
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                }

                for (float f = 0; f < 12f; f++)
                {
                    float progress = f / 12f;
                    float rot = progress * MathHelper.ToRadians(360);
                    rot += Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 velocity = rot.ToRotationVector2() * Main.rand.NextFloat(4f, 25f);
                    var particle = FXUtil.GlowStretch(explosionCenter, velocity);
                    particle.InnerColor = Color.White;
                    particle.GlowColor = Color.LightCyan;
                    particle.OuterGlowColor = Color.Black;
                    particle.Duration = Main.rand.NextFloat(25, 50);
                    particle.BaseSize = Main.rand.NextFloat(0.09f, 0.18f);
                    particle.VectorScale *= 0.5f;

                }
                var sear = LegacyParticle.NewParticle<SearParticle>(explosionCenter, Vector2.Zero);
                sear.innerColor = Color.Cyan;
                sear.outerColor = Color.Blue;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = 0f;
            float width = Projectile.width * 0.8f * Size;
            Vector2 start = Projectile.Center;

            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
            Vector2 end = start + direction * BeamLength;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, width, ref _);
        }

        private float PerformBeamHitscan()
        {
            // By default, the hitscan interpolation starts at the Projectile's center.
            // If the host Prism is fully charged, the interpolation starts at the Prism's center instead.
            Vector2 samplingPoint = Projectile.Center;

            // Perform a laser scan to calculate the correct length of the beam.
            // Alternatively, if you want the beam to ignore tiles, just set it to be the max beam length with the following line.
            // return MaxBeamLength;
            float[] laserScanResults = new float[NumSamplePoints];


            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
            Collision.LaserScan(samplingPoint, direction, 0 * Projectile.scale, MaxBeamLength, laserScanResults);
            float averageLengthSample = 0f;
            for (int i = 0; i < laserScanResults.Length; ++i)
            {
                averageLengthSample += laserScanResults[i];
            }
            averageLengthSample /= NumSamplePoints;
            return averageLengthSample;
        }


        public float WidthFunction(float completionRatio)
        {
            float osc = VectorHelper.Osc(0.75f, 1f);

            float width = Projectile.timeLeft / 45f;
            return Projectile.width * Projectile.scale * osc * width * Size * 5;
        }
        public float WidthFunction2(float completionRatio)
        {
            return WidthFunction(completionRatio) * 0.5f;
        }

        public static Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Cyan, Color.White, ExtraMath.Osc(0f, 1f, speed: 32));
        }
        public static Color ColorFunction2(float completionRatio)
        {
            return Color.White;
        }


        public override bool PreDraw(ref Color lightColor)
        {
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelated);
            return false;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
      
        public void DrawPixelated(GraphicsDevice graphicsDevice)
        {
            //Put in the points
            //This is just a straight beam that collides with tiles
            BeamPoints.Clear();
            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
            for (int i = 0; i <= 8; i++)
            {
                Vector2 start = Projectile.Center;
                BeamPoints.Add(Vector2.Lerp(start, start + direction * BeamLength, i / 8f));
            }


            var shader = BasicLaserShader.Instance;
            shader.InnerColor = Color.White;
            shader.OuterColor = Color.Blue;
            shader.BlendState = BlendState.AlphaBlend;
            shader.LaserTexture = TrailRegistry.StarTrail;
            TrailDrawer.Draw(Main.spriteBatch, BeamPoints.ToArray(), ColorFunction, WidthFunction, shader);

            shader.BlendState = BlendState.AlphaBlend;
            TrailDrawer.Draw(Main.spriteBatch, BeamPoints.ToArray(), ColorFunction2, WidthFunction2, shader);
        }
    }
}