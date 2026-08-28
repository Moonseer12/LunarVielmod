using Stellamod.Common;
using Stellamod.Common.MagicCauldron;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Dusts;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Snow.WeaponsSN
{
    public class AuroranSeekerMinionBuff : MinionBuff<AuroranSeekerMinionProj> { }

    public class AuroranSeeker : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true; // This lets the player target anywhere on the whole screen while using a controller
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;

            ItemID.Sets.StaffMinionSlotsRequired[Type] = 1f; // The default value is 1, but other values are supported. See the docs for more guidance. 
        }

        public override void SetDefaults()
        {
            Item.damage = 10;
            Item.knockBack = 3f;
            Item.mana = 10; // mana cost
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.HoldUp; // how the player's arm moves when using the item
            Item.value = Item.sellPrice(gold: 30);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item44; // What sound should play when using the item

            // These below are needed for a minion weapon
            Item.noMelee = true; // this item doesn't do any melee damage
            Item.DamageType = DamageClass.Summon; // Makes the damage register as summon. If your item does not have any damage type, it becomes true damage (which means that damage scalars will not affect it). Be sure to have a damage type
            Item.buffType = ModContent.BuffType<AuroranSeekerMinionBuff>();
            // No buffTime because otherwise the item tooltip would say something like "1 minute duration"
            Item.shoot = ModContent.ProjectileType<AuroranSeekerMinionProj>(); // This item creates the minion projectile
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            // Here you can change where the minion is spawned. Most vanilla minions spawn at the cursor position
            position = Main.MouseWorld;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // This is needed so the buff that keeps your minion alive and allows you to despawn it properly applies
            player.AddBuff(Item.buffType, 2);

            // Minions have to be spawned manually, then have originalDamage assigned to the damage of the summon item
            var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer);
            projectile.originalDamage = Item.damage;

            // Since we spawned the projectile manually already, we do not need the game to spawn it for ourselves anymore, so return false
            return false;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<WinterbornShard, BlankStaff>();
        }
    }

    public class AuroranSeekerMinionProj : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Spragald");
            // Sets the amount of frames this minion has on its spritesheet
            Main.projFrames[Projectile.type] = 6;
            // This is necessary for right-click targeting
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

            Main.projPet[Projectile.type] = true; // Denotes that this projectile is a pet or minion

            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true; // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; ; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
        }

        public sealed override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.tileCollide = false; // Makes the minion go through tiles freely
            Projectile.damage = 15;
            // These below are needed for a minion weapon
            Projectile.friendly = true; // Only controls if it deals damage to enemies on contact (more on that later)
            Projectile.minion = true; // Declares this as a minion (has many effects)
            Projectile.DamageType = DamageClass.Summon; // Declares the damage type (needed for it to deal damage)
            Projectile.minionSlots = 1f; // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
            Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
            Projectile.scale = 1f;
        }

        public override bool MinionContactDamage()
        {
            return false;
        }

        // Her
        // e you can decide if your minion breaks things like grass or pots
        public override bool? CanCutTiles()
        {
            return true;
        }
        // This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)

        // The AI of this minion is split into multiple methods to avoid bloat. This method just passes values between calls actual parts of the AI.
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (!SummonHelper.CheckMinionActive<AuroranSeekerMinionBuff>(owner, Projectile))
                return;


            SearchForTargets(owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);
            int minionIndex = SummonHelper.GetProjectileIndex(Projectile);

            //Now we can calculate the circle position	
            int fireflyCount = owner.ownedProjectileCounts[Type];
            float degreesBetweenFirefly = 360 / (float)fireflyCount;
            float degrees = degreesBetweenFirefly * minionIndex;
            float circleDistance = 48f;

            Vector2 circlePosition = owner.Center + new Vector2(circleDistance, 0).RotatedBy(MathHelper.ToRadians(degrees + Main.GlobalTimeWrappedHourly * 64));
            Projectile.velocity = (circlePosition - Projectile.Center) * 0.1f;
            Visuals();

            Timer++;
            if (Timer >= 120 && foundTarget && distanceFromTarget < 2048)
            {

                if (Main.myPlayer == Projectile.owner)
                {
                    Vector2 velocity = -Vector2.UnitY * 8;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
                        ModContent.ProjectileType<SeekerProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
                }

                Timer = 0;
            }
        }

        private void SearchForTargets(Player owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter)
        {
            // Starting search distance
            distanceFromTarget = 700f;
            targetCenter = Projectile.position;
            foundTarget = false;

            // This code is required if your minion weapon has the targeting feature
            if (owner.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[owner.MinionAttackTargetNPC];
                float between = Vector2.Distance(npc.Center, Projectile.Center);

                // Reasonable distance away so it doesn't target across multiple screens
                if (between < 2000f)
                {
                    distanceFromTarget = between;
                    targetCenter = npc.Center;
                    foundTarget = true;
                }
            }

            if (!foundTarget)
            {
                // This code is required either way, used for finding a target
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];

                    if (npc.CanBeChasedBy())
                    {
                        float between = Vector2.Distance(npc.Center, Projectile.Center);
                        bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between;
                        bool inRange = between < distanceFromTarget;
                        bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);
                        // Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
                        // The number depends on various parameters seen in the movement code below. Test different ones out until it works alright
                        bool closeThroughWall = between < 100f;

                        if ((closest && inRange || !foundTarget) && (lineOfSight || closeThroughWall))
                        {
                            distanceFromTarget = between;
                            targetCenter = npc.Center;
                            foundTarget = true;
                        }
                    }
                }
            }
            // friendly needs to be set to true so the minion can deal contact damage
            // friendly needs to be set to false so it doesn't damage things like target dummies while idling
            // Both things depend on if it has a target or not, so it's just one assignment here
            // You don't need this assignment if your minion is shooting things instead of dealing contact damage
            Projectile.friendly = foundTarget;
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

    public class SeekerProj : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 18;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = 2;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 240;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override void AI()
        {

            Timer++;
            if (Timer < 60)
            {
                Projectile.velocity.Y *= 0.9f;
            }
            if (Timer == 71)
            {
                Projectile.velocity.Y = 2f;
            }
            if (Timer % 12 == 0)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(), Projectile.velocity * 0.1f, 0, Color.White, 1f).noGravity = true;
            }

            float maxDetectDistance = 1024;
            NPC nearest = ProjectileHelper.FindNearestEnemy(Projectile.position, maxDetectDistance);
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Timer > 71)
            {
                if (nearest != null)
                {
                    Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, nearest.Center, degreesToRotate: 6f);
                }
                if (Projectile.velocity.Length() < 15)
                    Projectile.velocity *= 1.02f;
            }

        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            int style = Main.rand.Next(0, 7);
            float num = Main.rand.Next(3, 8);
            Projectile.velocity *= 0.25f;
            if (Projectile.penetrate == 2)
                return;
            for (float n = 0; n < num; n++)
            {
                Vector2 velocity = Vector2.UnitY.RotateRandom(MathHelper.TwoPi) * Main.rand.NextFloat(1f, 6f);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, velocity,
                    ModContent.ProjectileType<AuroranGlyph>(), (int)(Projectile.damage * 0.5f), Projectile.knockBack, Projectile.owner, ai1: style);
            }
            for (float i = 0; i < 4; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: new Color(Main.rand.Next(0, 255), Main.rand.Next(0, 255), Main.rand.Next(0, 255)),
                    outerGlowColor: Color.Black,
                    baseSize: Main.rand.NextFloat(0.06f, 0.12f));
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }


        }


        private void DrawEnergyBall()
        {
            //Draw Code for the orb
            Texture2D texture = ModContent.Request<Texture2D>(TextureRegistry.EmptyGlowParticle).Value;
            Vector2 centerPos = Projectile.Center - Main.screenPosition;
            GlowCircleShader shader = GlowCircleShader.Instance;

            //How quickly it lerps between the colors
            shader.Speed = 10f;

            //This effects the distribution of colors
            shader.BasePower = 2.5f;

            //Radius of the circle
            shader.Size = 0.06f;


            //Colors
            Color startInner = Color.White;
            Color startGlow = new(VectorHelper.Osc(0f, 1f, speed: 2), VectorHelper.Osc(0f, 1f, speed: 4), VectorHelper.Osc(0f, 1f, speed: 7));
            Color startOuterGlow = Color.Black;

            if (Projectile.penetrate == 1)
            {
                startOuterGlow = startGlow;
                startGlow = Color.White;

                shader.Size *= 1.5f;
            }

            shader.InnerColor = startInner;
            shader.GlowColor = startGlow;
            shader.OuterGlowColor = startOuterGlow;

            //Idk i just included this to see how it would look
            //Don't go above 0.5;
            shader.Pixelation = 0.005f;

            //This affects the outer fade
            shader.OuterPower = 13.5f;
            shader.Apply();


            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Restart(blendState: BlendState.Additive, effect: shader.Effect);
            for (int i = 0; i < 2; i++)
            {
                spriteBatch.Draw(texture, centerPos, null, Color.White, Projectile.rotation, texture.Size() / 2f, 1f, SpriteEffects.None, 0);
            }

            spriteBatch.RestartDefaults();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawEnergyBall();
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for (int i = 0; i < 4; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.White, 1f).noGravity = true;
            }
        }
    }
    
    public class AuroranGlyph : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private int Style => (int)Projectile.ai[1];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Spragald");
            // Sets the amount of frames this minion has on its spritesheet

            // This is necessary for right-click targeting
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;

            // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = false; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.tileCollide = false; // Makes the minion go through tiles freely					// These below are needed for a minion weapon
            Projectile.friendly = true; // Only controls if it deals damage to enemies on contact (more on that later)// Declares this as a minion (has many effects)
            Projectile.DamageType = DamageClass.Summon; // Declares the damage type (needed for it to deal damage) // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
            Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
            Projectile.timeLeft = 60;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        // Here you can decide if your minion breaks things like grass or pots
        // The AI of this minion is split into multiple methods to avoid bloat. This method just passes values between calls actual parts of the AI.
        public override void AI()
        {
            Timer++;
            Projectile.velocity.Y -= 0.01f;
            Projectile.velocity.X *= 0.98f;
            Projectile.rotation += VectorHelper.Osc(-0.001f, 0.001f, offset: Projectile.whoAmI);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            switch (Style)
            {
                case 0:
                    //No other effects
                    break;
                case 1:
                    target.AddBuff(BuffID.OnFire, 180);
                    break;
                case 2:
                    target.AddBuff(BuffID.Poisoned, 180);
                    break;
                case 3:
                    target.AddBuff(BuffID.Confused, 180);
                    break;
                case 4:
                    target.AddBuff(BuffID.Lovestruck, 180);
                    break;
                case 5:
                    target.AddBuff(BuffID.Ichor, 180);
                    break;
                case 6:
                    target.AddBuff(BuffID.Frostburn, 180);
                    break;
            }
        }

        private string GetTexturePath()
        {
            string baseTexture = Texture;
            if (Style == 0)
                return Texture;
            else
            {
                return Texture + "_" + Style;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            string texturePath = GetTexturePath();
            Texture2D texture = ModContent.Request<Texture2D>(texturePath).Value;
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Restart(blendState: BlendState.Additive);

            float alphaProgress = (Timer - 40) / 20f;
            alphaProgress = 1f - alphaProgress;
            float num = 8f;
            Color spriteColor = Color.White;
            switch (Style)
            {
                case 1:
                    spriteColor = Color.OrangeRed;
                    break;
                case 2:
                    spriteColor = Color.GreenYellow;
                    break;
                case 3:
                    spriteColor = Color.Purple;
                    break;
                case 4:
                    spriteColor = Color.Pink;
                    break;
                case 5:
                    spriteColor = Color.Yellow;
                    break;
                case 6:
                    spriteColor = Color.Cyan;
                    break;
            }
            spriteColor *= alphaProgress;
            for (float f = 0; f < num; f++)
            {
                Vector2 offset = (((f / num) * MathHelper.TwoPi) + Main.GlobalTimeWrappedHourly).ToRotationVector2() * VectorHelper.Osc(2f, 7f);
                Color glowColor = spriteColor.MultiplyRGB(lightColor) * 0.4f;
                spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + offset, null, glowColor, Projectile.rotation, texture.Size() / 2f, 1f, SpriteEffects.None, 0);
            }


            for (int i = 0; i < 2; i++)
            {
                spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White.MultiplyRGB(lightColor) * alphaProgress, Projectile.rotation, texture.Size() / 2f, 1f, SpriteEffects.None, 0);
            }

            spriteBatch.RestartDefaults();
            return false;
        }
    }
}