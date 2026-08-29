using Stellamod.Common;
using Stellamod.Core.Bases;
using Stellamod.Core.Palettes;
using Stellamod.Core.SwingSystem;
using Stellamod.Dusts;
using Stellamod.Trailing;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Terror.WeaponsTR
{
    public class AssassinsSlash : BaseSwingItemV2
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 18;
            Item.shoot = ModContent.ProjectileType<AssassinsSlashSlash>();
            staminaProjectileShoot = ModContent.ProjectileType<AssassinsSlashStaminaSlash>();
            meleeWeaponType = MeleeWeaponType.Sword;
        }
    }

    public class AssassinsSlashSlash : BaseSwingProjectileV2
    {
        public override void DefineCombo()
        {
            base.DefineCombo();
            useAfterImage = true;
            SwingV2Helper.AddSwordSwingStyle(this);
            Trailer = TrailPresets.Assassin;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
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

    public class AssassinsSlashStaminaSlash : BaseSwingProjectileV2
    {
        private bool _hit;
        public override void DefineCombo()
        {
            base.DefineCombo();
            SoundStyle swingSound1 = SoundRegistry.HeavySwordSlash1;
            swingSound1.PitchVariance = 0.5f;
            Trailer = TrailPresets.Assassin;

            Add(new OvalSwing
            {
                Duration = 48,
                XSwingRadius = 160 / 1.5f,
                YSwingRadius = 24,
                SwingDegrees = 315,
                Easing = (lerpValue) => Easing.InOutExpo(lerpValue, 10),
                Sound = swingSound1,
            });
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (!_hit)
            {
                Vector2 swingDirection = Projectile.velocity.SafeNormalize(Vector2.Zero);
                Owner.velocity -= swingDirection * 2;
                FXUtil.ShakeCamera(target.Center, 1024, 8f);
                _hit = true;
            }
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero,
                ModContent.ProjectileType<Assassinate>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai1: target.whoAmI);
        }


        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            SoundStyle spearHit2 = SoundRegistry.NSwordHit1;
            spearHit2.PitchVariance = 0.2f;
            SoundEngine.PlaySound(spearHit2, Projectile.position);

            modifiers.FinalDamage *= 3;
            modifiers.Knockback *= 4;

        }
    }

    public class Assassinate : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private int NPC => (int)Projectile.ai[1];
        private ref float SlashCount => ref Projectile.ai[2];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.friendly = false;
        }

        public override void AI()
        {
            base.AI();
            NPC myNpc = Main.npc[NPC];
            if (!myNpc.active)
            {
                Projectile.Kill();
            }

            Timer++;
            if (Timer == 1)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), myNpc.Center, Vector2.Zero,
      ModContent.ProjectileType<AssassinsSpawnEffect>(), Projectile.damage * 2, 1, Projectile.owner, 0, 0);
            }
            if (Timer <= 10)
            {
                SpecialEffectsPlayer player = Main.LocalPlayer.GetModPlayer<SpecialEffectsPlayer>();
                player.blackWhiteStrength = 0.66f;
                player.blackWhiteThreshold = 0.5f;
            }
            if (Timer >= 20)
            {
                SpecialEffectsPlayer player = Main.LocalPlayer.GetModPlayer<SpecialEffectsPlayer>();
                player.blackWhiteStrength = 1f;
                player.blackWhiteThreshold = 0.5f;
            }
            if (Timer == 25)
            {
                for (int i = 0; i < 7; i++)
                {
                    Dust.NewDustPerfect(myNpc.Center, ModContent.DustType<SmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, default, 1f).noGravity = true;
                }

                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), myNpc.Center, Vector2.Zero,
                    ModContent.ProjectileType<AssassinsSpawnEffect>(), Projectile.damage * 2, 1, Projectile.owner, 0, 0);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), myNpc.Center, Vector2.Zero,
                        ModContent.ProjectileType<AssassinsSlashProj>(), 0, 1, Projectile.owner, 0, 0);
                    SlashCount++;
                    if (SlashCount >= 10)
                    {
                        Projectile.Kill();
                    }
                }
            }
            if (Timer >= 25)
            {
                Timer = 20;
            }
        }
    }

    public class AssassinsSlashProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            Projectile.width = 400;
            Projectile.height = 400;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 110;
            Projectile.timeLeft = 900;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override bool PreAI()
        {
            Projectile.ai[0]++;
            Projectile.alpha -= 40;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;
            if (Projectile.ai[0] <= 1)
            {
                int Sound = Main.rand.Next(1, 5);
                if (Sound == 1)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/AssassinsSlash"), Projectile.position);
                }
                if (Sound == 2)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/AssassinsSlashProj2"), Projectile.position);
                }
                if (Sound == 3)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/AssassinsSlashProj3"), Projectile.position);
                }
                if (Sound == 4)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/AssassinsSlashProj4"), Projectile.position);
                }

                Main.LocalPlayer.GetModPlayer<ShakePlayer>().ShakeAtPosition(Projectile.Center, 512f, 8);

                Projectile.rotation = Main.rand.Next(0, 360);
            }
            Projectile.spriteDirection = Projectile.direction;
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 2)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame >= 7)
                {
                    Projectile.active = false;
                }
            }
            return true;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.Dirt, 0, 60, 133);
            }
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;
    }

    public class AssassinsSpawnEffect : ModProjectile
    {
        public override string Texture => TextureRegistry.EmptyTexture;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 7;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public float Rot;
        public override void SetDefaults()
        {
            Projectile.aiStyle = 0;
            Projectile.alpha = 255;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 10;
            Projectile.timeLeft = 50;
            Projectile.height = 50;
            Projectile.width = 50;
            Projectile.extraUpdates = 1;
        }

        private float alphaCounter = 5;
        public override void AI()
        {
            Projectile.ai[0]++;
            if (Projectile.ai[0] == 1)
            {
                Rot = Main.rand.NextFloat(0.1f, 0.4f);
                Projectile.rotation += Rot;
            }
            alphaCounter -= 0.18f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D4 = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Extra_63").Value;
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(85f * alphaCounter), (int)(05f * alphaCounter), (int)(05f * alphaCounter), 0), Projectile.rotation, new Vector2(256, 256), 0.2f * (alphaCounter + 0.2f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(55f * alphaCounter), (int)(05f * alphaCounter), (int)(05f * alphaCounter), 0), Projectile.rotation, new Vector2(256, 256), 0.4f * (alphaCounter + 0.2f), SpriteEffects.None, 0f);
            return true;
        }
    }
}