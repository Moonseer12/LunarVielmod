using System;
using Stellamod.Content.Gores;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Ishtar.WeaponsIS
{
    public class RibbonStaffHold : ModProjectile
    {
        public Vector2[] BungeeGumPos = new Vector2[4];
        private ref float SwordRotation => ref Projectile.ai[1];
        public override void SetDefaults()
        {
            Projectile.width = 56;
            Projectile.height = 62;
            Projectile.aiStyle = 595;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = int.MaxValue;
        }

        public override void AI()
        {
            Aim();
        }

        private void Aim()
        {
            //Aiming Code
            Player player = Main.player[Projectile.owner];


            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);
            if (Main.myPlayer == Projectile.owner)
            {
                player.ChangeDir(Projectile.direction);
                SwordRotation = (Main.MouseWorld - player.Center).ToRotation();
                Projectile.netUpdate = true;
                if (!player.channel)
                    Projectile.Kill();
            }

            Projectile.velocity = SwordRotation.ToRotationVector2();
            Projectile.spriteDirection = player.direction;
            if (Projectile.spriteDirection == 1)
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            else
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi - MathHelper.PiOver4;


            Projectile.Center = playerCenter + Projectile.velocity * 32;// customization of the hitbox position

            //Interesting trail

            BungeeGumPos[0] = player.MountedCenter + new Vector2(-26, -24) + Projectile.velocity * 48;
            BungeeGumPos[1] = BungeeGumPos[0];
            BungeeGumPos[2] = Main.MouseWorld;
            BungeeGumPos[3] = BungeeGumPos[2];

            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);
        }

        public override bool ShouldUpdatePosition()
        {
            //Make velocity not move it
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {


            return false;
        }
    }

    public class RibbonStaffStart : ModProjectile
    {
        //AI
        private ref float Timer => ref Projectile.ai[0];
        private int BuffType => ModContent.BuffType<RibbonWrapped>();
        private Player Owner => Main.player[Projectile.owner];

        //Animation Stuff
        public Vector2[] CirclePos = new Vector2[48];
        public int FrameCounter;
        public int FrameTick;


        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 128;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = int.MaxValue;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Timer++;
            if (!Owner.channel)
            {
                Projectile.Kill();
            }
            else if (Main.myPlayer == Projectile.owner)
            {
                Projectile.Center = Main.MouseWorld;
                Projectile.netUpdate = true;
            }
            Visuals();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!target.HasBuff(BuffType))
            {
                SoundStyle soundStyle = new("Stellamod/Assets/Sounds/RibbonStaffWrap1");
                soundStyle.PitchVariance = 0.15f;
                SoundEngine.PlaySound(soundStyle, Projectile.position);

                //Spawn the wrapping projectile here
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero,
                    ModContent.ProjectileType<RibbonStaffTieProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai1: target.whoAmI);
            }
            target.AddBuff(BuffType, 120);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active)
                    continue;
                if (npc.HasBuff(BuffType))
                {
                    int buffIndex = npc.FindBuffIndex(BuffType);
                    npc.DelBuff(buffIndex);

                    if (Timer > 60)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), npc.Center, Vector2.Zero,
                            ModContent.ProjectileType<RibbonBoom>(), Projectile.damage * 7, Projectile.knockBack, Projectile.owner);
                    }
                }
            }
        }

        // Here you can decide if your minion breaks things like grass or pots
        public override bool? CanCutTiles()
        {
            return false;
        }

        // This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
        private void Visuals()
        {
            DrawHelper.DrawCircle(Projectile.Center, Projectile.width, CirclePos);
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D chainTexture = ModContent.Request<Texture2D>(Texture).Value;
            int frameCount = 8;
            int frameTime = 2;
            Rectangle animationFrame = chainTexture.AnimationFrame(
                ref FrameCounter, ref FrameTick, frameTime, frameCount, true);
            DrawHelper.DrawFlowerChains(chainTexture, CirclePos, animationFrame, 1f);
            return false;
        }
    }

    public class RibbonBoom : ModProjectile
    {
        private float _scale;
        private int _frameCounter;
        private int _frameTick;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 30;
        }

        public override void SetDefaults()
        {
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.width = 119;
            Projectile.height = 116;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;
            Projectile.scale = 1f;
            Projectile.tileCollide = false;
        }

        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void AI()
        {
            Timer++;
            if (Timer == 1)
            {
                _scale = 2f + Main.rand.NextFloat(0.75f, 1f);
                SoundEngine.PlaySound(SoundID.DD2_KoboldExplosion, Projectile.position);
                for (int i = 0; i < 16; i++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(90, 90);
                    Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
                        ModContent.GoreType<RibbonRed>());
                }

                for (int i = 0; i < Main.rand.Next(2, 5); i++)
                {
                    Vector2 velocity = Vector2.Zero;
                    velocity.X = Main.rand.NextFloat(-16, 16);
                    velocity.Y = Main.rand.NextFloat(-10, -20);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
                        ModContent.ProjectileType<RibbonStaffStreamerProj>(), Projectile.damage / 10, Projectile.knockBack / 10, Projectile.owner);
                }

                for (int i = 0; i < 8; i++)
                {
                    //Get a random velocity
                    Vector2 velocity = Main.rand.NextVector2Circular(4, 4);

                    //Get a random
                    float randScale = Main.rand.NextFloat(0.5f, 1.5f);
                }
            }

            Vector3 RGB = new(0.89f, 2.53f, 2.55f);
            // The multiplication here wasn't doing anything
            Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);
        }


        public override bool PreAI()
        {
            if (++_frameTick >= 1)
            {
                _frameTick = 0;
                if (++_frameCounter >= 30)
                {
                    _frameCounter = 0;
                }
            }
            return true;
        }


        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 50f);
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            float width = 129;
            float height = 129;
            Vector2 origin = new Vector2(width / 2, height / 2);
            int frameSpeed = 1;
            int frameCount = 30;
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Draw(texture, drawPosition,
                texture.AnimationFrame(ref _frameCounter, ref _frameTick, frameSpeed, frameCount, false),
                (Color)GetAlpha(lightColor), 0f, origin, _scale, SpriteEffects.None, 0f);
            return false;
        }
    }
    
    public class RibbonStaffStreamerProj : ModProjectile
    {
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.3f;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 1; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(4, 4);
                Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
                    ModContent.GoreType<RibbonRed>());
            }
        }
    }

    public class RibbonStaffTieProj : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private ref float TargetNPC => ref Projectile.ai[1];
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 14;
        }

        public override void SetDefaults()
        {
            Projectile.width = 96;
            Projectile.height = 120;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 48;
            Projectile.timeLeft = int.MaxValue;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            Projectile.alpha -= 10;
            int npcIndex = (int)TargetNPC;
            Timer++;
            if (Timer == 1)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(90, 90);
                    Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
                        ModContent.GoreType<RibbonRed>());
                }

                for (int i = 0; i < 3; i++)
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height,
                        ModContent.DustType<Dusts.GunFlash>(), newColor: Color.White);
                }
            }

            NPC target = Main.npc[npcIndex];
            if (!target.active || !target.HasBuff(ModContent.BuffType<RibbonWrapped>()))
            {
                Projectile.Kill();
            }
            else
            {
                Vector2 targetPos = target.Center + new Vector2(0.001f, 0.001f) + new Vector2(0, Projectile.height / 3);
                Vector2 directionToTarget = Projectile.Center.DirectionTo(targetPos);
                float dist = Vector2.Distance(Projectile.Center, targetPos);
                Projectile.velocity = (directionToTarget * dist) + new Vector2(0.001f, 0.001f);
            }

            //Animate
            int frameSpeed = 5;
            int projFrames = Main.projFrames[Projectile.type];
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= frameSpeed)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;

                if (Projectile.frame >= projFrames)
                {
                    Projectile.frame = projFrames - 1;
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundStyle soundStyle = new("Stellamod/Assets/Sounds/RibbonStaffBoom1");
            soundStyle.PitchVariance = 0.15f;
            SoundEngine.PlaySound(soundStyle, Projectile.position);
            for (int i = 0; i < 4; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(90, 90);
                Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
                    ModContent.GoreType<RibbonRed>());
            }
        }
    }

    public class RibbonWrapped : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = false;
            Main.buffNoTimeDisplay[Type] = true;
            BuffID.Sets.IsATagBuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.boss)
                return;
            npc.velocity *= 0.1f;
        }
    }

    public class RibbonStaff : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 67;
            Item.DamageType = DamageClass.Magic;
            Item.useAnimation = 24;
            Item.useTime = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item9;
            Item.mana = 12;
            Item.knockBack = 2;
            Item.rare = ItemRarityID.Lime;
            Item.noUseGraphic = true;
            Item.autoReuse = false;
            Item.channel = true;
            Item.shoot = ModContent.ProjectileType<RibbonStaffHold>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(player.GetSource_FromThis(), position, velocity,
                ModContent.ProjectileType<RibbonStaffStart>(), damage, knockback, player.whoAmI);
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }
    }
}