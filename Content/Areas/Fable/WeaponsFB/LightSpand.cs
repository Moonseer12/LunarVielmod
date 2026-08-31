using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.SwingSystem;
using Stellamod.Trailing;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Fable.WeaponsFB
{
    public class LightSpand : BaseSwingItemV2
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.shoot = ModContent.ProjectileType<LightSpandSlash>();
            staminaProjectileShoot = ModContent.ProjectileType<LightSpandStaminaSlash>();
            meleeWeaponType = MeleeWeaponType.Sword;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<AlcadizScrap, BlankSword>();
        }
    }

    public class LightSpandSlash : BaseSwingProjectileV2
    {
        public override void DefineCombo()
        {
            base.DefineCombo();
            SwingV2Helper.AddSwordSwingStyle(this);
            Trailer = TrailPresets.LightSpand;
            useAfterImage = true;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            SoundStyle spearHit = SoundRegistry.SpearHit1;
            spearHit.PitchVariance = 0.5f;
            SoundEngine.PlaySound(spearHit, Projectile.position);
            target.AddBuff(BuffID.OnFire, 120);
            if (ComboIndex == 5)
            {
                modifiers.FinalDamage *= 2;
            }
        }
    }

    public class LightSpandStaminaSlash : BaseSwingProjectileV2
    {
        private float _projTimer;
        public override void DefineCombo()
        {
            base.DefineCombo();
            SoundStyle swingSound1 = SoundRegistry.HeavySwordSlash1;
            swingSound1.PitchVariance = 0.5f;

            Add(new OvalSwing
            {
                Duration = 68,
                XSwingRadius = 160 / 1.5f,
                YSwingRadius = 80 / 1.5f,
                SwingDegrees = 270,
                Easing = (lerpValue) => Easing.InOutExpo(lerpValue, 10),
                Sound = swingSound1,
            });
            Trailer = TrailPresets.LightSpand;
            useAfterImage = true;
        }

        public override void AI()
        {
            base.AI();
            if (Interpolant > 0.4f && Interpolant < 0.6f)
            {
                _projTimer++;
                if (_projTimer % 25 == 0 && Main.myPlayer == Projectile.owner)
                {
                    Vector2 shootVelocity = Projectile.velocity;
                    shootVelocity = shootVelocity.RotatedByRandom(MathHelper.ToRadians(15));
                    shootVelocity *= Main.rand.NextFloat(0.66f, 1f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, shootVelocity,
                        ModContent.ProjectileType<LightSpandProg>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }
        }


        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);

            SoundStyle spearHit = SoundRegistry.CrystalHit1;
            spearHit.PitchVariance = 0.5f;
            SoundEngine.PlaySound(spearHit, Projectile.position);

            SoundStyle spearHit2 = SoundRegistry.NSwordHit1;
            spearHit2.PitchVariance = 0.2f;
            SoundEngine.PlaySound(spearHit2, Projectile.position);

            modifiers.FinalDamage *= 3;
            modifiers.Knockback *= 4;
        }
    }

    public class LightSpandProg : ModProjectile
    {
        private bool Moved;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shadow Hand");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.scale = 1.3f;
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.timeLeft = 250;
            Projectile.alpha = 0;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 3;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(3) && !target.boss)
            {
                target.AddBuff(BuffID.OnFire, 180);
            }
        }

        private float alphaCounter = 0;
        public override void AI()
        {
            Projectile.velocity *= 1.02f;
            Projectile.ai[1]++;
            if (!Moved && Projectile.ai[1] >= 0)
            {
                Projectile.spriteDirection = Projectile.direction;
                Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f + 3.14f;
                Projectile.alpha = 255;
                Moved = true;
            }
            if (Projectile.ai[1] >= 10)
            {
                Projectile.tileCollide = true;
            }
            if (Projectile.ai[1] <= 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SoftSummon2"), Projectile.position);
            }
            if (alphaCounter <= 1)
            {
                alphaCounter += 0.08f;
            }

            Projectile.spriteDirection = Projectile.direction;
            Projectile.rotation += 0.08f;
        }

        public override void OnKill(int timeLeft)
        {
            float speedXa = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
            float speedYa = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;

            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXa, Projectile.position.Y + speedYa, speedXa * 0, speedYa * 0,
                ModContent.ProjectileType<AlcadizBombExplosion>(), (int)(Projectile.damage * 1.5f), 0f, Projectile.owner, 0f, 0f);
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Starexplosion"), Projectile.position);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }


        public override bool PreDraw(ref Color lightColor)
        {

            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D texture2D4 = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/DimLight").Value;
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(85f * alphaCounter), (int)(45f * alphaCounter), (int)(15f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.17f * (7 + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(85f * alphaCounter), (int)(45f * alphaCounter), (int)(15f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.17f * (7 + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(85f * alphaCounter), (int)(45f * alphaCounter), (int)(15f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.07f * (7 + 0.6f), SpriteEffects.None, 0f);
            Lighting.AddLight(Projectile.Center, Color.Yellow.ToVector3() * 1.0f * Main.essScale);
        }
    }
}