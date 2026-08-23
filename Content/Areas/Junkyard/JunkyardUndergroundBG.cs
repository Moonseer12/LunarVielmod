using Stellamod.Assets;
using Stellamod.Core.Backgrounds;
using Terraria;

namespace Stellamod.Content.Areas.Junkyard;

public class JunkyardUndergroundBG : CustomBG
{
    public CustomBGLayer BackLayer;
    public CustomBGLayer MidLayer;
    public CustomBGLayer FrontLayer;
    public float FarParallax => 0.08f;
    public float MidParallax => 0.11f;
    public float CloseParallax => 0.20f;

    private void AddFarLayer()
    {
        BackLayer = new CustomBGLayer();
        BackLayer.SetTexture(AssetRegistry.Textures.BackgroundPath + "JunkyardUnderground_Far");
        BackLayer.Parallax = FarParallax;
        BackLayer.DrawOffset = Vector2.Zero;
        AddLayer(BackLayer);
    }

    private void AddMidLayer()
    {
        MidLayer = new CustomBGLayer();
        MidLayer.SetTexture(AssetRegistry.Textures.BackgroundPath + "JunkyardUnderground_Mid");
        MidLayer.Parallax = MidParallax;
        MidLayer.DrawOffset = Vector2.Zero;
        AddLayer(MidLayer);
    }

    private void AddCloseLayer()
    {
        FrontLayer = new CustomBGLayer();
        FrontLayer.SetTexture(AssetRegistry.Textures.BackgroundPath + "JunkyardUnderground_Close");
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
        //AddHoloLayer();
    }

    public override bool IsActive()
    {
        BackLayer.ParallaxOffset = new Vector2(750, 0);
        NoSurfaceLight = true;
        parallaxInBothWays = true;
        NoSurfaceOffset = true;
        DrawScale = 1f;
        DrawOffset = new Vector2(0, 100);
        DrawColor = Color.Lerp(Color.White, Color.Black, 0.85f);
        return Main.LocalPlayer.GetModPlayer<BiomePlayer>().ZoneJunkyard;
    }
}