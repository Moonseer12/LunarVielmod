using Stellamod.Assets;
using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Dusts;
using Stellamod.Content.GunSwapping;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.WeaponsPT
{
    public class SrTetanus : MiniGun
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 36;
            RightHand = true;

            //This number is in ticks
            AttackSpeed = 20;
            ShootCount = 2;

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
                Dust.NewDustPerfect(position, ModContent.DustType<GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.DarkGreen, Main.rand.NextFloat(0.4f, 0.8f));
            }

            Dust.NewDustPerfect(position, ModContent.DustType<GlowDust>(), new Vector2(0, 0), 125, Color.DarkGreen, 1);
            Vector2 vel = velocity * 16;
            vel = vel.RotatedByRandom(MathHelper.PiOver4 / 15);
            if (Main.myPlayer == player.whoAmI)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), position, vel,
                ModContent.ProjectileType<SrTetanusProj>(), damage, knockback, player.whoAmI);
            }
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/gun1"), position);
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<MarshScrap, BlankGun>();
        }
    }

    public class SrTetanusProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
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
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.CursedTorch,
                        speedX, speedY, Scale: scale);
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Venom, 180);
            target.AddBuff(BuffID.Slow, 180);
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 0.66f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public static Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.DarkGreen * 0.33f, Color.Transparent, completionRatio);
        }

        //Visual Stuffs
        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawSimpleTrail(Projectile, WidthFunction, ColorFunction, TrailRegistry.WhispyTrail);
            return base.PreDraw(ref lightColor);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 4; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DarkGreen, 1f).noGravity = true;
            }
        }
    }
}