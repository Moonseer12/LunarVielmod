using Stellamod.Assets;
using Stellamod.Common.Steins;
using Stellamod.Dusts;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Ammo
{
    public class LiliumArrow : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 12; // The damage for projectiles isn't actually 12, it actually is the damage combined with the projectile and the item together.
            Item.DamageType = DamageClass.Ranged;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true; // This marks the item as consumable, making it automatically be consumed when it's used as ammunition, or something else, if possible.
            Item.knockBack = 1.5f;
            Item.shoot = ModContent.ProjectileType<FlowerArrow>(); // The projectile that weapons fire when using this item as ammunition.
            Item.shootSpeed = 16f; // The speed of the projectile.
            Item.ammo = AmmoID.Arrow; // The ammo class this ammo belongs to.
        }
    }

    public class FlowerArrow : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public static Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(new Color(250, 100, 1, 125), Color.Transparent, completionRatio);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawSimpleTrail(Projectile, WidthFunction, ColorFunction, TrailRegistry.VortexTrail);
            DrawHelper.DrawAdditiveAfterImage(Projectile, Color.LightGoldenrodYellow, Color.LightGoldenrodYellow, ref lightColor);
            return base.PreDraw(ref lightColor);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 4; i++)
            {
                Dust.NewDust(target.position, Projectile.width, Projectile.height, ModContent.DustType<GunFlash>(), Scale: 0.8f);
                Dust.NewDustPerfect(target.position, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DarkGoldenrod, 1f).noGravity = true;
            }


            for (int i = 0; i < 6; i++)
            {
                float speedXa = -Projectile.velocity.X * Main.rand.NextFloat(-2.5f, 2.5f);
                float speedYa = -Projectile.velocity.Y * Main.rand.NextFloat(-2.5f, 2.5f);

                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X, Projectile.position.Y, speedXa * 0.5f, speedYa * 0.5f, ModContent.ProjectileType<VoltingShot>(), (int)(Projectile.damage * 0.3f), 0f, Projectile.owner, 0f, 0f);

            }

            base.OnHitNPC(target, hit, damageDone);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Grass, Projectile.position);

        }
    }
}