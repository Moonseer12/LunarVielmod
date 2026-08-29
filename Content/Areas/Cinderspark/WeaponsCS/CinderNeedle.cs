using Stellamod.Assets;
using Stellamod.Content.GunSwapping;
using Stellamod.Dusts;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.WeaponsCS
{
    public class CinderNeedle : MiniGun
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 20;
            LeftHand = true;

            SoundStyle soundStyle = new("Stellamod/Assets/Sounds/Gunsotp");
            soundStyle.PitchVariance = 0.5f;
            Item.UseSound = soundStyle;

            //Higher is faster
            AttackSpeed = 6;

            //Offset it so it doesn't hold gun by weird spot
            HolsterOffset = new Vector2(15, -6);

            //Recoil
            RecoilDistance = 3;
        }

        public override void Fire(Player player, Vector2 position, Vector2 velocity, int damage, float knockback)
        {
            base.Fire(player, position, velocity, damage, knockback);
            float spread = 0.4f;
            for (int k = 0; k < 7; k++)
            {
                Vector2 newDirection = velocity.RotatedByRandom(spread);
                Dust.NewDustPerfect(position, ModContent.DustType<GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.Red, Main.rand.NextFloat(0.2f, 0.5f));
            }
            Dust.NewDustPerfect(position, ModContent.DustType<GlowDust>(), new Vector2(0, 0), 125, Color.DarkRed, 1);
            for (int i = 0; i < Main.rand.Next(1, 3); i++)
            {
                Vector2 vel = velocity * 16;
                vel = vel.RotatedByRandom(MathHelper.PiOver4 / 15);
                if (Main.myPlayer == player.whoAmI)
                {
                    Projectile.NewProjectile(player.GetSource_FromThis(), position, vel,
                    ModContent.ProjectileType<CinderNeedleProj>(), damage, knockback, player.whoAmI);
                }
            }

            SoundStyle soundStyle = new("Stellamod/Assets/Sounds/Gunsotp");
            soundStyle.PitchVariance = 0.5f;
            SoundEngine.PlaySound(soundStyle, position);
        }
    }

    public class CinderNeedleProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.5f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Visuals();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 180);
        }


        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 0.5f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public static Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.DarkOrange * 0.3f, Color.Transparent, completionRatio);
        }

        //Visual Stuffs
        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawSimpleTrail(Projectile, WidthFunction, ColorFunction, TrailRegistry.WhispyTrail);
            return base.PreDraw(ref lightColor);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 2; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.OrangeRed, 0.5f).noGravity = true;
            }

            for (int i = 0; i < 1; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DarkGray, 0.5f).noGravity = true;
            }
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
    }
}