using Stellamod.Content.Areas.Underground.TilesUG;
using System;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Stellamod.Content.Areas.Underground;

public class TreeCavesPass : GenPass
{
    public TreeCavesPass() : base("Tree Caves", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Tree-like Caves carve deeply...";
        var genRand = WorldGen.genRand;
        //High Tree Caves
        int worldsEndEdge = 3300;
        int maxX = (int)(Main.maxTilesX * 0.8f);
        for (int x = worldsEndEdge; x < maxX; x++)
        {
            int caveMakerSteps = 32;
            for (int j = 0; j < caveMakerSteps; j++)
            {
                int y = genRand.Next((int)GenVars.worldSurfaceLow - 25, (int)GenVars.rockLayerHigh);
                Tile tile = Main.tile[x, y];
                if (tile.TileType == TileID.Sand ||
                    tile.TileType == TileID.Mud ||
                    tile.TileType == TileID.SnowBlock ||
                    tile.TileType == TileID.IceBlock)
                    continue;
                if (!genRand.NextBool(1512))
                    continue;
                int caveWidth = genRand.Next(4, 7);
                int caveSteps = genRand.Next(50, 80);

                //Cave position in tiles
                Vector2 cavePosition = new(x, y);

                //Starting cave direction
                Vector2 baseCaveDirection = Vector2.UnitY;//.RotatedBy(WorldGen.genRand.NextFloatDirection() * 0.54f);

                //How much the tile runner is gonna carve out
                Vector2 caveStrength = new(12, 14);

                //Chance to open up
                int splitDenominator = 4;
                GenerateTreeCaves(cavePosition, baseCaveDirection, caveStrength, caveWidth, caveSteps,
                    splitDenominator);
            }
        }
    }

    public static void GenerateTreeCaves(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength,
        int caveWidth,
        int caveSteps,
        int splitDenominator)
    {
        var genRand = WorldGen.genRand;

        //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
        float i = cavePosition.X;
        Vector2 caveVelocity = baseCaveDirection;
        Vector2 pullDirection = genRand.NextVector2Circular(1, 1);
        Vector2 targetPosition = caveVelocity + pullDirection;
        float sharpness = 1;
        int counter = 1;
        for (int j = 0; j < caveSteps; j++)
        {
            //Homing
            float degreesToRotate = sharpness;
            float length = caveVelocity.Length();
            float targetAngle = (targetPosition - caveVelocity).ToRotation();
            Vector2 newVelocity = caveVelocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(degreesToRotate)).ToRotationVector2() * length;
            caveVelocity = newVelocity;


            if (genRand.NextBool(3))
            {
                targetPosition = targetPosition.RotatedByRandom(MathHelper.ToRadians(30));
            }

            if (genRand.NextBool(splitDenominator) && j > 4)
            {
                int clearingCaveWidth = caveWidth / 2;
                int clearingCaveSteps = caveSteps;

                //Cave position in tiles
                Vector2 clearingPosition = new((int)cavePosition.X, (int)cavePosition.Y);

                //Starting cave direction
                float dir = counter % 2 == 0 ? 1 : -1;
                counter++;
                Vector2 clearingCaveDirection = baseCaveDirection.RotatedBy(dir * MathHelper.PiOver2);

                //How much the tile runner is gonna carve out
                Vector2 clearingCaveStrength = caveStrength * 0.5f;

                GenerateTreeCaves(clearingPosition,
                    clearingCaveDirection,
                    clearingCaveStrength,
                    clearingCaveWidth,
                    clearingCaveSteps,
                    splitDenominator * 640);
            }

            if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
            {
                /*
                //digging 
                ShapeData shapeData = new ShapeData();
                Point point = new Point((int)cavePosition.X, (int)cavePosition.Y);
                WorldUtils.Gen(point, new Shapes.Circle(3, 3), new Actions.ClearTile());
                */

                WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                    genRand.NextFloat(caveStrength.X, caveStrength.Y),
                    genRand.Next(4, 5), -1);
            }

            // Update the cave position.
            cavePosition += caveVelocity * caveWidth * 0.5f;
            //  caveStrength *= 0.99f;
        }
    }
}

