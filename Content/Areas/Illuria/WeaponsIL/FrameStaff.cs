using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.WeaponsIL
{
    public class FrameStaff : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 98;
            Item.DamageType = DamageClass.Magic;
            Item.knockBack = 1;
            Item.mana = 12;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.autoReuse = false;
            Item.UseSound = SoundID.DD2_BookStaffCast;
            Item.shoot = ModContent.ProjectileType<FrameStaffConnectorProj>();
            Item.shootSpeed = 0;
            Item.channel = true;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.shoot = ModContent.ProjectileType<FrameStaffNodeProj>();
            }
            else
            {
                Item.shoot = ModContent.ProjectileType<FrameStaffConnectorProj>();
            }
            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                if (player.ownedProjectileCounts[type] < 10)
                {
                    Projectile.NewProjectile(player.GetSource_FromThis(), Main.MouseWorld, velocity, type, damage, knockback, player.whoAmI);
                }

                return false;
            }
            //


            position = Main.MouseWorld;
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<IllurineScale, BlankStaff>();
        }
    }

    public class FrameStaffConnectorProj : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        Vector2[] ConnectorPos;
        int FrameTick;
        int FrameCounter;

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.timeLeft = int.MaxValue;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 14;
        }

        public override void AI()
        {
            AI_Channel();
            AI_FillPoints();
        }

        private void AI_Channel()
        {

            //Channeling
            Player player = Main.player[Projectile.owner];

            Timer++;
            if (Timer % 58 == 0)
            {
                SoundEngine.PlaySound(SoundRegistry.LaserChannel, player.position);
            }

            if (player.noItems || player.CCed || player.dead || !player.active)
                Projectile.Kill();


            if (Main.myPlayer == Projectile.owner)
            {
                if (!player.channel)
                    Projectile.Kill();
            }
        }

        private void AI_FillPoints()
        {
            //Get the points to connect
            List<Vector2> points = new();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (!Main.projectile[i].active)
                    continue;

                if (Main.projectile[i].owner != Projectile.owner)
                    continue;

                if (Main.projectile[i].type == ModContent.ProjectileType<FrameStaffNodeProj>())
                {
                    points.Add(Main.projectile[i].Center);
                }
            }

            ConnectorPos = points.ToArray();
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            float progress = 1f - (ConnectorPos.Length / 10f);
            float multiplier = progress * 2f;
            modifiers.FinalDamage *= multiplier;
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * 8;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public static Color ColorFunction(float completionRatio)
        {
            Color startColor = Color.Yellow;
            Color endColor = Color.Transparent;
            return Color.Lerp(startColor, endColor, completionRatio);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //This damages everything in the trail
            Vector2[] positions = ConnectorPos;
            float collisionPoint = 0;
            for (int i = 1; i < positions.Length; i++)
            {
                Vector2 position = positions[i];
                Vector2 previousPosition = positions[i - 1];
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), position, previousPosition, 6, ref collisionPoint))
                    return true;
            }

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (ConnectorPos.Length == 0)
                return false;

            Texture2D chainTexture = ModContent.Request<Texture2D>(Texture).Value;
            int frameCount = 8;
            int frameTime = 2;
            Rectangle animationFrame = chainTexture.AnimationFrame(
                ref FrameCounter, ref FrameTick, frameTime, frameCount, true);
            DrawHelper.DrawSupernovaChains(chainTexture, ConnectorPos, animationFrame, Projectile.alpha / 255f);
            return false;
        }
    }

    public class FrameStaffNodeProj : ModProjectile
    {
        private const float Whiten_Time = 60;
        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        private float WhiteTimer
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 40;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.timeLeft = 720;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            //Oscillate movement
            Timer++;
            if (Timer < 60)
            {
                float ySpeed = Timer / 60;
                ySpeed = Easing.SpikeInOutCirc(ySpeed);
                Projectile.velocity = new Vector2(0, -ySpeed);
            }
            else if (Timer < 120)
            {
                //Inverse
                float ySpeed = 1f - ((Timer - 60) / 60);
                ySpeed = Easing.SpikeInOutCirc(ySpeed);
                Projectile.velocity = new Vector2(0, ySpeed);
            }
            if (Timer == 120)
            {
                Timer = 0;
            }

            if (IsActivated())
            {
                WhiteTimer++;
                WhiteTimer = MathHelper.Clamp(WhiteTimer, 0, Whiten_Time);
            }
            else
            {
                WhiteTimer--;
                WhiteTimer = MathHelper.Clamp(WhiteTimer, 0, Whiten_Time);
            }
        }

        private bool IsActivated()
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (!p.active)
                    continue;
                if (p.owner != Projectile.owner)
                    continue;
                if (p.type != ModContent.ProjectileType<FrameStaffConnectorProj>())
                    continue;

                return true;
            }

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawAdditiveAfterImage(Projectile, Color.Aquamarine, Color.Transparent, ref lightColor);
            return base.PreDraw(ref lightColor);
        }

        public override void PostDraw(Color lightColor)
        {
            string glowTexture = Texture + "_White";
            Texture2D whiteTexture = ModContent.Request<Texture2D>(glowTexture).Value;

            Vector2 textureSize = new(38, 40);
            Vector2 drawOrigin = textureSize / 2;

            //Lerping
            float progress = WhiteTimer / Whiten_Time;
            Color drawColor = Color.Lerp(Color.Transparent, Color.White, progress);
            Vector2 drawPosition = Projectile.position - Main.screenPosition + drawOrigin;
            Main.spriteBatch.Draw(whiteTexture, drawPosition, null, drawColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
        }
    }
}