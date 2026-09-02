using Stellamod.Content.Areas.Tundra.Snow.WeaponsSN;
using Stellamod.Content.Dusts;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital.WeaponsRC
{
    public class EverneanSlicer : ModItem
    {
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(2, 60));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }
        public override void SetDefaults()
        {
            Item.damage = 80;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 15;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<EverneanProj>();
            Item.shootSpeed = 10f;
            Item.noUseGraphic = true;
            Item.noMelee = true;
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
            Vector2 offset = new Vector2(Item.width / 2 - frameOrigin.X, Item.height - frame.Height);
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

                spriteBatch.Draw(texture, drawPos + new Vector2(0f, 8f).RotatedBy(radians) * time, frame, new Color(90, 70, 255, 50), rotation, frameOrigin, scale, SpriteEffects.None, 0);
            }

            for (float i = 0f; i < 1f; i += 0.34f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;

                spriteBatch.Draw(texture, drawPos + new Vector2(0f, 4f).RotatedBy(radians) * time, frame, new Color(140, 120, 255, 77), rotation, frameOrigin, scale, SpriteEffects.None, 0);
            }

            return true;
        }
    }

    public class EverneanProj : ModProjectile
    {
        public override string Texture => TextureRegistry.EmptyTexture;
        private ref float Timer => ref Projectile.ai[0];
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
            Projectile.penetrate = -1;
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
            Timer++;
            if (Timer == 1)
            {
                StartVelocity = Projectile.velocity;
            }
            if (Timer == 2)
            {


                Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f + 3.14f;
                Projectile.velocity = -Projectile.velocity;
            }
            if (Timer == 5 || Timer == 10 || Timer == 15 || Timer == 20)
            {
                var EntitySource = Projectile.GetSource_FromThis();
                if (Main.rand.NextBool(8) && Main.myPlayer == Projectile.owner)
                {
                    int randy = Main.rand.Next(-50, 50);
                    int randx = Main.rand.Next(-50, 50);
                    Projectile.NewProjectile(EntitySource, Projectile.Center.X + randx, Projectile.Center.Y + randy, StartVelocity.X, StartVelocity.Y, ModContent.ProjectileType<Altride4>(), Projectile.damage * 4, 1, Projectile.owner, 0, 0);
                    Projectile.NewProjectile(EntitySource, Projectile.Center.X + randx, Projectile.Center.Y + randy, StartVelocity.X, StartVelocity.Y, ModContent.ProjectileType<Radial2>(), Projectile.damage * 0, 1, Projectile.owner, 0, 0);
                }

                if (Main.rand.NextBool(3) && Main.myPlayer == Projectile.owner)
                {
                    int randy = Main.rand.Next(-50, 50);
                    int randx = Main.rand.Next(-50, 50);
                    Projectile.NewProjectile(EntitySource, Projectile.Center.X + randx, Projectile.Center.Y + randy, StartVelocity.X, StartVelocity.Y, ModContent.ProjectileType<Altride4>(), Projectile.damage * 2, 1, Projectile.owner, 0, 0);
                    Projectile.NewProjectile(EntitySource, Projectile.Center.X + randx, Projectile.Center.Y + randy, StartVelocity.X, StartVelocity.Y, ModContent.ProjectileType<Radial2>(), Projectile.damage * 0, 1, Projectile.owner, 0, 0);

                }
                else
                {
                    if (Main.myPlayer == Projectile.owner)
                    {
                        int randy = Main.rand.Next(-75, 75);
                        int randx = Main.rand.Next(-75, 75);
                        Projectile.NewProjectile(EntitySource, Projectile.Center.X + randx, Projectile.Center.Y + randy, StartVelocity.X, StartVelocity.Y, ModContent.ProjectileType<Altride5>(), Projectile.damage, 1, Projectile.owner, 0, 0);
                        Projectile.NewProjectile(EntitySource, Projectile.Center.X + randx, Projectile.Center.Y + randy, StartVelocity.X, StartVelocity.Y, ModContent.ProjectileType<Radial>(), Projectile.damage * 0, 1, Projectile.owner, 0, 0);
                    }
                }
            }
            if (Timer >= 0 && Timer <= 20)
            {
                Projectile.velocity *= .86f;

            }
            if (Timer == 20)
            {
                Projectile.velocity = -Projectile.velocity;
            }
            if (Timer >= 21 && Timer <= 60)
            {
                Projectile.velocity /= .90f;

            }
            if (Timer == 60)
            {
                Projectile.tileCollide = true;
            }
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
            return false;
        }
        public override void PostDraw(Color lightColor)
        {

        }
    }

    public class Altride5 : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[1];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Pericarditis");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.boss)
            {
                OnKill(1);
            }
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.penetrate = 1;
            Projectile.knockBack = 12.9f;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            AIType = ProjectileID.Bullet;
            Projectile.scale = 0.4f;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            Timer++;
            Projectile.velocity *= 1.02f;
            if (Timer == 1)
            {

                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/AssassinsKnifeHit2"), Projectile.position);

                for (int j = 0; j < 10; j++)
                {
                    Vector2 vector2 = Vector2.UnitX * -Projectile.width / 2f;
                    vector2 += -Vector2.UnitY.RotatedBy(j * 3.141591734f / 6f, default) * new Vector2(8f, 16f);
                    vector2 = vector2.RotatedBy(Projectile.rotation - 1.57079637f, default);
                    int num8 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.GoldCoin, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                    Main.dust[num8].scale = 1.3f;
                    Main.dust[num8].noGravity = true;
                    Main.dust[num8].position = Projectile.Center + vector2;
                    Main.dust[num8].velocity = Projectile.velocity * 0.1f;
                    Main.dust[num8].noLight = true;
                    Main.dust[num8].velocity = Vector2.Normalize(Projectile.Center - Projectile.velocity * 3f - Main.dust[num8].position) * 1.25f;
                }

            }

            if (Timer % 12 == 0)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(), Projectile.velocity * 0.1f, 0, Color.Yellow, Main.rand.NextFloat(1f, 1.5f)).noGravity = true;
            }

        }

        public override void OnKill(int timeLeft)
        {
            int Sound = Main.rand.Next(1, 3);
            if (Sound == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/FungalFlaceBall1"), Projectile.position);
            }
            else
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/FungalFlaceBall2"), Projectile.position);
            }


            for (int i = 0; i < 2; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(), Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(30)) * Main.rand.NextFloat(0.2f, 1f), 0, Color.Yellow, Main.rand.NextFloat(1f, 2f)).noGravity = true;
            }

            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);
            SoundEngine.PlaySound(SoundID.DD2_BetsysWrathImpact, Projectile.position);
            FXUtil.ShakeCamera(Projectile.Center, 1024, 2f);
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
                Main.EntitySpriteDraw(texture, drawPosition + drawOffset, null, Color.DarkGoldenrod with { A = 160 } * Projectile.Opacity, Projectile.rotation, texture.Size() * 0.5f, scale, 0, 0);
            }

            for (int i = 0; i < 7; i++)
            {
                float scaleFactor = 1f - i / 6f;
                Vector2 drawOffset = Projectile.velocity * i * -0.34f;
                Main.EntitySpriteDraw(texture, drawPosition + drawOffset, null, drawColor with { A = 160 } * Projectile.Opacity, Projectile.rotation, texture.Size() * 0.5f, scale * scaleFactor, 0, 0);
            }

            Main.EntitySpriteDraw(texture, drawPosition, null, drawColor with { A = 130 }, Projectile.rotation, texture.Size() * 0.5f, scale, 0, 0);
            Main.instance.LoadProjectile(Projectile.type);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.DarkGoldenrod) * (float)(((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length) / 2);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Lighting.AddLight(Projectile.Center, Color.Gold.ToVector3() * 1.75f * Main.essScale);
        }
    }

    public class Radial : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 34;
        }


        public override void SetDefaults()
        {
            Projectile.width = 150;
            Projectile.height = 70;
            Projectile.penetrate = -1;
            Projectile.knockBack = 12.9f;
            Projectile.aiStyle = 1;
            Projectile.timeLeft = 68;
            AIType = ProjectileID.Bullet;
            Projectile.scale = 1f;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.93f;
            DrawHelper.AnimateTopToBottom(Projectile, 2);
            Lighting.AddLight(Projectile.Center, Color.Gold.ToVector3() * 1.75f * Main.essScale);
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
            float drawScale = 0.5f;
            spriteBatch.Draw(texture, drawPos, frame, drawColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);
            return false;
        }
    }

    public class Radial2 : Radial
    {

    }
}