public class RavineCavesPass : GenPass
{
    public RavineCavesPass() : base("Ravine Caves", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Giant Ravines";

        int walkerWidth = 64;
        int walkerSteps = 4000;
        var genRand = WorldGen.genRand;
        void Carve(int x, int y)
        {
            Point walkerPoint = new(x, y);
            Point originalPoint = walkerPoint;
            for (int s = 0; s < walkerSteps; s++)
            {
                switch (genRand.Next(4))
                {
                    case 0:
                        walkerPoint.X--;
                        break;
                    case 1:
                        walkerPoint.X++;
                        break;
                    case 2:
                        walkerPoint.Y++;
                        break;
                    case 3:
                        walkerPoint.Y--;
                        break;
                }
                walkerPoint = TileUtilities.Clamp(walkerPoint);
                Tile tile = Main.tile[walkerPoint];
                tile.ClearTile();

                //Reset if walking too far
                int dx = Math.Abs(walkerPoint.X - originalPoint.X);
                int dy = Math.Abs(walkerPoint.Y - originalPoint.Y);
                if (dx > walkerWidth || dy > walkerWidth)
                {
                    walkerPoint = originalPoint;
                }
            }
        }

        float numRavines = 5;
        for (float ravines = 0; ravines < numRavines; ravines++)
        {
            for (int s = 0; s < Main.maxTilesX; s += 4)
            {
                float p = ravines / numRavines;
                int x = s;
                int y = (int)MathHelper.Lerp(ModContent.GetInstance<VeilGen>().HeatedDepthsEnd, ModContent.GetInstance<VeilGen>().HeatedDepthsStart, p);
                walkerWidth = (int)MathHelper.Lerp(16, 3, p);
                Carve(x, y);
                //Random chance to skip several steps, which will create gaps in the caves
                if (genRand.NextBool(128))
                {
                    s += 144;
                }
            }
        }



        //Vertical Caves

        //Here we're going to use the same technique i used in the darkspace
        FastNoiseLite topFNL = new();
        topFNL.SetSeed(genRand.Next(0, int.MaxValue));
        topFNL.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        topFNL.SetFrequency(0.15f);
        topFNL.SetDomainWarpAmp(10);
        topFNL.SetDomainWarpType(FastNoiseLite.DomainWarpType.OpenSimplex2);

        FastNoiseLite bottomFNL = new();
        bottomFNL.SetSeed(genRand.Next(0, int.MaxValue));
        bottomFNL.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        bottomFNL.SetFrequency(0.15f);
        bottomFNL.SetDomainWarpAmp(10);
        bottomFNL.SetDomainWarpType(FastNoiseLite.DomainWarpType.OpenSimplex2);

        float numCaves = Main.maxTilesX * (float)Main.maxTilesY * 0.00012f;
        for (float f = 0; f < numCaves; f++)
        {
            //Reset the seed for each cave
            topFNL.SetSeed(genRand.Next(0, int.MaxValue));
            bottomFNL.SetSeed(genRand.Next(0, int.MaxValue));

            int sx = genRand.Next(0, Main.maxTilesX);
            int sy = genRand.Next(ModContent.GetInstance<VeilGen>().HeatedDepthsStart, ModContent.GetInstance<VeilGen>().HeatedDepthsEnd);
            Tile startTile = Main.tile[sx, sy];

            //Only place on air, guaranteeing that the cave connects to another cave
            if (startTile.HasTile)
                continue;

            int minCaveDistance = genRand.Next(3, 4);
            int maxCaveDistance = genRand.Next(6, 8);
            int steps = genRand.Next(72, 154);
            int dir = genRand.NextBool(2) ? -1 : 1;
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

                int y = sy + s * dir;
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


        //Place Lava Bowls
        float numLavaBowls = numCaves * 2;
        int padding = 30;
        for (float f = 0; f < numLavaBowls; f++)
        {
            //Reset the seed for each cave
            int sx = genRand.Next(padding, Main.maxTilesX - padding);
            int sy = genRand.Next(ModContent.GetInstance<VeilGen>().HeatedDepthsStart, ModContent.GetInstance<VeilGen>().HeatedDepthsEnd);
            Tile startTile = Main.tile[sx, sy];

            //Only place on air, guaranteeing that the lava is inside of a cave/exposed to air
            if (startTile.HasTile)
                continue;

            //Gotta land on a solid tile
            while (!startTile.HasTile && sy < Main.UnderworldLayer)
            {
                sy++;
                startTile = Main.tile[sx, sy];
            }

            //Dimensions of the lava bowl
            int width = genRand.Next(5, 12);
            int depth = genRand.Next(5, 12);
            int left = sx - width / 2;
            int right = sx + width / 2;
            for (int x = left; x < right; x++)
            {
                float numSteps = right - left;
                int d = (int)MathHelper.Lerp(0, depth, EasingFunction.QuadraticBump((x - left) / numSteps));
                for (int y = sy; y < sy + d; y++)
                {
                    Tile tile = Main.tile[x, y];
                    tile.ClearTile();
                    tile.LiquidAmount = 255;
                    tile.LiquidType = LiquidID.Lava;
                }
            }
        }


        CellularAutomataParams @params = new CellularAutomataParams() with { Steps = 2, RandomFill = 55, BirthLimit = 4, DeathLimit = 4 };
        Rectangle smoothRectangle = new(0, ModContent.GetInstance<VeilGen>().HeatedDepthsStart, Main.maxTilesX, ModContent.GetInstance<VeilGen>().HeatedDepthsEnd - ModContent.GetInstance<VeilGen>().HeatedDepthsStart);
        VeilGen.AutomataSmoothErase(smoothRectangle, in @params);
    }
}

public class DeepCavesPass : GenPass
{
    public DeepCavesPass() : base("Deep Caves", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Caves cut deep...";
        //Bottom Ravines


        //First we should generate corridors starting from the top of the stone layer all the way to darkspace
        //Actually they just cut through the whole world, ignoring ice and jungle / desert
        var genRand = WorldGen.genRand;
        float maxCaveCount = Main.maxTilesX * Main.maxTilesY * 0.000005f;
        float maxAttemptCount = maxCaveCount * 10;
        float placedCaves = 0;
        int padding = 1000;

        FastNoiseLite fnl = new();
        fnl.SetSeed(genRand.Next(0, int.MaxValue));
        fnl.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        fnl.SetFrequency(0.15f);
        fnl.SetDomainWarpAmp(10);
        fnl.SetDomainWarpType(FastNoiseLite.DomainWarpType.OpenSimplex2);
        for (int i = 0; i < maxAttemptCount; i++)
        {
            int x = genRand.Next(padding + 1700, Main.maxTilesX - padding);
            int y = genRand.Next((int)GenVars.rockLayerHigh, ModContent.GetInstance<VeilGen>().DarkspaceStart);
            if (VeilGen.IsTileNearby(x, y, distance: 50, TileSets.BlockMineshafts))
                continue;

            Tile tile = Main.tile[x, y];
            if (Main.tileSolid[tile.TileType] && tile.HasTile && TileID.Sets.Stone[tile.TileType])
            {
                fnl.SetSeed(genRand.Next(0, int.MaxValue));
                Vector2 initialDirection = Vector2.UnitY.RotateRandom(MathHelper.Pi);
                int caveSteps = 800;
                int walkerSteps = genRand.Next(200, 400);
                int walkerWidth = (int)MathHelper.Lerp(2, 5, (float)(y - (float)GenVars.rockLayerHigh) / (ModContent.GetInstance<VeilGen>().DarkspaceStart - (float)GenVars.rockLayerHigh));
                VeilGen.PlaceDeepCuttingCave(new Point(x, y).ToWorldCoordinates(), initialDirection, caveSteps, walkerSteps, walkerWidth, genRand, fnl);
                placedCaves++;
                if (placedCaves >= maxCaveCount)
                    break;
            }
        }
    }
}

public class ExtraCavesPass : GenPass
{
    public ExtraCavesPass() : base("Extra Caves", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Simple Caves";
        var genRand = WorldGen.genRand;
        float maxCaveCount = Main.maxTilesX * Main.maxTilesY * 0.00001f;
        float maxAttemptCount = maxCaveCount * 10;
        float placedCaves = 0;
        int padding = 2000;
        for (int i = 0; i < maxAttemptCount; i++)
        {
            int x = genRand.Next(padding, Main.maxTilesX - padding);
            int y = genRand.Next((int)GenVars.rockLayerHigh, ModContent.GetInstance<VeilGen>().DarkspaceStart);
            if (VeilGen.IsTileNearby(x, y, distance: 50, TileSets.BlockMineshafts))
                continue;

            Tile tile = Main.tile[x, y];
            if (Main.tileSolid[tile.TileType] && tile.HasTile && TileID.Sets.Stone[tile.TileType])
            {
                WorldGen.Caverer(x, y);
                placedCaves++;
                if (placedCaves >= maxCaveCount)
                    break;
            }
        }
    }
}

public class CavernWatersPass : GenPass
{
    public CavernWatersPass() : base("Cavern Waters", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Cave Waters...";
        var genRand = WorldGen.genRand;
        float maxCount = Main.maxTilesX * Main.maxTilesY * 0.0000003f;
        float maxAttemptCount = maxCount * 10;
        float placed = 0;
        int padding = 250;

        for (int i = 0; i < maxAttemptCount; i++)
        {
            int x = genRand.Next(padding, Main.maxTilesX - padding);
            int y = genRand.Next((int)GenVars.rockLayerHigh, ModContent.GetInstance<VeilGen>().DarkspaceStart);
            if (VeilGen.IsTileNearby(x, y, distance: 50, TileSets.BlockMineshafts))
                continue;

            Tile startTile = Main.tile[x, y];
            if (!startTile.HasTile)
            {
                int waterBlotchSize = genRand.Next(12, 20);
                Rectangle placementRect = new(x - waterBlotchSize, y - waterBlotchSize, waterBlotchSize * 2, waterBlotchSize * 2);
                placementRect = TileUtilities.Clamp(placementRect);
                for (int tx = placementRect.Left; tx < placementRect.Right; tx++)
                {
                    for (int ty = placementRect.Top; ty < placementRect.Bottom; ty++)
                    {
                        Tile tile = Main.tile[tx, ty];
                        tile.LiquidType = LiquidID.Water;
                        tile.LiquidAmount = 255;
                    }
                }

                placed++;
                if (placed >= maxCount)
                    break;
            }
        }
    }
}

public class DarkstonePass : GenPass
{
    public DarkstonePass() : base("Darkstone", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Blackening Stones for racist effect";
        var genRand = WorldGen.genRand;
        float maxCaveCount = Main.maxTilesX * Main.maxTilesY * 0.00008f;
        for (int k = 0; k < maxCaveCount; k++)
        {
            int x = genRand.Next(0, Main.maxTilesX);
            int y = genRand.Next((int)GenVars.rockLayerHigh, ModContent.GetInstance<VeilGen>().DarkspaceStart);
            if (!TileID.Sets.Stone[Main.tile[x, y].TileType])
                continue;

            VeilGen.Walker(x, y, WorldGen.genRand.Next(128, 256), ModContent.TileType<DiminishedStone>(), 24);
        }
    }
}

public class GrassPass : GenPass
{
    public GrassPass() : base("Grass", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Grassing Caves";
        var genRand = WorldGen.genRand;
        int fluff = 10;
        int startFloweringY = (int)(Main.worldSurface - 25);
        int startGrassingY = startFloweringY - 600;
        for (int x = fluff; x < Main.maxTilesX - fluff; x++)
        {
            for (int y = startGrassingY; y < (int)Main.worldSurface + 600; y++)
            {
                Tile tile = Main.tile[x, y];
                if (!tile.HasTile)
                    continue;
                if (!VeilGen.IsTileExposedToAirCardinal(x, y))
                    continue;

                if (tile.TileType == TileID.Dirt || tile.TileType == TileID.Stone || tile.TileType == TileID.Grass)
                {
                    tile.TileType = TileID.Grass;
                    VeilGen.WallWalker(x, y, genRand.Next(2, 6) * 3, WallID.FlowerUnsafe, 3);
                }
            }
        }
    }
}