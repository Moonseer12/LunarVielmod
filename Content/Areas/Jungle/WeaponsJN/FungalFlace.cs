using Stellamod.Content.Dusts;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Jungle.WeaponsJN
{
    public class FungalFlace : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 30;
            Item.DamageType = DamageClass.Magic;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6;
            Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/SwordThrow");
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<FungalFlaceBall>();
            Item.shootSpeed = 14f;
            Item.mana = 8;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float numberProjectiles = 5;
            float rotation = MathHelper.ToRadians(14);
            position += Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 45f;
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * 1f; // This defines the projectile roatation and speed. .4f == projectile speed
                Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, Item.knockBack, player.whoAmI);
            }
            return false;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
    }

    public class FungalFlaceBall : ModProjectile
    {


        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }
        public override void PostDraw(Color lightColor)
        {
            Lighting.AddLight(Projectile.Center, Color.LightPink.ToVector3() * 1.75f * Main.essScale);
            if (Main.rand.NextBool(5))
            {
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Plantera_Pink, 0f, 0f, 150, Color.White, 1f);
                Main.dust[dustnumber].velocity *= 0.3f;
            }

        }
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Shuriken);
            AIType = ProjectileID.Shuriken;
            Projectile.penetrate = 1;
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.timeLeft = 700;
            Projectile.alpha = 255;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        public override void AI()
        {

            if (Projectile.ai[1] <= 3)
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
            }
            Projectile.ai[1]++;
            if (Projectile.ai[1] >= 10)
            {
                Projectile.tileCollide = true;
            }
            if (Projectile.alpha >= 0)
            {
                Projectile.alpha -= 2;
            }
            Projectile.velocity.Y -= 0.01f;
        }
        public override void OnKill(int timeLeft)
        {
            var EntitySource = Projectile.GetSource_Death();
            for (int i = 0; i < 3; i++)
            {
                Projectile.NewProjectile(EntitySource, Projectile.Center.X, Projectile.Center.Y, Main.rand.Next(-2, 2), Main.rand.Next(-2, 2),
                    ModContent.ProjectileType<FungalFlaceCloud>(), 5, 1, Projectile.owner);
            }
            for (int i = 0; i < 14; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DarkGoldenrod, 1f).noGravity = true;
            }

            for (int i = 0; i < 14; i++)
            {

                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DarkGoldenrod, 1f).noGravity = true;
            }
            for (int i = 0; i < 2; i++)
            {
                Projectile.timeLeft = 2;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(EntitySource, Projectile.Center.X, Projectile.Center.Y, Main.rand.Next(-2, 2), Main.rand.Next(-2, 2),
                        ModContent.ProjectileType<FungalFlaceCloudGreen>(), 5, 1, Projectile.owner);
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(-2, 2)).RotatedByRandom(19.0), 0, Color.DarkOliveGreen, 1f).noGravity = true;
            }
            for (int i = 0; i < 20; i++)
            {
                int num1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.VenomStaff, 0f, -2f, 0, default, .8f);
                Main.dust[num1].noGravity = true;
                Main.dust[num1].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[num1].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                if (Main.dust[num1].position != Projectile.Center)
                    Main.dust[num1].velocity = Projectile.DirectionTo(Main.dust[num1].position) * 6f;
                int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.VenomStaff, 0f, -2f, 0, default, .8f);
                Main.dust[num].noGravity = true;
                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                if (Main.dust[num].position != Projectile.Center)
                    Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 6f;
            }
            int Sound = Main.rand.Next(1, 3);
            if (Sound == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/FungalFlaceBall3"), Projectile.position);
            }
            else
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/FungalFlaceBall4"), Projectile.position);
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
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.VenomStaff, 0f, 0f, 150, Color.MediumPurple, 1f);
                Main.dust[dustnumber].velocity *= 0.3f;
                Main.dust[dustnumber].noGravity = true;
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Main.instance.LoadProjectile(Projectile.type);
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            // Redraw the projectile with the color not influenced by light
            Vector2 drawOrigin = new(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Pink) * (float)((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length / 2);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return true;
        }

    }


    public class FungalFlaceCloud : ModProjectile
    {
        public override string Texture => TextureRegistry.EmptyTexture;

        public override void SetDefaults()
        {

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 210;
            Projectile.tileCollide = false;
            Projectile.height = 35;
            Projectile.width = 35;
            Projectile.penetrate = 10;
            AIType = ProjectileID.Bullet;
            Projectile.extraUpdates = 1;
        }

        public override bool PreAI()
        {
            Projectile.alpha++;

            float num = 1f - Projectile.alpha / 255f;
            Projectile.velocity *= .98f;
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
            num *= Projectile.scale;
            Lighting.AddLight(Projectile.Center, 0.3f * num, 0.2f * num, 0.1f * num);
            Projectile.rotation = Projectile.velocity.X / 2f;
            return true;

        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 255f);
        }
        public override void AI()
        {
            Projectile.rotation += 0.3f;
            if (Main.rand.NextBool(40))
            {
                for (int i = 0; i < 2; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(-2, 2)).RotatedByRandom(19.0), 0, Color.Pink, 0.4f).noGravity = true;
                }
            }
        }
    }

    public class FungalFlaceCloudGreen : ModProjectile
    {
        public override string Texture => TextureRegistry.EmptyTexture;

        public override void SetDefaults()
        {

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 210;
            Projectile.tileCollide = false;
            Projectile.height = 35;
            Projectile.width = 35;
            Projectile.penetrate = 10;
            AIType = ProjectileID.Bullet;
            Projectile.extraUpdates = 1;
        }

        public override bool PreAI()
        {
            Projectile.alpha++;

            float num = 1f - Projectile.alpha / 255f;
            Projectile.velocity *= .98f;
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
            num *= Projectile.scale;
            Lighting.AddLight(Projectile.Center, 0.3f * num, 0.2f * num, 0.1f * num);
            Projectile.rotation = Projectile.velocity.X / 2f;
            return true;

        }
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 255f);
        }
        public override void AI()
        {
            Projectile.rotation += 0.3f;
            if (Main.rand.NextBool(40))
            {
                for (int i = 0; i < 1; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(-2, 2)).RotatedByRandom(19.0), 0, Color.DarkGreen, 0.4f).noGravity = true;
                }
            }
        }
    }
}