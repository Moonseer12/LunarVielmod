using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Jungle.AccJN
{
    public class RadiantPlayer : ModPlayer
    {
        public bool RadiantBomb = false;
        public int RadiantBombCooldown = 0;

        public override void OnHitAnything(float x, float y, Entity victim)
        {
            if (RadiantBomb && RadiantBombCooldown <= 0)
            {
                for (int d = 0; d < 4; d++)
                {
                    float speedXa = Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-1f, 1f);
                    float speedYa = Main.rand.Next(10, 15) * 0.01f + Main.rand.Next(-1, 1);
                    Projectile.NewProjectile(Player.GetSource_OnHit(victim), (int)victim.Center.X, (int)victim.Center.Y, speedXa * 0, speedYa * 0, ModContent.ProjectileType<GoldsSpawnEffect>(), 490, 1f, Player.whoAmI);
                    Projectile.NewProjectile(Player.GetSource_OnHit(victim), (int)victim.Center.X, (int)victim.Center.Y, speedXa * 0.7f, speedYa * 0.6f, ModContent.ProjectileType<GoldsSlashProj>(), 400, 1f, Player.whoAmI);
                    Projectile.NewProjectile(Player.GetSource_OnHit(victim), (int)victim.Center.X, (int)victim.Center.Y, speedXa * 0.5f, speedYa * 0.3f, ModContent.ProjectileType<GoldsSlashProj>(), 405, 1f, Player.whoAmI);
                    Projectile.NewProjectile(Player.GetSource_OnHit(victim), (int)victim.Center.X, (int)victim.Center.Y, speedXa * 1.3f, speedYa * 0.3f, ModContent.ProjectileType<GoldsSlashProj>(), 405, 1f, Player.whoAmI);
                    Projectile.NewProjectile(Player.GetSource_OnHit(victim), (int)victim.Center.X, (int)victim.Center.Y, speedXa * 1f, speedYa * 1.5f, ModContent.ProjectileType<GoldsSlashProj>(), 401, 1f, Player.whoAmI);
                }
                RadiantBombCooldown = 220;
            }
        }

        public override void ResetEffects()
        {
            RadiantBomb = false;
        }
    }

    public class RadiantBomb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 60;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 32;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 106;
            Projectile.height = 106;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
            Projectile.timeLeft = int.MaxValue;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            bool hasSetBonus = player.GetModPlayer<RadiantPlayer>().RadiantBomb;
            if (!hasSetBonus)
            {
                Projectile.Kill();
                return;
            }
            Projectile.rotation += 0.05f;
            if (player.noItems || player.CCed || player.dead || !player.active)
                Projectile.Kill();

            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);
            float swordRotation = 0f;
            if (Main.myPlayer == Projectile.owner)
            {
                player.ChangeDir(Projectile.direction);
                swordRotation = (Main.MouseWorld - player.Center).ToRotation();
            }

            Projectile.velocity = swordRotation.ToRotationVector2();
            Projectile.Center = playerCenter + Projectile.velocity * 1f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override bool PreAI()
        {
            if (++Projectile.frameCounter >= 1)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 60)
                {
                    Projectile.frame = 0;
                }
            }
            return true;
        }
    }

    public class GoldsSpawnEffect : ModProjectile
    {
        public override string Texture => TextureRegistry.EmptyTexture;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 7;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

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
                Projectile.rotation = Main.rand.Next(0, 360);
            }

            alphaCounter -= 0.18f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D4 = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Extra_62").Value;
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(85f * alphaCounter), (int)(75f * alphaCounter), (int)(05f * alphaCounter), 0), Projectile.rotation, new Vector2(37, 37), 0.4f * (alphaCounter + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(55f * alphaCounter), (int)(75f * alphaCounter), (int)(05f * alphaCounter), 0), Projectile.rotation, new Vector2(37, 37), 0.6f * (alphaCounter + 0.6f), SpriteEffects.None, 0f);
            return true;
        }
    }

    public class GoldsSlashProj : ModProjectile
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
                FXUtil.ShakeCamera(Projectile.Center, 512f, 32f);
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
                Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.GoldCoin, 0, 60, 133);
            }
        }
        
        public override Color? GetAlpha(Color lightColor) => Color.White;
    }

    public class ZuiBomb : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<RadiantPlayer>().RadiantBombCooldown--;
            player.GetModPlayer<RadiantPlayer>().RadiantBomb = true;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<RadiantBomb>()] == 0)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<RadiantBomb>(), 10, 4, player.whoAmI);
            }
        }
    }
}