using Stellamod.Assets;
using Stellamod.Core.Backgrounds;
using Stellamod.Core.Effects;
using Terraria;

namespace Stellamod.Content.Areas.PunkerTown
{
    public class RainforestBG : CustomBG
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DrawScale = 1.5f;
            CustomBGLayer backLayer = new();
            backLayer.SetTexture(AssetRegistry.Textures.BackgroundPath + "RainforestBack");
            backLayer.Parallax = 0.2f;
            backLayer.DrawOffset = Vector2.Zero;
            AddLayer(backLayer);

            CustomBGLayer midLayer = new();
            midLayer.SetTexture(AssetRegistry.Textures.BackgroundPath + "RainforestMiddle");
            midLayer.Parallax = 0.35f;
            midLayer.DrawOffset = Vector2.Zero;
            AddLayer(midLayer);

            CustomBGLayer midFogLayer = new();
            midFogLayer.SetTexture(AssetRegistry.Textures.BackgroundPath + "RainforestMiddleGradient");
            midFogLayer.Parallax = 0.35f;
            midFogLayer.DrawOffset = Vector2.Zero;


            MistShader midMistShader = new();
            midMistShader.StartColor = Color.DarkGray * 0.25f;
            midMistShader.EndColor = Color.Transparent;
            midFogLayer.Shader = midMistShader;
            AddLayer(midFogLayer);

            CustomBGLayer frontLayer = new();
            frontLayer.SetTexture(AssetRegistry.Textures.BackgroundPath + "RainforestFront");
            frontLayer.Parallax = 0.5f;
            frontLayer.DrawOffset = Vector2.Zero;
            AddLayer(frontLayer);

            CustomBGLayer frontFogLayer = new();
            frontFogLayer.SetTexture(AssetRegistry.Textures.BackgroundPath + "RainforestFrontGradient");
            frontFogLayer.Parallax = 0.5f;
            frontFogLayer.DrawOffset = Vector2.Zero;


            MistShader frontMistShader = new();
            frontMistShader.StartColor = Color.DarkGray * 0.5f;
            frontMistShader.EndColor = Color.Transparent;
            frontFogLayer.Shader = frontMistShader;
            AddLayer(frontFogLayer);

        }

        public override bool IsActive()
        {
            LocalParallaxSpeed = 0.3f;
            DrawOffset = new Vector2(0, 450);
            return Main.LocalPlayer.GetModPlayer<BiomePlayer>().ZoneMarsh;
        }
    }
}