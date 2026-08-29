using Stellamod.Assets;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.GunSwapping
{
    public class CarrotPatrol : MiniGun
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 82;
            LeftHand = true;

            //Higher is faster
            AttackSpeed = 7;

            //Offset it so it doesn't hold gun by weird spot
            HolsterOffset = new Vector2(15, -6);

            //Recoil
            RecoilDistance = 3;
        }

        public override void Fire(Player player, Vector2 position, Vector2 velocity, int damage, float knockback)
        {
            base.Fire(player, position, velocity, damage, knockback);
            //Treat this like a normal shoot function
            float spread = 0.4f;
            for (int k = 0; k < 7; k++)
            {
                Vector2 newDirection = velocity.RotatedByRandom(spread);
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.DarkOrange, Main.rand.NextFloat(0.2f, 0.5f));
            }
            Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.DarkOrange, 1);
            int projectileType = ModContent.ProjectileType<CarrotPatrolProj>();
            if (Main.myPlayer == player.whoAmI)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), position, velocity * 16, projectileType, damage, knockback, player.whoAmI);
            }

            int Sound = Main.rand.Next(1, 3);
            if (Sound == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/MiniPistol"), position);
            }
            else
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/MiniPistol3"), position);
            }
        }

    }

    public class CarrotPatrolProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            base.AI();
            Projectile.velocity.Y += 0.4f;
            Projectile.rotation += Projectile.velocity.Length() * 0.05f;
            if (Main.rand.NextBool(8))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Plantera_Green);

            }
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 0.5f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public static Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.DarkGreen * 0.3f, Color.Transparent, completionRatio);
        }

        //Visual Stuffs
        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawSimpleTrail(Projectile, WidthFunction, ColorFunction, TrailRegistry.WhispyTrail);
            return base.PreDraw(ref lightColor);
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for (int i = 0; i < Main.rand.Next(2, 5); i++)
            {
                Vector2 velocity = -Projectile.velocity;
                velocity *= Main.rand.NextFloat(0.2f, 1f);
                velocity = velocity.RotatedByRandom(MathHelper.PiOver4);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, velocity,
                    ModContent.ProjectileType<CarrotPatrolShardProj>(), Projectile.damage / 3, Projectile.knockBack, Projectile.owner);
            }
        }
    }

    public class CarrotPatrolShardProj : ModProjectile
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            base.AI();
            Projectile.velocity.Y += 0.4f;
            Projectile.rotation += Projectile.velocity.Length() * 0.05f;
            if (Main.rand.NextBool(8))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Plantera_Green);
            }
        }
    }
}