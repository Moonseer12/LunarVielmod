using Stellamod.Assets;
using Stellamod.Core.Backgrounds;
using Stellamod.Core.Effects;
using Terraria;

namespace Stellamod.Content.Areas.Dungeon;

public class MistyDungeonBG : CustomBG
{
    public override void SetDrawDefaults()
    {
        base.SetDrawDefaults();
        DrawScale = 1.5f;
        DrawOffset = new Vector2(0, 0);
        DrawColor = Color.Lerp(Color.White, Color.Black, 0.5f);
        NoSurfaceLight = true;
        NoSurfaceOffset = true;
        NoParallaxY = true;
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Layers.Clear();
        CustomBGLayer backLayer = new();
        backLayer.SetTexture(AssetRegistry.Textures.BackgroundPath + "MistyDungeon_Back");
        backLayer.Parallax = 0.2f;
        backLayer.DrawOffset = Vector2.Zero;
        AddLayer(backLayer);

        //Guh
        CustomBGLayer midLayer = new();
        midLayer.SetTexture(AssetRegistry.Textures.BackgroundPath + "MistyDungeon_Mid");
        midLayer.Parallax = 0.35f;
        midLayer.DrawOffset = Vector2.Zero;
        AddLayer(midLayer);


        CustomBGLayer midFogLayer = new();
        midFogLayer.SetTexture(AssetRegistry.Textures.BackgroundPath + "RainforestMiddleGradient");
        midFogLayer.Parallax = 0.35f;
        midFogLayer.DrawOffset = Vector2.Zero;


        
        CustomBGLayer midFogLayer2 = new();
        midFogLayer2.SetTexture(AssetRegistry.Textures.BackgroundPath + "RainforestMiddleGradient");
        midFogLayer2.Parallax = 0.35f;
        midFogLayer2.DrawOffset = Vector2.Zero;

        MistShader midMistShader2 = new();
        midMistShader2.StartColor = Color.Transparent;
        midMistShader2.EndColor = Color.Blue * 0.5f;
        midFogLayer2.Shader = midMistShader2;
        AddLayer(midFogLayer2);
        


        CustomBGLayer frontLayer = new();
        frontLayer.SetTexture(AssetRegistry.Textures.BackgroundPath + "MistyDungeon_Top");
        frontLayer.Parallax = 0.4f;
        frontLayer.DrawOffset = Vector2.Zero;
        AddLayer(frontLayer);

        CustomBGLayer front2Layer = new();
        front2Layer.SetTexture(AssetRegistry.Textures.BackgroundPath + "MistyDungeon_TopTop");
        front2Layer.Parallax = 0.5f;
        front2Layer.DrawOffset = Vector2.Zero;
        AddLayer(front2Layer);
    }
    public override int GetParallaxYStartHeight()
    {
        return base.GetParallaxYStartHeight();
    }

    public override bool IsActive()
    {
        LocalParallaxSpeed = 0.1f;
        return Main.LocalPlayer.GetModPlayer<BiomePlayer>().ZoneMistyDungeon;
    }
}
