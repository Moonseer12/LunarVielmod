using Stellamod.Content.Gores;
using Stellamod.Content.Trailers;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.WeaponsPT
{
    public class TheMarksman : ModItem
    {
        public override void SetDefaults()
        {
            Item.staff[Item.type] = true;
            Item.damage = 100;
            Item.mana = 50;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4f;
            Item.DamageType = DamageClass.Magic;
            Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/StormDragon_LightingZap");
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<MarksmanLightningProj>();
            Item.shootSpeed = 15f;
            Item.crit = 4;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            position = Main.MouseWorld + new Vector2(0, -768);
            velocity = Vector2.UnitY * velocity.Length();
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }
    }

    public class MarksmanLightningProj : ModProjectile
    {
        public float BeamLength;
        public Vector2[] BeamPoints;
        public override string Texture => TextureRegistry.EmptyTexture;
        private ref float Timer => ref Projectile.ai[0];

        public CoreLightning Lightning { get; set; } = new CoreLightning();
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 30;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            float targetBeamLength = ProjectileHelper.PerformBeamHitscan(Projectile.position, Vector2.UnitY, 2400);
            BeamLength = targetBeamLength;
            if (Timer == 1)
            {
                //Sound Effect Goooo
                SoundStyle lightningSoundStyle = new("Stellamod/Assets/Sounds/StormDragon_LightingZap");
                lightningSoundStyle.PitchVariance = 0.1f;
                SoundEngine.PlaySound(lightningSoundStyle, Projectile.position);

                Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
                for (int i = 0; i < 16; i++)
                {
                    Vector2 dustSpawnPoint = Projectile.Center + direction * BeamLength;
                    Vector2 dustVelocity = Main.rand.NextVector2Circular(8, 8);
                    Dust d = Dust.NewDustPerfect(dustSpawnPoint, DustID.GoldCoin, dustVelocity, Scale: 0.5f);
                    d.noGravity = true;
                }


                Vector2 lightningHitPos = Projectile.position + new Vector2(0, BeamLength);
                FXUtil.ShakeCamera(lightningHitPos, 1024, 32);

                for (int i = 0; i < 2; i++)
                {
                    Vector2 velocity = -Vector2.UnitY * Main.rand.NextFloat(4, 8);
                    velocity = velocity.RotatedByRandom(MathHelper.ToRadians(24));

                    Gore.NewGore(Projectile.GetSource_FromThis(), lightningHitPos, velocity,
                        ModContent.GoreType<FableRock1>());

                    velocity = -Vector2.UnitY * Main.rand.NextFloat(4, 8);
                    velocity = velocity.RotatedByRandom(MathHelper.ToRadians(24));

                    Gore.NewGore(Projectile.GetSource_FromThis(), lightningHitPos, velocity,
                        ModContent.GoreType<FableRock2>());

                    velocity = -Vector2.UnitY * Main.rand.NextFloat(4, 8);
                    velocity = velocity.RotatedByRandom(MathHelper.ToRadians(24));

                    Gore.NewGore(Projectile.GetSource_FromThis(), lightningHitPos, velocity,
                        ModContent.GoreType<FableRock3>());

                    velocity = -Vector2.UnitY * Main.rand.NextFloat(4, 8);
                    velocity = velocity.RotatedByRandom(MathHelper.ToRadians(24));

                    Gore.NewGore(Projectile.GetSource_FromThis(), lightningHitPos, velocity,
                        ModContent.GoreType<FableRock4>());
                }

            }

            for (int i = 0; i < Lightning.Trails.Length; i++)
            {
                float progress = i / (float)Lightning.Trails.Length;
                var trail = Lightning.Trails[i];
                trail.LightningRandomOffsetRange = MathHelper.Lerp(32, 8, progress) * MathHelper.Lerp(2f, 0, Timer / 30f);
                trail.LightningRandomExpand = MathHelper.Lerp(64, 16, progress);
                trail.PrimaryColor = Color.Lerp(Color.White, Color.Yellow, progress);
                trail.NoiseColor = Color.Lerp(Color.White, Color.Yellow, progress);
            }

            //Setup lightning stuff
            //Should make it scale in/out
            float lightningProgress = Timer / 30f;
            float easedLightningProgress = Easing.SpikeOutCirc(lightningProgress);
            Lightning.WidthMultiplier = MathHelper.Lerp(0f, 8, easedLightningProgress);
            if (Timer % 3 == 0)
            {
                List<Vector2> beamPoints = new List<Vector2>();
                Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
                for (int i = 0; i <= 8; i++)
                {
                    float maxProgress = MathHelper.Lerp(0f, 1f, Easing.OutExpo(Timer / 15f));
                    float progress = MathHelper.Lerp(0f, maxProgress, i / 8f);
                    beamPoints.Add(Vector2.Lerp(Projectile.Center, Projectile.Center + direction * BeamLength, progress));
                }
                BeamPoints = beamPoints.ToArray();
                Lightning.RandomPositions(BeamPoints);
            }
        }

        public override bool? CanDamage()
        {
            return Timer > 5 && Timer < 30;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = 0f;
            float width = Projectile.width * 0.8f;
            Vector2 start = Projectile.Center;

            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
            Vector2 end = start + direction * (BeamLength);
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, width, ref _);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Lightning.Draw(spriteBatch, BeamPoints, Projectile.oldRot);
            return false;
        }
    }
}