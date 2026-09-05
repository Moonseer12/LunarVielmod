using Stellamod.Assets;
using Stellamod.Common.MagicCauldron;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Dusts;
using Stellamod.Core.Bases;
using Stellamod.Core.Pixelation;
using Stellamod.Content.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.WeaponsIL;

public class Orion : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToArtifact();
        Item.damage = 540;
        Item.DamageType = DamageClass.Magic;

        Item.useTime = 32;
        Item.useAnimation = 32;
        Item.useStyle = ItemUseStyleID.Swing;

        Item.knockBack = 6;
        Item.noUseGraphic = true;

        Item.autoReuse = true;
        Item.shoot = ModContent.ProjectileType<OrionProj>();
        Item.shootSpeed = 15;
        Item.mana = 16;
    }

    public override void AddRecipes()
    {
        this.RegisterBrew<IllurineScale, BlankStaff>();
    }
}

public class OrionProj : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 27;
        ProjectileID.Sets.TrailingMode[Type] = 2;
        Main.projFrames[Type] = 3;
    }

    public override void SetDefaults()
    {
        Projectile.width = 30;
        Projectile.height = 30;
        Projectile.penetrate = -1;
        Projectile.friendly = true;
        Projectile.hostile = false;
    }

    public override void AI()
    {
        Timer++;
        Projectile.velocity.Y += 0.1f;
        Projectile.rotation = Projectile.velocity.ToRotation();


        if (Timer % 15 == 0)
        {
            //Spawn Star
            if (this.OwnedByLocalClient())
            {
                Vector2 offset = Main.rand.NextVector2Circular(24, 24);
                Vector2 velocity = Main.rand.NextVector2Circular(2, 2);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + offset, velocity,
                    ModContent.ProjectileType<OrionStarProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }

        if (Main.rand.NextBool(4))
        {
            var sp = SparkleParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity.SafeNormalize(Vector2.Zero));
            sp.dampening = 0.05f;
            sp.noTileCollide = true;
            sp.outerColor = Color.Blue;
            sp.innerColor = Color.White;
            sp.Scale *= 0.6f;
            sp.fast = true;
            sp.gravity = 0;
        }

        if (Timer % 8 == 0)
        {
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height,
                ModContent.DustType<Sparkle>(), newColor: Color.White);
        }
    }


    public override void OnKill(int timeLeft)
    {
        if (this.OwnedByLocalClient())
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<SiriusBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

        }

        SoundStyle sound;
        //Play Sound
        switch (Main.rand.Next(2))
        {
            default:
                sound = new SoundStyle("Stellamod/Assets/Sounds/M38F30Bomb1");
                //SoundEngine.PlaySound(, Projectile.position);
                break;
            case 1:
                sound = new SoundStyle("Stellamod/Assets/Sounds/M38F30Bomb2");
                break;
        }

        sound.PitchVariance = 0.7f;
        SoundEngine.PlaySound(sound, Projectile.position);
    }

    public float WidthFunction(float completionRatio)
    {
        float baseWidth = Projectile.scale * Projectile.width;
        return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
    }

    public static Color ColorFunction(float completionRatio)
    {
        return Color.Lerp(ColorFunctions.Niivin, Color.Black, completionRatio);
    }

    private void DrawAuraTrail(GraphicsDevice gDevice)
    {
        BasicLaserShader basicLaserShader = ShaderContent.GetInstance<BasicLaserShader>();
        basicLaserShader.LaserTexture = AssetManager.LaserTextures.Aura;
        basicLaserShader.InnerColor = Color.White;
        basicLaserShader.OuterColor = Color.SkyBlue;
        TrailDrawer.Draw(Projectile.oldPos, ColorFunction, WidthFunction, basicLaserShader, Projectile.Size * 0.5f);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawAuraTrail);
        DrawUtilities.DrawSpriteAfterImage(Main.spriteBatch, Projectile, Color.Blue, Color.Purple * 0.4f, 0.3f);
        SpritebatchDrawer orionDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        Main.spriteBatch.Draw(orionDrawer);

        orionDrawer.VerticalFrame(1, Main.projFrames[Type]);
        orionDrawer.color = Color.Lerp(Color.White, Color.Transparent, EasingFunction.InOutSine(Timer / 38));
        Main.spriteBatch.Draw(orionDrawer);

        //Main.spriteBatch.Draw(orionDrawer);

        orionDrawer.VerticalFrame(2, Main.projFrames[Type]);
        orionDrawer.color = Color.Lerp(Color.White, Color.SkyBlue, ExtraMath.Osc(0f, 1f, speed: 12));
        Main.spriteBatch.Draw(orionDrawer);
        return false;
    }
}
public class OrionStarProj : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.width = 8;
        Projectile.height = 8;
        Projectile.friendly = false;
        Projectile.hostile = false;
        Projectile.tileCollide = false;
    }

    public override void AI()
    {
        Timer++;
        if (Timer % 4 == 0)
        {
            Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(32, 32), DustID.GemSapphire, Scale: 0.6f);
        }

        if(Timer == 25)
        {
           for(float f = 0; f < 4; f++)
            {
                var dp = DustParticle.Spawn(Projectile.Center, Main.rand.NextVector2Circular(8, 8));
                dp.dampening = 0.05f;
                dp.outerColor = Color.SkyBlue;
                dp.innerColor = Color.White;
                dp.gravity = 0;
                dp.noTileCollide = true;
               
            }
        }

        if (Timer >= 30)
        {
            if (this.OwnedByLocalClient())
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                    ModContent.ProjectileType<OrionStarBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

            }

            var sound = SoundID.DD2_ExplosiveTrapExplode with { PitchVariance = 0.7f };
            sound.Volume = 0.4f;
            SoundEngine.PlaySound(sound, Projectile.position);
            for (float f = 0; f < 8; f++)
            {
                var dp = DustParticle.Spawn(Projectile.Center, Main.rand.NextVector2Circular(20, 20));
                dp.dampening = 0.1f;
                dp.outerColor = Color.SkyBlue;
                dp.innerColor = Color.White;
                dp.gravity = 0;
                dp.noTileCollide = true;
                dp.Scale *= 0.6f;
                dp.fast = true;
                dp.superFast = true;
            }
            PixelPrimitiveCircleFactory.CreateGenericBoom(Projectile.Center, Color.SkyBlue, Color.DarkBlue, 25, 40);
            var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.SkyBlue, Color.Blue, Color.DarkBlue, duration: 10, baseSize: 0.17f);
            fx.Scale *= 1.6f;
            Projectile.Kill();
            Timer = 0;
        }
        Projectile.velocity *= 0.9f;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        SpritebatchDrawer starDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        float alpha = MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(Timer / 15f));
        starDrawer.color = Color.Lerp(Color.White, Color.Lerp(Color.White, Color.Blue, 0.75f), ExtraMath.Osc(0f, 1f, speed: 16)) * alpha;
        starDrawer.rotation = MathHelper.Lerp(0.75f, 0f, EasingFunction.InOutSine(Timer / 30f));
        starDrawer.scale *= MathHelper.Lerp(2f, 0f, EasingFunction.InOutSine(Timer / 30f));
        Main.spriteBatch.Draw(starDrawer);

        starDrawer.color = Color.Lerp(Color.SkyBlue, Color.White, ExtraMath.Osc(0f, 1f, speed: 16)) * alpha;
        starDrawer.VerticalFrame(1, Main.projFrames[Type]);
        Main.spriteBatch.Draw(starDrawer);
        return false;

    }
}

    public class OrionStarBoom : ModProjectile
    {
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
            Projectile.width = 129;
            Projectile.height = 129;
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
            Vector3 RGB = new(0.89f, 2.53f, 2.55f);
            // The multiplication here wasn't doing anything
            Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);
        }


        public override bool PreAI()
        {
            Timer++;
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
                (Color)GetAlpha(lightColor), 0f, origin, 2f, SpriteEffects.None, 0f);

            SpritebatchDrawer blackStarDrawer = SpritebatchDrawer.FromTextureAsset(TextureAssets.Projectile[ModContent.ProjectileType<OrionStarProj>()], Projectile.Center);
            blackStarDrawer.VerticalFrame(0, 2);
            blackStarDrawer.CenterOrigin();
            blackStarDrawer.color = Color.Black;
            blackStarDrawer.scale *= MathHelper.Lerp(2f, 0f, Timer / 30f);
            spriteBatch.Draw(blackStarDrawer);
            return false;
        }
    }