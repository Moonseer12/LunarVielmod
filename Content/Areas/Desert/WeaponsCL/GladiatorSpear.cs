using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.SwingSystem;
using Stellamod.Trailing;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Desert.WeaponsCL
{
    public class GladiatorSpear : BaseSwingItemV2
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 14;
            Item.shoot = ModContent.ProjectileType<GladiatorSpearSlash>();
            staminaProjectileShoot = ModContent.ProjectileType<GladiatorSpearStaminaSlash>();
            meleeWeaponType = MeleeWeaponType.Spear;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-3f, -2f);
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<GintzlMetal, BlankSword>();
        }
    }

    public class GladiatorSpearSlash : BaseSwingProjectileV2
    {
        public override void DefineCombo()
        {
            base.DefineCombo();
            SwingV2Helper.AddSpearSwingStyle(this);
            Trailer = TrailPresets.GladiatorSpear;
            useAfterImage = true;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            SoundStyle spearHit = SoundRegistry.SpearHit1;
            spearHit.PitchVariance = 0.5f;
            SoundEngine.PlaySound(spearHit, Projectile.position);
            if (IsFinishingSwing())
            {
                DamageHelper.PercentIncreasedamage(ref modifiers, 1f);
            }
        }
    }

    public class GladiatorSpearStaminaSlash : BaseSwingProjectileV2
    {
        private bool _fire;
        public override void DefineCombo()
        {
            base.DefineCombo();
            SoundStyle swingSound1 = SoundRegistry.HeavySwordSlash1;
            swingSound1.PitchVariance = 0.5f;

            Trailer = TrailPresets.LightSpand;
            Add(new OvalSwing
            {
                Duration = 44,
                XSwingRadius = 160 / 1.5f,
                YSwingRadius = 80 / 1.5f,
                SwingDegrees = 270,
                Easing = (lerpValue) => Easing.InOutExpo(lerpValue, 10),
                Sound = swingSound1
            });
        }

        public override void AI()
        {
            base.AI();

            Vector2 swingDirection = Projectile.velocity.SafeNormalize(Vector2.Zero);
            if (Interpolant > 0.5f && !_fire)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    Vector2 shootVelocity = Projectile.velocity;

                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center + new Vector2(0, -4), Projectile.velocity,
                        ModContent.ProjectileType<GladiatorSpearProg>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner);
                }
                _fire = true;
            }
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

    public class GladiatorSpearProg : ModProjectile
    {
        bool Moved;
        Vector2 StartVelocity;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Gladiator Spear");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 2;
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.timeLeft = 700;
            Projectile.alpha = 255;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        public override void AI()
        {
            Projectile.ai[1]++;
            if (!Moved && Projectile.ai[1] >= 0)
            {
                Moved = true;
            }
            if (Projectile.ai[1] == 1)
            {

                StartVelocity = Projectile.velocity;
            }
            if (Projectile.ai[1] == 2)
            {


                Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f + 3.14f;
                Projectile.velocity = -Projectile.velocity;
            }
            if (Projectile.ai[1] == 5 || Projectile.ai[1] == 10 || Projectile.ai[1] == 15 || Projectile.ai[1] == 20)
            {
                var EntitySource = Projectile.GetSource_FromThis();
                if (Main.rand.NextBool(8))
                {
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Projectile.NewProjectile(EntitySource, Projectile.Center.X, Projectile.Center.Y, StartVelocity.X, StartVelocity.Y, ModContent.ProjectileType<GladiatorSpearMirageProgRed>(), Projectile.damage * 2, 1, Projectile.owner, 0, 0);
                    }

                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GladiatorMirageRed"), Projectile.position);
                }
                else
                {
                    int Sound = Main.rand.Next(1, 3);
                    if (Sound == 1)
                    {
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GladiatorMirage1"), Projectile.position);
                    }
                    else
                    {
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GladiatorMirage2"), Projectile.position);
                    }
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Projectile.NewProjectile(EntitySource, Projectile.Center.X, Projectile.Center.Y, StartVelocity.X, StartVelocity.Y, ModContent.ProjectileType<GladiatorSpearMirageProg>(), Projectile.damage, 1, Projectile.owner, 0, 0);
                    }
                }
            }
            if (Projectile.ai[1] >= 0 && Projectile.ai[1] <= 20)
            {
                Projectile.velocity *= .86f;

            }
            if (Projectile.ai[1] == 20)
            {
                Projectile.velocity = -Projectile.velocity;
            }
            if (Projectile.ai[1] >= 21 && Projectile.ai[1] <= 60)
            {
                Projectile.velocity /= .90f;

            }
            if (Projectile.ai[1] == 60)
            {
                Projectile.tileCollide = true;
            }




        }
        public override void OnKill(int timeLeft)
        {
            FXUtil.GlowCircleBoom(Projectile.Center,
                innerColor: Color.White,
                glowColor: Color.Yellow,
                outerGlowColor: Color.Red, duration: 25, baseSize: 0.06f);

            for (float i = 0; i < 2; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                rot += Main.rand.NextFloat(-0.5f, 0.5f);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.Yellow,
                    outerGlowColor: Color.Red,
                    baseSize: Main.rand.NextFloat(0.05f, 0.1f),
                    duration: Main.rand.NextFloat(15, 25));
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }

            for (int i = 0; i < 20; i++)
            {
                int num1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Gold, 0f, -2f, 0, default, .8f);
                Main.dust[num1].noGravity = true;
                Main.dust[num1].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[num1].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                if (Main.dust[num1].position != Projectile.Center)
                    Main.dust[num1].velocity = Projectile.DirectionTo(Main.dust[num1].position) * 6f;
                int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Gold, 0f, -2f, 0, default, .8f);
                Main.dust[num].noGravity = true;
                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                if (Main.dust[num].position != Projectile.Center)
                    Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 6f;
            }
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Main.rand.NextBool(5))
            {
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Gold, 0f, 0f, 150, Color.Gold, 1f);
                Main.dust[dustnumber].velocity *= 0.3f;
                Main.dust[dustnumber].noGravity = true;
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Main.instance.LoadProjectile(Projectile.type);
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            // Redraw the projectile with the color not influenced by light
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Gold) * (float)(((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length) / 2);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return true;
        }
        public override void PostDraw(Color lightColor)
        {
            Lighting.AddLight(Projectile.Center, Color.PaleGoldenrod.ToVector3() * 1.75f * Main.essScale);

        }
    }

    public class GladiatorSpearMirageProg : ModProjectile
    {
        bool Moved;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Gladiator Spear");
            ProjectileID.Sets.TrailingMode[Type] = 1;
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.width = 25;
            Projectile.height = 25;
            Projectile.timeLeft = 700;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        public override void AI()
        {
            if (Projectile.alpha >= 10)
            {
                Projectile.alpha -= 10;
            }

            Projectile.ai[1]++;
            if (!Moved && Projectile.ai[1] >= 0)
            {
                Moved = true;
            }
            if (Projectile.ai[1] == 1)
            {
                Projectile.alpha = 255;
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.position.X = Main.rand.NextFloat(Projectile.position.X - 120, Projectile.position.X + 120);
                    Projectile.position.Y = Main.rand.NextFloat(Projectile.position.Y - 120, Projectile.position.Y + 120);
                    Projectile.netUpdate = true;
                }

            }
            if (Projectile.ai[1] == 2)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f + 3.14f;
                Projectile.velocity = -Projectile.velocity;
            }
            if (Projectile.ai[1] >= 0 && Projectile.ai[1] <= 20)
            {
                Projectile.velocity *= .86f;

            }
            if (Projectile.ai[1] == 60)
            {
                Projectile.tileCollide = true;
            }
            if (Projectile.ai[1] == 20)
            {
                Projectile.velocity = -Projectile.velocity;
            }
            if (Projectile.ai[1] >= 21 && Projectile.ai[1] <= 60)
            {
                Projectile.velocity /= .90f;

            }




        }
        public override void OnKill(int timeLeft)
        {
            FXUtil.GlowCircleBoom(Projectile.Center,
                innerColor: Color.White,
                glowColor: Color.Yellow,
                outerGlowColor: Color.Red, duration: 25, baseSize: 0.06f);

            for (float i = 0; i < 2; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                rot += Main.rand.NextFloat(-0.5f, 0.5f);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.Yellow,
                    outerGlowColor: Color.Red,
                    baseSize: Main.rand.NextFloat(0.05f, 0.1f),
                    duration: Main.rand.NextFloat(15, 25));
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Lerp(new Color(253, 255, 71), new Color(182, 83, 38), 1f / Projectile.oldPos.Length * k) * (1f - 1f / Projectile.oldPos.Length * k));
                Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return true;
        }

        public override void PostDraw(Color lightColor)
        {
            Lighting.AddLight(Projectile.Center, Color.PaleGoldenrod.ToVector3() * 1.75f * Main.essScale);

        }
    }

    public class GladiatorSpearMirageProgRed : ModProjectile
    {
        bool Moved;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Gladiator Spear");
            ProjectileID.Sets.TrailingMode[Type] = 1;
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 5;
            Projectile.width = 25;
            Projectile.height = 25;
            Projectile.timeLeft = 700;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        public override void AI()
        {
            if (Projectile.alpha >= 50)
            {
                Projectile.alpha -= 10;
            }

            Projectile.ai[1]++;
            if (!Moved && Projectile.ai[1] >= 0)
            {
                Moved = true;
            }
            if (Projectile.ai[1] == 1)
            {
                Projectile.alpha = 255;
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.position.X = Main.rand.NextFloat(Projectile.position.X - 150, Projectile.position.X + 150);
                    Projectile.position.Y = Main.rand.NextFloat(Projectile.position.Y - 150, Projectile.position.Y + 150);
                    Projectile.netUpdate = true;
                }

            }
            if (Projectile.ai[1] == 2)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f + 3.14f;
                Projectile.velocity = -Projectile.velocity;
            }
            if (Projectile.ai[1] >= 0 && Projectile.ai[1] <= 20)
            {
                Projectile.velocity *= .86f;

            }
            if (Projectile.ai[1] == 20)
            {
                Projectile.velocity = -Projectile.velocity;
            }
            if (Projectile.ai[1] >= 21 && Projectile.ai[1] <= 60)
            {
                Projectile.velocity /= .90f;

            }
            if (Projectile.ai[1] == 60)
            {
                Projectile.tileCollide = true;
            }




        }
        public override void OnKill(int timeLeft)
        {
            FXUtil.GlowCircleBoom(Projectile.Center,
                innerColor: Color.White,
                glowColor: Color.Yellow,
                outerGlowColor: Color.Red, duration: 25, baseSize: 0.06f);

            for (float i = 0; i < 2; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                rot += Main.rand.NextFloat(-0.5f, 0.5f);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.Yellow,
                    outerGlowColor: Color.Red,
                    baseSize: Main.rand.NextFloat(0.05f, 0.1f),
                    duration: Main.rand.NextFloat(15, 25));
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Lerp(new Color(255, 8, 55), new Color(99, 39, 51), 1f / Projectile.oldPos.Length * k) * (1f - 1f / Projectile.oldPos.Length * k));
                Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return true;
        }
        public override void PostDraw(Color lightColor)
        {
            Lighting.AddLight(Projectile.Center, Color.IndianRed.ToVector3() * 1.75f * Main.essScale);

        }
    }
}