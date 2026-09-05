using Stellamod.Content.Areas.WorldsEnd.TilesWE;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Stellamod.Content.Areas.WorldsEnd;

public class WorldsEndPass : GenPass
{
    public WorldsEndPass() : base("Worlds End", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Ending the World";

        int startTileX = 0;
        int endTileX = ModContent.GetInstance<VeilGen>().RoyalCapitalLocation.X;
        int maxDepth = 125;
        int minDepth = 25;
        int grass = ModContent.TileType<WhiteGrass>();

        TileID.Sets.CanBeClearedDuringGeneration[grass] = false;
        TileID.Sets.CanBeClearedDuringOreRunner[grass] = false;

        Rectangle treeRect = new(0, ModContent.GetInstance<VeilGen>().RoyalCapitalLocation.Y - 32, ModContent.GetInstance<VeilGen>().RoyalCapitalLocation.X, 500);
        VeilGen.ClearTrees(treeRect);

        //Create a base for all the grass
        for (int tileX = startTileX; tileX < endTileX; tileX++)
        {
            int startY = (int)Main.worldSurface - 100;
            while (!WorldGen.SolidTile(tileX, startY))
                startY++;

            float width = endTileX - startTileX;
            float ratio = (tileX - startTileX) / width;
            int depth = (int)MathHelper.SmoothStep(maxDepth, minDepth, ratio);
            for (int tileY = startY; tileY < startY + depth; tileY++)
            {
                WorldGen.TileRunner(tileX, tileY, 2, 4, grass);
            }
        }


        Point startSlope = ModContent.GetInstance<VeilGen>().RoyalCapitalLocation;
        startSlope.X -= 250;

        int startSlopeY = startSlope.Y;

        for (int tileX = startSlope.X; tileX < endTileX; tileX++)
        {
            float ratio = (tileX - startSlope.X) / (float)(endTileX - startSlope.X);
            float y = MathHelper.SmoothStep(0f, 27, ratio);
            int tileY = (int)(startSlopeY - y);
            for (int innerY = tileY; innerY < startSlopeY; innerY++)
            {
                Tile tile = Main.tile[tileX, innerY];
                tile.ClearTile();
                tile.TileType = (ushort)grass;
                tile.TileFrameX = -1;
                tile.TileFrameY = -1;
                //  WorldGen.PlaceTile(tileX, innerY, grass, forced: true);
            }
        }

        //Generate water bowl
        int maxLakeDepth = 65;
        Point waterStart = new();
        waterStart.X = 4;
        waterStart.Y = (int)Main.worldSurface - 100;
        while (!WorldGen.SolidTile(waterStart))
            waterStart.Y++;

        Point waterEnd = new();
        waterEnd.X = waterStart.X + 300;
        waterEnd.Y = (int)Main.worldSurface - 100;
        while (!WorldGen.SolidTile(waterEnd))
            waterEnd.Y++;
        for (int lakeX = waterStart.X; lakeX < waterEnd.X; lakeX++)
        {
            float ratio = (lakeX - waterStart.X) / (float)(waterEnd.X - waterStart.X);
            float bump = EasingFunction.QuadraticBump(ratio);
            int depth = (int)MathHelper.Lerp(0, maxLakeDepth, bump);

            int startY = (int)Main.worldSurface - 100;
            while (!WorldGen.SolidTile(lakeX, startY))
                startY++;
            int endY = startY + depth;
            int d = 0;
            for (int lakeY = startY; lakeY < endY; lakeY++)
            {
                Tile tile = Main.tile[lakeX, lakeY];
                tile.ClearEverything();
                d++;
                if (d > 10)
                {

                    WorldGen.PlaceLiquid(lakeX, lakeY, (byte)LiquidID.Water, byte.MaxValue);
                }

            }
        }

        VeilGen.ClearLonelyTiles(treeRect);
    }
}