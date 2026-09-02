using Stellamod.Content.Dusts;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.WeaponsSH
{
    public class Verstibloom : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 16;
            Item.DamageType = DamageClass.Melee;
            Item.mana = 3;
            Item.useTime = 23;
            Item.useAnimation = 23;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 7;
            Item.UseSound = SoundID.DD2_MonkStaffSwing;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<VerstiSwing>();
            Item.shootSpeed = 10f;
            Item.noUseGraphic = true;
            Item.noMelee = true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);

            if (Item.shoot == ModContent.ProjectileType<VerstiSwing>())
                Item.shoot = ModContent.ProjectileType<VerstiSwing2>();
            else
                Item.shoot = ModContent.ProjectileType<VerstiSwing>();

            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }


    }

    public class VerstiSwing : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("FrostSwProj");
            Main.projFrames[Projectile.type] = 9;
        }
        public override void SetDefaults()
        {
            Projectile.width = 416;
            Projectile.height = 241;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 18;
            Projectile.ignoreWater = true;
        }
        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 2)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            Vector2 angle = new(Projectile.ai[0], Projectile.ai[1]);
            Projectile.rotation = angle.ToRotation();
            Player player = Main.player[Projectile.owner];
            Projectile.position = player.Center + angle - new Vector2(Projectile.width / 2, Projectile.height / 2);
            if (Projectile.timeLeft == 2)
            {
                Projectile.friendly = false;
            }


            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? 1 : -1;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 50f);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ShakeScreenPosition.Shake = 4;
            for (int i = 0; i < 8; i++)
            {
                Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 3)).RotatedByRandom(19.0), 0, Color.PaleVioletRed, 0.5f).noGravity = true;
            }
            for (int i = 0; i < 4; i++)
            {
                Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.ForestGreen, 0.5f).noGravity = true;
            }
            for (int i = 0; i < 8; i++)
            {
                Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 3)).RotatedByRandom(19.0), 0, Color.Green, 0.5f).noGravity = true;
            }
            base.OnHitNPC(target, hit, damageDone);

        }
    }

    public class VerstiSwing2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("FrostSwProj");
            Main.projFrames[Projectile.type] = 9;
        }
        public override void SetDefaults()
        {
            Projectile.width = 416;
            Projectile.height = 241;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 18;
            Projectile.ignoreWater = true;
        }
        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 2)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            Vector2 angle = new Vector2(Projectile.ai[0], Projectile.ai[1]);
            Projectile.rotation = angle.ToRotation();
            Player player = Main.player[Projectile.owner];
            Projectile.position = player.Center + angle - new Vector2(Projectile.width / 2, Projectile.height / 2);
            if (Projectile.timeLeft == 2)
            {
                Projectile.friendly = false;
            }


            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? 1 : -1;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 50f);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ShakeScreenPosition.Shake = 4;
            for (int i = 0; i < 8; i++)
            {
                Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 3)).RotatedByRandom(19.0), 0, Color.PaleVioletRed, 0.5f).noGravity = true;
            }
            for (int i = 0; i < 4; i++)
            {
                Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.ForestGreen, 0.5f).noGravity = true;
            }
            for (int i = 0; i < 8; i++)
            {
                Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 3)).RotatedByRandom(19.0), 0, Color.Green, 0.5f).noGravity = true;
            }
            base.OnHitNPC(target, hit, damageDone);

        }
    }
}