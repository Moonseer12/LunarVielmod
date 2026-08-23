using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Backgrounds;
using Terraria;

namespace Stellamod.Content.Areas.WaterSide;

public class HarmonicCoralwaysBG : CustomBG
{
    public HologramShader Shader;
    public CustomBGLayer BackLayer;
    public CustomBGLayer MidLayer;
    public CustomBGLayer FrontLayer;
    public float FarParallax => 0.08f;
    public float MidParallax => 0.11f;
    public float CloseParallax => 0.20f;

    private void AddFarLayer()
    {
        BackLayer = new CustomBGLayer();
        BackLayer.SetTexture(AssetRegistry.Textures.BackgroundPath + "HarmonicCoralwaysFar");
        BackLayer.Parallax = FarParallax;
        BackLayer.DrawOffset = Vector2.Zero;
        AddLayer(BackLayer);
    }

    private void AddMidLayer()
    {
        MidLayer = new CustomBGLayer();
        MidLayer.SetTexture(AssetRegistry.Textures.BackgroundPath + "HarmonicCoralwaysMid");
        MidLayer.Parallax = MidParallax;
        MidLayer.DrawOffset = Vector2.Zero;
        AddLayer(MidLayer);
    }

    private void AddCloseLayer()
    {
        FrontLayer = new CustomBGLayer();
        FrontLayer.SetTexture(AssetRegistry.Textures.BackgroundPath + "HarmonicCoralwaysClose");
        FrontLayer.Parallax = CloseParallax;
        FrontLayer.DrawOffset = Vector2.Zero;
        AddLayer(FrontLayer);
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        DrawScale = 1.5f;
        AddFarLayer();
        AddMidLayer();
        AddCloseLayer();
    }

    public override bool IsActive()
    {
        BackLayer.ParallaxOffset = new Vector2(750, 0);
        NoSurfaceLight = true;
        parallaxInBothWays = true;
        NoSurfaceOffset = true;
        DrawScale = 1.2f;
        DrawOffset = new Vector2(0, 100);
        if (Main.LocalPlayer.GetModPlayer<BiomePlayer>().ZoneDeepBelowCoralways)
        {
            DrawColor = Color.Lerp(DrawColor, Color.Lerp(Color.White, Color.Black, 0.85f), 0.1f);
        }
        else
        {
            DrawColor = Color.Lerp(DrawColor, Color.White, 0.1f);
        }
     
        return Main.LocalPlayer.GetModPlayer<BiomePlayer>().ZoneHarmonicCoralways;
    }
}
