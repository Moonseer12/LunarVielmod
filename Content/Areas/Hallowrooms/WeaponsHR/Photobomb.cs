using Stellamod.Common.MagicCauldron;
using Stellamod.Content.Areas.Hallowrooms.ArmorHR;
using Stellamod.Content.CommonMaterials;
using Stellamod.Dusts;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Hallowrooms.WeaponsHR
{

    public class Photobomb : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 9;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 32;
            Item.height = 25;
            Item.useTime = 98;
            Item.useAnimation = 98;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 10;
            Item.rare = ItemRarityID.LightPurple;
            Item.autoReuse = false;
            Item.shootSpeed = 30f;
            Item.shoot = ModContent.ProjectileType<PhotobombProj>();
            Item.scale = 0.8f;
            Item.noMelee = true; // The projectile will do the damage and not the item
            Item.value = Item.buyPrice(silver: 7);
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.noMelee = true;

        }


        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<KaleidoscopicInk, BlankGun>();
        }
    }

    public class PhotobombProj : ModProjectile
    {
        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        private ref float SwordRotation => ref Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.damage = 0;
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.aiStyle = 595;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
            Projectile.timeLeft = 101;
        }

        public override bool? CanDamage()
        {
            return false;
        }


        public float beens = 0;

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < Main.rand.Next(1, 3); i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(16, 16);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<SplashProj>(), 0, 0, Projectile.owner);
            }
        }

        public override void AI()
        {
            Timer++;
            beens++;
            if (Timer > 155)
            {
                // Our timer has finished, do something here:
                // Main.PlaySound, Dust.NewDust, Projectile.NewProjectile, etc. Up to you.		
                SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/MorrowSalfi"));
                Timer = 0;
            }

            Player player = Main.player[Projectile.owner];
            if (player.noItems || player.CCed || player.dead || !player.active)
                Projectile.Kill();

            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);
            if (Main.myPlayer == Projectile.owner)
            {
                player.ChangeDir(Projectile.direction);
                SwordRotation = (Main.MouseWorld - player.Center).ToRotation();
                Projectile.netUpdate = true;
                if (!player.channel)
                    Projectile.Kill();
            }

            Projectile.velocity = SwordRotation.ToRotationVector2();
            Projectile.spriteDirection = player.direction;
            if (Projectile.spriteDirection == 1)
                Projectile.rotation = Projectile.velocity.ToRotation();
            else
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;


            if (beens == 5)
            {
                float speedX = Projectile.velocity.X * 10;
                float speedY = Projectile.velocity.Y * 7;

                Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, AmmoID.Bullet), Projectile.Center, Projectile.velocity * 12f, ProjectileID.PainterPaintball, Projectile.damage * 1, Projectile.knockBack, Projectile.owner);

                beens = 0;
            }


            if (Timer == 20)
            {
                float speedX = Projectile.velocity.X * 10;
                float speedY = Projectile.velocity.Y * 7;

                Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, AmmoID.Bullet), Projectile.Center, Projectile.velocity * 12f,
                    ModContent.ProjectileType<PhotobombShot>(), Projectile.damage * 1, Projectile.knockBack, Projectile.owner);

                ShakeScreenPosition.Shake = 4;
                SoundEngine.PlaySound(SoundID.DD2_LightningBugZap, Projectile.Center);
            }

            if (Timer == 60)
            {
                ShakeScreenPosition.Shake = 4;
                Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, AmmoID.Bullet), Projectile.Center, Projectile.velocity * 10f,
                    ModContent.ProjectileType<PhotobombShot>(), Projectile.damage * 1, Projectile.knockBack, Projectile.owner);
                SoundEngine.PlaySound(SoundID.DD2_LightningBugZap, Projectile.Center);
            }

            if (Timer == 100)
            {
                ShakeScreenPosition.Shake = 4;
                Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, AmmoID.Bullet), Projectile.Center, Projectile.velocity * 10f,
                    ModContent.ProjectileType<PhotobombShot>(), Projectile.damage * 1, Projectile.knockBack, Projectile.owner);
                SoundEngine.PlaySound(SoundID.DD2_LightningBugZap, Projectile.Center);
            }

            Projectile.Center = playerCenter + Projectile.velocity * 1f;// customization of the hitbox position

            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);

            if (++Projectile.frameCounter >= 1)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 1)
                {
                    Projectile.frame = 0;
                }
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
            origin.X = Projectile.spriteDirection == 1 ? sourceRectangle.Width - 30 : 30; // Customization of the sprite position

            Color drawColor = Projectile.GetAlpha(lightColor);
            Main.EntitySpriteDraw((Texture2D)TextureAssets.Projectile[Projectile.type], Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }

    public class PhotobombShot : ModProjectile
    {
        public override string Texture => TextureRegistry.EmptyTexture;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Bullet);
            AIType = ProjectileID.Bullet;
            Projectile.penetrate = 1;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(2))
                target.AddBuff(BuffID.Confused, 180);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
                Projectile.Kill();
            else
            {
                if (Projectile.velocity.X != oldVelocity.X)
                    Projectile.velocity.X = -oldVelocity.X;

                if (Projectile.velocity.Y != oldVelocity.Y)
                    Projectile.velocity.Y = -oldVelocity.Y;
            }

            SoundEngine.PlaySound(SoundID.DD2_LightningBugZap, Projectile.Center);
            for (int i = 0; i < 25; i++)
            {
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob2>(), (Vector2.One * Main.rand.Next(1, 8)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
                Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob3>(), speed * 2, 0, default(Color), 4f).noGravity = false;
            }

            for (int i = 0; i < 7; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<PaintBlob1>());
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob5>(), (Vector2.One * Main.rand.Next(1, 8)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
            }
            return false;
        }

        public override bool PreAI()
        {

            Player player = Main.player[Projectile.owner];
            if (Main.rand.NextBool(8))
            {
                Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob3>(), speed * 2, 0, default(Color), 4f).noGravity = false;

            }

            if (Main.rand.NextBool(8))
            {

                Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob5>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;

            }

            if (Main.rand.NextBool(8))
            {
                Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob4>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;

            }


            if (Main.rand.NextBool(9))
            {
                float speedXa = Main.rand.NextFloat(-60f, 60f);
                float speedYa = Main.rand.Next(-60, 60);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb1>(), (Projectile.damage * 2) + player.GetModPlayer<ArtisanPlayer>().PPPaintDMG2, 1, Projectile.owner, 0, 0);
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob3>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
            }

            if (Main.rand.NextBool(10))
            {
                float speedXa = Main.rand.NextFloat(-60f, 60f);
                float speedYa = Main.rand.Next(-60, 60);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb2>(), Projectile.damage + player.GetModPlayer<ArtisanPlayer>().PPPaintDMG2, 1, Projectile.owner, 0, 0);
            }

            if (Main.rand.NextBool(20))
            {
                float speedXa = Main.rand.NextFloat(-60f, 60f);
                float speedYa = Main.rand.Next(-60, 60);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb3>(), (Projectile.damage * 3) + player.GetModPlayer<ArtisanPlayer>().PPPaintDMG2, 1, Projectile.owner, 0, 0);
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob2>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
            }


            if (Main.rand.NextBool(20))
            {
                float speedXa = Main.rand.NextFloat(-60f, 60f);
                float speedYa = Main.rand.Next(-60, 60);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb5>(), (Projectile.damage * 3) + player.GetModPlayer<ArtisanPlayer>().PPPaintDMG2, 1, Projectile.owner, 0, 0);
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob2>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
            }

            if (Main.rand.NextBool(8))
            {
                float speedXa = Main.rand.NextFloat(-60f, 60f);
                float speedYa = Main.rand.Next(-60, 60);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb4>(), (Projectile.damage * 3) + player.GetModPlayer<ArtisanPlayer>().PPPaintDMG2, 1, Projectile.owner, 0, 0);
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob2>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
            }

            if (Main.rand.NextBool(12))
            {
                float speedXa = Main.rand.NextFloat(-60f, 60f);
                float speedYa = Main.rand.Next(-60, 60);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb6>(), (Projectile.damage * 3) + player.GetModPlayer<ArtisanPlayer>().PPPaintDMG2, 1, Projectile.owner, 0, 0);
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob2>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
            }

            if (player.GetModPlayer<ArtisanPlayer>().PPPaintI)
            {
                if (Main.rand.NextBool(10))
                {
                    float speedXa = Main.rand.NextFloat(-60f, 60f);
                    float speedYa = Main.rand.Next(-60, 60);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb7>(), (Projectile.damage * 4) + player.GetModPlayer<ArtisanPlayer>().PPPaintDMG2, 1, Projectile.owner, 0, 0);
                    Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob5>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
                }
            }

            if (player.GetModPlayer<ArtisanPlayer>().PPPaintII)
            {
                if (Main.rand.NextBool(20))
                {
                    float speedXa = Main.rand.NextFloat(-35f, 35f);
                    float speedYa = Main.rand.Next(-35, 35);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb8>(), (Projectile.damage * 3) + player.GetModPlayer<ArtisanPlayer>().PPPaintDMG2, 1, Projectile.owner, 0, 0);
                    Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob1>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
                }
            }


            return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Main.rand.NextBool(5))
            {
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<PaintBlob2>(), 0f, 0f, 150, Color.White, 1f);
                Main.dust[dustnumber].velocity *= 0.3f;
                Main.dust[dustnumber].noGravity = true;
            }

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Lighting.AddLight(Projectile.Center, Color.Orange.ToVector3() * 1.75f * Main.essScale);
            if (Main.rand.NextBool(5))
            {
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<PaintBlob3>(), 0f, 0f, 150, Color.White, 1f);
                Main.dust[dustnumber].velocity *= 0.3f;
            }
        }

        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            SoundEngine.PlaySound(SoundID.DD2_LightningBugZap, Projectile.Center);
            for (int i = 0; i < 15; i++)
            {
                Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);

                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob1>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default, 4f).noGravity = false;
            }


            if (Main.rand.NextBool(2))
            {
                float speedXa = Main.rand.NextFloat(-80f, 80f);
                float speedYa = Main.rand.Next(-80, 80);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb1>(), (Projectile.damage * 2) + player.GetModPlayer<ArtisanPlayer>().PPPaintDMG2, 1, Projectile.owner, 0, 0);
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob3>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default, 4f).noGravity = false;
            }

            if (Main.rand.NextBool(1))
            {
                float speedXa = Main.rand.NextFloat(-80f, 80f);
                float speedYa = Main.rand.Next(-80, 80);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb2>(), Projectile.damage + player.GetModPlayer<ArtisanPlayer>().PPPaintDMG2, 1, Projectile.owner, 0, 0);
            }

            if (Main.rand.NextBool(4))
            {
                float speedXa = Main.rand.NextFloat(-80f, 80f);
                float speedYa = Main.rand.Next(-80, 80);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb3>(), (Projectile.damage * 3) + player.GetModPlayer<ArtisanPlayer>().PPPaintDMG2, 1, Projectile.owner, 0, 0);
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob2>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
            }

            if (player.GetModPlayer<ArtisanPlayer>().PPPaintI)
            {
                if (Main.rand.NextBool(4))
                {
                    float speedXa = Main.rand.NextFloat(-80f, 80f);
                    float speedYa = Main.rand.Next(-80, 80);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb7>(), (Projectile.damage * 4) + player.GetModPlayer<ArtisanPlayer>().PPPaintDMG2, 1, Projectile.owner, 0, 0);
                    Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob5>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
                }
            }

            if (player.GetModPlayer<ArtisanPlayer>().PPPaintII)
            {
                if (Main.rand.NextBool(7))
                {
                    float speedXa = Main.rand.NextFloat(-35f, 35f);
                    float speedYa = Main.rand.Next(-35, 35);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb8>(), (Projectile.damage * 3) + player.GetModPlayer<ArtisanPlayer>().PPPaintDMG2, 1, Projectile.owner, 0, 0);
                    Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob1>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
                }
            }
        }
    }
}