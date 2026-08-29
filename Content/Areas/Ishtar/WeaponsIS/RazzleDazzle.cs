using Stellamod.Dusts;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Ishtar.WeaponsIS
{
    public class RazzleDazzle : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 62;
            Item.width = 44;
            Item.height = 80;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Lime;

            Item.shootSpeed = 15;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Ranged;

            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 16f;
            Item.useAmmo = AmmoID.Arrow;
            Item.UseSound = SoundID.Item5;
            Item.useAnimation = 24;
            Item.useTime = 24;
            Item.consumeAmmoOnLastShotOnly = true;
            Item.noMelee = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2f, 0f);
        }


        private int _combo;
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            _combo++;
            if (_combo == 3)
            {
                SoundEngine.PlaySound(SoundID.Item78, position);
                type = ModContent.ProjectileType<RazzleDazzleProj>();
                _combo = 0;
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.PiOver4 / 7), type, damage, knockback, player.whoAmI);
            Projectile.NewProjectile(source, position, velocity.RotatedBy(-MathHelper.PiOver4 / 7), type, damage, knockback, player.whoAmI);
            return false;
        }
    }

    public class RazzleDazzleProj : ModProjectile
    {
        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 30;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            Timer++;
            Projectile.velocity.Y += 0.05f;

            float rotation = Timer / 30 * MathHelper.TwoPi;
            Projectile.rotation = Projectile.velocity.ToRotation() + rotation;
        }

        public override bool PreDraw(ref Color lightColor)
        {

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 12; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Sparkle>(),
                    (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(MathHelper.TwoPi), 0, Color.White, 1f).noGravity = false;
            }

            for (int i = 0; i < Main.rand.Next(2, 5); i++)
            {
                Vector2 velocity = -Projectile.velocity;
                velocity = velocity.RotatedByRandom(MathHelper.PiOver4 + MathHelper.PiOver4 / 2);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, velocity,
                             ProjectileID.BabySpider, Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }
    }
}
