using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.GunSwapping;
using Stellamod.Dusts;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Snow.WeaponsSN
{
    public class MintyBlast : MiniGun
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 10;
            RightHand = true;

            SoundStyle soundStyle = new("Stellamod/Assets/Sounds/HarmonicBlasphemy1");
            soundStyle.PitchVariance = 0.5f;
            Item.UseSound = soundStyle;

            //Higher is slower
            AttackSpeed = 3;

            //Offset it so it doesn't hold gun by weird spot
            HolsterOffset = new Vector2(15, -6);

            //Recoil
            RecoilDistance = 0;
            RecoilRotation = 0;
            RecoilRotationMini = 0;
        }

        public override void Fire(Player player, Vector2 position, Vector2 velocity, int damage, float knockback)
        {
            base.Fire(player, position, velocity, damage, knockback);
            if (!player.PickAmmo(Item, out int projToShoot, out float speed, out int newDamage, out float knockBack, out int usedAmmoItemId))
                return;
            float spread = 0.4f;
            for (int k = 0; k < 7; k++)
            {
                Vector2 newDirection = velocity.RotatedByRandom(spread);
                Dust.NewDustPerfect(position, ModContent.DustType<GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.DarkBlue, Main.rand.NextFloat(0.2f, 0.5f));
            }
            Dust.NewDustPerfect(position, ModContent.DustType<GlowDust>(), new Vector2(0, 0), 125, Color.Blue, 1);
            projToShoot = Main.rand.Next([ModContent.ProjectileType<FroBall2>(), ModContent.ProjectileType<FroBall1>()]);

            if (Main.myPlayer == player.whoAmI)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), position, velocity * 8, projToShoot, damage, knockback, player.whoAmI);
            }

            SoundStyle soundStyle = new("Stellamod/Assets/Sounds/HarmonicBlasphemy1");
            soundStyle.PitchVariance = 0.5f;
            SoundEngine.PlaySound(soundStyle, position);
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<WinterbornShard, BlankGun>();
        }
    }

    public class MintyBlastProj : ModProjectile
    {
        //Don't change the sample points, 3 is good enough
        private const int NumSamplePoints = 3;

        private const float MaxBeamLength = 2400f;

        public float BeamLength;
        public List<Vector2> BeamPoints;

        //No texture for this
        public override string Texture => TextureRegistry.EmptyTexture;

        float Timer;
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 20;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            BeamPoints = new();
        }

        public override void AI()
        {
            float targetBeamLength = PerformBeamHitscan();
            BeamLength = targetBeamLength;
            Timer++;
            if (Timer == 1)
            {
                Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
                Vector2 explosionCenter = Projectile.Center + direction * BeamLength;

                for (int i = 0; i < 5; i++)
                {
                    Dust.NewDustPerfect(explosionCenter, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.NextFloat(0.00f, 1.00f)).RotatedByRandom(19.0), 0, Color.LightSkyBlue, 1f).noGravity = true;
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = 0f;
            float width = Projectile.width * 0.8f * 0.4f;
            Vector2 start = Projectile.Center;

            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
            Vector2 end = start + direction * (BeamLength - 80f);
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, width, ref _);
        }

        private float PerformBeamHitscan()
        {
            // By default, the hitscan interpolation starts at the Projectile's center.
            // If the host Prism is fully charged, the interpolation starts at the Prism's center instead.
            Vector2 samplingPoint = Projectile.Center;

            // Perform a laser scan to calculate the correct length of the beam.
            // Alternatively, if you want the beam to ignore tiles, just set it to be the max beam length with the following line.
            // return MaxBeamLength;
            float[] laserScanResults = new float[NumSamplePoints];


            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
            Collision.LaserScan(samplingPoint, direction, 0 * Projectile.scale, MaxBeamLength, laserScanResults);
            float averageLengthSample = 0f;
            for (int i = 0; i < laserScanResults.Length; ++i)
            {
                averageLengthSample += laserScanResults[i];
            }
            averageLengthSample /= NumSamplePoints;
            return averageLengthSample;
        }

        public override bool PreDraw(ref Color lightColor) => false;
        public override bool ShouldUpdatePosition() => false;
    }


    public class FroBall1 : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.damage = 12;
            Projectile.width = 12;
            Projectile.height = 24;
            Projectile.light = 1.5f;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ownerHitCheck = true;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }
        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public float Timer2;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            switch (Main.rand.Next(0, 4))
            {
                case 0:
                    target.AddBuff(BuffID.Frostburn, 120);
                    break;
                case 1:
                    target.AddBuff(BuffID.Frostburn, 320);
                    break;
                case 2:
                    target.AddBuff(BuffID.Frostburn2, 120);
                    break;
                case 3:
                    target.AddBuff(BuffID.Frostburn, 60);
                    break;
            }
        }
        public override void AI()
        {
            Timer2++;
            Projectile.velocity *= 0.99f;
            Timer++;
            if (Timer == 4)
            {
                Timer = 0;
            }



            float maxDetectRadius = 3f; // The maximum radius at which a projectile can detect a target
            float projSpeed = 25f; // The speed at which the projectile moves towards the target

            if (Timer2 == 0)
            {
                maxDetectRadius = 0f;

            }

            if (Timer2 == 25)
            {
                maxDetectRadius = 0f;
                Timer2 = 0;
            }



            // Trying to find NPC closest to the projectile
            NPC closestNPC = FindClosestNPC(maxDetectRadius);
            if (closestNPC == null)
                return;

            // If found, change the velocity of the projectile and turn it in the direction of the target
            // Use the SafeNormalize extension method to avoid NaNs returned by Vector2.Normalize when the vector is zero
            Projectile.velocity = (closestNPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * projSpeed;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);

        }
        // Finding the closest NPC to attack within maxDetectDistance range
        // If not found then returns null
        public NPC FindClosestNPC(float maxDetectDistance)
        {
            NPC closestNPC = null;

            // Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            // Loop through all NPCs(max always 200)
            for (int k = 0; k < Main.maxNPCs; k++)
            {
                NPC target = Main.npc[k];
                // Check if NPC able to be targeted. It means that NPC is
                // 1. active (alive)
                // 2. chaseable (e.g. not a cultist archer)
                // 3. max life bigger than 5 (e.g. not a critter)
                // 4. can take damage (e.g. moonlord core after all it's parts are downed)
                // 5. hostile (!friendly)
                // 6. not immortal (e.g. not a target dummy)
                if (target.CanBeChasedBy())
                {
                    // The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

                    // Check if it is within the radius
                    if (sqrDistanceToTarget < sqrMaxDetectDistance)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        closestNPC = target;
                    }
                }
            }

            Projectile.rotation += 0.1f;
            {


                Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? 1 : -1;
                Projectile.rotation = Projectile.velocity.ToRotation();
                if (Projectile.velocity.Y > 25f)
                {
                    Projectile.velocity.Y = 25f;
                }
            }
            return closestNPC;
        }



    }


    public class FroBall2 : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.damage = 12;
            Projectile.width = 12;
            Projectile.height = 24;
            Projectile.light = 1.5f;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ownerHitCheck = true;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }
        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public float Timer2;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            switch (Main.rand.Next(0, 4))
            {
                case 0:
                    target.AddBuff(BuffID.Frostburn, 120);
                    break;
                case 1:
                    target.AddBuff(BuffID.Frostburn, 320);
                    break;
                case 2:
                    target.AddBuff(BuffID.Frostburn2, 120);
                    break;
                case 3:
                    target.AddBuff(BuffID.Frostburn, 60);
                    break;
            }
        }
        public override void AI()
        {
            Timer2++;
            Projectile.velocity *= 0.99f;
            Timer++;
            if (Timer == 4)
            {



                for (int j = 0; j < 1; j++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(0.2f, 0.2f);
                    Vector2 speed2 = Projectile.velocity / 2 + Main.rand.NextVector2Circular(0.5f, 0.5f);
                }
                Timer = 0;
            }



            float maxDetectRadius = 3f; // The maximum radius at which a projectile can detect a target
            float projSpeed = 25f; // The speed at which the projectile moves towards the target

            if (Timer2 == 0)
            {
                maxDetectRadius = 0f;

            }

            if (Timer2 == 25)
            {
                maxDetectRadius = 0f;
                Timer2 = 0;
            }



            // Trying to find NPC closest to the projectile
            NPC closestNPC = FindClosestNPC(maxDetectRadius);
            if (closestNPC == null)
                return;

            // If found, change the velocity of the projectile and turn it in the direction of the target
            // Use the SafeNormalize extension method to avoid NaNs returned by Vector2.Normalize when the vector is zero
            Projectile.velocity = (closestNPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * projSpeed;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);

        }
        // Finding the closest NPC to attack within maxDetectDistance range
        // If not found then returns null
        public NPC FindClosestNPC(float maxDetectDistance)
        {
            NPC closestNPC = null;

            // Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            // Loop through all NPCs(max always 200)
            for (int k = 0; k < Main.maxNPCs; k++)
            {
                NPC target = Main.npc[k];
                // Check if NPC able to be targeted. It means that NPC is
                // 1. active (alive)
                // 2. chaseable (e.g. not a cultist archer)
                // 3. max life bigger than 5 (e.g. not a critter)
                // 4. can take damage (e.g. moonlord core after all it's parts are downed)
                // 5. hostile (!friendly)
                // 6. not immortal (e.g. not a target dummy)
                if (target.CanBeChasedBy())
                {
                    // The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

                    // Check if it is within the radius
                    if (sqrDistanceToTarget < sqrMaxDetectDistance)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        closestNPC = target;
                    }
                }
            }

            Projectile.rotation += 0.1f;
            {


                Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? 1 : -1;
                Projectile.rotation = Projectile.velocity.ToRotation();
                if (Projectile.velocity.Y > 25f)
                {
                    Projectile.velocity.Y = 25f;
                }
            }
            return closestNPC;
        }



    }
}