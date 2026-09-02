using Stellamod.Content.Dusts;
using Stellamod.Content.Gores;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Jungle.WeaponsJN
{
    public class Opinine : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 110;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4;
            Item.shootSpeed = 15;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Ranged;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 16f;
            Item.useAmmo = AmmoID.Arrow;
            Item.UseSound = SoundID.Item5;
            Item.useAnimation = 12;
            Item.useTime = 4; // one third of useAnimation
            Item.reuseDelay = 14;
            Item.noMelee = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {


            int numProjectiles = Main.rand.Next(1, 2);
            for (int p = 0; p < numProjectiles; p++)
            {
                // Rotate the velocity randomly by 30 degrees at max.
                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(5));
                newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                Projectile.NewProjectileDirect(source, position, newVelocity, ModContent.ProjectileType<GoldenBaha>(), damage, knockback, player.whoAmI);
            }


            return false;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2f, 0f);
        }
    }

    public class GoldenBaha : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            int gore1 = GoreHelper.TypeFallingLeafWhite;
            int gore2 = GoreHelper.TypeFallingLeafRed;
            for (int i = 0; i < 2; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(8, 8);
                Gore.NewGore(Projectile.GetSource_FromThis(), target.position, velocity, gore1);

                velocity = velocity.RotatedByRandom(MathHelper.TwoPi);
                Gore.NewGore(Projectile.GetSource_FromThis(), target.position, velocity, gore2);
            }

            for (int i = 0; i < 4; i++)
            {
                Dust.NewDust(target.position, Projectile.width, Projectile.height,
                    ModContent.DustType<GunFlash>(), Scale: 0.8f);
                Dust.NewDustPerfect(target.Center,
                    ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 2)).RotatedByRandom(19.0), 0, Color.DarkGoldenrod, 1f).noGravity = true;
            }

            if (Main.rand.NextBool(2))
                target.AddBuff(BuffID.OnFire3, 180);
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.knockBack = 12.9f;
            Projectile.friendly = true;
            Projectile.timeLeft = 120;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Timer++;
            if (Timer % 5 == 0)
            {
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GoldCoin, 0f, 0f, 25, Color.White, 1f);
                Main.dust[dustnumber].velocity *= 0.3f;
                Main.dust[dustnumber].noGravity = true;
            }
            Projectile.velocity *= 1.02f;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90);
        }

        public override void OnKill(int timeLeft)
        {
            FXUtil.ShakeCamera(Projectile.Center, 512f, 4);
            for (int i = 0; i < 16; i++)
            {
                float progress = i / 16f;
                float rot = progress * MathHelper.TwoPi;
                Vector2 vel = rot.ToRotationVector2();
                Dust.NewDustPerfect(Projectile.Center, DustID.GoldCoin, vel, 0, default, 1f).noGravity = false;
            }
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
            Lighting.AddLight(Projectile.Center, Color.Orange.ToVector3() * 1.75f * Main.essScale);
        }
    }
}
