using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Dusts;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital.WeaponsRC
{

    public class Bincle : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 120;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 7;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2;
            Item.autoReuse = false;
            Item.shootSpeed = 30f;
            Item.shoot = ModContent.ProjectileType<BincleProj>();
            Item.scale = 0.8f;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;


        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }


        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<AlcaricMush, BlankStaff>();
        }
    }

    public class BincleProj : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private Player Owner => Main.player[Projectile.owner];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Type] = 24;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = int.MaxValue;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            SpriteBatch spriteBatch = Main.spriteBatch;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Rectangle frame = Projectile.Frame();
            Vector2 drawOrigin = frame.Size() / 2f;
            Color drawColor = Color.White.MultiplyRGB(lightColor);
            drawColor.A = 0;
            float drawRotation = Projectile.rotation;
            float drawScale = MathHelper.Lerp(0f, 1f, Easing.InOutCubic(Timer / 15f));
            spriteBatch.Draw(texture, drawPos, frame, drawColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);
            return false;
        }

        private bool ShouldConsumeMana()
        {
            // Should mana be consumed this frame?
            bool consume = Timer % 6 == 0;
            return consume;
        }

        public override void AI()
        {
            base.AI();
            Timer++;


            if (Main.myPlayer == Projectile.owner)
            {
                bool manaIsAvailable = !ShouldConsumeMana() || Owner.CheckMana(Owner.HeldItem.mana, true, false);
                Projectile.velocity = (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.Zero);
                // The Prism immediately stops functioning if the player is Cursed (player.noItems) or "Crowd Controlled", e.g. the Frozen debuff.
                // player.channel indicates whether the player is still holding down the mouse button to use the item.
                bool stillInUse = Owner.channel && manaIsAvailable && !Owner.noItems && !Owner.CCed;
                if (stillInUse && Timer % 6 == 0)
                {
                    Vector2 spawnPos = Projectile.Center - Projectile.velocity * 65;
                    Vector2 shootVelocity = Projectile.velocity * 12;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPos, shootVelocity,
                        ModContent.ProjectileType<BrincShot>(), (int)(Projectile.damage), Projectile.knockBack, Projectile.owner);
                }
                else if (!stillInUse)
                {
                    Projectile.Kill();
                }


                Projectile.netUpdate = true;
            }

            DrawHelper.AnimateTopToBottom(Projectile, 2);
            Owner.ChangeDir(Projectile.direction);
            Projectile.spriteDirection = Owner.direction;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.Center = Owner.Center + Projectile.velocity * 120;

            if (Timer == 1)
            {
                FXUtil.GlowCircleBoom(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.LightPink,
                    outerGlowColor: Color.DarkViolet, duration: 15, baseSize: 0.12f);
            }

            SetHoldPosition();
        }

        private void SetHoldPosition()
        {
            if (Main.myPlayer == Projectile.owner)
            {
                Owner.direction = Main.MouseWorld.X > Owner.MountedCenter.X ? 1 : -1;
            }

            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(90f)); // set arm position (90 degree offset since arm starts lowered)
            Vector2 armPosition = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)Math.PI / 2); // get position of hand

            armPosition.Y += Owner.gfxOffY;
            Owner.heldProj = Projectile.whoAmI;
        }
    }


    public class BrincShot : ModProjectile
    {
        int Spin = 0;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Sun Death");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.timeLeft = 400;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }

        private bool Moved;
        private float alphaCounter = 0;
        public override void AI()
        {

            Projectile.ai[1]++;

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




            Projectile.velocity.Y += 0.04f;
            Projectile.rotation += 0.3f;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.ai[0]++;
            if (Projectile.ai[0] == 2)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    float offsetX = Main.rand.Next(-200, 200) * 0.01f;
                    float offsetY = Main.rand.Next(-200, 200) * 0.01f;
                    Projectile.velocity.X += offsetX;
                    Projectile.velocity.Y += offsetY;
                    Projectile.netUpdate = true;
                }

                int Sound = Main.rand.Next(1, 4);
                if (Sound == 1)
                {
                    SoundEngine.PlaySound((SoundID.Item42), Projectile.position);
                }
                if (Sound == 2)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Morrowarrow"), Projectile.position);
                }
                if (Sound == 3)
                {
                    SoundStyle soundStyle = SoundRegistry.ExplosionCrystalShard;
                    soundStyle.PitchVariance = 0.33f;
                    SoundEngine.PlaySound(soundStyle, Projectile.position);

                }
                Spin = Main.rand.Next(0, 2);
            }
        }
        public override void OnKill(int timeLeft)
        {
            var EntitySource = Projectile.GetSource_Death();
            FXUtil.ShakeCamera(Projectile.Center, 524f, 2);
            for (int i = 0; i < 2; i++)
            {
                int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowDust>(), 0f, -2f, 0, default, 1.5f);
                Main.dust[num].noGravity = true;
                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 6f;
            }

            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Purple, 0.7f).noGravity = true;
            }
            for (int i = 0; i < 6; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 100, Color.Gray, 0.4f).noGravity = true;
            }
            for (int i = 0; i < 2; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.BoneTorch, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(25.0), 0, default, 0.6f).noGravity = true;
            }
            for (int i = 0; i < 2; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.BoneTorch, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(25.0), 0, default, 0.2f).noGravity = false;
            }
            SoundEngine.PlaySound(SoundID.DD2_BetsysWrathImpact, Projectile.position);
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
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(65f * alphaCounter), (int)(5f * alphaCounter), (int)(85f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.17f * (7 + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(65f * alphaCounter), (int)(5f * alphaCounter), (int)(85f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.17f * (7 + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(65f * alphaCounter), (int)(15f * alphaCounter), (int)(85f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.07f * (7 + 0.6f), SpriteEffects.None, 0f);
            Lighting.AddLight(Projectile.Center, Color.Orange.ToVector3() * 1.0f * Main.essScale);
        }
    }
}