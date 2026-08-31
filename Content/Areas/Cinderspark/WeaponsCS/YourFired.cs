using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common;
using Stellamod.Dusts;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.WeaponsCS;

    public class YourFired : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 170;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 12;
            Item.shootSpeed = 25;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.DamageType = DamageClass.Ranged;
            Item.shoot = ModContent.ProjectileType<YourFiredProj>();
            Item.shootSpeed = 20f;
            Item.useAnimation = 36;
            Item.useTime = 36;
            Item.consumable = false;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-3f, -2f);
        }
    }

public class YourFiredProj : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.width = 38;
        Projectile.height = 40;
        Projectile.friendly = true;
        Projectile.timeLeft = 300;
    }

    public override void AI()
    {
        Timer++;
        if (Timer == 1)
        {
            //Effects
            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, Projectile.position);
        }


        Projectile.velocity.Y += 0.3f;
        Projectile.rotation = Projectile.velocity.ToRotation();
        // And create bright light.
        Lighting.AddLight(Projectile.Center, Color.OrangeRed.ToVector3() * 0.78f * MathF.Sin(Timer * 0.5f));
    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/RekFireballDeath"), Projectile.position);
        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/CombusterReady"), Projectile.position);
        float num = 8;
        float maxDelay = 30;
        for (int i = 0; i < num; i++)
        {
            float clusterRadius = 256;
            float progress = i / (float)num;
            float delay = progress * maxDelay;
            Vector2 randPosition = Projectile.Center + Main.rand.NextVector2Circular(clusterRadius, clusterRadius);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), randPosition, Vector2.Zero,
                ModContent.ProjectileType<YourFiredExplosionProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai1: delay);
        }
    }
}

    public class YourFiredExplosionProj : ModProjectile
    {
        //Texture
        public override string Texture => TextureRegistry.EmptyTexture;

        //AI
        private float LifeTime => 32f;
        private ref float Timer => ref Projectile.ai[0];
        private ref float DelayTimer => ref Projectile.ai[1];
        private float Progress
        {
            get
            {
                float p = Timer / LifeTime;
                return MathHelper.Clamp(p, 0, 1);
            }
        }

        //Draw Code

        public static int DrawMode;
        private bool SpawnDustCircle;

        //Trailing
        private Asset<Texture2D> FrontTrailTexture => TrailRegistry.WaterTrail;
        private MiscShaderData FrontTrailShader => TrailRegistry.LaserShader;

        private Asset<Texture2D> BackTrailTexture => TrailRegistry.WhispyTrail;
        private MiscShaderData BackTrailShader => TrailRegistry.LaserShader;

        //Radius
        private float StartRadius => 4;
        private float EndRadius => Main.rand.NextFloat(128, 196);
        private float Width => Main.rand.NextFloat(32, 64);

        //Colors
        private Color FrontCircleStartDrawColor => Color.White;
        private Color FrontCircleEndDrawColor => Color.OrangeRed;
        private Color BackCircleStartDrawColor => Color.Lerp(Color.White, Color.OrangeRed, 0.4f);
        private Color BackCircleEndDrawColor => Color.Lerp(Color.DarkGoldenrod, Color.OrangeRed, 0.7f);
        private Vector2[] CirclePos;

        public override void SetDefaults()
        {
            Projectile.width = 384;
            Projectile.height = 384;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.timeLeft = (int)LifeTime;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;

            //Points on the circle
            CirclePos = new Vector2[64];
        }

        public override void AI()
        {
            if (DelayTimer > 0)
            {
                Projectile.friendly = false;
                Projectile.timeLeft = (int)LifeTime;
                DelayTimer--;
                return;
            }

            Projectile.friendly = true;
            Timer++;
            if (Timer == 1)
            {
                Main.LocalPlayer.GetModPlayer<ShakePlayer>().ShakeAtPosition(Projectile.Center, 1024, 16f);
                SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Kaboom"), Projectile.position);
                for (int i = 0; i < 4; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DarkGray, 1f).noGravity = true;
                }
            }

            AI_ExpandCircle();
            AI_DustCircle();
        }

        private void AI_ExpandCircle()
        {
            float easedProgess = Easing.InOutCirc(Progress);
            float radius = MathHelper.Lerp(StartRadius, EndRadius, easedProgess);
            DrawCircle(radius);
        }

        private void AI_DustCircle()
        {
            if (!SpawnDustCircle && Timer >= 15)
            {
                for (int i = 0; i < 48; i++)
                {
                    Vector2 rand = Main.rand.NextVector2CircularEdge(EndRadius, EndRadius);
                    Vector2 pos = Projectile.Center + rand;
                    Dust d = Dust.NewDustPerfect(pos, ModContent.DustType<GlowDust>(), Vector2.Zero,
                        newColor: BackCircleStartDrawColor,
                        Scale: Main.rand.NextFloat(0.3f, 0.6f));
                    d.noGravity = true;
                }
                SpawnDustCircle = true;
            }
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        private void DrawCircle(float radius)
        {
            Vector2 startDirection = Vector2.UnitY;
            for (int i = 0; i < CirclePos.Length; i++)
            {
                float circleProgress = i / (float)CirclePos.Length;
                float radiansToRotateBy = circleProgress * (MathHelper.TwoPi + MathHelper.PiOver4 / 2);
                CirclePos[i] = Projectile.Center + startDirection.RotatedBy(radiansToRotateBy) * radius;
            }
        }

        public float WidthFunction(float completionRatio)
        {
            float width = Width;
            float startExplosionScale = 4f;
            float endExplosionScale = 0f;
            float easedProgess = Easing.OutCirc(Progress);
            float scale = MathHelper.Lerp(startExplosionScale, endExplosionScale, easedProgess);
            switch (DrawMode)
            {
                default:
                case 0:
                    return Projectile.scale * scale * width * Easing.SpikeInOutCirc(Progress);
                case 1:
                    return Projectile.scale * width * 2.2f * Easing.SpikeInOutCirc(Progress);

            }
        }

        public static Color ColorFunction(float completionRatio)
        {
            switch (DrawMode)
            {
                default:
                case 0:
                    //Front Trail
                    return Color.Transparent;
                case 1:
                    //Back Trail
                    return Color.Transparent;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 180);
        }
    }