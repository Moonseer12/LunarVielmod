using Stellamod.WorldG;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Stellamod.Content.Areas.Tundra.MoonspiralTower;

public class MoonspiralTowerPass : GenPass
{
    public MoonspiralTowerPass() : base("Moonspiral Tower", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Moonspiral Tower";
        Point snowCenter = ModContent.GetInstance<VeilGen>().SnowClumpOriginPoint;

        string structurePath = $"Structures/MoonspiralTower";
        Rectangle structureRect = Structurizer.ReadRectangle(structurePath);
        snowCenter.X -= structureRect.Width / 2;
        snowCenter.Y -= 120;
        snowCenter.Y += 8;
        Structurizer.ReadStruct(snowCenter, structurePath, Structurizer.DefaultTileBlend);
    }
}