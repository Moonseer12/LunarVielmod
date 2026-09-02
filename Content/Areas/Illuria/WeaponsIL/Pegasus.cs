using System.Collections.Generic;
using Stellamod.Common;
using Stellamod.Common.Shaders;
using Stellamod.Content.Dusts;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Stellamod.Content.Areas.Illuria.WeaponsIL
{
    public class PegasusMinionBuff : MinionBuff<PegasusMinionProj> { }

    public class Pegasus : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 106;
            Item.knockBack = 3f;
            Item.mana = 10;
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.HoldUp;

            // These below are needed for a minion weapon
            Item.noMelee = true;
            Item.DamageType = DamageClass.Summon;
            Item.buffType = ModContent.BuffType<PegasusMinionBuff>();

            // No buffTime because otherwise the item tooltip would say something like "1 minute duration"
            Item.shoot = ModContent.ProjectileType<PegasusMinionProj>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.HasBuff(ModContent.BuffType<PegasusMinionBuff>()))
                return false;
            // This is needed so the buff that keeps your minion alive and allows you to despawn it properly applies
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GSummon"), player.position);
            // Here you can change where the minion is spawned. Most vanilla minions spawn at the cursor position.
            // This is needed so the buff that keeps your minion alive and allows you to despawn it properly applies
            player.AddBuff(Item.buffType, 2);

            // Minions have to be spawned manually, then have originalDamage assigned to the damage of the summon item
            position = Main.MouseWorld;
            var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback,
                player.whoAmI, 0);
            projectile.originalDamage = Item.damage;

            projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback,
                player.whoAmI, 1);
            projectile.originalDamage = Item.damage;

            projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback,
                player.whoAmI, 2);
            projectile.originalDamage = Item.damage;

            // Since we spawned the projectile manually already, we do not need the game to spawn it for ourselves anymore, so return false
            return false;
        }
    }

    public class PegasusMinionProj : ModProjectile
    {
        private enum ActionState
        {
            Frost,
            Stars,
            Lightning
        }

        private ActionState State
        {
            get => (ActionState)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }

        private ref float Timer => ref Projectile.ai[1];
        private ref float RotTimer => ref Projectile.ai[2];
        private float WhiteTimer = 0f;
        private IEntitySource EntitySource => Projectile.GetSource_FromThis();
        private Player Owner => Main.player[Projectile.owner];
        public override void SetStaticDefaults()
        {
            // This is necessary for right-click targeting
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true; // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.tileCollide = false; // Makes the minion go through tiles freely
            Projectile.friendly = true; // Only controls if it deals damage to enemies on contact (more on that later)
            Projectile.minion = true; // Declares this as a minion (has many effects)
            Projectile.DamageType = DamageClass.Summon; // Declares the damage type (needed for it to deal damage)
            Projectile.minionSlots = 1f; // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
            Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
            Projectile.scale = 1f;
        }


        // Here you can decide if your minion breaks things like grass or pots
        public override bool? CanCutTiles()
        {
            return true;
        }

        // This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
        public override bool MinionContactDamage()
        {
            return false;
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 0.3f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            switch (State)
            {
                default:
                case ActionState.Frost:
                    return Color.Lerp(Color.LightCyan, Color.Transparent, completionRatio) * 0.7f;
                case ActionState.Stars:
                    return Color.Lerp(Color.Blue, Color.Transparent, completionRatio) * 0.7f;
                case ActionState.Lightning:
                    return Color.Lerp(Color.DarkGoldenrod, Color.Transparent, completionRatio) * 0.7f;
            }
        }

        private string GetTexturePath()
        {
            switch (State)
            {
                default:
                case ActionState.Frost:
                    return $"{Texture}_Frost";
                case ActionState.Lightning:
                    return $"{Texture}_Lightning";
                case ActionState.Stars:
                    return $"{Texture}_Stars";
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            string texturePath = GetTexturePath();
            Texture2D texture = ModContent.Request<Texture2D>(texturePath).Value;
            Vector2 drawPosition = Projectile.position + texture.Size() * 0.5f - Main.screenPosition;
            Rectangle? sourceRectangle = null;
            Color drawColor = Color.White;
            float drawRotation = Projectile.rotation;
            Vector2 drawOrigin = texture.Size() * 0.5f;
            float drawScale = Projectile.scale;

            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Draw(texture, drawPosition, sourceRectangle, drawColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            base.PostDraw(lightColor);
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            var shader = ShaderRegistry.MiscSilPixelShader;

            //The color to lerp to
            shader.UseColor(Color.White);

            //Should be between 0-1
            //1 being fully opaque
            //0 being the original color
            if (WhiteTimer <= 0)
                WhiteTimer = 0f;
            shader.UseSaturation(WhiteTimer);

            // Call Apply to apply the shader to the SpriteBatch. Only 1 shader can be active at a time.
            shader.Apply(null);

            string texturePath = GetTexturePath();
            Texture2D texture = ModContent.Request<Texture2D>(texturePath).Value;
            Vector2 drawPosition = Projectile.position + (texture.Size() * 0.5f) - Main.screenPosition;
            Rectangle? sourceRectangle = null;
            Color drawColor = Color.White;
            float drawRotation = Projectile.rotation;
            Vector2 drawOrigin = texture.Size() * 0.5f;
            float drawScale = Projectile.scale;

            spriteBatch.Draw(texture, drawPosition, sourceRectangle, drawColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);

            spriteBatch.End();
            spriteBatch.Begin();
        }

        public override void AI()
        {
            if (!SummonHelper.CheckMinionActive<PegasusMinionBuff>(Owner, Projectile))
                return;

            WhiteTimer -= 0.01f;

            AI_Movement();
            switch (State)
            {
                case ActionState.Frost:
                    AI_Frost();
                    break;
                case ActionState.Stars:
                    AI_Stars();
                    break;
                case ActionState.Lightning:
                    AI_Lightning();
                    break;
            }

            Projectile.rotation = Projectile.velocity.X * 0.05f;

            // Some visuals here
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
        }

        private Color GetMainColor()
        {
            switch (State)
            {
                default:
                case ActionState.Frost:
                    return Color.LightCyan;
                case ActionState.Stars:
                    return Color.Blue;
                case ActionState.Lightning:
                    return Color.DarkGoldenrod;
            }
        }

        private void AI_Movement()
        {
            RotTimer++;
            if (RotTimer % 6 == 0)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(), Projectile.velocity * 0.1f, 0, GetMainColor(), Main.rand.NextFloat(1f, 1.5f));

            }
            float offset = (MathHelper.TwoPi / 3f) * (float)State;
            float circleDistance = 128;
            Vector2 circlePosition = Owner.Center + new Vector2(circleDistance, 0)
                .RotatedBy(offset + RotTimer * 0.02f);

            //Oscillate movement
            // float ySpeed = MathF.Sin(offset + RotTimer * 0.05f);
            // circlePosition.Y += ySpeed;
            Projectile.Center = circlePosition;
        }

        private void AI_Frost()
        {
            SummonHelper.SearchForTargets(Owner, Projectile,
                out bool foundTarget,
                out float distanceFromTarget,
                out Vector2 targetCenter);

            if (!foundTarget)
                return;
            Timer++;
            if (Timer >= 30)
            {
                WhiteTimer = 1f;
                Vector2 velocity = Projectile.Center.DirectionTo(targetCenter) * 24;
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(EntitySource, Projectile.Center, velocity,
    ModContent.ProjectileType<PegasusMinionFrostBombProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }

                SoundStyle soundStyle = SoundRegistry.IceyWind;
                soundStyle.PitchVariance = 0.33f;
                SoundEngine.PlaySound(soundStyle, Projectile.position);
                Timer = 0;
            }
        }

        private void AI_Lightning()
        {
            SummonHelper.SearchForTargets(Owner, Projectile,
                out bool foundTarget,
                out float distanceFromTarget,
                out Vector2 targetCenter);

            if (!foundTarget)
                return;
            Timer++;
            if (Timer >= 240)
            {
                WhiteTimer = 1f;
                Vector2 velocity = Projectile.Center.DirectionTo(targetCenter) * 96;
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(EntitySource, Projectile.Center, velocity,
                    ModContent.ProjectileType<PegasusMinionLightningProj>(), Projectile.damage * 8, Projectile.knockBack, Projectile.owner);
                    Projectile.NewProjectile(EntitySource, Projectile.Center, velocity.RotatedByRandom(MathHelper.PiOver4) * 0.5f,
                       ModContent.ProjectileType<PegasusMinionLightningProj>(), Projectile.damage * 8, Projectile.knockBack, Projectile.owner);
                    Projectile.NewProjectile(EntitySource, Projectile.Center, velocity.RotatedByRandom(MathHelper.PiOver4) * 0.5f,
                       ModContent.ProjectileType<PegasusMinionLightningProj>(), Projectile.damage * 8, Projectile.knockBack, Projectile.owner);
                }
                SoundStyle soundStyle = SoundRegistry.Lightning2;
                soundStyle.PitchVariance = 0.33f;
                SoundEngine.PlaySound(soundStyle, Projectile.position);
                Timer = 0;
            }
        }

        private void AI_Stars()
        {
            SummonHelper.SearchForTargets(Owner, Projectile,
                out bool foundTarget,
                out float distanceFromTarget,
                out Vector2 targetCenter);

            if (!foundTarget)
                return;
            Timer++;
            if (Timer >= 60 && Timer % 8 == 0)
            {
                WhiteTimer = 1f;
                if (Main.myPlayer == Projectile.owner)
                {
                    Vector2 velocity = Projectile.Center.DirectionTo(targetCenter) * 15;
                    Projectile.NewProjectile(EntitySource, Projectile.Center, velocity.RotatedByRandom(MathHelper.PiOver4 / 3),
                        ModContent.ProjectileType<PegasusMinionStarProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }
            if (Timer >= 120)
            {
                Timer = 0;
            }
        }
    }

    public class PegasusMinionFrostBombProj : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyGlowParticle;
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
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlyphDust>(), newColor: Color.LightCyan, Scale: Main.rand.NextFloat(1f, 2f));
                Dust dust = Main.dust[dustIndex];
                dust.velocity = Vector2.Zero;
            }
            Lighting.AddLight(Projectile.Center, Color.LightCyan.ToVector3() * 0.2f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            target.AddBuff(BuffID.Frostburn, 120);
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
            shader.BasePower = 2.5f;

            //Radius of the circle
            shader.Size = 0.06f;


            //Colors
            Color startInner = Color.White;
            Color startGlow = Color.Lerp(Color.LightCyan, Color.CadetBlue, VectorHelper.Osc(0f, 1f, speed: 3f));
            Color startOuterGlow = Color.Lerp(Color.Blue, Color.Blue, VectorHelper.Osc(0f, 1f, speed: 3f));

            shader.InnerColor = startInner;
            shader.GlowColor = startGlow;
            shader.OuterGlowColor = startOuterGlow;

            //Idk i just included this to see how it would look
            //Don't go above 0.5;
            shader.Pixelation = 0.005f;

            //This affects the outer fade
            shader.OuterPower = 13.5f;
            shader.Apply();


            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Restart(blendState: BlendState.Additive, effect: shader.Effect);
            for (int i = 0; i < 2; i++)
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
              glowColor: Color.LightCyan,
              outerGlowColor: Color.Blue, duration: 25f, baseSize: 0.06f);
            for (int i = 0; i < 4; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.LightCyan, 1f).noGravity = true;
            }
        }
    }

    public class PegasusMinionLightningProj : ModProjectile
    {
        public override string Texture => TextureRegistry.EmptyTexture;
        private ref float Timer => ref Projectile.ai[0];
        private int Seed
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        private float Lifetime => 48;
        private Vector2[] LightningPos;

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Summon;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.timeLeft = (int)Lifetime;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }


        public override void AI()
        {
            if (Seed != 0)
            {
                //Calculate
                UnifiedRandom random = new(Seed);
                List<Vector2> points = new List<Vector2>();
                Vector2 currentPoint = Projectile.Center;
                points.Add(currentPoint);

                int numPoints = 24;
                for (int i = 0; i < numPoints; i++)
                {
                    Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
                    direction = direction.RotatedByRandom(MathHelper.ToRadians(30));
                    float distance = random.NextFloat(2, Projectile.velocity.Length());
                    currentPoint = currentPoint + direction * distance;
                    points.Add(currentPoint);
                }

                LightningPos = points.ToArray();
                Seed = 0;
            }

            Timer++;
            if (Timer == 1 && Main.myPlayer == Projectile.owner)
            {
                Seed = Main.rand.Next(1, int.MaxValue);
                Projectile.netUpdate = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //Electrifying!!!! nEMIES!!!
            target.AddBuff(BuffID.Electrified, 120);
            SoundEngine.PlaySound(SoundID.DD2_LightningBugZap, Projectile.position);

            for (int i = 0; i < 8; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(1, 1);
                var d = Dust.NewDustPerfect(Projectile.Center, DustID.Electric, speed, Scale: 1.5f);
                d.noGravity = true;
            }
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * 30;
            float progress = Timer / (float)Lifetime;
            float easedProgress = Easing.InOutExpo(progress);
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio) * (1f - easedProgress);
        }

        public Color ColorFunction(float completionRatio)
        {
            Color startColor = Color.White;
            Color endColor = Color.Transparent;
            return Color.Lerp(startColor, endColor, completionRatio);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //This damages everything in the trail
            Vector2[] positions = LightningPos;
            if (positions == null)
                return false;
            float collisionPoint = 0;
            for (int i = 1; i < positions.Length; i++)
            {
                Vector2 position = positions[i];
                Vector2 previousPosition = positions[i - 1];
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), position, previousPosition, 1, ref collisionPoint))
                    return true;
            }
            return base.Colliding(projHitbox, targetHitbox);
        }
    }

    public class PegasusMinionStarProj : ModProjectile
    {
        public override string Texture => TextureRegistry.ZuiEffect;


        private Vector2 OldVelocity;
        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        private bool HasBounced
        {
            get => Projectile.ai[1] == 1;
            set
            {
                if (value == true)
                {
                    Projectile.ai[1] = 1;
                }
                else
                {
                    Projectile.ai[1] = 0;
                }
            }
        }

        private ref float VelTimer => ref Projectile.ai[2];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 360;
        }

        public override void AI()
        {


            Timer++;
            if (Timer == 1)
            {
                OldVelocity = Projectile.velocity;
            }
            float maxDetectDistance = 512;
            NPC closestNpc = NPCHelper.FindClosestNPC(Projectile.position, maxDetectDistance);
            if (closestNpc != null)
            {
                Vector2 velocityToTarget = Projectile.Center.DirectionTo(closestNpc.Center) * OldVelocity.Length();
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, velocityToTarget, 0.2f);
            }

            VelTimer++;
            if (Timer == 1)
            {
                SoundStyle soundStyle = SoundRegistry.Niivi_StarSummon;
                soundStyle.PitchVariance = 0.15f;
                SoundEngine.PlaySound(soundStyle, Projectile.position);
            }
            Projectile.rotation += 0.05f;
            Lighting.AddLight(Projectile.position, Color.White.ToVector3() * 0.78f);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(
                Color.White.R,
                Color.White.G,
                Color.White.B, 0) * (1f - Projectile.alpha / 50f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Draw the texture
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 drawSize = texture.Size();
            Vector2 drawOrigin = drawSize / 2;

            float scale = 1f;
            Color drawColor = (Color)GetAlpha(lightColor);
            SpriteBatch spriteBatch = Main.spriteBatch;
            for (int i = 0; i < 2; i++)
            {
                float rotOffset = MathHelper.TwoPi * (i / 4f);
                rotOffset += Timer * 0.003f;
                float drawScale = scale * (i / 4f);
                spriteBatch.Draw(texture, drawPosition, null, drawColor, Projectile.rotation + rotOffset,
                    drawOrigin, drawScale, SpriteEffects.None, 0f);
            }
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundStyle soundStyle = SoundRegistry.Niivi_StarringDeath;
            soundStyle.PitchVariance = 0.1f;
            SoundEngine.PlaySound(soundStyle, Projectile.position);
        }
    }
}