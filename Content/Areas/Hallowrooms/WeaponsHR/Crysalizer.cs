using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Dusts;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Hallowrooms.WeaponsHR
{
    public class Crysalizer : ModItem
    {
        public int WinterboundArrow;

        public override void SetDefaults()
        {
            Item.damage = 58;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4;
            Item.shootSpeed = 15;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Ranged;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 26f;
            Item.useAmmo = AmmoID.Arrow;
            Item.UseSound = SoundID.Item5;
            Item.useAnimation = 28;
            Item.useTime = 28;
            Item.consumeAmmoOnLastShotOnly = true;
            Item.noMelee = true;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<KaleidoscopicInk, BlankBow>();
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2f, 0f);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            WinterboundArrow += 1;
            if (WinterboundArrow >= 3)
            {
                WinterboundArrow = 0;
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/WinterboundArrow"), player.position);
                type = ModContent.ProjectileType<CrysalizerArrow1>();
                Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, damage, knockback, player.whoAmI);
            }
            if (WinterboundArrow == 2)
            {

                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/WinterboundArrow"), player.position);
                type = ModContent.ProjectileType<CrysalizerArrow2>();
                Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, damage, knockback, player.whoAmI);
            }
            if (WinterboundArrow == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/WinterboundArrow"), player.position);
                type = ModContent.ProjectileType<CrysalizerArrow3>();
                Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, damage, knockback, player.whoAmI);
            }


            return false;
        }
    }

    public class CrysalizerArrow1 : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[1];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Archarilite Arrow");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;

            Projectile.knockBack = 12.9f;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            AIType = ProjectileID.Bullet;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
        }

        public override void AI()
        {
            Timer++;
            NPC nearest = ProjectileHelper.FindNearestEnemy(Projectile.position, 1024);
            if (nearest != null && Projectile.penetrate < 3)
            {
                Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, nearest.Center, degreesToRotate: 6f);
            }
            if (Main.rand.NextBool(5))
            {
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Electric, 0f, 0f, 150, Color.White, 1f);
                Main.dust[dustnumber].velocity *= 0.3f;
                Main.dust[dustnumber].noGravity = true;
            }
            if (Timer % 6 == 0)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(), Projectile.velocity * 0.1f, 0, Color.Teal, Main.rand.NextFloat(1f, 1.5f));
            }

            Lighting.AddLight(Projectile.Center, Color.Orange.ToVector3() * 1.75f * Main.essScale);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }
            Projectile.velocity *= 0.75f;
            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
            {
                Projectile.Kill();
            }
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            switch (Main.rand.Next(4))
            {
                case 0:
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Crysalizer1"), Projectile.position);
                    break;
                case 1:
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Crysalizer2"), Projectile.position);
                    break;
                case 2:
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Crysalizer3"), Projectile.position);
                    break;
                case 3:
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Crysalizer4"), Projectile.position);
                    break;
            }
            for (float f = 0; f < 12; f++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(),
                    (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.Teal, Main.rand.NextFloat(1f, 3f)).noGravity = true;
            }
        }


        public override bool PreDraw(ref Color lightColor)
        {


            return false;
        }
    }

    public class CrysalizerArrow2 : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[1];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Archarilite Arrow");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;

            Projectile.knockBack = 12.9f;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            AIType = ProjectileID.Bullet;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

        }

        public override void AI()
        {
            Timer++;
            NPC nearest = ProjectileHelper.FindNearestEnemy(Projectile.position, 1024);
            if (nearest != null && Projectile.penetrate < 3)
            {
                Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, nearest.Center, degreesToRotate: 6f);
            }
            if (Timer % 6 == 0)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(), Projectile.velocity * 0.1f, 0, Color.Pink, Main.rand.NextFloat(1f, 1.5f));
            }

            if (Main.rand.NextBool(5))
            {
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.PinkTorch, 0f, 0f, 150, Color.White, 1f);
                Main.dust[dustnumber].velocity *= 0.3f;
            }
            Lighting.AddLight(Projectile.Center, Color.Orange.ToVector3() * 1.75f * Main.essScale);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }
            Projectile.velocity *= 0.75f;
            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
            {
                Projectile.Kill();
            }
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            switch (Main.rand.Next(4))
            {
                case 0:
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Crysalizer1"), Projectile.position);
                    break;
                case 1:
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Crysalizer2"), Projectile.position);
                    break;
                case 2:
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Crysalizer3"), Projectile.position);
                    break;
                case 3:
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Crysalizer4"), Projectile.position);
                    break;
            }
            for (float f = 0; f < 12; f++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(),
                    (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.Pink, Main.rand.NextFloat(1f, 3f)).noGravity = true;
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
    }

    public class CrysalizerArrow3 : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[1];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Archarilite Arrow");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.knockBack = 12.9f;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            AIType = ProjectileID.Bullet;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
        }

        public override void AI()
        {
            Timer++;
            NPC nearest = ProjectileHelper.FindNearestEnemy(Projectile.position, 1024);
            if (nearest != null && Projectile.penetrate < 3)
            {
                Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, nearest.Center, degreesToRotate: 6f);
            }
            if (Timer % 6 == 0)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(), Projectile.velocity * 0.1f, 0, Color.CornflowerBlue, Main.rand.NextFloat(1f, 1.5f));
            }
            if (Main.rand.NextBool(5))
            {
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.BlueFairy, 0f, 0f, 150, Color.White, 1f);
                Main.dust[dustnumber].velocity *= 0.3f;
            }
            Lighting.AddLight(Projectile.Center, Color.Orange.ToVector3() * 1.75f * Main.essScale);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }
            Projectile.velocity *= 0.75f;
            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
            {
                Projectile.Kill();
            }
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            switch (Main.rand.Next(4))
            {
                case 0:
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Crysalizer1"), Projectile.position);
                    break;
                case 1:
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Crysalizer2"), Projectile.position);
                    break;
                case 2:
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Crysalizer3"), Projectile.position);
                    break;
                case 3:
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Crysalizer4"), Projectile.position);
                    break;
            }
            for (float f = 0; f < 12; f++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(),
                    (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.CornflowerBlue, Main.rand.NextFloat(1f, 3f)).noGravity = true;
            }
        }


        public override bool PreDraw(ref Color lightColor)
        {

            return false;
        }
    }
}