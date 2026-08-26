using Stellamod.Content.Areas.Cinderspark.TilesCS;
using Stellamod.Content.Areas.Underground.TilesUG;
using Stellamod.WorldG;
using System;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Stellamod.Content.Areas.Cinderspark;

public class CindersparkPass : GenPass
{
    public CindersparkPass() : base("Cinderspark", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Searing the deepest caverns";
        ushort dirtTile = (ushort)ModContent.TileType<CindersparkDirt>();
        var genRand = WorldGen.genRand;

        ModContent.GetInstance<VeilGen>().CindersparkStart = Main.maxTilesY - 10;
        ModContent.GetInstance<VeilGen>().CindersparkEnd = 0;
        for (int x = 0; x < Main.maxTilesX; x++)
        {
            int yMax = Main.UnderworldLayer - (Main.maxTilesY / 20);
            int yMin = yMax - 150;

            ModContent.GetInstance<VeilGen>().CindersparkStart = Math.Min(ModContent.GetInstance<VeilGen>().CindersparkStart, yMin);
            ModContent.GetInstance<VeilGen>().CindersparkEnd = Math.Max(ModContent.GetInstance<VeilGen>().CindersparkEnd, yMax);


            float ratio = x / (float)Main.maxTilesX;

            float y = yMin;
            y += MathF.Sin(ratio * 64) * 10;
            y += MathF.Sin(ratio * 64) * 4;
            int startY = (int)y;
            int endY = startY;
            // We go down until we hit a solid tile or go under the world's surface
            while (endY <= Main.UnderworldLayer)
            {
                endY++;
            }


            for (int j = startY; j < endY; j++)
            {
                Tile t = Main.tile[x, j];
                t.ClearEverything();
                t.TileType = dirtTile;
                t.HasTile = true;
            }
        }
    }
}

