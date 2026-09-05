using Stellamod.Content.Areas.Underground.TilesUG;
using Terraria;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Stellamod.Content.Areas.Underground;

public class OresPass : GenPass
{
    public OresPass() : base("Ores", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "World Glistens with shines of the Glistening Moon";
        for (int k = 0; k < (int)(Main.maxTilesX * Main.maxTilesY * 6E-05); k++)
        {
            int x = WorldGen.genRand.Next(0, Main.maxTilesX);
            int y = WorldGen.genRand.Next((int)GenVars.rockLayerHigh, ModContent.GetInstance<VeilGen>().DarkspaceStart);
            Tile tile = Main.tile[x, y];
            if (!tile.HasTile)
                continue;
            VeilGen.QuickOrePatch(x, y, ModContent.TileType<GlisteningOreTile>());
        }
        int count = (int)(Main.maxTilesX * Main.maxTilesY * 6E-05);
        for (int k = 0; k < count; k++)
        {
            int x = WorldGen.genRand.Next(0, Main.maxTilesX);
            int y = WorldGen.genRand.Next(ModContent.GetInstance<VeilGen>().HeatedDepthsStart, ModContent.GetInstance<VeilGen>().HeatedDepthsEnd);
            Tile tile = Main.tile[x, y];
            if (!tile.HasTile)
                continue;

            VeilGen.QuickOrePatch(x, y, ModContent.TileType<DragonpieceOre>());
            progress.Set(k / (float)count);
        }
    }
}