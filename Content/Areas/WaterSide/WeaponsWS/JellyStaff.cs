using Stellamod.Common;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WaterSide.WeaponsWS
{
    public class JellyMinionBuff : MinionBuff<JellyMinionProj> { }

    public class JellyStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true; // This lets the player target anywhere on the whole screen while using a controller
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 14;
            Item.knockBack = 3f;
            Item.mana = 10;
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.Swing;

            // These below are needed for a minion weapon
            Item.noMelee = true;
            Item.DamageType = DamageClass.Summon;
            Item.buffType = ModContent.BuffType<JellyMinionBuff>();
            // No buffTime because otherwise the item tooltip would say something like "1 minute duration"
            Item.shoot = ModContent.ProjectileType<JellyMinionProj>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // This is needed so the buff that keeps your minion alive and allows you to despawn it properly applies
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GSummon"), player.position);
            // Here you can change where the minion is spawned. Most vanilla minions spawn at the cursor position.
            // This is needed so the buff that keeps your minion alive and allows you to despawn it properly applies
            player.AddBuff(Item.buffType, 2);

            // Minions have to be spawned manually, then have originalDamage assigned to the damage of the summon item
            position = Main.MouseWorld;
            var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            projectile.originalDamage = Item.damage;

            // Since we spawned the projectile manually already, we do not need the game to spawn it for ourselves anymore, so return false
            return false;
        }
    }

    public class JellyMinionProj : ModProjectile
    {
        private static float _orbitingOffset;
        Player Owner => Main.player[Projectile.owner];
        ref float Timer => ref Projectile.ai[0];
        ref float TimerOffset => ref Projectile.ai[1];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Jelly Minion");
            // Sets the amount of frames this minion has on its spritesheet
            Main.projFrames[Projectile.type] = 4;
            // This is necessary for right-click targeting
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

            // These below are needed for a minion
            // Denotes that this projectile is a pet or minion
            Main.projPet[Projectile.type] = true;
            // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            // Don't mistake this with "if this is true, then it will automatically home". It is just for damage reduction for certain NPCs
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public sealed override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 28;
            // Makes the minion go through tiles freely
            Projectile.tileCollide = false;

            // These below are needed for a minion weapon
            // Only controls if it deals damage to enemies on contact (more on that later)
            Projectile.friendly = true;
            // Only determines the damage type
            Projectile.minion = true;
            // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
            Projectile.minionSlots = 1f;
            // Needed so the minion doesn't despawn on collision with enemies or tiles
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        // Here you can decide if your minion breaks things like grass or pots
        public override bool? CanCutTiles()
        {
            return false;
        }

        // This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
        public override bool MinionContactDamage()
        {
            return false;
        }

        public override void AI()
        {
            if (!SummonHelper.CheckMinionActive<JellyMinionBuff>(Owner, Projectile))
                return;

            _orbitingOffset += 0.03f;
            Projectile.Center = CalculateCirclePosition(Owner);
            SummonHelper.SearchForTargets(Owner, Projectile,
                out bool foundTarget,
                out float distanceFromTarget,
                out Vector2 targetCenter);

            if (foundTarget)
            {
                Timer++;
                if (Timer < 120)
                {
                    ChargeVisuals(Timer, 80);
                }

                if (Timer >= 120 + TimerOffset)
                {
                    if (Main.myPlayer == Projectile.owner)
                    {
                        TimerOffset = Main.rand.Next(0, 30);
                        Projectile.netUpdate = true;
                    }

                    SoundEngine.PlaySound(SoundID.DD2_LightningAuraZap);
                    Vector2 directionToTarget = Projectile.Center.DirectionTo(targetCenter);
                    Vector2 velocityToTarget = directionToTarget * 1;
                    int numProjectiles = Main.rand.Next(1, 3);
                    for (int p = 0; p < numProjectiles; p++)
                    {
                        // Rotate the velocity randomly by 30 degrees at max.
                        Vector2 newVelocity = velocityToTarget.RotatedByRandom(MathHelper.ToRadians(6));
                        newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                        Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, newVelocity,
                            ModContent.ProjectileType<JellyStaffLightningProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                    Timer = 0;
                }
            }
            else
            {

            }
            Visuals();
        }
        private void ChargeVisuals(float timer, float maxTimer)
        {

            float progress = timer / maxTimer;
            float minParticleSpawnSpeed = 8;
            float maxParticleSpawnSpeed = 2;
            int particleSpawnSpeed = (int)MathHelper.Lerp(minParticleSpawnSpeed, maxParticleSpawnSpeed, progress);
            if (timer % particleSpawnSpeed == 0)
            {
                for (int i = 0; i < 1; i++)
                {
                    Vector2 pos = Projectile.Center + Main.rand.NextVector2CircularEdge(64, 64);
                    Vector2 vel = (Projectile.Center - pos).SafeNormalize(Vector2.Zero) * 4;
                    Dust d = Dust.NewDustPerfect(pos, DustID.Electric, vel, 0, Color.White);
                    d.noGravity = true;
                }
            }
        }
        private Vector2 CalculateCirclePosition(Player owner)
        {
            //Get the index of this minion
            int minionIndex = SummonHelper.GetProjectileIndex(Projectile);

            //Now we can calculate the circle position	
            int fireflyCount = owner.ownedProjectileCounts[Type];
            float degreesBetween = 360 / (float)fireflyCount;
            float degrees = degreesBetween * minionIndex;
            float circleDistance = 64;
            Vector2 circlePosition = owner.Center + new Vector2(circleDistance, 0).RotatedBy(MathHelper.ToRadians(degrees + _orbitingOffset));
            return circlePosition;
        }

        private void Visuals()
        {
            // So it will lean slightly towards the direction it's moving
            Projectile.rotation = Projectile.velocity.X * 0.05f;

            // This is a simple "loop through all frames from top to bottom" animation
            int frameSpeed = 5;
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= frameSpeed)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }

            // Some visuals here
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
        }
    }

    public class JellyStaffLightningProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // Sets the amount of frames this minion has on its spritesheet
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 18;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.timeLeft = 180;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 25;
            Projectile.tileCollide = false;
        }

        private ref float AI_Timer => ref Projectile.ai[0];
        private ref float AI_Pattern => ref Projectile.ai[1];
        public override void AI()
        {
            AI_Timer++;
            //This runs every other frame
            if (AI_Timer % 2 == 0)
            {
                float degrees = 12;
                if (Main.myPlayer == Projectile.owner)
                {
                    if (AI_Pattern == 0)
                    {              //Randomly teleport to make the jagged effect
                        Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
                        direction = direction.RotatedBy(MathHelper.ToRadians(degrees));
                        float distance = Main.rand.NextFloat(16, 180);
                        Projectile.Center = Projectile.Center + direction * distance;
                        AI_Pattern++;
                    }
                    else if (AI_Pattern == 1)
                    {
                        Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
                        direction = direction.RotatedBy(MathHelper.ToRadians(-degrees));
                        float distance = Main.rand.NextFloat(16, 180);
                        Projectile.Center = Projectile.Center + direction * distance;
                        AI_Pattern--;
                    }
                    Projectile.netUpdate = true;
                }
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
        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * 12;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public static Color ColorFunction(float completionRatio)
        {
            Color startColor = Color.Blue;
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
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), position, previousPosition, 10, ref collisionPoint))
                    return true;
            }
            return base.Colliding(projHitbox, targetHitbox);
        }
    }
}