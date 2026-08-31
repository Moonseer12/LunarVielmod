using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital.WeaponsRC
{
    public class Yumiko : ModItem
    {
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(1, 60));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 225;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 80;
            Item.useAnimation = 80;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 15;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<YumikoShot>();
            Item.shootSpeed = 80f;
            Item.noMelee = true;
            Item.noUseGraphic = true;


        }


        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float numberProjectiles = 4;
            float numberProjectiles2 = 3;
            float rotation = MathHelper.ToRadians(20);
            float rotation2 = MathHelper.ToRadians(30);
            position += Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 45f;
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * 1f;
                Vector2 perturbedSpeed2 = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.Lerp(-rotation2, rotation2, i / (numberProjectiles2 - 1))) * 1f;// This defines the projectile roatation and speed. .4f == projectile speed
                Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, Item.knockBack, player.whoAmI);
                Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed2.X, perturbedSpeed2.Y, ModContent.ProjectileType<YumikoShot2>(), damage, Item.knockBack, player.whoAmI);
            }

            int Sound = Main.rand.Next(1, 4);
            if (Sound == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Yumiko"), position);
            }
            if (Sound == 2)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Yumiko2"), position);
            }
            if (Sound == 3)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Yumiko3"), position);

            }
            if (Sound == 4)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Yumiko4"), position);

            }
            return false;
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
    }

    public class YumikoShot : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("SalfaCirle");
            Main.projFrames[Projectile.type] = 20;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 35;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = 130;
            Projectile.height = 373;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.knockBack = 12.9f;
            Projectile.aiStyle = 1;
            AIType = ProjectileID.Bullet;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;

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

        public float nigga = 0;
        public override void AI()
        {

            nigga++;

            Projectile.velocity *= 1.02f;
            if (nigga < 2)
            {
                ShakeScreenPosition.Shake = 13;
            }


            if (++Projectile.frameCounter >= 2)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 20)
                {
                    Projectile.frame = 0;
                }
            }


        }

        public override bool PreDraw(ref Color lightColor)
        {


            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            return true;
        }
    }
        public class YumikoShot2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("SalfaCirle");
            Main.projFrames[Projectile.type] = 20;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 35;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = 130;
            Projectile.height = 373;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.knockBack = 12.9f;
            Projectile.aiStyle = 1;
            AIType = ProjectileID.Bullet;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;

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

        public float nigga = 0;
        public override void AI()
        {

            nigga++;

            Projectile.velocity *= 1.02f;
            if (nigga < 2)
            {
                ShakeScreenPosition.Shake = 9;
            }


            if (++Projectile.frameCounter >= 2)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 20)
                {
                    Projectile.frame = 0;
                }
            }


        }

    }
}