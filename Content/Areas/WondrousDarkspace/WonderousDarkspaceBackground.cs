using ReLogic.Content;
using Stellamod.Core.Backgrounds;
using Stellamod.Core.Effects;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WondrousDarkspace;

public class WonderousDarkspaceBackground : CustomBG
{
    private Asset<Texture2D> _backgroundTextureAsset;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        _backgroundTextureAsset = ModContent.Request<Texture2D>(AssetRegistry.Textures.BackgroundPath2 + "Darkspace");
    }

    public override bool UseCustomDrawing()
    {
        return true;
    }
    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        Color fadeToColor = Color.White;
        fadeToColor.A = 0;
        BackgroundHelper.DrawSimpleAtlassedBackground(spriteBatch, BackgroundHelper.AtlassedBackgroundDraw.Default with
        {
            cameraMovement = CameraMovement,
            bg = _backgroundTextureAsset,
            numBackgrounds = 6,
            fadeToColor = fadeToColor,
            alpha = Alpha,
            parallax = new Vector2(0.01f, 0f),
            baseColor = Color.White
        });
    }

    public override bool IsActive()
    {
        return false;
    }
}


public class DarkspaceBG : CustomBG
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        DrawScale = 1f;
        DrawOffset = Vector2.Zero;
        NoSurfaceOffset = true;
        NoSurfaceLight = true;
        CustomBGLayer backLayer = new();
        backLayer.SetTexture(AssetRegistry.Textures.BackgroundPath + "DarkspaceBottom");
        backLayer.Parallax = 0.2f;
        backLayer.DrawOffset = Vector2.Zero;
        AddLayer(backLayer);

        CustomBGLayer midLayer = new();
        midLayer.SetTexture(AssetRegistry.Textures.BackgroundPath + "DarkspaceMid");
        midLayer.Parallax = 0.35f;
        midLayer.DrawOffset = Vector2.Zero;
        AddLayer(midLayer);

        CustomBGLayer midFogLayer = new();
        midFogLayer.SetTexture(AssetRegistry.Textures.BackgroundPath + "DarkspaceMidGradient");
        midFogLayer.Parallax = 0.35f;
        midFogLayer.DrawOffset = Vector2.Zero;

        MistShader midMistShader = new();
        midMistShader.StartColor = Color.Purple * 0.25f;
        midMistShader.EndColor = Color.Transparent;
        midFogLayer.Shader = midMistShader;
        AddLayer(midFogLayer);

        CustomBGLayer frontLayer = new();
        frontLayer.SetTexture(AssetRegistry.Textures.BackgroundPath + "DarkspaceFront");
        frontLayer.Parallax = 0.4f;
        frontLayer.DrawOffset = Vector2.Zero;
        AddLayer(frontLayer);

        CustomBGLayer front2Layer = new();
        front2Layer.SetTexture(AssetRegistry.Textures.BackgroundPath + "DarkspaceFrontGradient");
        front2Layer.Parallax = 0.5f;
        front2Layer.DrawOffset = Vector2.Zero;

        CustomBGLayer frontFogLayer = new();
        frontFogLayer.SetTexture(AssetRegistry.Textures.BackgroundPath + "RainforestFrontGradient");
        frontFogLayer.Parallax = 0.5f;
        frontFogLayer.DrawOffset = Vector2.Zero;

        MistShader frontMistShader = new();
        frontMistShader.StartColor = Color.Pink * 0.5f;
        frontMistShader.EndColor = Color.Transparent;
        frontFogLayer.Shader = frontMistShader;
        AddLayer(frontFogLayer);
    }
    public override int GetParallaxYStartHeight()
    {

        int yMax = Main.UnderworldLayer - (Main.maxTilesY / 6);
        int yMin = yMax - 12;
        int yMid = (yMin + yMax) / 2;
        return yMid * 16;
    }

    public override bool IsActive()
    {
        DrawOffset = new Vector2(0, 64);
        return Main.LocalPlayer.GetModPlayer<BiomePlayer>().ZoneWonder;
    }
}