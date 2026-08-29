using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Junkyard.WeaponsJY
{
    public class RazorBragett : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 8;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.noUseGraphic = true;
            Item.height = 40;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ItemRarityID.Blue;
            Item.crit = 30;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BrackettProj>();
            Item.shootSpeed = 8f;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<MechanizedSoul, BlankOrb>();
        }
    }

    public class BrackettProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.BoulderStaffOfEarth);
            AIType = ProjectileID.BoulderStaffOfEarth;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.scale = 1f;
            Projectile.height = 32;
            Projectile.width = 32;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(2))
                target.AddBuff(BuffID.Poisoned, 180);

            ShakeScreenPosition.Shake = 4;
            float speedX = Projectile.velocity.X;
            float speedY = Projectile.velocity.Y;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedX, Projectile.position.Y + speedY, speedX, speedY, ModContent.ProjectileType<BrackettThrough>(), (int)(Projectile.damage * 1.2), 0f, Projectile.owner, 0f, 0f);
            SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Saw1"));
            Projectile.Kill();

        }

        public override bool PreAI()
        {
            if (Main.rand.NextBool(12))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Dirt);
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.JunglePlants);
            }

            return true;
        }

        public override void AI()
        {
            Projectile.rotation += 0.1f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.LoadProjectile(Projectile.type);
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            // Redraw the projectile with the color not influenced by light
            Vector2 drawOrigin = new(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (float i = 0; i < 4; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                var particle = FXUtil.GlowCircleLongBoom(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.LightGray,
                    outerGlowColor: Color.Black, baseSize: 0.06f);
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }
            for (int i = 0; i < 16; i++)
            {
                float p = i / 16f;
                float rot = p * MathHelper.ToRadians(360);
                Vector2 vel = rot.ToRotationVector2() * 6;
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Dirt, vel * 0.2f);
                d = Dust.NewDustPerfect(Projectile.Center, DustID.SilverCoin, vel * 0.4f);
                d.noGravity = true;
            }

        }
    }

    public class BrackettThrough : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = 4;
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.penetrate = 8;
            Projectile.knockBack = 12.9f;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.velocity.Y = Projectile.velocity.Y * 2;
            Projectile.velocity.X = Projectile.velocity.X * 2;
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

            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            for (int i = 0; i < 7; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SilverCoin);
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch);
            }
            return false;
        }


        int yummers = 0;
        public override void AI()
        {

            yummers++;

            if (yummers < 20)
            {
                Projectile.tileCollide = false;
            }

            if (yummers > 20)
            {
                Projectile.tileCollide = true;
            }

            Projectile.ai[1]++;
            Projectile.spriteDirection = Projectile.direction;
            if (Projectile.ai[1] == 1)
            {
                float A = Main.rand.Next(0, 2);
                if (A == 0)
                {
                    SoundEngine.PlaySound(SoundID.Zombie83, Projectile.position);
                    SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Saw1"), Projectile.position);
                }
                else
                {
                    SoundEngine.PlaySound(SoundID.Zombie82, Projectile.position);
                    SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Saw1"), Projectile.position);
                }
            }


            Projectile.alpha = Math.Max(0, Projectile.alpha - 25);

            bool flag25 = false;
            int jim = 1;
            for (int index1 = 0; index1 < 200; index1++)
            {
                if (Main.npc[index1].CanBeChasedBy(Projectile, false)
                    && Projectile.Distance(Main.npc[index1].Center) < 1200
                    && Collision.CanHit(Projectile.Center, 1, 1, Main.npc[index1].Center, 1, 1))
                {
                    flag25 = true;
                    jim = index1;
                }
            }

            if (flag25)
            {
                Projectile.velocity *= 1.05f;
                float num1 = 10f;
                Vector2 vector2 = new(Projectile.position.X + Projectile.width * 0.5f, Projectile.position.Y + Projectile.height * 0.5f);
                float num2 = Main.npc[jim].Center.X - vector2.X;
                float num3 = Main.npc[jim].Center.Y - vector2.Y;
                float num4 = (float)Math.Sqrt((double)num2 * num2 + num3 * num3);
                float num5 = num1 / num4;
                float num6 = num2 * num5;
                float num7 = num3 * num5;
                int num8 = 10;
                Projectile.velocity.X = (Projectile.velocity.X * (num8 - 1) + num6) / num8;
                Projectile.velocity.Y = (Projectile.velocity.Y * (num8 - 1) + num7) / num8;
            }

            if (Main.rand.NextBool(5))
            {
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 150, Color.White, 1f);
                Main.dust[dustnumber].velocity *= 0.3f;
            }
            Projectile.rotation -= 0.3f;
            Lighting.AddLight(Projectile.Center, Color.LightGoldenrodYellow.ToVector3() * 1.75f * Main.essScale);

        }

        public override void OnKill(int timeLeft)
        {
            int Sound = Main.rand.Next(1, 3);
            if (Sound == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/AssassinsKnifeHit"), Projectile.position);
            }
            else
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/AssassinsKnifeHit2"), Projectile.position);
            }
            for (float i = 0; i < 4; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                var particle = FXUtil.GlowCircleLongBoom(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.LightGray,
                    outerGlowColor: Color.Black, baseSize: 0.06f);
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }
            for (int i = 0; i < 16; i++)
            {
                float p = (float)i / 16f;
                float rot = p * MathHelper.ToRadians(360);
                Vector2 vel = rot.ToRotationVector2() * 6;
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Dirt, vel * 0.2f);
                d = Dust.NewDustPerfect(Projectile.Center, DustID.SilverCoin, vel * 0.4f);
                d.noGravity = true;
            }
        }

        Vector2 DrawOffset;
        float alphaCounter = 7;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;





            Texture2D texture2D4 = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Spiin").Value;
            Main.spriteBatch.Draw(texture2D4, DrawOffset - Main.screenPosition, null, new Color((int)(35f * alphaCounter), (int)(25f * alphaCounter), (int)(05f * alphaCounter), 0), Projectile.rotation, new Vector2(200, 200), 0.07f * (5 + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, DrawOffset - Main.screenPosition, null, new Color((int)(35f * alphaCounter), (int)(25f * alphaCounter), (int)(05f * alphaCounter), 0), Projectile.rotation, new Vector2(200, 200), 0.07f * (5 + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, DrawOffset - Main.screenPosition, null, new Color((int)(35f * alphaCounter), (int)(25f * alphaCounter), (int)(05f * alphaCounter), 0), Projectile.rotation, new Vector2(200, 200), 0.07f * (5 + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, DrawOffset - Main.screenPosition, null, new Color((int)(35f * alphaCounter), (int)(25f * alphaCounter), (int)(05f * alphaCounter), 0), Projectile.rotation, new Vector2(200, 200), 0.07f * (5 + 0.6f), SpriteEffects.None, 0f);


            Main.instance.LoadProjectile(Projectile.type);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Lerp(new Color(200, 105, 25), new Color(151, 146, 101), 1f / Projectile.oldPos.Length * k) * (1f - 1f / Projectile.oldPos.Length * k));
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.oldRot[k] + Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {

        }
    }
}