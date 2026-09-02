using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital.WeaponsRC
{
    public class Nekomara : ModItem
    {
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(1, 60));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 60;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 5;
            Item.useAnimation = 100;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 15;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<NekomaraProj>();
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

                spriteBatch.Draw(texture, drawPos + new Vector2(0f, 8f).RotatedBy(radians) * time, frame, new Color(200, 70, 255, 77), rotation, frameOrigin, scale, SpriteEffects.None, 0);
            }

            for (float i = 0f; i < 1f; i += 0.34f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;

                spriteBatch.Draw(texture, drawPos + new Vector2(0f, 4f).RotatedBy(radians) * time, frame, new Color(255, 255, 255, 67), rotation, frameOrigin, scale, SpriteEffects.None, 0);
            }

            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, Main.MouseWorld + new Vector2(0, -300), Vector2.Zero, type, damage, knockback, player.whoAmI, 0f, 0f);
            return false;

        }
    }

    public class NekomaraProj : ModProjectile
    {
        bool Moved;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 50;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {

            Projectile.penetrate = 1;
            Projectile.width = 30;
            Projectile.height = 70;
            Projectile.timeLeft = 250;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.Kill();
            return false;
        }

        float alphaCounter = 1;
        public override void AI()
        {
            Projectile.velocity /= 0.96f;
            Projectile.ai[1]++;
            if (!Moved && Projectile.ai[1] >= 0)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.velocity.X = Main.rand.NextFloat(-5, 5);
                    Projectile.netUpdate = true;
                }

                Projectile.velocity.Y = 10;
                Projectile.spriteDirection = Projectile.direction;
                Projectile.alpha = 255;
                Moved = true;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f + 3.14f;

            if (Projectile.ai[1] <= 1)
            {
                Projectile.scale = 1.5f;

            }
            if (Main.rand.NextBool(3))
            {
                int dustnumber = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.SilverCoin, 0f, 0f, 150, Color.White, 1f);
                Main.dust[dustnumber].velocity *= 0.3f;
                Main.dust[dustnumber].velocity.Y += Main.rand.Next(-2, 2);
                Main.dust[dustnumber].velocity.X += Main.rand.Next(-2, 2);
                Main.dust[dustnumber].noGravity = true;
                Main.dust[dustnumber].noLight = false;
            }



            Projectile.spriteDirection = Projectile.direction;
        }
        public override void OnKill(int timeLeft)
        {


            float speedXa = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
            float speedYa = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;
            FXUtil.ShakeCamera(Projectile.Center, 2212f, 6f);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXa, Projectile.position.Y + speedYa, speedXa * 0, speedYa * 0, ModContent.ProjectileType<KaBoomSigil>(), (int)(Projectile.damage * 1.5f), 0f, Projectile.owner, 0f, 0f);
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Starblast"), Projectile.position);
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D texture2D4 = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/DimLight").Value;
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(60f * alphaCounter), (int)(25f * alphaCounter), (int)(55f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.37f * (7 + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(60f * alphaCounter), (int)(25f * alphaCounter), (int)(55f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.27f * (7 + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(60f * alphaCounter), (int)(45f * alphaCounter), (int)(15f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.27f * (7 + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(60f * alphaCounter), (int)(45f * alphaCounter), (int)(15f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.17f * (7 + 0.6f), SpriteEffects.None, 0f);
            Lighting.AddLight(Projectile.Center, Color.HotPink.ToVector3() * 2.0f * Main.essScale);

        }
    }
}