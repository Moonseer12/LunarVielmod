using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.MoonspiralTower.WeaponsMT
{
    public class Curlistine : ModItem
    {
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {

            // Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
            var line = new TooltipLine(Mod, "Curlistine", "(A) Very High Damage Scaling with frost explosions!")
            {
                OverrideColor = new Color(220, 87, 24)

            };
            tooltips.Add(line);





            base.ModifyTooltips(tooltips);



        }

        public override void SetDefaults()
        {

            Item.damage = 15; // Sets the Item's damage. Note that projectiles shot by this weapon will use its and the used ammunition's damage added together.
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 30; // The Item's use time in ticks (60 ticks == 1 second.)
            Item.useAnimation = 30; // The length of the Item's use animation in ticks (60 ticks == 1 second.)
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.staff[Item.type] = true;
            Item.noMelee = true; //so the Item's animation doesn't do damage
            Item.knockBack = 11;
            Item.UseSound = SoundID.Item101; // The sound that this Item plays when used.
            Item.shoot = ModContent.ProjectileType<CurlistineProj>();
            Item.shootSpeed = 2f; // the speed of the projectile (measured in pixels per frame)
            Item.channel = true;

            Item.autoReuse = true;


        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<PearlescentScrap, BlankSword>();
        }
    }

    public class CurlistineProj : ModProjectile
    {
        NPC target;
        int afterImgCancelDrawCount = 0;

        Vector2 endPoint;
        Vector2 controlPoint1;
        Vector2 controlPoint2;
        Vector2 initialPos;
        Vector2 wantedEndPoint;
        bool initialization = false;
        float AoERadiusSquared = 36000;
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

        public override void AI()
        {








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
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.Center.DistanceSQ(Projectile.Center) < AoERadiusSquared && !npc.dontTakeDamage)
                    {
                        NPC.HitInfo hitInfo = new();
                        hitInfo.Damage = Projectile.damage;
                        //(int)Main.player[Projectile.owner].GetDamage(DamageClass.Summon).ApplyTo(Projectile.damage)
                        hitInfo.DamageType = DamageClass.Melee;
                        npc.StrikeNPC(hitInfo);
                    }
                }
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
            Projectile.penetrate = 3;
            Projectile.usesIDStaticNPCImmunity = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 120;
            Projectile.Size = new Vector2(30, 30);
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 200;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Color afterImgColor = Main.hslToRgb(Projectile.ai[1], 1, 0.5f);
            float opacityForSparkles = 1 - (float)afterImgCancelDrawCount / 30;
            afterImgColor.A = 70;
            afterImgColor.B = 255;
            afterImgColor.G = 215;
            afterImgColor.R = 96;
            Main.instance.LoadProjectile(ProjectileID.RainbowRodBullet);
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            for (int i = afterImgCancelDrawCount + 1; i < Projectile.oldPos.Length; i++)
            {
                //if(i % 2 == 0)
                float rotationToDraw;
                Vector2 interpolatedPos;
                for (float j = 0; j < 1; j += 0.25f)
                {
                    if (i == 0)
                    {
                        rotationToDraw = Utils.AngleLerp(Projectile.rotation, Projectile.oldRot[0], j);
                        interpolatedPos = Vector2.Lerp(Projectile.Center, Projectile.oldPos[0] + Projectile.Size / 2, j);
                    }
                    else
                    {
                        interpolatedPos = Vector2.Lerp(Projectile.oldPos[i - 1] + Projectile.Size / 2, Projectile.oldPos[i] + Projectile.Size / 2, j);
                        rotationToDraw = Utils.AngleLerp(Projectile.oldRot[i - 1], Projectile.oldRot[i], j);
                    }
                    Main.EntitySpriteDraw(texture, interpolatedPos - Main.screenPosition + Projectile.Size / 2, null, afterImgColor * (1 - i / (float)Projectile.oldPos.Length), rotationToDraw, texture.Size() / 2, 1, SpriteEffects.None, 0);
                }
            }

            return false;
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ShakeScreenPosition.Shake = 4;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<FrostKaboom>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);

            SoundEngine.PlaySound(SoundID.Item110, Projectile.position);
        }

    }

    public class FrostKaboom : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("FrostShotIN");
            Main.projFrames[Projectile.type] = 20;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = 128;
            Projectile.height = 129;

            Projectile.timeLeft = 40;
            Projectile.scale = 1.2f;
            Projectile.penetrate = -1;
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
            Projectile.rotation += 0.03f;
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
                if (++Projectile.frame >= 20)
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

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);

        }
    }
}