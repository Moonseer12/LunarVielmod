using ReLogic.Content;
using Stellamod.Core.Backgrounds;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss;

public class AbyssBackground : CustomBG
{
    private Asset<Texture2D> _backgroundTextureAsset;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        _backgroundTextureAsset = ModContent.Request<Texture2D>(AssetRegistry.Textures.BackgroundPath2 + "Abyss");
    }


    public override bool UseCustomDrawing()
    {
        return true;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        float xMovement = CameraMovement.X;


        //Calculate center of the abyss
        Point AbyssCenter = new Point();
        AbyssCenter.Y = (int)(SavedGenerationParameters.RockLayerHigh + Main.maxTilesY * 0.15);
        AbyssCenter.Y -= 20;
        int abyssHigh = AbyssCenter.Y - 500;

        float abyssHighWorld = abyssHigh * 16;
        float yMovement = Main.Camera.Center.Y - abyssHighWorld;
        BackgroundHelper.DrawSimpleAtlassedBackground(spriteBatch, BackgroundHelper.AtlassedBackgroundDraw.Default with
        {
            fadeToColor = Color.Transparent,
            numBackgrounds = 3,
            parallax = new Vector2(0.003f, 0.003f),
            bg = _backgroundTextureAsset,
            cameraMovement = new Vector2(xMovement, yMovement),
            alpha = Alpha,
            parallaxOffset = new Vector2(0, -0.15f)
        });
    }

    public override bool IsActive()
    {
        return Main.LocalPlayer.GetModPlayer<BiomePlayer>().ZoneAbyss;
    }
}
