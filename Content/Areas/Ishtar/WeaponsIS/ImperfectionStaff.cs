using Stellamod.Common;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Ishtar.WeaponsIS
{
    public class ImperfectionStaff : ModItem
    {
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {

            // Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
            var line = new TooltipLine(Mod, "Perfectionss", "(A) Extremely good for targeting! Needs an enemy alive to work!")
            {
                OverrideColor = new Color(220, 87, 24)

            };
            tooltips.Add(line);





            base.ModifyTooltips(tooltips);



        }

        public override void SetDefaults()
        {

            Item.damage = 49; // Sets the Item's damage. Note that projectiles shot by this weapon will use its and the used ammunition's damage added together.
            Item.DamageType = DamageClass.Magic;
            Item.useTime = 30; // The Item's use time in ticks (60 ticks == 1 second.)
            Item.useAnimation = 30; // The length of the Item's use animation in ticks (60 ticks == 1 second.)
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.staff[Item.type] = true;
            Item.noMelee = true; //so the Item's animation doesn't do damage
            Item.knockBack = 6;
            Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/GhostExcalibur1"); // The sound that this Item plays when used.
            Item.shoot = ModContent.ProjectileType<ImperfectionProj>();
            Item.shootSpeed = 2f; // the speed of the projectile (measured in pixels per frame)
            Item.channel = true;
            Item.mana = 6;

            Item.autoReuse = true;


        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float numberProjectiles = 6;
            float rotation = MathHelper.ToRadians(14);
            position += Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 45f;
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * 1f; // This defines the projectile roatation and speed. .4f == projectile speed
                Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, Item.knockBack, player.whoAmI);
            }
            return false;
        }

    }

    public class ImperfectionProj : ModProjectile
    {
        NPC target;
        int afterImgCancelDrawCount = 0;

        Vector2 endPoint;
        Vector2 controlPoint1;
        Vector2 controlPoint2;
        Vector2 initialPos;
        Vector2 wantedEndPoint;
        bool initialization = false;
        float t = 0;

        public static Vector2 CubicBezier(Vector2 start, Vector2 controlPoint1, Vector2 controlPoint2, Vector2 end, float t)
        {
            float tSquared = t * t;
            float tCubed = t * t * t;
            return
                -(start * (-tCubed + (3 * tSquared) - (3 * t) - 1) +
                controlPoint1 * ((3 * tCubed) - (6 * tSquared) + (3 * t)) +
                controlPoint2 * ((-3 * tCubed) + (3 * tSquared)) +
                end * tCubed);
        }
        private float alphaCounter = 0;
        public override void AI()
        {





            if (alphaCounter < 3)
            {
                alphaCounter += 0.08f;
            }


            if (!initialization)
            {
                initialPos = Projectile.Center;
                endPoint = Projectile.Center;
            }
            float distanceSQ = float.MaxValue;
            if (target == null || !target.active)
                for (int i = 0; i < Main.npc.Length; i++)
                {
                    if ((target == null || Main.npc[i].DistanceSQ(Projectile.Center) < distanceSQ) && Main.npc[i].active && !Main.npc[i].friendly && !Main.npc[i].dontTakeDamage && Main.npc[i].type != NPCID.CultistBossClone)
                    {
                        target = Main.npc[i];
                        distanceSQ = Projectile.Center.DistanceSQ(target.Center);
                    }
                }
            if (target != null && target.DistanceSQ(Projectile.Center) < 10000000 && target.active)
            {
                wantedEndPoint = initialPos - (target.Center - initialPos);
                if (Projectile.ai[0] < 10)
                {
                    endPoint = wantedEndPoint;
                }
            }
            if (!initialization)
            {
                controlPoint1 = Projectile.Center + Main.rand.NextVector2CircularEdge(1000, 1000);
                controlPoint2 = endPoint + Main.rand.NextVector2CircularEdge(1000, 1000);
                //controlPoint2 = Vector2.Lerp(endPoint, initialPos, 0.33f) + Main.player[Projectile.owner].velocity * 70;
                //if (target != null)
                //    controlPoint1 = Vector2.Lerp(endPoint, initialPos, 0.66f) + target.velocity * 70;
                //else
                //    Projectile.Kill();
                initialization = true;
            }
            Projectile.velocity = Vector2.Zero;
            Projectile.rotation = (Projectile.Center - CubicBezier(initialPos, controlPoint1, controlPoint2, endPoint, t + 0.025f)).ToRotation() - MathHelper.PiOver2;
            endPoint = endPoint.MoveTowards(wantedEndPoint, 16);
            if (t > 1)
            {
                afterImgCancelDrawCount++;
            }
            else if (target != null)
            {
                Projectile.Center = CubicBezier(initialPos, controlPoint1, controlPoint2, endPoint, t);
            }
            if (target == null || Projectile.ai[0] > 200)
                Projectile.Kill();

            t += 0.01f;

            Projectile.ai[0]++;

        }
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            //DisplayName.SetDefault("Stardust bolt");
            //DisplayName.AddTranslation(8, "Tiro de Pó Estelar");
        }
        public override void SetDefaults()
        {
            Projectile.penetrate = 2;
            Projectile.usesIDStaticNPCImmunity = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 120;
            Projectile.Size = new Vector2(12, 12);
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 200;
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
            Lighting.AddLight(Projectile.Center, Color.PaleVioletRed.ToVector3() * 1.75f * Main.essScale);
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 30; i++)
            {
                Main.rand.NextVector2CircularEdge(1f, 1f);
            }

            Projectile.Kill();
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);
            SoundEngine.PlaySound(SoundID.DD2_BetsysWrathImpact, Projectile.position);
            Main.LocalPlayer.GetModPlayer<ShakePlayer>().ShakeAtPosition(Projectile.Center, 1024f, 4f);
        }
    }
}