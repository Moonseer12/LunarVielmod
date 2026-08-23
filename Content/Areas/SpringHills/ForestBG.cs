using Stellamod.Assets;
using Stellamod.Core.Backgrounds;
using Terraria;

namespace Stellamod.Content.Areas.SpringHills;

public class ForestBG : CustomBG
{
    public CustomBGLayer BleedLayer;
    public CustomBGLayer BackLayer;
    public CustomBGLayer MidLayer;
    public CustomBGLayer FrontLayer;

    public float FarParallax => 0.08f;
    public float MidParallax => 0.11f;
    public float CloseParallax => 0.13f;

    private void AddFarLayer()
    {
        BackLayer = new CustomBGLayer();
        BackLayer.SetTexture(AssetRegistry.Textures.BackgroundPath + "ForestFar");
        BackLayer.Parallax = FarParallax;
        BackLayer.DrawOffset = Vector2.Zero;
        AddLayer(BackLayer);
    }

    private void AddMidLayer()
    {
        MidLayer = new CustomBGLayer();
        MidLayer.SetTexture(AssetRegistry.Textures.BackgroundPath + "ForestMid");
        MidLayer.Parallax = MidParallax;
        MidLayer.DrawOffset = Vector2.Zero;
        AddLayer(MidLayer);
    }

    private void AddCloseLayer()
    {
        FrontLayer = new CustomBGLayer();
        FrontLayer.SetTexture(AssetRegistry.Textures.BackgroundPath + "ForestFront");
        FrontLayer.Parallax = CloseParallax;
        FrontLayer.DrawOffset = Vector2.Zero;
        AddLayer(FrontLayer);
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        DrawScale = 1f;
        AddFarLayer();
        AddMidLayer();
        AddCloseLayer();
        BleedLayer = new CustomBGLayer();
        BleedLayer.SetTexture(AssetRegistry.Textures.BackgroundPath + "ForestUnderground");
        BleedLayer.Parallax = CloseParallax;
        BleedLayer.DrawOffset = new Vector2(0, BleedLayer.Texture.Size().Y * DrawScale * 2);
        AddLayer(BleedLayer);
    }

    public override bool IsActive()
    {
        FrontLayer.Parallax = CloseParallax;
        DrawScale = 1f;
        DrawOffset = new Vector2(0, 400);
        ParallaxYFactor = 0.35f;
        BiomePlayer biomePlayer = Main.LocalPlayer.GetModPlayer<BiomePlayer>();
        bool isActive = biomePlayer.ZoneForest || biomePlayer.ZoneSpringHills || biomePlayer.ZoneEveroseVillage || Main.LocalPlayer.GetModPlayer<BiomePlayer>().ZoneVillage;
        return isActive;
    }
}
