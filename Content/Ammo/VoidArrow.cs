using Stellamod.Assets;
using Stellamod.Core.MaskingShaderSystem;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Ammo
{
    public class VoidArrowItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 24; 
            Item.DamageType = DamageClass.Ranged;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true; 
            Item.knockBack = 1.5f;
            Item.shoot = ModContent.ProjectileType<VoidArrow>();
            Item.shootSpeed = 16f;
            Item.ammo = AmmoID.Arrow; 
        }
    }

    public class VoidArrow : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
        }

        //Trails
        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public static Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(new Color(60, 0, 118, 125), Color.Transparent, completionRatio);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawSimpleTrail(Projectile, WidthFunction, ColorFunction, TrailRegistry.VortexTrail);
            DrawHelper.DrawAdditiveAfterImage(Projectile, ColorFunctions.MiracleVoid, Color.Black, ref lightColor);
            return base.PreDraw(ref lightColor);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item89, Projectile.position);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<AlcaricMushBoom>(), Projectile.damage, 0f, Projectile.owner);
        }
    }

    public class AlcaricMushBoom : ModProjectile, IDrawMaskShader
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("FrostShotIN");
            Main.projFrames[Projectile.type] = 48;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = false;
            Projectile.width = 128;
            Projectile.height = 128;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 48;
            Projectile.scale = 1f;

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
            Projectile.tileCollide = false;
            if (++Projectile.frameCounter >= 1)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 48)
                {
                    Projectile.frame = 0;
                }
            }
            return true;
        }

        public MiscShaderData GetMaskDrawShader()
        {
            var shaderData = GameShaders.Misc["LunarVeil:SimpleDistortion"];
            shaderData.Shader.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * 15);
            shaderData.Shader.Parameters["distortion"].SetValue(0f);
            shaderData.Shader.Parameters["distortingNoiseTexture"].SetValue(TextureRegistry.CloudNoise2.Value);
            return shaderData;
        }

        public void DrawMask(SpriteBatch spriteBatch)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, Projectile.Frame(), Color.White, Projectile.rotation, Projectile.Frame().Size() / 2f, Projectile.scale, SpriteEffects.None, 0f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}