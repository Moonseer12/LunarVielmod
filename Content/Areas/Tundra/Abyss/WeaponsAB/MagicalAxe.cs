using Stellamod.Common.MagicCauldron;
using Stellamod.Content.Areas.WaterSide.WeaponsWS;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Dusts;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.WeaponsAB
{
    public class MagicalAxe : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 70;
            Item.mana = 30;
            Item.useTime = 44;
            Item.useAnimation = 44;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.knockBack = 0f;
            Item.DamageType = DamageClass.Magic;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<MagicalAxeBoomer>();
            Item.autoReuse = true;
            Item.noUseGraphic = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI, 0f, 0f);
            return false;

        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<ConvulgingMater, BlankStaff>();
        }
    }

    public class MagicalAxeBoomer : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                for (float f = 0; f < 4; f++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(),
                        (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.Blue, Main.rand.NextFloat(1f, 3f)).noGravity = true;
                }
                for (float f = 0; f < 4; f++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(),
                        (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.Blue, Main.rand.NextFloat(1f, 3f)).noGravity = true;
                }
                for (float i = 0; i < 4; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(360);
                    rot += Main.rand.NextFloat(-0.5f, 0.5f);
                    var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                        innerColor: Color.White,
                        glowColor: Color.Blue,
                        outerGlowColor: Color.Black,
                        duration: Main.rand.NextFloat(6, 12),
                        baseSize: Main.rand.NextFloat(0.01f, 0.05f));
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                }

                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/StarFlower1"), Projectile.position);
            }

            if (Timer == 10)
            {
                for (float f = 0; f < 4; f++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(),
                        (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.Blue, Main.rand.NextFloat(1f, 3f)).noGravity = true;
                }
                for (float f = 0; f < 4; f++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(),
                        (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.Blue, Main.rand.NextFloat(1f, 3f)).noGravity = true;
                }
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                        ModContent.ProjectileType<SparklyBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }

                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/StarFlower3"), Projectile.position);
                ShakeScreenPosition.Shake = 3;
                FXUtil.ShakeCamera(Projectile.position, 1024, 4);

            }
            if (Timer == 30)
            {
                for (float f = 0; f < 4; f++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(),
                        (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.Blue, Main.rand.NextFloat(1f, 3f)).noGravity = true;
                }
                for (float f = 0; f < 4; f++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(),
                        (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.Blue, Main.rand.NextFloat(1f, 3f)).noGravity = true;
                }
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                        ModContent.ProjectileType<BlossomBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/StarSheith"), Projectile.position);
                FXUtil.ShakeCamera(Projectile.position, 1024, 4);
                Projectile.Kill();
            }

        }
    }

    public class BlossomBoom : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("FrostShotIN");
            Main.projFrames[Projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            Projectile.localNPCHitCooldown = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.friendly = true;
            Projectile.width = 334;
            Projectile.height = 292;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 21;
            Projectile.scale = 1f;
        }

        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public override void AI()
        {

            Vector3 RGB = new(0.89f, 2.53f, 2.55f);
            // The multiplication here wasn't doing anything
            Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);

        }



        public override bool PreAI()
        {
            Projectile.tileCollide = false;
            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 7)
                {
                    Projectile.frame = 0;
                }
            }
            return true;


        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 50f);
        }


    }
}