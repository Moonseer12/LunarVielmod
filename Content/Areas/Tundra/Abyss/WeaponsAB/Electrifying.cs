using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.GunSwapping;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Stellamod.Content.Areas.Tundra.Abyss.WeaponsAB
{
    public class Electrifying : MiniGun
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 16;
            LeftHand = true;
            RightHand = true;
            TwoHands = true;

            SoundStyle soundStyle = new("Stellamod/Assets/Sounds/GunElectric");
            soundStyle.PitchVariance = 0.5f;
            Item.UseSound = soundStyle;

            //Higher is faster
            AttackSpeed = 4;

            //Offset it so it doesn't hold gun by weird spot
            HolsterOffset = new Vector2(15, -6);

            //Recoil
            RecoilDistance = 3;
        }

        public override void Fire(Player player, Vector2 position, Vector2 velocity, int damage, float knockback)
        {
            base.Fire(player, position, velocity, damage, knockback);
            if (player.PickAmmo(Item, out int projToShoot, out float speed, out int newDamage, out float knockBack, out int usedAmmoItemId))
            {
                float spread = 0.4f;
                for (int k = 0; k < 4; k++)
                {
                    Vector2 newDirection = velocity.RotatedByRandom(spread);
                    Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.LightCyan, Main.rand.NextFloat(0.2f, 0.5f));
                }
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.LightCyan, 1);
                int numProjectiles = Main.rand.Next(1, 2);
                for (int p = 0; p < numProjectiles; p++)
                {
                    // Rotate the velocity randomly by 30 degrees at max.
                    Vector2 vel = velocity * 8;
                    Vector2 newVelocity = vel.RotatedByRandom(MathHelper.ToRadians(6));
                    newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                    if (Main.myPlayer == player.whoAmI)
                    {
                        Projectile.NewProjectileDirect(player.GetSource_FromThis(), position, newVelocity,
                        ModContent.ProjectileType<ElectrifyingProj>(), damage, knockback, player.whoAmI);
                    }
                }

                SoundStyle soundStyle = new("Stellamod/Assets/Sounds/GunShootNew9");
                soundStyle.PitchVariance = 0.5f;
                SoundEngine.PlaySound(soundStyle, position);
            }
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<ConvulgingMater, BlankGun>();
        }
    }

    public class ElectrifyingProj : ModProjectile
    {
        float Timer;
        bool FadeOut;
        public override void SetStaticDefaults()
        {
            // Sets the amount of frames this minion has on its spritesheet
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 18;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.width = 16;
            Projectile.height = 16;

            Projectile.penetrate = -1;
            Projectile.timeLeft = 360;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = Projectile.timeLeft;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 4;
        }

        private ref float AI_Timer => ref Projectile.ai[0];
        private ref float Distance => ref Projectile.ai[1];
        private ref float Seed => ref Projectile.ai[2];
        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Projectile.oldPos[i] = Projectile.position;
            }
        }

        public override void AI()
        {
            AI_Timer++;

            if (AI_Timer % 2 == 0)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    Distance = Main.rand.NextFloat(2, 8);
                    Seed = Main.rand.Next(0, int.MaxValue);
                    Projectile.netUpdate = true;
                }
            }

            if (Distance != 0)
            {
                //Randomly teleport to make the jagged effect
                UnifiedRandom random = new((int)Seed);
                float maxRadians = MathHelper.ToRadians(210);
                double radians = random.NextDouble() * maxRadians - Main.rand.NextDouble() * maxRadians;

                Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
                direction = direction.RotatedByRandom(radians);
                Projectile.Center = Projectile.Center + direction * Distance;
                Distance = 0;
            }

            if (FadeOut)
            {
                Timer++;
            }

            //Dunno if this is needed but whatever
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //Electrifying!!!! nEMIES!!!
            target.AddBuff(BuffID.Electrified, 120);
            SoundEngine.PlaySound(SoundID.DD2_LightningBugZap, Projectile.position);

            for (int i = 0; i < 8; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(1, 1);
                var d = Dust.NewDustPerfect(Projectile.Center, DustID.Electric, speed, Scale: 1.5f);
                d.noGravity = true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            FadeOut = true;
            return false;
        }
        public float WidthFunction(float completionRatio)
        {
            float fadeScale = Timer / 30;
            fadeScale = MathHelper.Clamp(fadeScale, 0f, 1f);
            fadeScale = 1f - fadeScale;
            float baseWidth = Projectile.scale * 8 * fadeScale;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public static Color ColorFunction(float completionRatio)
        {
            Color startColor = Color.Cyan;
            Color endColor = Color.Transparent;
            return Color.Lerp(startColor, endColor, completionRatio);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //This damages everything in the trail
            Vector2[] positions = Projectile.oldPos;
            float collisionPoint = 0;
            for (int i = 1; i < positions.Length; i++)
            {
                Vector2 position = positions[i];
                Vector2 previousPosition = positions[i - 1];
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), position, previousPosition, 1, ref collisionPoint))
                    return true;
            }
            return base.Colliding(projHitbox, targetHitbox);
        }
    }
}
