using Stellamod.Content.Areas;
using Stellamod.Core.Palettes;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.Rendering;

public class AbyssPixelWaterStyle : PixelWaterStyle
{
    public override bool IsActive(Player player)
    {

        return player.GetModPlayer<BiomePlayer>().ZoneAbyss;
    }
    public override void ModifyPixelWater(ref PixelWater pixelWater)
    {
        base.ModifyPixelWater(ref pixelWater);
        pixelWater.StartGradientColor = Color.White;
        pixelWater.EndGradientColor = Color.White;
        pixelWater.BackgroundColor = Color.Cyan;
        pixelWater.CausticsColor = Color.White;
        pixelWater.NoiseTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/WaterCaustics");
        pixelWater.CausticsTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/WaterCaustics");
        pixelWater.TilingMultiplier = Vector2.One;
        pixelWater.Palette = PaletteAssets.FromPaletteFile(PaletteAssets.ABYSSWATER).Value;
        pixelWater.vibrant = true;
        pixelWater.ignoreSkyColor = true;
        pixelWater.noLighting = true;
    }
}