public class CindersparkCavesPass : GenPass
{
    public CindersparkCavesPass() : base("Cinderspark Caves", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Making Cinderspark Caves";
        var genRand = WorldGen.genRand;


        //Here we're going to use the same technique i used in the darkspace
        FastNoiseLite topFNL = new FastNoiseLite();
        topFNL.SetSeed(genRand.Next(0, int.MaxValue));
        topFNL.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        topFNL.SetFrequency(0.15f);
        topFNL.SetDomainWarpAmp(10);
        topFNL.SetDomainWarpType(FastNoiseLite.DomainWarpType.OpenSimplex2);

        FastNoiseLite bottomFNL = new FastNoiseLite();
        bottomFNL.SetSeed(genRand.Next(0, int.MaxValue));
        bottomFNL.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        bottomFNL.SetFrequency(0.15f);
        bottomFNL.SetDomainWarpAmp(10);
        bottomFNL.SetDomainWarpType(FastNoiseLite.DomainWarpType.OpenSimplex2);

        //The cinderspark is defined by long narrow passage ways
        //So just pick a random point, decide to go left and right, and go from there
        //Then sprinkle vertical caves so you can actually move down in the place
        float numCaves = Main.maxTilesX * Main.maxTilesY * 0.000004f;
        for (float f = 0; f < numCaves; f++)
        {
            //Reset the seed for each cave
            topFNL.SetSeed(genRand.Next(0, int.MaxValue));
            bottomFNL.SetSeed(genRand.Next(0, int.MaxValue));

            int sx = genRand.Next(0, Main.maxTilesX);
            int sy = genRand.Next(ModContent.GetInstance<VeilGen>().CindersparkStart, Main.UnderworldLayer);
            int minCaveDistance = genRand.Next(4, 5);
            int maxCaveDistance = genRand.Next(8, 10);
            int steps = genRand.Next(128, 900);
            int dir = genRand.NextBool(2) ? 1 : -1;
            for (int s = 0; s < steps; s++)
            {
                float SampleNoise(int x, int y)
                {
                    return topFNL.GetNoise(x * 0.05f, y * 0.05f) * 0.5f + 0.5f;
                }
                float SampleNoise2(int x, int y)
                {
                    return bottomFNL.GetNoise(x * 0.05f, y * 0.05f) * 0.5f + 0.5f;
                }

                int x = sx + s * dir;
                if (x < 0 || x >= Main.maxTilesX)
                    break;

                float topNoise = SampleNoise(x, sy);
                float bottomNoise = SampleNoise2(x, sy);

                //Cave middle up
                int topDistance = (int)MathHelper.Lerp(minCaveDistance, maxCaveDistance, topNoise) + genRand.Next(-1, 1);
                for (int y = 0; y < topDistance; y++)
                {
                    Tile tile = Main.tile[x, sy - y];
                    tile.ClearEverything();
                }

                //Cave middle down
                int bottomDistance = (int)MathHelper.Lerp(minCaveDistance, maxCaveDistance, bottomNoise) + genRand.Next(-1, 1);
                for (int y = 0; y < bottomDistance; y++)
                {
                    Tile tile = Main.tile[x, sy + y];
                    tile.ClearEverything();
                }
            }
        }


        //Vertical Caves
        for (float f = 0; f < numCaves; f++)
        {
            //Reset the seed for each cave
            topFNL.SetSeed(genRand.Next(0, int.MaxValue));
            bottomFNL.SetSeed(genRand.Next(0, int.MaxValue));

            int sx = genRand.Next(0, Main.maxTilesX);
            int sy = genRand.Next(ModContent.GetInstance<VeilGen>().CindersparkStart, Main.UnderworldLayer);
            Tile startTile = Main.tile[sx, sy];

            //Only place on air, guaranteeing that the cave connects to another cave
            if (startTile.HasTile)
                continue;

            int minCaveDistance = genRand.Next(3, 4);
            int maxCaveDistance = genRand.Next(6, 8);
            int steps = genRand.Next(32, 100);
            for (int s = 0; s < steps; s++)
            {
                float SampleNoise(int x, int y)
                {
                    return topFNL.GetNoise(x * 0.05f, y * 0.05f) * 0.5f + 0.5f;
                }
                float SampleNoise2(int x, int y)
                {
                    return bottomFNL.GetNoise(x * 0.05f, y * 0.05f) * 0.5f + 0.5f;
                }

                int y = sy + s;
                if (y <= 0 || y >= Main.maxTilesY)
                    break;

                float topNoise = SampleNoise(sx, y);
                float bottomNoise = SampleNoise2(sx, y);

                //Cave middle up
                int topDistance = (int)MathHelper.Lerp(minCaveDistance, maxCaveDistance, topNoise) + genRand.Next(-1, 1);
                for (int x = 0; x < topDistance; x++)
                {
                    int newX = sx - x;
                    if (newX <= 0)
                        break;

                    Tile tile = Main.tile[newX, y];
                    tile.ClearEverything();
                }

                //Cave middle down
                int bottomDistance = (int)MathHelper.Lerp(minCaveDistance, maxCaveDistance, bottomNoise) + genRand.Next(-1, 1);
                for (int x = 0; x < bottomDistance; x++)
                {
                    int newX = sx + x;
                    if (newX >= Main.maxTilesX)
                        break;
                    Tile tile = Main.tile[newX, y];
                    tile.ClearEverything();
                }
            }
        }

        //Smoothing will get rid of the lonely tiles
        Rectangle smoothingRect = new(0, ModContent.GetInstance<VeilGen>().CindersparkStart, Main.maxTilesX, Main.UnderworldLayer - ModContent.GetInstance<VeilGen>().CindersparkStart);
        CellularAutomataParams @params = new CellularAutomataParams() with { Steps = 3, RandomFill = 55, BirthLimit = 4, DeathLimit = 4 };
        VeilGen.AutomataSmoothErase(smoothingRect, in @params);
    }
}

