using Stellamod.Common.MagicCauldron;
using Stellamod.Content.Areas.Desert.AccCL;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.MaskingShaderSystem;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital.WeaponsRC
{
    public class AlcadBomb : ModItem
    {
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 7));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.LastPrism);
            Item.mana = 4;
            Item.damage = 140;
            Item.shootSpeed = 30f;
            Item.shoot = ModContent.ProjectileType<AlcadBombHeldProj>();
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-3f, -2f);
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] == 0;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<AlcaricMush, BlankStaff>();
        }
    }

    public class AlcadBombHeldProj : ModProjectile
    {
        public override string Texture => "Stellamod/Content/Areas/RoyalCapital/WeaponsRC/AlcadBomb";
        private ref float Timer => ref Projectile.ai[0];
        private Player Owner => Main.player[Projectile.owner];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Type] = 7;
            ProjectileID.Sets.NeedsUUID[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = int.MaxValue;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        private bool ShouldConsumeMana()
        {
            return Timer % 4 == 0;
        }

        private void UpdateDamageForManaSickness(Player player)
        {
            Projectile.damage = (int)player.GetDamage(DamageClass.Magic).ApplyTo(player.HeldItem.damage);
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer % 4 == 0)
            {
                if (!Owner.CheckMana(Owner.HeldItem.mana, true))
                {
                    Projectile.Kill();
                }
            }

            UpdateDamageForManaSickness(Owner);
            Projectile.velocity = -Vector2.UnitY;
            Projectile.Center = Owner.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * 40;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            DrawHelper.AnimateTopToBottom(Projectile, 5);
            HandleOwner();
            SetHandPosition();
        }

        private void HandleOwner()
        {
            if (Main.myPlayer != Projectile.owner)
                return;

            if (Timer == 2)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, Projectile.velocity,
                    ModContent.ProjectileType<AlcadBombProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
            bool stillInUse = Owner.channel && !Owner.noItems && !Owner.CCed;
            if (!stillInUse)
            {
                Projectile.Kill();
            }
        }
        private void SetHandPosition()
        {
            // Set composite arm allows you to set the rotation of the arm and stretch of the front and back arms independently
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(90f)); // set arm position (90 degree offset since arm starts lowered)
            Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)Math.PI / 2); // get position of hand
            Owner.heldProj = Projectile.whoAmI; // set held projectile to this projectile
        }

        protected virtual void DrawTomeSprite(ref Color lightColor)
        {
            Texture2D closeYourTomeTyrant = ModContent.Request<Texture2D>(Texture).Value;
            SpriteBatch spriteBatch = Main.spriteBatch;

            //Calculate Drawing Vars
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            //We can add cool oscillation here
            drawPos.Y += MathHelper.Lerp(-5, 5, VectorHelper.Osc(0f, 1f, speed: 3));


            Vector2 drawOrigin = Projectile.Frame().Size() / 2f;
            Color drawColor = Color.White.MultiplyRGB(lightColor);
            float drawScale = Projectile.scale;
            float drawRotation = Projectile.rotation;
            SpriteEffects drawEffects = SpriteEffects.None;
            float layerDepth = 0;
            float glowDistanceOffset = 4;
            float glowRotationSpeed = 0.05f;

            //Draw Glow Effects
            //Let's do some additive glow
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
            for (float f = 0; f < 1; f += 0.2f)
            {
                float rotation = (f * MathHelper.TwoPi) + Timer * glowRotationSpeed;
                Vector2 velocityRot = rotation.ToRotationVector2();
                velocityRot *= glowDistanceOffset;

                Vector2 glowDrawPos = drawPos + velocityRot;
                spriteBatch.Draw(closeYourTomeTyrant, glowDrawPos, Projectile.Frame(), drawColor, drawRotation, drawOrigin, drawScale, drawEffects, layerDepth);
            }
            spriteBatch.End();
            spriteBatch.Begin();


            //Actually draw it
            spriteBatch.Draw(closeYourTomeTyrant, drawPos, Projectile.Frame(), drawColor, drawRotation, drawOrigin, drawScale, drawEffects, layerDepth);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawTomeSprite(ref lightColor);
            return false;
        }
    }

    public class AlcadBombSuckDraw
    {
        public AlcadBombSuckDraw()
        {
            oldPos = new Vector2[16];
            scale = 1f;
            randScale = Main.rand.NextFloat(0.275f, 1f);
        }
        public Vector2[] oldPos;
        public Vector2 position;
        public float rotation;
        public float timer;
        public float scale;
        public float randScale;
    }

    public class AlcadBombProj : ModProjectile,
          IPreDrawMaskShader,
          IDrawMaskShader
    {
        private float _drawScale;
        private float _scaleOutMult;
        private List<AlcadBombSuckDraw> _suckDraws = new();

        private ref float Timer => ref Projectile.ai[0];
        private ref float Die => ref Projectile.ai[1];
        public bool IsCharged => Timer >= 60;

        private Player Owner => Main.player[Projectile.owner];
        public override void SetDefaults()
        {
            Projectile.width = 128;
            Projectile.height = 128;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = int.MaxValue;
        }

        private bool ShouldConsumeMana()
        {
            return Timer % 7 == 0;
        }
        private void UpdateDamageForManaSickness(Player player)
        {
            Projectile.damage = (int)player.GetDamage(DamageClass.Magic).ApplyTo(player.HeldItem.damage);
        }
        private void AI_Attack()
        {
            //here we handle calculating when to attack!
            if (Main.myPlayer != Projectile.owner)
                return;

            Player player = Owner;
            UpdateDamageForManaSickness(player);
            bool stillInUse = player.channel && player.ownedProjectileCounts[ModContent.ProjectileType<AlcadBombHeldProj>()] > 0;
            if (!stillInUse)
            {
                Die = 1;
                Projectile.netUpdate = true;
            }
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            Projectile.velocity = Vector2.Zero;
            if (Timer == 1 && Main.myPlayer == Projectile.owner)
            {
                float maxBeamLength = Vector2.Distance(Owner.Center, Main.MouseWorld);
                Vector2 direction = (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.Zero);
                float length = ProjectileHelper.PerformBeamHitscan(Owner.Center, direction, maxBeamLength);
                Projectile.Center = Owner.Center + direction * length;
                Projectile.netUpdate = true;
            }

            if (Timer == 10)
            {
                for (float f = 0; f < 32; f++)
                {
                    float progress = f / 32;
                    float rot = progress * MathHelper.TwoPi;
                    Vector2 vel = rot.ToRotationVector2() * 4;
                    Dust.NewDustPerfect(Projectile.Center, DustID.CorruptTorch, vel);
                }
                SoundStyle explodeStyle = new("Stellamod/Assets/Sounds/STARGROP");
                SoundEngine.PlaySound(explodeStyle, Projectile.position);
                FXUtil.ShakeCamera(Projectile.position, 1024, 10);
            }

            if (Timer < 60)
            {
                float progress = Timer / 60f;
                float easedProgress = Easing.OutExpo(progress);
                float scale = MathHelper.Lerp(0f, Main.rand.NextFloat(0.95f, 1f), easedProgress);
                _drawScale = scale;
                _scaleOutMult = 1f;
            }

            if (Die == 0)
            {
                _drawScale += 0.005f;
                if (_drawScale >= 2f)
                {
                    _drawScale = 2f;
                }
            }
            else if (Die == 1)
            {
                _drawScale *= 0.926f;
                if (_drawScale <= 0.01f)
                {
                    Projectile.Kill();
                }
            }



            AI_Suck();
            AI_Attack();
            AI_KeepAlive();
            ManageSuckDraws();
        }

        private void AI_Suck()
        {
            if (Die == 1)
                return;
            foreach (NPC npc in Main.ActiveNPCs)
            {

                if (!npc.CanBeChasedBy())
                    continue;

                float distance = Vector2.Distance(npc.Center, Projectile.Center);
                float maxPullDistance = 800 * MathHelper.Clamp(Timer / 60f, 0f, 1f);
                if (distance <= maxPullDistance)
                {
                    Vector2 blowVelocity = Projectile.Center - npc.Center;
                    float p = distance / maxPullDistance;
                    p = 1f - p;
                    blowVelocity *= 0.0052f * p * MathHelper.Clamp(Timer / 60f, 0f, 1f);
                    npc.GetGlobalNPC<RuneOfWindBlowNPC>().BlowVelocity = blowVelocity;
                }
            }
        }
        private void AI_KeepAlive()
        {
            Player player = Main.player[Projectile.owner];
            if (player.noItems || player.CCed || player.dead || !player.active)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    Die = 1;
                    Projectile.netUpdate = true;
                }
            }

            if (Main.myPlayer == Projectile.owner)
            {
                if (!player.channel)
                {
                    Die = 1;
                    Projectile.netUpdate = true;
                }
            }
        }

        private void ManageSuckDraws()
        {
            float radius = 82;
            if (Timer % 1 == 0)
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2CircularEdge(radius, radius);
                AlcadBombSuckDraw suckDraw = new AlcadBombSuckDraw
                {
                    position = pos,
                    rotation = (Projectile.Center - pos).SafeNormalize(Vector2.Zero).ToRotation()
                };
                _suckDraws.Add(suckDraw);
            }


            //Mange them all
            for (int i = 0; i < _suckDraws.Count; i++)
            {
                AlcadBombSuckDraw suckDraw = _suckDraws[i];
                suckDraw.timer++;
                float progress = suckDraw.timer / 60f;
                float easedProgress = Easing.InOutCubic(progress);
                suckDraw.position = Vector2.Lerp(suckDraw.position, Projectile.Center, easedProgress);
                suckDraw.scale = _drawScale;

                //Update old pos
                for (int j = suckDraw.oldPos.Length - 1; j > 0; j--)
                {
                    suckDraw.oldPos[j] = suckDraw.oldPos[j - 1];
                }
                if (suckDraw.oldPos.Length > 0)
                    suckDraw.oldPos[0] = suckDraw.position;
            }
        }

        public MiscShaderData GetMaskDrawShader()
        {
            //Use the defaults
            var shaderData = GameShaders.Misc["LunarVeil:SimpleDistortion"];
            shaderData.Shader.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * 15);
            shaderData.Shader.Parameters["distortion"].SetValue(0.2f);
            shaderData.Shader.Parameters["distortingNoiseTexture"].SetValue(TextureRegistry.CloudNoise2.Value);
            return shaderData;
        }

        public void PreDrawMask(SpriteBatch spriteBatch)
        {
            var shaderData = GameShaders.Misc["LunarVeil:SimpleDistortion"];
            shaderData.Shader.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * 15);
            shaderData.Shader.Parameters["distortion"].SetValue(0.2f);
            shaderData.Shader.Parameters["distortingNoiseTexture"].SetValue(TextureRegistry.CloudNoise2.Value);
            shaderData.Apply();

            Texture2D texture = ModContent.Request<Texture2D>(Texture + "_Outline").Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Color drawColor = Color.White;
            Vector2 drawOrigin = texture.Size() / 2f;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer,
                   shaderData.Shader, Main.GameViewMatrix.TransformationMatrix);

            for (int i = 0; i < _suckDraws.Count; i++)
            {
                AlcadBombSuckDraw suckDraw = _suckDraws[i];
                DrawSuckParticle2(spriteBatch, suckDraw);
            }

            //spriteBatch.Draw(texture, drawPos, null, drawColor, Projectile.rotation, drawOrigin, _drawScale * _scaleOutMult, SpriteEffects.None, 0f);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }

        private void DrawSuckParticle(SpriteBatch spriteBatch, AlcadBombSuckDraw draw)
        {
            //PrimDrawer ??= new PrimDrawer(null, null, GameShaders.Misc["VampKnives:SuperSimpleTrail"]);
            //draw.DrawPrims(PrimDrawer);

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = draw.position - Main.screenPosition;
            Color drawColor = Color.White;
            Vector2 drawOrigin = texture.Size() / 2f;
            Vector2 drawScale = new Vector2(4f, 0.25f) * draw.randScale;
            drawScale *= Easing.SpikeOutCirc(draw.timer / 60f);
            float drawRot = draw.rotation;
            spriteBatch.Draw(texture, drawPos, null, drawColor, drawRot, drawOrigin, drawScale * _drawScale * _scaleOutMult, SpriteEffects.None, 0f);
        }

        private void DrawSuckParticle2(SpriteBatch spriteBatch, AlcadBombSuckDraw draw)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture + "_Outline").Value;
            Vector2 drawPos = draw.position - Main.screenPosition;
            Color drawColor = Color.White;
            Vector2 drawOrigin = texture.Size() / 2f;
            Vector2 drawScale = new Vector2(4f, 0.25f) * draw.randScale;
            drawScale *= Easing.SpikeOutCirc(draw.timer / 60f);
            float drawRot = draw.rotation;
            spriteBatch.Draw(texture, drawPos, null, drawColor, drawRot, drawOrigin, drawScale * _drawScale * _scaleOutMult, SpriteEffects.None, 0f);
        }

        public void DrawMask(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Color drawColor = Color.White;
            Vector2 drawOrigin = texture.Size() / 2f;

            for (int i = 0; i < _suckDraws.Count; i++)
            {
                AlcadBombSuckDraw suckDraw = _suckDraws[i];
                DrawSuckParticle(spriteBatch, suckDraw);
            }

            //Draw Main Texture
            //  spriteBatch.Draw(texture, drawPos, null, drawColor, Projectile.rotation, drawOrigin, _drawScale * _scaleOutMult, SpriteEffects.None, 0f);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}