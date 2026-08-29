using Stellamod.Assets;
using Stellamod.Common;
using Stellamod.Content.GunSwapping;
using Stellamod.Dusts;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.WeaponsCS
{
    public class BurnBlast : MiniGun
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 50;

            //Setting this to width and height of the texture cause idk
            Item.width = 62;
            Item.height = 38;

            RightHand = true;

            //This number is in ticks
            AttackSpeed = 60;

            //Offset it so it doesn't hold gun by weird spot
            HolsterOffset = new Vector2(0, -6);
        }

        public override void Fire(Player player, Vector2 position, Vector2 velocity, int damage, float knockback)
        {
            base.Fire(player, position, velocity, damage, knockback);
            //Treat this like a normal shoot function
            float spread = 0.4f;
            for (int k = 0; k < 14; k++)
            {
                Vector2 newDirection = velocity.RotatedByRandom(spread);
                Dust.NewDustPerfect(position, ModContent.DustType<GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.Orange, Main.rand.NextFloat(0.4f, 0.8f));
            }

            Dust.NewDustPerfect(position, ModContent.DustType<GlowDust>(), new Vector2(0, 0), 125, Color.DarkRed, 1);
            if (Main.myPlayer == player.whoAmI)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), position, velocity * 16,
                ModContent.ProjectileType<BurnBlastProj>(), damage, knockback, player.whoAmI);
            }

            player.GetModPlayer<ShakePlayer>().ShakeAtPosition(position, 1024f, 16f);
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/gun1"), position);
        }
    }

    public class BurnBlastProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.height = 22;
            Projectile.width = 32;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 120;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.33f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Visuals();
        }

        private void Visuals()
        {
            float radius = 1 / 6f;
            if (Main.rand.NextBool(12))
            {
                for (int i = 0; i < 2; i++)
                {
                    float speedX = Main.rand.NextFloat(-radius, radius);
                    float speedY = Main.rand.NextFloat(-radius, radius);
                    float scale = Main.rand.NextFloat(0.66f, 1f);
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.InfernoFork,
                        speedX, speedY, Scale: scale);
                }
            }
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 180);
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public static Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.DarkOrange, Color.Transparent, completionRatio);
        }

        //Visual Stuffs
        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawSimpleTrail(Projectile, WidthFunction, ColorFunction, TrailRegistry.WhispyTrail);
            return base.PreDraw(ref lightColor);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 14; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.OrangeRed, 1f).noGravity = true;
            }
            for (int i = 0; i < 14; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DarkGray, 1f).noGravity = true;
            }
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);
            Main.LocalPlayer.GetModPlayer<ShakePlayer>().ShakeAtPosition(Projectile.Center, 1024f, 32f);
            for (int i = 0; i < Main.rand.Next(2, 4); i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(16f, 16f);
                int index = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
                    ProjectileID.GreekFire3, Projectile.damage, 0f, Projectile.owner);
                Main.projectile[index].friendly = true;
                Main.projectile[index].hostile = false;
            }



            for (int i = 0; i < Main.rand.Next(2, 7); i++)
            {
                Vector2 velocity = Projectile.velocity;
                velocity = -velocity;
                velocity = velocity.RotatedByRandom(MathHelper.PiOver4 + MathHelper.PiOver2);
                velocity *= Main.rand.NextFloat(0.5f, 1f);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, velocity,
                    ModContent.ProjectileType<CinderFlameball>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }
    }
}