public class HardRocksPass : GenPass
{
    public HardRocksPass() : base("Charred Stones", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Hardening Stones";
        var genRand = WorldGen.genRand;
        int start = ModContent.GetInstance<VeilGen>().DarkspaceEnd;
        int end = Main.UnderworldLayer;
        ModContent.GetInstance<VeilGen>().HeatedDepthsStart = start;
        ModContent.GetInstance<VeilGen>().HeatedDepthsEnd = ModContent.GetInstance<VeilGen>().CindersparkStart;
        ushort charredStoneType = (ushort)ModContent.TileType<CharredStone>();
        for (int x = 0; x < Main.maxTilesX; x++)
        {
            for (int y = start; y < end; y++)
            {
                Tile tile = Main.tile[x, y];
                if (tile.TileType == TileID.Stone || tile.TileType == TileID.Dirt)
                    tile.TileType = charredStoneType;
            }
        }

        int charredStoneTypeInt = ModContent.TileType<CharredStone>();
        for (int x = 0; x < Main.maxTilesX; x++)
        {
            if (!genRand.NextBool(8))
                continue;

            int y = start + genRand.Next(-3, 3);
            Tile tile = Main.tile[x, y];
            if (tile.TileType != charredStoneTypeInt)
                continue;

            int steps = genRand.Next(400, 600);
            int maxDist = 8;
            VeilGen.Walker(x, y, steps, charredStoneTypeInt, maxDist);

            //Place at bottom of layer too
            y = end + genRand.Next(-3, 3);
            tile = Main.tile[x, y];
            if (tile.TileType != charredStoneTypeInt)
                continue;
            VeilGen.Walker(x, y, steps, charredStoneTypeInt, maxDist);
        }

        //Turn some of the charred stones to obsidian
        for (int x = 0; x < Main.maxTilesX; x++)
        {
            if (!genRand.NextBool(16))
                continue;

            int y = (int)MathHelper.Lerp(start, end, genRand.NextFloat());
            Tile tile = Main.tile[x, y];
            if (tile.TileType == charredStoneTypeInt)
            {
                int steps = genRand.Next(30, 90);
                int maxDist = 4;
                VeilGen.Walker(x, y, steps, TileID.Obsidian, maxDist);
            }
        }
    }
}

public class SkullrunnerPass : GenPass
{
    public SkullrunnerPass() : base("Skullrunner", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Getting dunked on";
        bool placed = false;
        int attempts = 0;
        while (!placed && attempts++ < 10000000)
        {
            // Select a place in the first 6th of the world, avoiding the oceans
            int smx = ModContent.GetInstance<VeilGen>().ManorLocation.X + WorldGen.genRand.Next(-200, 200);
            smx -= 600;

            int smy = ModContent.GetInstance<VeilGen>().ManorLocation.Y;
            Point Loc = new(smx, smy);

            string path = "Structures/Skullrunner";
            Structurizer.ProtectStructure(Loc, path);
            placed = true;
        }
    }
}

public class ManorPass : GenPass
{
    public ManorPass() : base("Manor", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        /*progress.Message = "Ereshkigal secretly hiding Sigfried";
        bool placed = false;
        int attempts = 0;
        while (!placed && attempts++ < 10000000)
        {
            int smx = WorldGen.genRand.Next((Main.maxTilesX / 2) - 200, (Main.maxTilesX / 2) + 50);
            int smy = Main.UnderworldLayer - 400;
            Tile tile = Main.tile[smx, smy];
            while (!WorldGen.SolidTile(smx, smy) && smy <= Main.UnderworldLayer && (!(tile.TileType == ModContent.TileType<CindersparkDirt>())))
            {
                smy++;
                tile = Main.tile[smx, smy];
            }
            if (smy > Main.UnderworldLayer - 20)
            {
                continue;
            }
            for (int da = 0; da < 1; da++)
            {
                Point Loc = new(smx, smy + 350);
                string path = "Structures/Underground/Manor";
                ManorLocation = Loc;
                StructureLoader.ProtectStructure(Loc, path);
                GenVars.structures.AddProtectedStructure(new Rectangle(smx, smy, 433, 100));
                placed = true;
            }
        }*/
    }
}

public class HardWallsPass : GenPass
{
    public HardWallsPass() : base("Charred Stone Walls", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Hardening Walls";
        var genRand = WorldGen.genRand;
        int start = ModContent.GetInstance<VeilGen>().DarkspaceStart - 700;
        int end = Main.UnderworldLayer;
        int[] wallTypes = [
            WallID.ObsidianBackUnsafe,
            WallID.RocksUnsafe1,
            WallID.Cave4Unsafe,
            WallID.Cave5Unsafe
        ];



        int charredStoneTypeInt = ModContent.TileType<CharredStone>();
        int padding = 2;
        for (int x = padding; x < Main.maxTilesX - padding; x++)
        {
            for (int y = start; y < end; y++)
            {
                Tile tile = Main.tile[x, y];
                if (tile.TileType == charredStoneTypeInt && tile.HasTile && VeilGen.IsTileExposedToAirCardinal(x, y))
                {
                    if (genRand.NextBool(3))
                    {
                        int steps = genRand.Next(30, 90);
                        int maxDist = 3;
                        VeilGen.WallWalker(x, y, steps, wallTypes[genRand.Next(4)], maxDist, PaintID.BlackPaint);
                    }
                }
            }

            progress.Set((x - (float)padding) / (Main.maxTilesX - (float)padding));
        }
    }
}