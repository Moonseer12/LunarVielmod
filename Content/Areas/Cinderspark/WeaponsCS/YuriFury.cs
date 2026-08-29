using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.WeaponsCS
{
    public class YuriFury : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 22;
            Item.width = 50;
            Item.height = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 3, 1, 29);
            Item.rare = ItemRarityID.Blue;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Ranged;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 30f;
            Item.useAmmo = AmmoID.Arrow;
            Item.UseSound = SoundID.Item5;
            Item.useAnimation = 25;
            Item.useTime = 25;
            Item.channel = true;
            Item.scale = 1f;
            Item.consumeAmmoOnLastShotOnly = true;
            Item.noMelee = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5f, 0f);
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<Cinderscrap, BlankBow>();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int Sound = Main.rand.Next(1, 3);
            if (Sound == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/ArchariliteEnergyShot"));
            }
            else
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/ArchariliteEnergyShot2"));
            }


            Item.useTime = 25;
            Item.shootSpeed = 30f;
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<HeatedShot>(), damage * 2, knockback, player.whoAmI);
            float numberProjectiles = 4;
            float rotation = MathHelper.ToRadians(15);
            position += Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 25f;
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .4f; // This defines the projectile roatation and speed. .4f == projectile speed
                Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, ProjectileID.DD2PhoenixBowShot, damage, knockback, player.whoAmI);
            }


            return false;
        }



    }

    public class HeatedShot : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("SalfaCirle");
            Main.projFrames[Projectile.type] = 21;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 216;
            Projectile.height = 120;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.knockBack = 12.9f;
            Projectile.aiStyle = 1;
            AIType = ProjectileID.Bullet;
            Projectile.DamageType = DamageClass.Magic;
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
            Projectile.rotation = Projectile.velocity.ToRotation();
            Vector3 RGB = new(1.00f, 0.37f, 0.30f);
            // The multiplication here wasn't doing anything
            Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);

            int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f);
            Main.dust[dust].scale = 0.6f;


            nigga++;

            Projectile.velocity *= 1.04f;
            if (nigga < 2)
            {
                ShakeScreenPosition.Shake = 5;
            }


            if (++Projectile.frameCounter >= 2)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 21)
                {
                    Projectile.frame = 0;
                }
            }


        }

     
        public override bool PreDraw(ref Color lightColor)
        {

            return true;
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(3))
            {
                target.AddBuff(BuffID.OnFire, 180);
            }

        }

    }
}
