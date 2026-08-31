using Stellamod.Common;
using Stellamod.Core.Bases;
using Stellamod.Core.SwingSystem;
using Stellamod.Dusts;
using Stellamod.Trailing;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.WeaponsIL
{
    public class Sirius : BaseSwingItemV2
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 300;

            Item.useTime = 32;
            Item.useAnimation = 32;
            Item.useStyle = ItemUseStyleID.Swing;

            Item.knockBack = 6;
            Item.shoot = ModContent.ProjectileType<SiriusSlash>();
            Item.shootSpeed = 20;
            staminaProjectileShoot = ModContent.ProjectileType<SiriusProj>();
            meleeWeaponType = MeleeWeaponType.Spear;
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

    public class SiriusSlash : BaseSwingProjectileV2
    {
        public override void DefineCombo()
        {
            base.DefineCombo();
            SwingV2Helper.AddSpearSwingStyle(this);
            Trailer = TrailPresets.Sirius;
            useAfterImage = true;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            SoundStyle spearHit = SoundRegistry.SpearHit1;
            spearHit.PitchVariance = 0.5f;
            SoundEngine.PlaySound(spearHit, Projectile.position);
            if (ComboIndex == 5)
            {
                modifiers.FinalDamage *= 2;
            }
        }
    }


    public class SiriusProj : ModProjectile
    {
        const float Exploding_Time = 90;
        enum ActionState
        {
            Throwing,
            Lodged_In_Tile,
            Lodged_In_NPC
        }

        private int _targetNpc = -1;
        private Vector2 _targetOffset;

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

        float ExplodingTimer;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.localNPCHitCooldown = 45;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            switch (State)
            {
                case ActionState.Throwing:
                    Projectile.velocity.Y += 0.1f;
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
                    if (Main.rand.NextBool(4))
                    {
                        Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlyphDust>(), newColor: Color.Blue, Scale: Main.rand.NextFloat(0.5f, 1f));
                    }
                    break;
                case ActionState.Lodged_In_NPC:
                    StickToTarget();
                    Exploding();
                    break;
                case ActionState.Lodged_In_Tile:
                    Projectile.velocity = Vector2.Zero;
                    Exploding();
                    break;
            }

        }

        private void StickToTarget()
        {
            NPC target = Main.npc[_targetNpc];
            if (!target.active)
            {
                Projectile.Kill();
            }

            Vector2 targetPos = target.position - _targetOffset;
            Vector2 directionToTarget = Projectile.position.DirectionTo(targetPos);
            float dist = Vector2.Distance(Projectile.position, targetPos);
            Projectile.velocity = (directionToTarget * dist) + new Vector2(0.001f, 0.001f);
        }

        private void ChargeVisuals(float timer, float maxTimer)
        {
            float progress = timer / maxTimer;
            float minParticleSpawnSpeed = 24;
            float maxParticleSpawnSpeed = 12;
            int particleSpawnSpeed = (int)MathHelper.Lerp(minParticleSpawnSpeed, maxParticleSpawnSpeed, progress);
            if (timer % particleSpawnSpeed == 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 pos = Projectile.Center + Main.rand.NextVector2CircularEdge(168, 168);
                    Vector2 vel = (Projectile.Center - pos).SafeNormalize(Vector2.Zero) * 5;
                    var d = Dust.NewDustPerfect(pos, ModContent.DustType<GlowDust>(), vel, newColor: Color.White, Scale: 0.5f);
                    d.noGravity = true;
                }
            }
        }

        private void Exploding()
        {
            ChargeVisuals(ExplodingTimer, Exploding_Time);

            if (ExplodingTimer == 1)
            {
                //Play charging up sound
                SoundStyle summonSoundStyle = new("Stellamod/Assets/Sounds/RisingSummon");
                SoundEngine.PlaySound(summonSoundStyle, Projectile.position);
            }

            ExplodingTimer++;
            if (ExplodingTimer >= Exploding_Time)
            {
                SoundStyle summonSoundStyle = new("Stellamod/Assets/Sounds/RisingSummon");
                SoundEngine.FindActiveSound(summonSoundStyle)?.Stop();

                switch (Main.rand.Next(2))
                {
                    case 0:
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/M38F30Bomb1"), Projectile.position);
                        break;
                    case 1:
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/M38F30Bomb2"), Projectile.position);
                        break;
                }

                for (int i = 0; i < 14; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.LightSkyBlue, 1f).noGravity = true;
                }

                Main.LocalPlayer.GetModPlayer<ShakePlayer>().ShakeAtPosition(Projectile.Center, 1024f, 32f);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                    ModContent.ProjectileType<SiriusBoom>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner);
                //KABOOM
                Projectile.Kill();
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (_targetNpc == -1)
            {
                _targetNpc = target.whoAmI;
                _targetOffset = target.position - Projectile.position + new Vector2(0.001f, 0.001f);
                State = ActionState.Lodged_In_NPC;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            State = ActionState.Lodged_In_Tile;
            Projectile.position += oldVelocity;
            return false;
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 0.5f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public static Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(ColorFunctions.Niivin, Color.Transparent, completionRatio);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            this.DrawCentered(ref lightColor);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            string glowTexture = Texture + "_White";
            Texture2D whiteTexture = ModContent.Request<Texture2D>(glowTexture).Value;

            Vector2 textureSize = new(70, 74);
            Vector2 drawOrigin = textureSize / 2;

            //Lerping
            float progress = ExplodingTimer / Exploding_Time;
            Color drawColor = Color.Lerp(Color.Transparent, Color.White, progress);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Main.spriteBatch.Draw(whiteTexture, drawPosition, null, drawColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
        }
    }

    public class SiriusBoom : ModProjectile
    {
        private int _frameCounter;
        private int _frameTick;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 30;
        }

        public override void SetDefaults()
        {
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.width = 129;
            Projectile.height = 129;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;
            Projectile.scale = 1f;
            Projectile.tileCollide = false;
        }

        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void AI()
        {
            Timer++;
            if(Timer == 1)
            {
                FXUtil.GlowCircleBoom(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.Yellow,
                    outerGlowColor: Color.Blue, duration: 25, baseSize: 0.24f);

                for (float i = 0; i < 8; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(360);
                    rot += Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 offset = rot.ToRotationVector2() * 24;
                    var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                        innerColor: Color.White,
                        glowColor: Color.Yellow,
                        outerGlowColor: Color.Blue,
                        baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                        duration: Main.rand.NextFloat(15, 25));
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                }
            }
            Vector3 RGB = new(0.89f, 2.53f, 2.55f);
            // The multiplication here wasn't doing anything
            Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);
        }


        public override bool PreAI()
        {
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


        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 50f);
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            float width = 129;
            float height = 129;
            Vector2 origin = new(width / 2, height / 2);
            int frameSpeed = 1;
            int frameCount = 30;
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Draw(texture, drawPosition,
                texture.AnimationFrame(ref _frameCounter, ref _frameTick, frameSpeed, frameCount, false),
                (Color)GetAlpha(lightColor), 0f, origin, 3f, SpriteEffects.None, 0f);
            return false;
        }
    }
}