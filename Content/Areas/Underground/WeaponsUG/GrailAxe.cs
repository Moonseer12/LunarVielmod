using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Underground.WeaponsUG
{
    public class GrailAxe : ModItem
    {
        public int AttackCounter = 1;
        public int combowombo = 0;
        public override void SetDefaults()
        {
            Item.damage = 45;
            Item.DamageType = DamageClass.Generic;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 12;
            Item.noMelee = true;

            Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/StarSheith");
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<GrailAxeProj>();
            Item.shootSpeed = 20f;
            Item.noUseGraphic = true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ModContent.ProjectileType<GrailAxeProj>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            int dir = AttackCounter;
            AttackCounter = -AttackCounter;
            Projectile.NewProjectile(source, position, velocity, type, damage * 3, knockback, player.whoAmI, 1, dir);



            float numberProjectiles2 = 4;
            float rotation = MathHelper.ToRadians(20);
            position += Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 45f;
            for (int i = 0; i < numberProjectiles2; i++)
            {

                Vector2 perturbedSpeed2 = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles2 - 1))) * 1f;// This defines the projectile roatation and speed. .4f == projectile speed

                Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed2.X, perturbedSpeed2.Y, ModContent.ProjectileType<GrailShot>(), damage, Item.knockBack, player.whoAmI);
            }
            Projectile.NewProjectile(source, position, velocity * 1f, ModContent.ProjectileType<GrailAxeProj>(), damage * 1, knockback, player.whoAmI, 1, dir);

            return false;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<MinersGold, BlankSword>();
        }
    }

    public class GrailAxeProj : ModProjectile
    {
        public float holdOffset = 30f;
        private bool ParticleSpawned;
        private int SwingTime => (int)(40 / Owner.GetAttackSpeed(DamageClass.Generic));
        private Player Owner => Main.player[Projectile.owner];
        public override void SetDefaults()
        {

            Projectile.damage = 10;
            Projectile.timeLeft = SwingTime;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.height = 74;
            Projectile.width = 74;
            Projectile.friendly = true;
            Projectile.scale = 1f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
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

        public override void AI()
        {
            Vector3 RGB = new(1.28f, 0f, 1.28f);
            float multiplier = 0.2f;
            RGB *= multiplier;
            Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);

            for (int i = 0; i < 1; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position - Projectile.velocity, Projectile.width, Projectile.height, DustID.IceTorch, 0, 0, 100, Color.Violet, 1f);
                dust.noGravity = true;
                dust.velocity *= 2f;
                Dust.NewDustDirect(Projectile.position - Projectile.velocity, Projectile.width, Projectile.height, DustID.IceTorch, 0f, 0f, 1000, Color.Violet, 1f);
            }

            int dir = (int)Projectile.ai[1];
            float swingProgress = Lerp(Utils.GetLerpValue(0f, SwingTime, Projectile.timeLeft));
            // the actual rotation it should have
            float defRot = Projectile.velocity.ToRotation();
            // starting rotation
            float endSet = MathHelper.PiOver2 / 0.2f;
            float start = defRot - endSet;

            // ending rotation
            float end = defRot + endSet;
            // current rotation obv
            float rotation = dir == 1 ? start.AngleLerp(end, swingProgress) : start.AngleLerp(end, 1f - swingProgress);
            // offsetted cuz sword sprite
            Vector2 position = Owner.RotatedRelativePoint(Owner.MountedCenter);
            position += rotation.ToRotationVector2() * holdOffset;
            Projectile.Center = position;
            Projectile.rotation = (position - Owner.Center).ToRotation() + MathHelper.PiOver4;

            Owner.heldProj = Projectile.whoAmI;
            Owner.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
            Owner.itemRotation = rotation * Owner.direction;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;


            if (!ParticleSpawned)
            {
                ParticleSpawned = true;
            }
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
            Color drawColor = Projectile.GetAlpha(lightColor);

            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }

    public class GrailShot : ModProjectile
    {
        private bool Moved;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shadow Hand");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = 35;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.timeLeft = 250;
            Projectile.alpha = 0;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(3) && !target.boss)
            {
                target.AddBuff(BuffID.Confused, 180);
            }
        }

        private float alphaCounter = 0;
        public override void AI()
        {
            Projectile.velocity *= 0.96f;
            Projectile.ai[1]++;
            if (!Moved && Projectile.ai[1] >= 0)
            {
                Projectile.spriteDirection = Projectile.direction;
                Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f + 3.14f;
                Projectile.alpha = 255;
                Moved = true;
            }
            if (Projectile.ai[1] >= 20)
            {
                Projectile.tileCollide = true;
            }


            if (Projectile.ai[1] == 160)
            {
                var EntitySource = Projectile.GetSource_Death();
                Projectile.NewProjectile(EntitySource, Projectile.Center.X, Projectile.Center.Y, 0, 0, ModContent.ProjectileType<KaBoomMagic2>(), Projectile.damage, 1, Projectile.owner, 0, 0);
                SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Binding_Abyss_Rune"));
                Projectile.Kill();
            }
            if (Projectile.timeLeft <= 50)
            {
                Projectile.alpha += 4;
                alphaCounter -= 0.08f;
            }
            else
            {
                if (alphaCounter <= 1)
                {
                    alphaCounter += 0.08f;
                }
            }

            Projectile.spriteDirection = Projectile.direction;
            Projectile.rotation += 0.08f;
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
            Lighting.AddLight(Projectile.Center, Color.LightCyan.ToVector3() * 1.75f * Main.essScale);
        }
    }

    public class KaBoomMagic2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = false;
            Projectile.width = 196;
            Projectile.height = 150;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 18;
            Projectile.scale = 1f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
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
            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 6)
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
}