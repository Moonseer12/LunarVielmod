using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital.WeaponsRC
{
    public class Angelenthal : ModItem
    {
        public int AttackCounter = 1;
        public int combowombo = 0;

        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(1, 60));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 90;
            Item.DamageType = DamageClass.Generic;
            Item.useTime = 5;
            Item.useAnimation = 5;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 15;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<AngelenthalProj1>();
            Item.shootSpeed = 10f;
            Item.noUseGraphic = true;
            Item.noMelee = true;


        }



        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            // Draw the periodic glow effect behind the item when dropped in the world (hence PreDrawInWorld)
            Texture2D texture = TextureAssets.Item[Item.type].Value;

            Rectangle frame;

            if (Main.itemAnimations[Item.type] != null)
            {
                // In case this item is animated, this picks the correct frame
                frame = Main.itemAnimations[Item.type].GetFrame(texture, Main.itemFrameCounter[whoAmI]);
            }
            else
            {
                frame = texture.Frame();
            }

            Vector2 frameOrigin = frame.Size() / 2f;
            Vector2 offset = new Vector2(Item.width / 2 - frameOrigin.X, Item.height - frame.Height);
            Vector2 drawPos = Item.position - Main.screenPosition + frameOrigin + offset;

            float time = Main.GlobalTimeWrappedHourly;
            float timer = Item.timeSinceItemSpawned / 240f + time * 0.04f;

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

                spriteBatch.Draw(texture, drawPos + new Vector2(0f, 8f).RotatedBy(radians) * time, frame, new Color(200, 70, 255, 77), rotation, frameOrigin, scale, SpriteEffects.None, 0);
            }

            for (float i = 0f; i < 1f; i += 0.34f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;

                spriteBatch.Draw(texture, drawPos + new Vector2(0f, 4f).RotatedBy(radians) * time, frame, new Color(255, 255, 255, 67), rotation, frameOrigin, scale, SpriteEffects.None, 0);
            }

            return true;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.shoot = ModContent.ProjectileType<AngelenthalThrow>();
                Item.noUseGraphic = true;
                Item.useStyle = ItemUseStyleID.Shoot;
                Item.noMelee = true; //so the Item's animation doesn't do damage
                Item.knockBack = 11;
                Item.autoReuse = true;
                Item.useTurn = true;
                Item.DamageType = DamageClass.Melee;
                Item.shootSpeed = 20f;
                Item.useAnimation = 20;
                Item.useTime = 45;
                Item.consumeAmmoOnLastShotOnly = true;

            }
            else
            {

            }

            return base.CanUseItem(player);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {

                return true;
            }



            if (player.altFunctionUse != 2)
            {


                int dir = AttackCounter;
                AttackCounter = -AttackCounter;
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<AngelenthalProj1>(), damage, knockback, player.whoAmI, 1, dir);


            }


            return false;
        }





    }

    public class AngelenthalProj1 : ModProjectile
    {
    public override string Texture => TextureRegistry.EmptyTexture;
        public static bool swung = false;
        public int SwingTime = 10;
        public float holdOffset = 50f;  //This offsets the outward position for swing
        public int combowombo;
        private bool _initialized;
        private int timer;
        private bool ParticleSpawned;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 50;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        private Player Owner => Main.player[Projectile.owner];

        public float SwingDistance;
        public float Curvature;

        public ref float AiState => ref Projectile.ai[1];
        private Vector2 returnPosOffset; //The position of the projectile when it starts returning to the player from being hooked
        private Vector2 npcHookOffset = Vector2.Zero; //Used to determine the offset from the hooked npc's center
        private float npcHookRotation; //Stores the projectile's rotation when hitting an npc
        private NPC hookNPC; //The npc the projectile is hooked into

        public const float THROW_RANGE = 320; //Peak distance from player when thrown out, in pixels
        public const float HOOK_MAXRANGE = 800; //Maximum distance between owner and hooked enemies before it automatically rips out
        public const int HOOK_HITTIME = 20; //Time between damage ticks while hooked in
        public const int RETURN_TIME = 6; //Time it takes for the projectile to return to the owner after being ripped out

        public bool Flip = false;
        public bool Slam = false;
        public bool PreSlam = false;

        public Vector2 CurrentBase = Vector2.Zero;
        public override void SetDefaults()
        {
            Projectile.timeLeft = SwingTime;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.height = 90;
            Projectile.width = 98;
            Projectile.friendly = true;
            Projectile.scale = 1f;
        }

        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public virtual float Lerp(float val)
        {
            return val == 1f ? 1f : (val == 1f ? 1f : (float)Math.Pow(2, val * 10f - 10f) / 2f);
        }

        public float death = 0;
        public override void AI()
        {
            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
            death++;
            if (death == 5)
            {
                Projectile p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.position, Projectile.velocity * 0, ModContent.ProjectileType<AngelenthalP>(), Projectile.damage * 1, 0f, Projectile.owner, 0f, 0f);
                p.rotation = direction.ToRotation();
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SwordSlice"), Projectile.position);


            }

            if (death == 15)
            {
                Projectile p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.position, Projectile.velocity * 0, ModContent.ProjectileType<AngelenthalP3>(), Projectile.damage * 1, 0f, Projectile.owner, 0f, 0f);
                p.rotation = direction.ToRotation();
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SwordSlice"), Projectile.position);

            }

            if (death == 25)
            {
                Projectile p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.position, Projectile.velocity * 0, ModContent.ProjectileType<AngelenthalP>(), Projectile.damage * 1, 0f, Projectile.owner, 0f, 0f);
                p.rotation = direction.ToRotation();
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SwordSlice"), Projectile.position);


            }

            if (death == 35)
            {
                Projectile p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.position, Projectile.velocity * 0, ModContent.ProjectileType<AngelenthalP3>(), Projectile.damage * 1, 0f, Projectile.owner, 0f, 0f);
                p.rotation = direction.ToRotation();
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SwordSlice"), Projectile.position);


            }

            if (death == 45)
            {
                Projectile p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.position, Projectile.velocity * 0, ModContent.ProjectileType<AngelenthalP>(), Projectile.damage * 1, 0f, Projectile.owner, 0f, 0f);
                p.rotation = direction.ToRotation();
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SwordSlice"), Projectile.position);



            }

            if (death == 55)
            {
                Projectile p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.position, Projectile.velocity * 0, ModContent.ProjectileType<AngelenthalP3>(), Projectile.damage * 1, 0f, Projectile.owner, 0f, 0f);
                p.rotation = direction.ToRotation();
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SwordSlice"), Projectile.position);


            }

            if (death == 65)
            {
                Projectile p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.position, Projectile.velocity * 0, ModContent.ProjectileType<AngelenthalP2>(), Projectile.damage * 2, 0f, Projectile.owner, 0f, 0f);
                p.rotation = direction.ToRotation();
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SwordOfGlactia2"), Projectile.position);



            }



            Player player = Main.player[Projectile.owner];
            if (!_initialized)
            {
                timer++;

                SwingTime = (int)(100 / player.GetAttackSpeed(DamageClass.Melee));
                Projectile.alpha = 255;
                Projectile.timeLeft = SwingTime;
                _initialized = true;
                Projectile.damage -= 9999;
                //Projectile.netUpdate = true;

            }
            else if (_initialized)
            {
                if (!player.active || player.dead || player.CCed || player.noItems)
                {
                    return;
                }
                Projectile.alpha = 0;
                if (timer == 1)
                {
                    Projectile.damage += 9999;
                    Projectile.damage *= 3;

                    timer++;
                }
                Vector3 RGB = new Vector3(1.28f, 0f, 1.28f);
                float multiplier = 0.2f;
                float max = 2.25f;
                float min = 1.0f;
                RGB *= multiplier;
                if (RGB.X > max)
                {
                    multiplier = 0.5f;
                }
                if (RGB.X < min)
                {
                    multiplier = 1.5f;
                }
                Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);
                Projectile.usesLocalNPCImmunity = true;
                Projectile.localNPCHitCooldown = 10000;
                for (int i = 0; i < 1; i++)
                {
                    Dust dust = Dust.NewDustDirect(Projectile.position - Projectile.velocity, Projectile.width, Projectile.height, DustID.SilverCoin, 0, 0, 100, Color.Violet, 1f);
                    dust.noGravity = true;
                    dust.velocity *= 2f;

                }
                int dir = (int)Projectile.ai[1];
                float swingProgress = Lerp(Utils.GetLerpValue(0f, SwingTime, Projectile.timeLeft));
                // the actual rotation it should have
                float defRot = Projectile.velocity.ToRotation();
                // starting rotation
                float endSet = ((MathHelper.PiOver2) / 0.2f);
                float start = defRot - endSet;

                // ending rotation
                float end = defRot + endSet;
                // current rotation obv
                float rotation = dir == 1 ? start.AngleLerp(end, swingProgress) : start.AngleLerp(end, 1f - swingProgress);
                // offsetted cuz sword sprite
                Vector2 position = Main.MouseWorld;
                position += rotation.ToRotationVector2() * holdOffset;
                Projectile.Center = position;
                Projectile.rotation = (position - player.Center).ToRotation() + MathHelper.PiOver4;

                player.heldProj = Projectile.whoAmI;
                player.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
                player.itemRotation = rotation * player.direction;
                player.itemTime = 2;
                player.itemAnimation = 2;
                //Projectile.netUpdate = true;


                if (!ParticleSpawned)
                {


                    ParticleSpawned = true;
                }

            }
        }
        private Vector2 GetSwingPosition(float progress)
        {
            //Starts at owner center, goes to peak range, then returns to owner center
            float distance = MathHelper.Clamp(SwingDistance, THROW_RANGE * 0.1f, THROW_RANGE) * MathHelper.Lerp((float)Math.Sin(progress * MathHelper.Pi), 1, 0.04f);
            distance = Math.Max(distance, 100); //Dont be too close to player

            float angleMaxDeviation = MathHelper.Pi / 1.2f;
            float angleOffset = Owner.direction * (Flip ? -1 : 1) * MathHelper.Lerp(-angleMaxDeviation, angleMaxDeviation, progress); //Moves clockwise if player is facing right, counterclockwise if facing left
            return Projectile.velocity.RotatedBy(angleOffset) * distance;
        }


        public override bool ShouldUpdatePosition() => false;


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
        }



        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Color drawColor = new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 50f);

            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);




            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Main.instance.LoadProjectile(Projectile.type);


            // Redraw the projectile with the color not influenced by light
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Lerp(new Color(93, 203, 243), new Color(59, 72, 168), 1f / Projectile.oldPos.Length * k) * (1f - 1f / Projectile.oldPos.Length * k));
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(SwingTime);
            writer.Write(SwingDistance);
            writer.WriteVector2(returnPosOffset);
            writer.WriteVector2(npcHookOffset);
            writer.Write(npcHookRotation);
            writer.Write(Flip);
            writer.Write(Slam);
            writer.Write(Curvature);

            if (hookNPC == default(NPC)) //Write a -1 instead if the npc isnt set
                writer.Write(-1);
            else
                writer.Write(hookNPC.whoAmI);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            SwingTime = reader.ReadInt32();
            SwingDistance = reader.ReadSingle();
            returnPosOffset = reader.ReadVector2();
            npcHookOffset = reader.ReadVector2();
            npcHookRotation = reader.ReadSingle();
            Flip = reader.ReadBoolean();
            Slam = reader.ReadBoolean();
            Curvature = reader.ReadSingle();

            int whoAmI = reader.ReadInt32(); //Read the whoami value sent
            if (whoAmI == -1) //If its a -1, sync that the npc hasn't been set yet
                hookNPC = default;
            else
                hookNPC = Main.npc[whoAmI];
        }
    }

    public class AngelenthalP : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("FrostShotIN");
            Main.projFrames[Projectile.type] = 8;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.width = 245;
            Projectile.height = 190;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 16;
            Projectile.scale = 1f;

        }
        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public override void AI()
        {
            Projectile.rotation -= 0.01f;
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
                if (++Projectile.frame >= 8)
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

    public class AngelenthalP2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("FrostShotIN");
            Main.projFrames[Projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.width = 302;
            Projectile.height = 158;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 14;
            Projectile.scale = 1f;

        }
        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public override void AI()
        {
            Projectile.rotation -= 0.01f;
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
            return new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 50f);
        }


    }

    public class AngelenthalP3 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("FrostShotIN");
            Main.projFrames[Projectile.type] = 8;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.width = 245;
            Projectile.height = 190;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 16;
            Projectile.scale = 1f;

        }
        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public override void AI()
        {
            Projectile.rotation -= 0.01f;
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
                if (++Projectile.frame >= 8)
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


    public class AngelenthalThrow : ModProjectile
    {
        bool Moved;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 35;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = 5;
            Projectile.width = 79;
            Projectile.height = 79;
            Projectile.timeLeft = 660;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override bool PreDraw(ref Color lightColor)
        {

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            if (Projectile.spriteDirection != 1)
            {
                DrawOffset.X = Projectile.Center.X - 18;
                DrawOffset.Y = Projectile.Center.Y;
            }
            else
            {
                DrawOffset.X = Projectile.Center.X - 25;
                DrawOffset.Y = Projectile.Center.Y;
            }

            Texture2D texture2D4 = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Spiin").Value;
            Main.spriteBatch.Draw(texture2D4, DrawOffset - Main.screenPosition, null, new Color((int)(85f * alphaCounter), (int)(65f * alphaCounter), (int)(85f * alphaCounter), 0), Projectile.rotation, new Vector2(200, 200), 0.07f * (5 + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, DrawOffset - Main.screenPosition, null, new Color((int)(85f * alphaCounter), (int)(65f * alphaCounter), (int)(85f * alphaCounter), 0), Projectile.rotation, new Vector2(200, 200), 0.07f * (5 + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, DrawOffset - Main.screenPosition, null, new Color((int)(85f * alphaCounter), (int)(65f * alphaCounter), (int)(85f * alphaCounter), 0), Projectile.rotation, new Vector2(200, 200), 0.07f * (5 + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, DrawOffset - Main.screenPosition, null, new Color((int)(85f * alphaCounter), (int)(65f * alphaCounter), (int)(85f * alphaCounter), 0), Projectile.rotation, new Vector2(200, 200), 0.07f * (5 + 0.6f), SpriteEffects.None, 0f);
            SpriteEffects Effects = Projectile.spriteDirection != 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Vector2 scale = new(Projectile.scale, 1f);
            Color drawColor = Color.Purple;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            for (int i = 0; i < 8; i++)
            {
                Vector2 drawOffset = (MathHelper.TwoPi * i / 8f).ToRotationVector2() * 4f;
                Main.EntitySpriteDraw(texture, DrawOffset - Main.screenPosition + drawOffset, null, Color.Yellow with { A = 160 } * Projectile.Opacity, Projectile.rotation, texture.Size() * 0.5f, scale, Effects, 0);
            }
            for (int i = 0; i < 7; i++)
            {
                float scaleFactor = 1f - i / 6f;
                Vector2 drawOffset = Projectile.velocity * i * -0.34f;
                Main.EntitySpriteDraw(texture, DrawOffset - Main.screenPosition + drawOffset, null, drawColor with { A = 160 } * Projectile.Opacity, Projectile.rotation, texture.Size() * 0.5f, scale * scaleFactor, Effects, 0);
            }
            Main.EntitySpriteDraw(texture, DrawOffset - Main.screenPosition, null, drawColor with { A = 250 }, Projectile.rotation, texture.Size() * 0.5f, scale, Effects, 0);

            if (Main.rand.NextBool(5))
            {
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 150, Color.OrangeRed, 1f);
                Main.dust[dustnumber].velocity *= 0.3f;
                Main.dust[dustnumber].noGravity = true;
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Main.instance.LoadProjectile(Projectile.type);

            // Redraw the projectile with the color not influenced by light
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Lerp(new Color(254, 231, 97), new Color(247, 118, 34), 1f / Projectile.oldPos.Length * k) * (1f - 1f / Projectile.oldPos.Length * k));
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, Effects, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Color drawColor2 = Projectile.GetAlpha(lightColor);

            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor2, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);




            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Main.instance.LoadProjectile(Projectile.type);


            // Redraw the projectile with the color not influenced by light
            Vector2 drawOrigin2 = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin2 + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Lerp(new Color(93, 203, 243), new Color(59, 72, 168), 1f / Projectile.oldPos.Length * k) * (1f - 1f / Projectile.oldPos.Length * k));
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin2, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);


            return false;
        }



        public override void AI()
        {
            float distance2 = 128;
            float particleSpeed = 8;
            Vector2 position = Projectile.Center + Main.rand.NextVector2CircularEdge(distance2, distance2);
            Vector2 speed = (Projectile.Center - position).SafeNormalize(Vector2.Zero) * particleSpeed;


            Projectile.velocity *= .96f;
            Projectile.ai[1]++;
            if (!Moved && Projectile.ai[1] >= 0)
            {
                SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, Projectile.position);

                Projectile.spriteDirection = Projectile.direction;
                Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f + 3.14f;
                for (int j = 0; j < 10; j++)
                {
                    Vector2 vector2 = Vector2.UnitX * -Projectile.width / 2f;
                    vector2 += -Vector2.UnitY.RotatedBy(j * 3.141591734f / 6f, default) * new Vector2(8f, 16f);
                    vector2 = vector2.RotatedBy(Projectile.rotation - 1.57079637f, default);
                    int num8 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.GoldCoin, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                    Main.dust[num8].scale = 1.3f;
                    Main.dust[num8].noGravity = true;
                    Main.dust[num8].position = Projectile.Center + vector2;
                    Main.dust[num8].velocity = Projectile.velocity * 0.1f;
                    Main.dust[num8].noLight = true;
                    Main.dust[num8].velocity = Vector2.Normalize(Projectile.Center - Projectile.velocity * 3f - Main.dust[num8].position) * 1.25f;
                }
                Moved = true;
            }

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (npc.active && !npc.friendly && !npc.boss)
                {
                    float distance = Vector2.Distance(Projectile.Center, npc.Center);
                    if (distance <= 200)
                    {
                        Vector2 direction = npc.Center - Projectile.Center;
                        direction.Normalize();
                        npc.velocity -= direction * 0.5f;
                    }
                }
            }

            Vector2 ParOffset;
            if (Projectile.ai[1] >= 60)
            {
                ParOffset.X = Projectile.Center.X - 18;
                ParOffset.Y = Projectile.Center.Y;
                if (Main.rand.NextBool(1))
                {
                    int dustnumber = Dust.NewDust(ParOffset, Projectile.width, Projectile.height, DustID.SilverCoin, 0f, 0f, 150, Color.White, 3f);
                    Main.dust[dustnumber].velocity *= 0.3f;
                    Main.dust[dustnumber].noGravity = true;
                }
                Projectile.velocity.Y += 3.2f;
            }
            if (Projectile.ai[1] >= 20)
            {
                Projectile.rotation /= 1.20f;
                alphaCounter -= 0.09f;
                Projectile.tileCollide = true;
            }
            else
            {
                if (alphaCounter <= 2)
                {
                    alphaCounter += 0.15f;
                }
                Projectile.rotation += -0.6f;
            }

            Projectile.spriteDirection = Projectile.direction;
        }

        Vector2 BombOffset;
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Infernis1"), Projectile.position);

            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Binding_Abyss_Rune"), Projectile.position);
            FXUtil.ShakeCamera(Projectile.Center, 512f, 120f);
            var EntitySource = Projectile.GetSource_FromThis();
            int fireball = Projectile.NewProjectile(EntitySource, Projectile.Center.X, Projectile.Center.Y, 0, 0, ModContent.ProjectileType<KaBoomFenix>(), Projectile.damage * 2, 1, Projectile.owner);
            Projectile ichor = Main.projectile[fireball];
            ichor.hostile = false;
            ichor.friendly = true;

            if (Projectile.ai[1] >= 60)
            {
                BombOffset.X = Projectile.Center.X - 50;
                BombOffset.Y = Projectile.Center.Y;
                Projectile.NewProjectile(EntitySource, BombOffset.X, BombOffset.Y, 0, 0,
                    ModContent.ProjectileType<KaBoomSigil>(), Projectile.damage, 1, Projectile.owner);

                BombOffset.X = Projectile.Center.X + 70;
                BombOffset.Y = Projectile.Center.Y;
                Projectile.NewProjectile(EntitySource, BombOffset.X, BombOffset.Y, 0, 0,
                    ModContent.ProjectileType<KaBoomSigil>(), Projectile.damage, 1, Projectile.owner);

                BombOffset.X = Projectile.Center.X - 150;
                BombOffset.Y = Projectile.Center.Y;
                Projectile.NewProjectile(EntitySource, BombOffset.X, BombOffset.Y, 0, 0,
                    ModContent.ProjectileType<KaBoomSigil>(), Projectile.damage, 1, Projectile.owner);

                BombOffset.X = Projectile.Center.X + 170;
                BombOffset.Y = Projectile.Center.Y;
                Projectile.NewProjectile(EntitySource, BombOffset.X, BombOffset.Y, 0, 0,
                    ModContent.ProjectileType<KaBoomSigil>(), Projectile.damage, 1, Projectile.owner);

                BombOffset.X = Projectile.Center.X - 250;
                BombOffset.Y = Projectile.Center.Y;
                Projectile.NewProjectile(EntitySource, BombOffset.X, BombOffset.Y, 0, 0,
                    ModContent.ProjectileType<KaBoomSigil>(), Projectile.damage, 1, Projectile.owner);

                BombOffset.X = Projectile.Center.X + 270;
                BombOffset.Y = Projectile.Center.Y;
                Projectile.NewProjectile(EntitySource, BombOffset.X, BombOffset.Y, 0, 0,
                    ModContent.ProjectileType<KaBoomSigil>(), Projectile.damage, 1, Projectile.owner);

                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.position, -Projectile.velocity,
                    ProjectileID.LostSoulFriendly, Projectile.damage * 1, 0f, Projectile.owner);

                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.position, -Projectile.velocity * 0.5f,
                    ProjectileID.LostSoulFriendly, Projectile.damage * 1, 0f, Projectile.owner);

                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.position, -Projectile.velocity * 1.5f,
                    ProjectileID.LostSoulFriendly, Projectile.damage * 1, 0f, Projectile.owner);
            }



            SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen, Projectile.position);
            Projectile.ownerHitCheck = true;

            int radius = 250;

            // Damage enemies within the splash radius
            for (int i = 0; i < Main.npc.Length; i++)
            {
                NPC target = Main.npc[i];
                if (target.active && !target.friendly && Vector2.Distance(Projectile.Center, target.Center) < radius)
                {
                    int damage = Projectile.damage * 2;
                    target.SimpleStrikeNPC(damage: 12, 0);
                }
            }

            for (int i = 0; i < 150; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                var d = Dust.NewDustPerfect(Projectile.Center, DustID.SilverFlame, speed * 11, Scale: 3f);
                ;
                d.noGravity = true;
            }



        }

        float alphaCounter = 0;
        Vector2 DrawOffset;




        public override void PostDraw(Color lightColor)
        {
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 1.75f * Main.essScale);
        }
    }
}