using Stellamod.Content.Dusts;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Desert.WeaponsCL
{
    public class Relagis : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 80;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 7;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2;
            Item.autoReuse = false;
            Item.shootSpeed = 30f;
            Item.shoot = ModContent.ProjectileType<RadiantOrb>();
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }
    }

    public class RadiantOrb : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private Player Owner => Main.player[Projectile.owner];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Type] = 72;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = int.MaxValue;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            SpriteBatch spriteBatch = Main.spriteBatch;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Rectangle frame = Projectile.Frame();
            Vector2 drawOrigin = frame.Size() / 2f;
            Color drawColor = Color.White.MultiplyRGB(lightColor);
            drawColor.A = 0;
            float drawRotation = Projectile.rotation;
            float drawScale = MathHelper.Lerp(0f, 1f, Easing.InOutCubic(Timer / 15f));
            spriteBatch.Draw(texture, drawPos, frame, drawColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);
            return false;
        }

        private bool ShouldConsumeMana()
        {
            // Should mana be consumed this frame?
            bool consume = Timer % 6 == 0;
            return consume;
        }

        public override void AI()
        {
            base.AI();
            Timer++;


            if (Main.myPlayer == Projectile.owner)
            {
                bool manaIsAvailable = !ShouldConsumeMana() || Owner.CheckMana(Owner.HeldItem.mana, true, false);

                // The Prism immediately stops functioning if the player is Cursed (player.noItems) or "Crowd Controlled", e.g. the Frozen debuff.
                // player.channel indicates whether the player is still holding down the mouse button to use the item.
                bool stillInUse = Owner.channel && manaIsAvailable && !Owner.noItems && !Owner.CCed;
                if (stillInUse && Timer % 6 == 0)
                {
                    Vector2 spawnPos = Projectile.Center;
                    Vector2 shootVelocity = Projectile.velocity * 12;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPos, shootVelocity,
                        ModContent.ProjectileType<GoldenHoes>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
                else if (!stillInUse)
                {
                    Projectile.Kill();
                }

                Projectile.velocity = (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.Zero);
                Projectile.netUpdate = true;
            }

            DrawHelper.AnimateTopToBottom(Projectile, 1);
            Owner.ChangeDir(Projectile.direction);
            Projectile.spriteDirection = Owner.direction;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.Center = Owner.Center + Projectile.velocity * 65;

            if (Timer == 1)
            {
                FXUtil.GlowCircleBoom(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.Yellow,
                    outerGlowColor: Color.DarkOrange, duration: 15, baseSize: 0.12f);
            }

            SetHoldPosition();
        }

        private void SetHoldPosition()
        {
            if (Main.myPlayer == Projectile.owner)
            {
                Owner.direction = Main.MouseWorld.X > Owner.MountedCenter.X ? 1 : -1;
            }

            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(90f)); // set arm position (90 degree offset since arm starts lowered)
            Vector2 armPosition = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)Math.PI / 2); // get position of hand

            armPosition.Y += Owner.gfxOffY;
            Owner.heldProj = Projectile.whoAmI;
        }
    }

    public class GoldenHoes : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            //The recording mode
        }
        public override void SetDefaults()
        {
            Projectile.damage = 100;
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.light = 1.5f;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = true;
            Projectile.penetrate = 2;
            Projectile.extraUpdates = 1;
            Projectile.hostile = false;

        }
        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        private ref float MaxDegreesRotate => ref Projectile.ai[1];
        public float Timer2;
        private bool Moved;
        private float alphaCounter = 0;
        int Spin = 0;
        public override void AI()
        {
            Timer++;
            if (Timer == 1)
            {
                int Sound = Main.rand.Next(1, 4);
                SoundStyle mySound = SoundID.Item42;
                if (Sound == 1)
                {
                    mySound = SoundID.Item42;
                }
                if (Sound == 2)
                {
                    mySound = new SoundStyle("Stellamod/Assets/Sounds/Morrowarrow");
                }
                if (Sound == 3)
                {
                    mySound = new SoundStyle("Stellamod/Assets/Sounds/CinderBraker");
                }
                mySound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(mySound, Projectile.position);
                if (Main.myPlayer == Projectile.owner)
                {
                    MaxDegreesRotate = Main.rand.NextFloat(0.5f, 4f);
                    Projectile.position += Main.rand.NextVector2Circular(32, 32);
                    Projectile.velocity = Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(15));
                    Projectile.netUpdate = true;
                }
            }

            float maxDetectDistance = 1024;
            NPC npc = ProjectileHelper.FindNearestEnemy(Projectile.position, maxDetectDistance);
            if (npc != null)
            {
                Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, npc.Center, degreesToRotate: MaxDegreesRotate);
            }
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


        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 2; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, default, 1f).noGravity = true;
            }

            float size = Main.rand.NextFloat(0.02f, 0.06f);
            float time = Main.rand.NextFloat(12, 24);
            FXUtil.GlowCircleBoom(Projectile.Center,
                innerColor: Color.White,
                glowColor: Color.Yellow,
                outerGlowColor: Color.DarkOrange, duration: time, baseSize: size);
            SoundEngine.PlaySound(SoundID.DD2_BetsysWrathImpact, Projectile.position);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

       

        public override bool PreDraw(ref Color lightColor)
        {
          
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D texture2D4 = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/DimLight").Value;
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(85f * alphaCounter), (int)(45f * alphaCounter), (int)(15f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.17f * (7 + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(85f * alphaCounter), (int)(45f * alphaCounter), (int)(15f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.17f * (7 + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(85f * alphaCounter), (int)(45f * alphaCounter), (int)(15f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.07f * (7 + 0.6f), SpriteEffects.None, 0f);
            Lighting.AddLight(Projectile.Center, Color.Yellow.ToVector3() * 1.0f * Main.essScale);
        }

    }
}