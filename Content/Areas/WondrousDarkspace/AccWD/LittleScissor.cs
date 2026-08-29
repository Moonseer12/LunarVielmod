using Stellamod.Content.Areas.WondrousDarkspace.WeaponsWD;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WondrousDarkspace.AccWD
{
    public class LittleScissorDashPlayer : ModPlayer
    {
        private int _riftDurationCounter;
        private int _riftCounter;
        // These indicate what direction is what in the timer arrays used
        public const int DashDown = 0;
        public const int DashUp = 1;
        public const int DashRight = 2;
        public const int DashLeft = 3;

        public const int DashCooldown = 25; // Time (frames) between starting dashes. If this is shorter than DashDuration you can start a new dash before an old one has finished
        public const int DashDuration = 5; // Duration of the dash afterimage effect in frames
        public const int RiftDuration = 30;
        // The initial velocity.  10 velocity is about 37.5 tiles/second or 50 mph
        public const float DashVelocity = 12f;

        // The direction the player has double tapped.  Defaults to -1 for no dash double tap
        public int DashDir = -1;

        // The fields related to the dash accessory
        public bool DashAccessoryEquipped;
        public int DashDelay = 0; // frames remaining till we can dash again
        public int DashTimer = 0; // frames remaining in the dash

        public override void ResetEffects()
        {
            // Reset our equipped flag. If the accessory is equipped somewhere, ExampleShield.UpdateAccessory will be called and set the flag before PreUpdateMovement
            DashAccessoryEquipped = false;

            // ResetEffects is called not long after player.doubleTapCardinalTimer's values have been set
            // When a directional key is pressed and released, vanilla starts a 15 tick (1/4 second) timer during which a second press activates a dash
            // If the timers are set to 15, then this is the first press just processed by the vanilla logic.  Otherwise, it's a double-tap
            if (Player.controlDown && Player.releaseDown && Player.doubleTapCardinalTimer[DashDown] < 15)
            {
                DashDir = DashDown;
            }
            else if (Player.controlUp && Player.releaseUp && Player.doubleTapCardinalTimer[DashUp] < 15)
            {
                DashDir = DashUp;
            }
            else if (Player.controlRight && Player.releaseRight && Player.doubleTapCardinalTimer[DashRight] < 15)
            {
                DashDir = DashRight;
            }
            else if (Player.controlLeft && Player.releaseLeft && Player.doubleTapCardinalTimer[DashLeft] < 15)
            {
                DashDir = DashLeft;
            }
            else
            {
                DashDir = -1;
            }
        }

        // This is the perfect place to apply dash movement, it's after the vanilla movement code, and before the player's position is modified based on velocity.
        // If they double tapped this frame, they'll move fast this frame
        public override void PreUpdateMovement()
        {
            // if the player can use our dash, has double tapped in a direction, and our dash isn't currently on cooldown
            if (CanUseDash() && DashDir != -1 && DashDelay == 0)
            {
                Vector2 newVelocity = Player.velocity;
                switch (DashDir)
                {
                    // Only apply the dash velocity if our current speed in the wanted direction is less than DashVelocity
                    case DashUp when Player.velocity.Y > -DashVelocity:
                    case DashDown when Player.velocity.Y < DashVelocity:
                        {
                            // Y-velocity is set here
                            // If the direction requested was DashUp, then we adjust the velocity to make the dash appear "faster" due to gravity being immediately in effect
                            // This adjustment is roughly 1.3x the intended dash velocity
                            float dashDirection = DashDir == DashDown ? 1 : -1.3f;
                            newVelocity.Y = dashDirection * DashVelocity;
                            break;
                        }
                    case DashLeft when Player.velocity.X > -DashVelocity:
                    case DashRight when Player.velocity.X < DashVelocity:
                        {
                            // X-velocity is set here
                            float dashDirection = DashDir == DashRight ? 1 : -1;
                            newVelocity.X = dashDirection * DashVelocity;
                            break;
                        }
                    default:
                        return; // not moving fast enough, so don't start our dash
                }

                // start our dash
                _riftDurationCounter = RiftDuration;
                DashDelay = DashCooldown;
                DashTimer = DashDuration;
                Player.velocity = newVelocity;

                //X Slash Visuals
                var xSlashPart1 = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, Vector2.Zero,
                    ModContent.ProjectileType<RipperSlashProjBig>(), 0, 0, owner: Player.whoAmI);
                var xSlashPart2 = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, Vector2.Zero,
                    ModContent.ProjectileType<RipperSlashProjBig>(), 0, 0, owner: Player.whoAmI);

                xSlashPart1.timeLeft = 1500;
                xSlashPart2.timeLeft = 1500;
                xSlashPart2.rotation = MathHelper.ToRadians(90);

                //Actual Attack Here
                var voidRiftProjectile1 = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, Vector2.Zero,
                    ModContent.ProjectileType<VoidRift>(), 120, 1, owner: Player.whoAmI);

                voidRiftProjectile1.timeLeft = 180;
                voidRiftProjectile1.rotation = MathHelper.ToRadians(-45);

                var voidRiftProjectile2 = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, Vector2.Zero,
                    ModContent.ProjectileType<VoidRift>(), 120, 1, owner: Player.whoAmI);

                voidRiftProjectile2.timeLeft = 180;
                voidRiftProjectile2.rotation = MathHelper.ToRadians(45);

                // Here you'd be able to set an effect that happens when the dash first activates
                // Some examples include:  the larger smoke effect from the Master Ninja Gear and Tabi
            }

            if (DashDelay > 0)
                DashDelay--;

            if (DashTimer > 0)
            { // dash is active
              // This is where we set the afterimage effect.  You can replace these two lines with whatever you want to happen during the dash
              // Some examples include:  spawning dust where the player is, adding buffs, making the player immune, etc.
              // Here we take advantage of "player.eocDash" and "player.armorEffectDrawShadowEOCShield" to get the Shield of Cthulhu's afterimage effect
                Player.eocDash = DashTimer;
                Player.armorEffectDrawShadowEOCShield = true;

                // count down frames remaining
                DashTimer--;

            }

            _riftCounter--;
            _riftDurationCounter--;
            if (_riftDurationCounter > 0 && _riftCounter <= 0)
            {
                Vector2 velocity = new Vector2(Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-4f, 4f));
                var proj = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, velocity,
                    ModContent.ProjectileType<LittleScissorVoidBolt>(), 54, 1, owner: Player.whoAmI);

                //Scale with all damage classe
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SyliaRiftClose"), Player.position);
                _riftCounter = 10;
            }
        }

        private bool CanUseDash()
        {
            return DashAccessoryEquipped
                && Player.dashType == 0 // player doesn't have Tabi or EoCShield equipped (give priority to those dashes)
                && !Player.setSolar // player isn't wearing solar armor
                && !Player.mount.Active; // player isn't mounted, since dashes on a mount look weird

        }
    }

    public class VoidRift : ModProjectile
    {
        private int _particleCounter;
        private const int Body_Particle_Count = 4;

        //Lower number = faster
        private const int Body_Particle_Rate = 2;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 30;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 150;
            Projectile.height = 40;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;

            //Local NPC hit time so it doesn't interfere with other piercing weps
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Draw The Body
            Vector3 huntrianColorXyz = DrawHelper.HuntrianColorOscillate(
                new Vector3(60, 0, 118),
                new Vector3(117, 1, 187),
                new Vector3(3, 3, 3), 0);

            DrawHelper.DrawDimLight(Projectile, huntrianColorXyz.X, huntrianColorXyz.Y, huntrianColorXyz.Z, ColorFunctions.MiracleVoid, lightColor, 1);
            DrawHelper.DrawAdditiveAfterImage(Projectile, ColorFunctions.MiracleVoid, Color.Black, ref lightColor);
            // Draw the periodic glow effect behind the item when dropped in the world (hence PreDrawInWorld)
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int projFrames = Main.projFrames[Projectile.type];
            int frameHeight = texture.Height / projFrames;
            int startY = frameHeight * Projectile.frame;

            Rectangle frame = new(0, startY, texture.Width, frameHeight);
            Vector2 drawOrigin = frame.Size() / 2f;
            float offsetX = 20f;
            drawOrigin.X = Projectile.spriteDirection == 1 ? frame.Width - offsetX : offsetX;

            Vector2 frameOrigin = frame.Size() / 2f;
            Vector2 offset = new(Projectile.width / 2 - frameOrigin.X, Projectile.height - frame.Height);
            Vector2 drawPos = Projectile.position - Main.screenPosition + frameOrigin + offset;

            float time = Main.GlobalTimeWrappedHourly;
            float timer = time * 0.04f;

            time %= 4f;
            time /= 2f;

            if (time >= 1f)
            {
                time = 2f - time;
            }

            time = time * 0.5f + 0.5f;
            for (float i = 0f; i < 1f; i += 0.25f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;
                Main.EntitySpriteDraw(texture, drawPos + new Vector2(0f, 8f).RotatedBy(radians) * time, frame, new Color(90, 70, 255, 50), Projectile.rotation, frameOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            for (float i = 0f; i < 1f; i += 0.34f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;
                Main.EntitySpriteDraw(texture, drawPos + new Vector2(0f, 4f).RotatedBy(radians) * time, frame, new Color(140, 120, 255, 77), Projectile.rotation, frameOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            return base.PreDraw(ref lightColor);
        }

        public override void AI()
        {
            Visuals();
        }

        private void Visuals()
        {
            _particleCounter++;
            if (_particleCounter > Body_Particle_Rate)
            {

                _particleCounter = 0;
            }

            float scaleOut = 20;
            if (Projectile.timeLeft < scaleOut && Projectile.DamageType != DamageClass.Summon)
            {
                Projectile.scale = MathHelper.Lerp(0f, 1f, Projectile.timeLeft / scaleOut);
            }

            DrawHelper.AnimateTopToBottom(Projectile, 3);
            Lighting.AddLight(Projectile.Center, Color.Pink.ToVector3() * 0.28f);
        }

        public override void OnKill(int timeLeft)
        {


            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SyliaRiftClose"));
        }
    }

    public class LittleScissorVoidBolt : ModProjectile
    {
        private int _particleCounter;
        private int _dustCounter;
        private float _projSpeed = 0;
        private float _maxProjSpeed;

        //AI Values
        //Visuals
        private const float Max_Proj_Speed_Min = 5f;
        private const float Max_Proj_Speed_Max = 10f;
        private const int Body_Radius = 4;
        private const int Body_Particle_Count = 1;
        private const int Kill_Particle_Count = 16;
        private const int Explosion_Particle_Count = 8;

        //Lower number = faster
        private const int Body_Particle_Rate = 1;
        private const int Body_Dust_Rate = 9;
        private const int Max_Detect_Radius = 1024;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            _maxProjSpeed = Main.rand.NextFloat(Max_Proj_Speed_Min, Max_Proj_Speed_Max);
            Projectile.width = 52;
            Projectile.height = 38;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = Main.rand.Next(200, 300);
        }

        public override void AI()
        {
            Visuals();
            // Trying to find NPC closest to the projectile
            NPC closestNPC = FindClosestNPC(Max_Detect_Radius);
            if (closestNPC == null)
                return;

            _projSpeed += 0.25f;
            if (_projSpeed > _maxProjSpeed)
            {
                _projSpeed = _maxProjSpeed;
            }
            // If found, change the velocity of the projectile and turn it in the direction of the target
            // Use the SafeNormalize extension method to avoid NaNs returned by Vector2.Normalize when the vector is zero
            Projectile.velocity = (closestNPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * _projSpeed;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

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

            return closestNPC;
        }

        //Visual Stuffs
        public override bool PreDraw(ref Color lightColor)
        {
            //Draw The Body
            Vector3 huntrianColorXyz = DrawHelper.HuntrianColorOscillate(
                new Vector3(60, 0, 118),
                new Vector3(117, 1, 187),
                new Vector3(3, 3, 3), 0);

            DrawHelper.DrawDimLight(Projectile, huntrianColorXyz.X, huntrianColorXyz.Y, huntrianColorXyz.Z, ColorFunctions.MiracleVoid, lightColor, 1);
            DrawHelper.DrawAdditiveAfterImage(Projectile, ColorFunctions.MiracleVoid, Color.Black, ref lightColor);
            return base.PreDraw(ref lightColor);
        }

        private void Visuals()
        {
            _particleCounter++;
            if (_particleCounter > Body_Particle_Rate)
            {

                _particleCounter = 0;
            }

            _dustCounter++;
            if (_dustCounter > Body_Dust_Rate)
            {
                Vector2 position = Projectile.Center + Main.rand.NextVector2Circular(Body_Radius / 2, Body_Radius / 2);
                Dust dust = Dust.NewDustPerfect(position, DustID.GemAmethyst, Scale: Main.rand.NextFloat(0.5f, 3f));
                dust.noGravity = true;
                _dustCounter = 0;
            }

            Projectile.scale = VectorHelper.Osc(0.9f, 1f, 5f);
            DrawHelper.AnimateTopToBottom(Projectile, 4);
            Lighting.AddLight(Projectile.Center, Color.Pink.ToVector3() * 0.28f);
        }

        public override void OnKill(int timeLeft)
        {
            //REPLACE SOUND AT SOME POINT
            SoundEngine.PlaySound(SoundID.DD2_BetsysWrathImpact, Projectile.position);
        }
    }
    
    public class LittleScissor : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.WhiteSmoke.ToVector3() * 0.55f * Main.essScale); // Makes this item glow when thrown out of inventory.
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<LittleScissorDashPlayer>().DashAccessoryEquipped = true;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            DrawHelper.DrawGlow2InWorld(Item, spriteBatch, ref rotation, ref scale, whoAmI);
            return true;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            //The below code makes this item hover up and down in the world
            //Don't forget to make the item have no gravity, otherwise there will be weird side effects
            float hoverSpeed = 5;
            float hoverRange = 0.2f;
            float y = VectorHelper.Osc(-hoverRange, hoverRange, hoverSpeed);
            Vector2 position = new Vector2(Item.position.X, Item.position.Y + y);
            Item.position = position;
        }
    }
}