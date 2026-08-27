using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.MoonspiralTower.WeaponsMT
{
    public class VixylPlayer : ModPlayer
    {
        public int parryCooldown;
        public int parryTimer;

        public override void PostUpdateEquips()
        {
            parryTimer--;
            if (parryTimer <= 0)
            {
                parryTimer = 0;
            }

            parryCooldown--;
            if (parryCooldown <= 0)
            {
                parryCooldown = 0;
            }
        }

        public override bool ConsumableDodge(Player.HurtInfo info)
        {
            if (parryTimer > 0 && parryCooldown <= 0)
            {
                parryTimer = 0;
                ParryEffects();
                return true;
            }

            return false;
        }

        public void ParryEffects()
        {
            //Brief invulnerability after parrying
            // Some sound and visual effects
            for (int i = 0; i < 50; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
                Dust d = Dust.NewDustPerfect(Player.Center + speed * 16, DustID.BlueCrystalShard, speed * 5, Scale: 1.5f);
                d.noGravity = true;
            }
            SoundEngine.PlaySound(SoundID.Shatter with { Pitch = 0.5f }, Player.position);

            //Spawn the big verlia slash projectile here
            //Setting the immune time
            Player.SetImmuneTimeForAllTypes(60);
            if (Player.whoAmI != Main.myPlayer)
            {
                return;
            }

            // Add the buff and assigning the cooldown time
            int time = 180;
            Player.AddBuff(ModContent.BuffType<VixylDodgeBuff>(), time);

            Vector2 velocity = Player.Center.DirectionTo(Main.MouseWorld);
            Projectile.NewProjectile(Player.GetSource_FromThis(),
                Player.Center, velocity, ModContent.ProjectileType<VixylParryProj>(), Player.HeldItem.damage * 4, Player.HeldItem.knockBack, Player.whoAmI);
            SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordSlice"), Player.position);

            parryCooldown = time;
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                SendExampleDodgeMessage(Player.whoAmI);
            }
        }

        public static void HandleExampleDodgeMessage(BinaryReader reader, int whoAmI)
        {
            int player = reader.ReadByte();
            if (Main.netMode == NetmodeID.Server)
            {
                player = whoAmI;
            }

            VixylPlayer vixylPlayer = Main.player[player].GetModPlayer<VixylPlayer>();
            vixylPlayer.ParryEffects();

            if (Main.netMode == NetmodeID.Server)
            {
                // If the server receives this message, it sends it to all other clients to sync the effects.
                SendExampleDodgeMessage(player);
            }
        }

        public static void SendExampleDodgeMessage(int whoAmI)
        {
            // This code is called by both the initial 
            ModPacket packet = ModContent.GetInstance<Stellamod>().GetPacket();
            packet.Write((byte)MessageType.Dodge);
            packet.Write((byte)whoAmI);
            packet.Send(ignoreClient: whoAmI);
        }
    }

    public class VixylParryProj : ModProjectile
    {
        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.width = 200;
            Projectile.height = 200;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 14;
            Projectile.localNPCHitCooldown = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.scale = 1.5f;
            DrawOffsetX = -100;
        }

        public override void AI()
        {

            Projectile.rotation = Projectile.velocity.ToRotation();
            Timer++;
            if (Timer == 2)
            {
                Projectile.scale *= 0.98f;
                Timer = 0;
            }


            if (Projectile.scale == 0f)
            {
                Projectile.Kill();
            }

            //Visual Stuff
            Vector3 RGB = new(0.89f, 2.53f, 2.55f);
            // The multiplication here wasn't doing anything
            Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);
        }

        public override bool PreAI()
        {
            Projectile.tileCollide = false;
            if (++Projectile.frameCounter >= 2)
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
            return new Color(200, 200, 200, 0) * (1f - Projectile.alpha / 50f);
        }
    }

    public class VixylSlashProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.width = 300;
            Projectile.height = 300;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 72;
            Projectile.localNPCHitCooldown = 6;
            Projectile.usesLocalNPCImmunity = true;
        }

        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        float trueFrame = 0;
        public void UpdateFrame(float speed, int minFrame, int maxFrame)
        {
            trueFrame += speed;
            if (trueFrame < minFrame)
            {
                trueFrame = minFrame;
            }
            if (trueFrame > maxFrame)
            {
                trueFrame = minFrame;
            }
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            Projectile.Center = owner.Center;
            owner.immune = true;
            owner.SetImmuneTimeForAllTypes(3);

            //Lighting
            Vector3 RGB = new(0.89f, 2.53f, 2.55f);

            // The multiplication here wasn't doing anything
            Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);
            UpdateFrame(0.8f, 1, 36);
        }


        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, 0) * (1f - Projectile.alpha / 50f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            Rectangle rectangle = new(0, 0, 285, 256);
            rectangle.X = (int)trueFrame % 6 * rectangle.Width;
            rectangle.Y = ((int)trueFrame - ((int)trueFrame % 6)) / 6 * rectangle.Height;

            Vector2 origin = new(rectangle.Width / 2, rectangle.Height / 2);
            SpriteBatch spriteBatch = Main.spriteBatch;
            float drawRotation = Projectile.rotation;
            float drawScale = 2f;

            spriteBatch.Draw(texture, drawPosition,
               rectangle,
                (Color)GetAlpha(lightColor), drawRotation, origin, drawScale, SpriteEffects.None, 0f);
            return false;
        }
    }

    public class VixylSwordProj : ModProjectile
    {
        private bool _init;
        public override string Texture => "Stellamod/Content/Areas/Tundra/MoonspiralTower/WeaponsMT/Vixyl";

        ref float Dir => ref Projectile.ai[0];

        //Swing Stats
        public float SwingDistance;
        public int SwingTime = 24 * Swing_Speed_Multiplier;
        public float holdOffset = 36;

        //Ending Swing Time so it doesn't immediately go away after the swing ends, makes it look cleaner I think
        public int EndSwingTime = 4 * Swing_Speed_Multiplier;

        //This is for smoothin the trail
        public static int Swing_Speed_Multiplier => 8;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;

            Projectile.scale = 1f;

            Projectile.extraUpdates = Swing_Speed_Multiplier - 1;
            Projectile.usesLocalNPCImmunity = true;

            //Multiplying by the thing so it's still 10 ticks
            Projectile.localNPCHitCooldown = 10 * Swing_Speed_Multiplier;
        }

        public override void AI()
        {
            base.AI();
            Player player = Main.player[Projectile.owner];
            if (!_init)
            {
                SwingTime = (int)(SwingTime / player.GetAttackSpeed(DamageClass.Melee));
                _init = true;
                Projectile.alpha = 255;
                Projectile.timeLeft = SwingTime + EndSwingTime;
            }
            else if (_init)
            {
                if (!player.active || player.dead || player.CCed || player.noItems)
                {
                    return;
                }

                Projectile.alpha = 0;
                Vector3 RGB = new(1.28f, 0f, 1.28f);
                float multiplier = 0.2f;
                RGB *= multiplier;

                Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);

                int dir = (int)Dir;

                //Get the swing progress
                float lerpValue = Utils.GetLerpValue(0f, SwingTime, Projectile.timeLeft, true);

                //Smooth it some more
                float swingProgress = Easing.InOutExpo(lerpValue, 10f);

                // the actual rotation it should have
                float defRot = Projectile.velocity.ToRotation();
                // starting rotation

                //How wide is the swing, in radians
                float swingRange = MathHelper.PiOver2 + MathHelper.PiOver4;
                float start = defRot - swingRange;

                // ending rotation
                float end = defRot + swingRange;

                // current rotation obv
                // angle lerp causes some weird things here, so just use a normal lerp
                float rotation = dir == 1 ? MathHelper.Lerp(start, end, swingProgress) : MathHelper.Lerp(end, start, swingProgress);

                // offsetted cuz sword sprite
                Vector2 position = player.RotatedRelativePoint(player.MountedCenter);
                position += rotation.ToRotationVector2() * holdOffset;
                Projectile.Center = position;
                Projectile.rotation = (position - player.Center).ToRotation() + MathHelper.PiOver4;

                player.heldProj = Projectile.whoAmI;
                player.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
                player.itemRotation = rotation * player.direction;
                player.itemTime = 2;
                player.itemAnimation = 2;


                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(90f)); // set arm position (90 degree offset since arm starts lowered)
                Vector2 armPosition = player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)Math.PI / 2); // get position of hand

                armPosition.Y += player.gfxOffY;
                Projectile.Center = armPosition; // Set projectile to arm position
                Projectile.Center += holdOffset * rotation.ToRotationVector2();
                //     Projectile.Center += new Vector2(-9, -9).RotatedBy(rotation);
                //  Projectile.position -= new Vector2(0, 4);
            }
        }
        
        
        public override bool PreDraw(ref Color lightColor)
        {
 

    
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2;
            Color drawColor = Projectile.GetAlpha(lightColor);


            Main.EntitySpriteDraw(texture,
               Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
               sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0); // drawing the sword itsel
     
            return false;

        }
    }

    public class VixylDodgeBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
        }
    }

    public class Vixyl : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 60;
            Item.height = 60;
            Item.damage = 34;
            Item.DamageType = DamageClass.Generic;

            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(gold: 30);
            Item.autoReuse = true;

            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.shoot = ModContent.ProjectileType<VixylSwordProj>();
            Item.shootSpeed = 15;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
            VixylPlayer vixylPlayer = player.GetModPlayer<VixylPlayer>();
            if (player.HasBuff(ModContent.BuffType<VixylDodgeBuff>()))
            {
                //Verli spam slashes
                type = ModContent.ProjectileType<VixylSlashProj>();
                SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordHoldVerlia"), position);
            }
            else if (vixylPlayer.parryCooldown <= 0 && !player.immune)
            {
                vixylPlayer.parryTimer = 18;
                //Normal slash
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SwordSheethe"), position);
            }
        }
    }
}
