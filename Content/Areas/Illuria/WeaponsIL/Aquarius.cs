using Stellamod.Assets;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.WeaponsIL
{
    public class Aquarius : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 200;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 15;

            Item.useTime = 32;
            Item.useAnimation = 32;
            Item.useStyle = ItemUseStyleID.Swing;

            Item.knockBack = 6;
            Item.channel = true;
            Item.noUseGraphic = true;

            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<AquariusHold>();
            Item.shootSpeed = 30;
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
    
    public class AquariusHold : ModProjectile
    {
        private float MagicCircleRotation;
        private float MagicCircleScale;

        private ref float Timer => ref Projectile.ai[0];
        private ref float SwordRotation => ref Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.damage = 0;
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.aiStyle = 595;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
            Projectile.timeLeft = int.MaxValue;
        }

        public override bool? CanDamage()
        {
            return false;
        }


        public override void AI()
        {
            Timer++;
            AI_Hold();
        }

        private void AI_Hold()
        {
            //Magic Circle Stuff
            MagicCircleRotation += MathHelper.PiOver4 / 24;
            MagicCircleScale += 0.01f;
            MagicCircleScale = MathHelper.Clamp(MagicCircleScale, 0, 0.4f);

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

            if (Timer % 45 == 0)
            {
                int manaChannelCost = player.HeldItem.mana;
                if (!player.CheckMana(manaChannelCost, true))
                {
                    Projectile.Kill();
                }
                else
                {
                    //Make a slash
                    FXUtil.ShakeCamera(Projectile.Center, 1024f, 32f);
                    SoundEngine.PlaySound(SoundID.Item21);
                    float maxSlashDistance = 1;
                    float slashDistance = Math.Min(maxSlashDistance, Vector2.Distance(player.Center, Main.MouseWorld));

                    Vector2 slashPosition = player.Center + player.Center.DirectionTo(Main.MouseWorld) * slashDistance;
                    Vector2 velocity = player.Center.DirectionTo(slashPosition) * 4;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), slashPosition, velocity,
                        ModContent.ProjectileType<AquariusSlash>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }
            if (Timer % 8 == 0)
            {
                int manaChannelCost = player.HeldItem.mana / 8;
                if (!player.CheckMana(manaChannelCost, true))
                {
                    Projectile.Kill();
                }
                else
                {
                    //Make a slash
                    FXUtil.ShakeCamera(Projectile.Center, 1024f, 2);
                    SoundEngine.PlaySound(SoundID.Item21);

                    Vector2 slashPosition = player.Center + Main.rand.NextVector2Circular(80, 80);
                    Vector2 velocity = player.Center.DirectionTo(slashPosition) * 8;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), slashPosition, velocity,
                        ModContent.ProjectileType<AquariusSlashMini>(), Projectile.damage / 4, Projectile.knockBack, Projectile.owner);
                }
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
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }

    public class AquariusSlash : ModProjectile
    {
        private int _frameCounter;
        private int _frameTick;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 24;
        }

        public override void SetDefaults()
        {
            Projectile.width = 310;
            Projectile.height = 310;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 24;
        }

        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override bool PreAI()
        {
            if (++_frameTick >= 1)
            {
                _frameTick = 0;
                if (++_frameCounter >= 24)
                {
                    _frameCounter = 0;
                }
            }
            return true;
        }

        public override void AI()
        {
            Vector3 RGB = new(0.89f, 2.53f, 2.55f);
            // The multiplication here wasn't doing anything
            Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);

            Timer++;
            if (Timer == 6)
            {

                for (int i = 0; i < Main.rand.Next(4, 9); i++)
                {
                    Vector2 velocity = Projectile.velocity.RotateRandom(MathHelper.PiOver4 / 1.2f) * Main.rand.NextFloat(0.3f, 1f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + velocity.SafeNormalize(Vector2.Zero) * 80, velocity,
                        ModContent.ProjectileType<AquariusWaterBolt>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }
        }

        public override bool ShouldUpdatePosition()
        {
            //Makes velocity not move the projectile
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 50f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            float rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver4 - (MathHelper.PiOver4 / 2);
            float width = 187;
            float height = 187;
            Vector2 origin = new Vector2(width / 2, height / 2);
            int frameSpeed = 1;
            int frameCount = 24;
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Draw(texture, drawPosition,
                texture.AnimationFrame(ref _frameCounter, ref _frameTick, frameSpeed, frameCount, false),
                (Color)GetAlpha(lightColor), rotation, origin, 1.8f, SpriteEffects.None, 0f);
            return false;
        }
    }

    public class AquariusSlashMini : ModProjectile
    {
        private int _frameCounter;
        private int _frameTick;
        private Vector2 _scale;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 24;
        }

        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 24;
        }

        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override bool PreAI()
        {
            if (++_frameTick >= 1)
            {
                _frameTick = 0;
                if (++_frameCounter >= 24)
                {
                    _frameCounter = 0;
                }
            }
            return true;
        }

        public override void AI()
        {
            Vector3 RGB = new(0.89f, 2.53f, 2.55f);
            // The multiplication here wasn't doing anything
            Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);
            Timer++;
            if (Timer == 1)
            {
                _scale = new Vector2(
                    Main.rand.NextFloat(0.5f, 0.8f),
                    Main.rand.NextFloat(0.5f, 0.8f));
            }

            _scale *= 1.03f;
            Projectile.alpha++;
        }

        public override bool ShouldUpdatePosition()
        {
            //Makes velocity not move the projectile
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 50f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

        }


        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            float rotation = Projectile.velocity.ToRotation();
            float width = 187;
            float height = 187;
            Vector2 origin = new Vector2(width / 2, height / 2);
            int frameSpeed = 1;
            int frameCount = 24;
            SpriteBatch spriteBatch = Main.spriteBatch;

            spriteBatch.Draw(texture, drawPosition,
                texture.AnimationFrame(ref _frameCounter, ref _frameTick, frameSpeed, frameCount, false),
                (Color)GetAlpha(lightColor), rotation, origin, _scale, SpriteEffects.None, 0f);
            return false;
        }
    }

    public class AquariusWaterBolt : ModProjectile
    {
        public override string Texture => TextureRegistry.EmptyTexture;
        
        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 24;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 3600;
            Projectile.extraUpdates = 3;
        }

        public override void AI()
        {
            Timer++;

            if (Timer % 6 == 0)
            {

            }

            if (Timer > 90)
            {
                NPC npc = NPCHelper.FindClosestNPC(Projectile.position, 512);
                if (npc != null)
                {
                    Vector2 targetVelocity = (npc.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 4;
                    // If found, change the velocity of the projectile and turn it in the direction of the target
                    // Use the SafeNormalize extension method to avoid NaNs returned by Vector2.Normalize when the vector is zero
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, targetVelocity, 0.2f);
                    Projectile.rotation = Projectile.velocity.ToRotation();
                }
            }
            else
            {
                Projectile.velocity.Y += 0.02f;
                Projectile.rotation = Projectile.velocity.ToRotation();
            }
        }

        public override void OnKill(int timeLeft)
        {

        }


        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public static Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(ColorFunctions.Niivin, Color.Black, completionRatio);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawSimpleTrail(Projectile, WidthFunction, ColorFunction, TrailRegistry.CausticTrail);
            return base.PreDraw(ref lightColor);
        }
    }
}