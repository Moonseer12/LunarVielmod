using ReLogic.Content;
using Stellamod.Common;
using Stellamod.Common.MagicCauldron;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Dusts;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.WeaponsIL
{
    public class CloudMinionBuff : MinionBuff<CloudMinionProj> { }

    public class Thunderstaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true; // This lets the player target anywhere on the whole screen while using a controller.
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
        }
        public override void SetDefaults()
        {
            Item.damage = 48;
            Item.knockBack = 3f;
            Item.mana = 10;
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.UseSound = SoundID.Item46;
            Item.DamageType = DamageClass.Summon;
            Item.buffType = ModContent.BuffType<CloudMinionBuff>();
            Item.shoot = ModContent.ProjectileType<CloudMinionProj>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 1000; ++i)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == Main.myPlayer && Main.projectile[i].type == Item.shoot)
                    return false;
            }

            position = Main.MouseWorld;
            var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer);
            projectile.originalDamage = Item.damage;

            player.AddBuff(Item.buffType, 2);
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GSummon"), player.position);
            // Here you can change where the minion is spawned. Most vanilla minions spawn at the cursor position.

            return false;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse != 2)
            {
                for (int i = 0; i < 1000; ++i)
                {
                    if (Main.projectile[i].active && Main.projectile[i].owner == Main.myPlayer && Main.projectile[i].type == Item.shoot)
                    {
                        Main.projectile[i].minionSlots += 1f;
                        Main.projectile[i].originalDamage = Item.damage + (int)(4 * Main.projectile[i].minionSlots);
                        if (Main.projectile[i].scale < 1.8f)
                        {
                            Main.projectile[i].scale += 0.1f;
                        }

                    }
                }
            }
            return true;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<IllurineScale, BlankStaff>();
        }
    }

    public class CloudMinionProj : ModProjectile
    {
        private float FlashTimer;
        private Vector2 FlashPos;
        private ref float Timer => ref Projectile.ai[0];
        private ref float LightningTimer => ref Projectile.ai[1];
        private ref float TornadoTimer => ref Projectile.ai[2];
        private bool DoLightning => Projectile.minionSlots >= 3;
        private bool DoTornado => Projectile.minionSlots >= 6;
        private Player Owner => Main.player[Projectile.owner];

        public override void SetStaticDefaults()
        {
            // Sets the amount of frames this minion has on its spritesheet
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;

            Main.projFrames[Projectile.type] = 4;
            Main.projPet[Projectile.type] = true; // Denotes that this projectile is a pet or minion
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true; // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
        }

        public override void SetDefaults()
        {
            Projectile.width = 128;
            Projectile.height = 34;
            Projectile.tileCollide = false; // Makes the minion go through tiles freely

            // These below are needed for a minion weapon
            Projectile.friendly = true; // Only controls if it deals damage to enemies on contact (more on that later)
            Projectile.minion = true; // Declares this as a minion (has many effects)
            Projectile.DamageType = DamageClass.Summon; // Declares the damage type (needed for it to deal damage)
            Projectile.minionSlots = 1f; // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
            Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (!SummonHelper.CheckMinionActive<CloudMinionBuff>(owner, Projectile))
                return;

            SummonHelper.SearchForTargets(Owner, Projectile, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);
            Timer++;
            if (Main.rand.NextBool(100))
            {


                FlashPos = Projectile.position + new Vector2(Main.rand.Next(0, Projectile.width), Main.rand.Next(0, Projectile.height));
                for (float f = 0; f < 16; f++)
                {
                    int d = Dust.NewDust(FlashPos, 1, 1, ModContent.DustType<GlyphDust>(), newColor: GetMainColor(), Scale: Main.rand.NextFloat(0.5f, 2f));
                    Main.dust[d].velocity = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(4f, 9f);
                }
                FlashTimer = 1.5f;
            }
            if (Timer % 6 == 0)
            {

                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlyphDust>(), newColor: GetMainColor(), Scale: Main.rand.NextFloat(0.5f, 2f));
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Rain, newColor: GetMainColor(), Scale: Main.rand.NextFloat(0.5f, 2f));
            }
            FlashTimer *= 0.912f;
            if (Timer > 12 && foundTarget)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    Vector2 offset = new Vector2(Main.rand.Next(0, Projectile.width), Main.rand.Next(0, Projectile.height));
                    Vector2 pos = Projectile.position + offset;
                    Vector2 velocity = (targetCenter - pos).SafeNormalize(Vector2.Zero) * 12;

                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos, velocity,
                            ProjectileID.WandOfFrostingFrost, Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
                Timer = 0;
            }
            if (DoLightning && foundTarget)
            {

                LightningTimer++;

                if (LightningTimer == 60)
                {
                    FlashTimer = 2f;
                    SoundEngine.PlaySound(SoundID.DD2_LightningBugZap, Projectile.position);
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.UnitY,
                            ModContent.ProjectileType<TempestLightningBolt>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                }
                if (LightningTimer > 240)
                {
                    LightningTimer = 0;
                }
            }

            if (DoTornado && foundTarget)
            {
                TornadoTimer++;
                if (TornadoTimer > 120 && TornadoTimer % 30 == 0)
                {
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                            ModContent.ProjectileType<ClimateTornadoProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                }
                if (TornadoTimer > 180)
                {

                    TornadoTimer = 0;
                }
            }
            Vector2 targetPosition = Owner.Center + new Vector2(0, -Projectile.height * 4);
            Projectile.velocity = (targetPosition - Projectile.Center) * 0.1f;
            // So it will lean slightly towards the direction it's moving
            Projectile.rotation = Projectile.velocity.X * 0.005f;


            // Some visuals here
            Lighting.AddLight(Projectile.Center, GetMainColor().ToVector3() * 2.5f);
        }


        private Color GetMainColor()
        {
            if (DoTornado)
                return Color.Green;
            if (DoLightning)
                return Color.Goldenrod;
            return Color.Cyan;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ThunderCloudShader shader = ThunderCloudShader.Instance;
            shader.CloudColor = Color.Lerp(GetMainColor(), Color.Black, 0.7f);
            shader.CloudColor = Color.White;
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            shader.SourceSize = texture.Size();


            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Color drawColor = Color.White.MultiplyRGB(lightColor);
            Vector2 drawOrigin = texture.Size() / 2f;
            float drawRotation = Projectile.rotation;
            shader.CloudColor = GetMainColor();
            shader.Apply();

            float off = 112;
            spriteBatch.Restart(effect: shader.Effect, sortMode: SpriteSortMode.Immediate, blendState: BlendState.Additive);
            spriteBatch.Draw(texture, drawPos + new Vector2(off, 0), null, drawColor, drawRotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            for (float f = 0; f < 8; f++)
            {
                float p = f / 8f;
                float rot = p * MathHelper.TwoPi;
                rot += Main.GlobalTimeWrappedHourly;
                Vector2 offset = rot.ToRotationVector2() * 8;
                spriteBatch.Draw(texture, drawPos + new Vector2(off, 0) + offset, null, drawColor, drawRotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            }
            shader.CloudColor = Color.Black;
            shader.Apply();
            spriteBatch.Restart(effect: shader.Effect, sortMode: SpriteSortMode.Immediate, blendState: BlendState.AlphaBlend);
            spriteBatch.Draw(texture, drawPos + new Vector2(off, 0), null, drawColor, drawRotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);

            shader.CloudColor = Color.Lerp(Color.Black, GetMainColor(), FlashTimer);
            shader.Apply();
            spriteBatch.Restart(effect: shader.Effect, sortMode: SpriteSortMode.Immediate, blendState: BlendState.Additive);


            for (int i = 0; i < 1; i++)
                spriteBatch.Draw(texture, drawPos + new Vector2(off, 0), null, drawColor, drawRotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);

            spriteBatch.RestartDefaults();


            shader.CloudColor = Color.Lerp(Color.Black, Color.Gold, FlashTimer);
            shader.Apply();
            spriteBatch.Restart(effect: shader.Effect, sortMode: SpriteSortMode.Immediate, blendState: BlendState.Additive);


            for (int i = 0; i < 1; i++)
                spriteBatch.Draw(texture, drawPos + new Vector2(off, 0), null, drawColor, drawRotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            spriteBatch.RestartDefaults();
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D texture2D4 = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/DimLight").Value;
            Color glowColor = GetMainColor();
            glowColor.A = 0;
            glowColor *= FlashTimer;
            for (int i = 0; i < 3; i++)
            {
                Main.spriteBatch.Draw(texture2D4, FlashPos - Main.screenPosition, null, glowColor, Projectile.rotation, new Vector2(32, 32), 0.17f * (7 + 0.6f), SpriteEffects.None, 0f);
            }
        }
    }

    public class TempestLightningBolt : ModProjectile
    {
        private Vector2[] _lightningArcPos = new Vector2[1];
        public const int Trail_Width = 24;
        private ref float Timer => ref Projectile.ai[0];

        private Vector2 TargetPosition;

        private Player Owner => Main.player[Projectile.owner];
        public CoreLightning Lightning { get; set; } = new CoreLightning();
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetStaticDefaults()
        {
            // Sets the amount of frames this minion has on its spritesheet
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 48;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.timeLeft = 120;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.tileCollide = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(TargetPosition);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            TargetPosition = reader.ReadVector2();
        }

        public override void AI()
        {
            if (TargetPosition == Vector2.Zero)
                TargetPosition = Owner.Center;

            Timer++;

            NPC nearest = ProjectileHelper.FindNearestEnemy(Projectile.Center, 1024);
            if (nearest != null)
            {
                TargetPosition = Vector2.Lerp(TargetPosition, nearest.Center, 0.2f);
            }
            Vector2 targetPosition = Owner.Center + new Vector2(0, -34 * 4);
            Projectile.position += (targetPosition - Projectile.Center) * 0.1f;
            Projectile.velocity = (TargetPosition - Projectile.Center).SafeNormalize(Vector2.Zero);


            //Dunno if this is needed but whatever
            Projectile.rotation = Projectile.velocity.ToRotation();
            _lightningArcPos = CalculateLightningArc();
            for (int i = 1; i < _lightningArcPos.Length - 1; i++)
            {
                float p = (float)i / (float)_lightningArcPos.Length - 1;
                ref Vector2 pos = ref _lightningArcPos[i];
                ref Vector2 nextPos = ref _lightningArcPos[i + 1];
                Vector2 vec = (nextPos - pos);
                vec = vec.RotatedBy(MathHelper.ToRadians(90));
                vec *= p;

                pos += vec * MathF.Sin(Main.GlobalTimeWrappedHourly * -12 + p * 24);
                pos += vec * MathF.Sin((Main.GlobalTimeWrappedHourly + 4) * -12 + p * 12);

            }

            for (int i = 0; i < Lightning.Trails.Length; i++)
            {
                float progress = (float)i / (float)Lightning.Trails.Length;
                var trail = Lightning.Trails[i];
                trail.LightningRandomOffsetRange = 4;
                trail.LightningRandomExpand = 24;
                trail.PrimaryColor = Color.Lerp(Color.White, Color.Yellow, progress);
                trail.NoiseColor = Color.Lerp(Color.White, Color.Yellow, progress);
                Lightning.WidthTrailFunction = WidthFunction;
            }
            if (Timer % 3 == 0)
            {
                Lightning.RandomPositions(_lightningArcPos);
                for (int i = 0; i < _lightningArcPos.Length - 3; i++)
                {
                    Vector2 pos = _lightningArcPos[i];
                    if (Main.rand.NextBool(8))
                    {
                        Dust.NewDustPerfect(pos, ModContent.DustType<GlyphDust>(), Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.2f, 1f), 0, Color.Goldenrod, Main.rand.NextFloat(1f, 2f)).noGravity = true;
                    }
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //Electrifying!!!! nEMIES!!!
            target.AddBuff(BuffID.Electrified, 120);
            SoundEngine.PlaySound(SoundID.DD2_LightningBugZap, Projectile.position);

            for (int i = 0; i < 8; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(4, 4);
                var d = Dust.NewDustPerfect(target.Center, DustID.Electric, speed, Scale: Main.rand.NextFloat(0.5f, 1.5f));
                d.noGravity = true;
            }
        }

        public float WidthFunction(float completionRatio)
        {
            return MathHelper.SmoothStep(24, 16, completionRatio) * Easing.SpikeOutCirc(Timer / 120f);
        }

        public Color ColorFunction(float completionRatio)
        {
            Color startColor = Color.Goldenrod;
            Color endColor = Color.Transparent;
            return Color.Lerp(startColor, endColor, completionRatio);
        }

        private Vector2[] CalculateLightningArc()
        {
            float teleportDistance = 96;
            Vector2 currentPosition = Projectile.position;
            List<Vector2> positions = new List<Vector2>();
            positions.Add(currentPosition);
            for (int i = 0; i < 48; i++)
            {
                Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
                float distance = 40;
                Vector2 newPosition = currentPosition + direction * distance;
                currentPosition = newPosition;
                positions.Add(currentPosition);



                Vector2 targetCenter = currentPosition;
                bool foundTarget = false;
                NPC nearest = ProjectileHelper.FindNearestEnemy(currentPosition, teleportDistance);
                if (nearest != null)
                {
                    targetCenter = nearest.Center;
                    positions.Add(targetCenter);
                    positions.Add(targetCenter);
                    break;
                }

                if (!foundTarget)
                {
                    float distanceToMouse = Vector2.Distance(currentPosition, TargetPosition);
                    if (distanceToMouse < teleportDistance)
                    {
                        positions.Add(TargetPosition);
                        positions.Add(TargetPosition);
                        break;
                    }
                }
            }


            return positions.ToArray();
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //This damages everything in the trail
            Vector2[] positions = _lightningArcPos;
            float collisionPoint = 0;
            for (int i = 1; i < positions.Length; i++)
            {
                Vector2 position = positions[i];
                Vector2 previousPosition = positions[i - 1];
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), position, previousPosition, Trail_Width, ref collisionPoint))
                    return true;
            }
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Lightning.Draw(spriteBatch, _lightningArcPos, Projectile.oldRot);

            Texture2D texture = ModContent.Request<Texture2D>(TextureRegistry.EmptyGlowParticle).Value;
            Vector2 centerPos = _lightningArcPos[_lightningArcPos.Length - 1] - Main.screenPosition;
            centerPos += Main.rand.NextVector2Circular(8, 8);
            GlowCircleShader shader = GlowCircleShader.Instance;

            //How quickly it lerps between the colors
            shader.Speed = 10f;

            //This effects the distribution of colors
            shader.BasePower = 2.5f;

            //Radius of the circle
            shader.Size = VectorHelper.Osc(0.09f, 0.14f, speed: 6);


            //Colors
            Color startInner = Color.White;
            Color startGlow = Color.Lerp(Color.Goldenrod, Color.Goldenrod, VectorHelper.Osc(0f, 1f, speed: 3f));
            Color startOuterGlow = Color.Lerp(Color.Black, Color.Black, VectorHelper.Osc(0f, 1f, speed: 3f));

            shader.InnerColor = startInner;
            shader.GlowColor = startGlow;
            shader.OuterGlowColor = startOuterGlow;

            //Idk i just included this to see how it would look
            //Don't go above 0.5;
            shader.Pixelation = 0.005f;

            //This affects the outer fade
            shader.OuterPower = 13.5f;
            shader.Apply();

            spriteBatch.Restart(blendState: BlendState.Additive, effect: shader.Effect);
            for (int i = 0; i < 2; i++)
            {
                spriteBatch.Draw(texture, centerPos, null, Color.White, Projectile.rotation, texture.Size() / 2f, 1f, SpriteEffects.None, 0);
            }

            spriteBatch.RestartDefaults();
            return false;
        }
    }

    public class ClimateTornadoProj : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[1];
        private float Scale = 0f;
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.scale = 1f;
            Projectile.tileCollide = false;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 20;
        }

        public override void AI()
        {
            Timer++;
            Projectile.rotation += 0.5f;

            float progress = Timer / 60f;
            float easedProgress = Easing.SpikeOutCirc(progress);
            Scale = MathHelper.Lerp(0f, 1f, easedProgress);

            Lighting.AddLight(Projectile.position, 1.5f, 0.7f, 2.5f);
            Lighting.Brightness(2, 2);
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Color drawColor = new(100, 255, 255, 0);
            Asset<Texture2D> vortexTexture = ModContent.Request<Texture2D>("Stellamod/Assets/Effects/VoxTexture");
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Draw(vortexTexture.Value, Projectile.Center - Main.screenPosition,
                          vortexTexture.Value.Bounds, drawColor, Projectile.rotation,
                          vortexTexture.Size() * 0.5f, Scale, SpriteEffects.None, 0);
            return false;
        }
    }
}