using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital.WeaponsRC
{
    public class Rhamenthal : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }
        public override void SetDefaults()
        {
            Item.damage = 75;
            Item.DamageType = DamageClass.Magic;
            Item.useTime = 75;
            Item.useAnimation = 75;
            Item.knockBack = 15;
            Item.shoot = ModContent.ProjectileType<RhamenthalProjHold>();
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = false;
            Item.shootSpeed = 30f;
            Item.scale = 0.8f;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;


        }


        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            // Draw the periodic glow effect behind the item when dropped in the world (hence PreDrawInWorld)
            Texture2D texture = TextureAssets.Item[Item.type].Value;

            Rectangle frame;

            if (Main.itemAnimations[Item.type] != null)
            {
                // In case this item is animated, this picks the correct frame
                frame = Main.itemAnimations[Item.type].GetFrame(texture, Main.itemFrameCounter[whoAmI]);
            }
            else
            {
                frame = texture.Frame();
            }

            Vector2 frameOrigin = frame.Size() / 2f;
            Vector2 offset = new(Item.width / 2 - frameOrigin.X, Item.height - frame.Height);
            Vector2 drawPos = Item.position - Main.screenPosition + frameOrigin + offset;

            float time = Main.GlobalTimeWrappedHourly;
            float timer = Item.timeSinceItemSpawned / 240f + time * 0.04f;

            time %= 4f;
            time /= 2f;

            if (time >= 1f)
            {
                time = 2f - time;
            }

            time = time * 0.5f + 0.5f;

            for (float i = 0f; i < 1f; i += 0.25f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;

                spriteBatch.Draw(texture, drawPos + new Vector2(0f, 8f).RotatedBy(radians) * time, frame, new Color(200, 70, 255, 77), rotation, frameOrigin, scale, SpriteEffects.None, 0);
            }

            for (float i = 0f; i < 1f; i += 0.34f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;

                spriteBatch.Draw(texture, drawPos + new Vector2(0f, 4f).RotatedBy(radians) * time, frame, new Color(255, 255, 255, 67), rotation, frameOrigin, scale, SpriteEffects.None, 0);
            }

            return true;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<AlcaricMush, BlankBow>();
        }
    }

    public class RhamenthalProjHold : ModProjectile
    {
        public override string Texture => TextureRegistry.EmptyTexture;
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
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
            Projectile.timeLeft = 80;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            Timer++;
            if (Timer > 75)
            {
                // Our timer has finished, do something here:
                // Main.PlaySound, Dust.NewDust, Projectile.NewProjectile, etc. Up to you.


                SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/CyroBolt2"), Projectile.position);
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


            if (Timer == 1)
            {
                float speedX = Projectile.velocity.X * 10;
                float speedY = Projectile.velocity.Y * 7;

                SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/CorsageRune1"), Projectile.position);
            }

            if (Timer == 1)
            {
                float speedX = Projectile.velocity.X * 10;
                float speedY = Projectile.velocity.Y * 7;


                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedX, Projectile.position.Y + speedY, speedX, speedY, ModContent.ProjectileType<RhamenthalProj>(), Projectile.damage * 1, 0f, Projectile.owner, 0f, 0f);
            }


            if (Timer == 75)
            {
                float speedX = Projectile.velocity.X * 30;
                float speedY = Projectile.velocity.Y * 30;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedX, Projectile.position.Y + speedY, speedX * 1f, speedY, ModContent.ProjectileType<RhamenthalShot>(), (int)(Projectile.damage * 12), 0f, Projectile.owner, 0f, 0f);
                SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Rhamenthal"), Projectile.position);
                ShakeScreenPosition.Shake = 9;
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
            Player player = Main.player[Projectile.owner];

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;
            Rectangle sourceRectangle = new(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            origin.X = Projectile.spriteDirection == 1 ? sourceRectangle.Width - 30 : 30; // Customization of the sprite position

            Color drawColor = Projectile.GetAlpha(lightColor);
            Main.EntitySpriteDraw((Texture2D)TextureAssets.Projectile[Projectile.type], Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            return false;



        }




    }
    
    public class RhamenthalProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("SalfaCirle");
            Main.projFrames[Projectile.type] = 28;
        }
        public override void SetDefaults()
        {
            Projectile.friendly = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 268;
            Projectile.height = 104;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            DrawOriginOffsetY = 0;
            Projectile.damage = 0;
            Projectile.timeLeft = 140;

        }
        public override bool PreAI()
        {

            Projectile.tileCollide = false;

            return true;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 50f);
        }
        public override void AI()
        {


            Player player = Main.player[Projectile.owner];
            if (player.noItems || player.CCed || player.dead || !player.active)
                Projectile.Kill();

            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);
            float swordRotation = 0f;
            if (Main.myPlayer == Projectile.owner)
            {
                player.ChangeDir(Projectile.direction);
                swordRotation = (Main.MouseWorld - player.Center).ToRotation();
                if (!player.channel)
                    Projectile.Kill();
            }
            Projectile.velocity = swordRotation.ToRotationVector2();

            Projectile.spriteDirection = player.direction;
            if (Projectile.spriteDirection == 1)
                Projectile.rotation = Projectile.velocity.ToRotation();
            else
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;

            Projectile.Center = playerCenter + new Vector2(90, 0).RotatedBy(swordRotation);

            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 28)
                {
                    Projectile.frame = 0;
                }
            }
        }
    }

    public class RhamenthalShot : ModProjectile
    {
        bool Moved;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shadow Hand");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.penetrate = 5;
            Projectile.width = 25;
            Projectile.height = 25;
            Projectile.timeLeft = 700;
            Projectile.alpha = 255;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(3) && !target.boss)
                target.AddBuff(BuffID.Confused, 180);
        }

        public float greb = 0;
        public override void AI()
        {
            greb++;

            if (greb == 5)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                    ModContent.ProjectileType<KaBoomFenix>(), Projectile.damage / 5, 0f, Projectile.owner, 0f, 0f);

                for (int i = 0; i < 5; i++)
                {
                }

                greb = 0;
            }
            Projectile.velocity *= 1f;
            Projectile.ai[1]++;
            if (!Moved && Projectile.ai[1] >= 0)
            {
                Projectile.spriteDirection = Projectile.direction;
                Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f + 3.14f;
                Projectile.alpha = 255;
                Moved = true;
            }
            if (Projectile.ai[1] >= 20)
            {
                Projectile.tileCollide = true;
            }
            if (Projectile.alpha >= 0)
            {
                Projectile.alpha -= 2;
            }

            Projectile.spriteDirection = Projectile.direction;
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f + 3.14f;
        }
        public override void OnKill(int timeLeft)
        {
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 scale = new(Projectile.scale, 1f);
            Color drawColor = Projectile.GetAlpha(lightColor);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            for (int i = 0; i < 8; i++)
            {
                Vector2 drawOffset = (MathHelper.TwoPi * i / 8f).ToRotationVector2() * 4f;
                Main.EntitySpriteDraw(texture, drawPosition + drawOffset, null, Color.MediumPurple with { A = 160 } * Projectile.Opacity, Projectile.rotation, texture.Size() * 0.5f, scale, 0, 0);
            }
            for (int i = 0; i < 7; i++)
            {
                float scaleFactor = 1f - i / 6f;
                Vector2 drawOffset = Projectile.velocity * i * -0.34f;
                Main.EntitySpriteDraw(texture, drawPosition + drawOffset, null, drawColor with { A = 160 } * Projectile.Opacity, Projectile.rotation, texture.Size() * 0.5f, scale * scaleFactor, 0, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Main.instance.LoadProjectile(Projectile.type);

            // Redraw the projectile with the color not influenced by light
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.MediumPurple) * (float)(((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length) / 2);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return true;
        }
        public override void PostDraw(Color lightColor)
        {
            Lighting.AddLight(Projectile.Center, Color.MediumPurple.ToVector3() * 1.75f * Main.essScale);

        }
    }

    public class KaBoomFenix : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("FrostShotIN");
            Main.projFrames[Projectile.type] = 10;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = 585;
            Projectile.height = 522;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;
            Projectile.scale = 1f;

        }
        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public override void AI()
        {
            Projectile.rotation -= 0.01f;
            Vector3 RGB = new(0.89f, 2.53f, 2.55f);
            // The multiplication here wasn't doing anything
            Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);

        }

        public override bool PreAI()
        {
            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 10)
                {
                    Projectile.frame = 0;
                }
            }
            return true;


        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 50f);
        }


    }
}