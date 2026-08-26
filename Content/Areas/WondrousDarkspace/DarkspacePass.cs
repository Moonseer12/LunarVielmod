using Stellamod.Content.Areas.WondrousDarkspace.TilesWD;
using Stellamod.WorldG;
using System;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Stellamod.Content.Areas.WondrousDarkspace;

public class ShimmerSpotPass : GenPass
{
    public ShimmerSpotPass() : base("Shimmer Spot", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        //If we don't do this we'll get a generation error
        progress.Message = "Faking the Shimmer";
        GenVars.shimmerPosition = new ReLogic.Utilities.Vector2D(Main.maxTilesX * 0.5f, Main.maxTilesY * 0.5f);
    }
}

public class DarkspacePass : GenPass
{
    public DarkspacePass() : base("Darkspace", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Creating a Dark Place.";

        var genRand = WorldGen.genRand;
        int yMax = ModContent.GetInstance<VeilGen>().CindersparkStart - 600;
        if (ModContent.GetInstance<VeilGen>().CindersparkStart == 0)
        {
            throw new ArgumentException("The Cinderspark is at the top of the world for some reason.");
        }

        int yMin = yMax - 250;
        int yMid = (yMin + yMax) / 2;

        ModContent.GetInstance<VeilGen>().DarkspaceStart = yMin;
        ModContent.GetInstance<VeilGen>().DarkspaceEnd = yMax;
        //Create a wavey blotch of granite
        //Instead of using GenActions or PlaceTile we can just set the tile directly, fastest way to do it.
        for (int x = 0; x < Main.maxTilesX; x++)
        {
            int dyMin = yMin + (int)MathF.Sin(x) * 8 + genRand.Next(-2, 2);
            int dyMax = yMax + (int)MathF.Sin(x * 0.05f) * 8 + genRand.Next(-2, 2);
            for (int y = dyMin; y < dyMax; y++)
            {
                Tile tile = Main.tile[x, y];
                tile.ClearTile();
                tile.HasTile = true;
                tile.TileFrameX = -1;
                tile.TileFrameY = -1;
                tile.TileType = TileID.Granite;
            }
        }
        progress.Set(0.33D);

        //Here's the algorithm we're going to try
        //We'll initialize a fast noise lite
        //We'll sample two points, each far from each other
        //then slowly move right and using the noise we create the variation in the caves
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

        int minCaveDistance = 35;
        int maxCaveDistance = 72;
        (int, int)[] heights = new (int, int)[Main.maxTilesX];
        for (int x = 0; x < Main.maxTilesX; x++)
        {
            float SampleNoise(int x, int y)
            {
                return topFNL.GetNoise(x * 0.05f, y * 0.05f) * 0.5f + 0.5f;
            }
            float SampleNoise2(int x, int y)
            {
                return bottomFNL.GetNoise(x * 0.05f, y * 0.05f) * 0.5f + 0.5f;
            }
            float topNoise = SampleNoise(x, yMid);
            float bottomNoise = SampleNoise2(x, yMid);

            //Cave middle up
            int topDistance = (int)MathHelper.Lerp(minCaveDistance, maxCaveDistance, topNoise) + genRand.Next(-1, 1);
            for (int y = 0; y < topDistance; y++)
            {
                Tile tile = Main.tile[x, yMid - y];
                tile.ClearEverything();
            }

            //Cave middle down
            int bottomDistance = (int)MathHelper.Lerp(minCaveDistance, maxCaveDistance, bottomNoise) + genRand.Next(-1, 1);
            for (int y = 0; y < bottomDistance; y++)
            {
                Tile tile = Main.tile[x, yMid + y];
                tile.ClearEverything();
            }
            heights[x] = (topDistance, bottomDistance);
        }

        //Walker algorithm over the entire cave to place granite blotches and what not
        for (int x = 0; x < heights.Length; x++)
        {
            if (!genRand.NextBool(4))
                continue;
            (int, int) height = heights[x];
            int heightToUse = genRand.NextBool(2) ? -height.Item1 : height.Item2;
            VeilGen.Walker(x, yMid + heightToUse, genRand.Next(32, 128), TileID.Granite, 10);
        }

        for (int x = 0; x < Main.maxTilesX; x++)
        {
            if (!genRand.NextBool(4))
                continue;
            VeilGen.Walker(x, ModContent.GetInstance<VeilGen>().DarkspaceStart, genRand.Next(64, 128), TileID.Granite, 15);
            VeilGen.Walker(x, ModContent.GetInstance<VeilGen>().DarkspaceEnd, genRand.Next(64, 128), TileID.Granite, 15);
        }
        //Then we go back through the cave, and create blotches of shimmer water in random spots
        //Again, not going to use gen actions here
        //Just going to create squares of shimmer water since it gets settled in a later pass
        int shimmerBlotchCount = 0;
        for (int x = 0; x < Main.maxTilesX; x++)
        {
            //1 in X chance per tile to generate shimmer pool
            if (!genRand.NextBool(128))
                continue;

            int shimmerBlotchSize = genRand.Next(8, 16);
            Rectangle shimmerRect = new Rectangle(x - shimmerBlotchSize, yMid - shimmerBlotchSize, shimmerBlotchSize * 2, shimmerBlotchSize * 2);
            shimmerRect = TileUtilities.Clamp(shimmerRect);
            for (int tx = shimmerRect.Left; tx < shimmerRect.Right; tx++)
            {
                for (int ty = shimmerRect.Top; ty < shimmerRect.Bottom; ty++)
                {
                    Tile tile = Main.tile[tx, ty];
                    tile.LiquidType = LiquidID.Shimmer;
                    tile.LiquidAmount = 255;
                }
            }
            shimmerBlotchCount++;
        }

        Console.WriteLine($"{shimmerBlotchCount} Darkspace Shimmer Blotches Placed");
        progress.Set(0.66D);

        //Here we're placing walls and silk tiles, this is a bit slow, so maybe optimize it a bit later.
        for (int x = 0; x < Main.maxTilesX; x++)
        {
            for (int y = yMin - 100; y < yMax + 100; y++)
            {
                Tile tile = Main.tile[x, y];
                if (!tile.HasTile)
                    continue;

                bool hasRight = (x + 1 < Main.maxTilesX) && !WorldGen.SolidOrSlopedTile(x + 1, y);
                bool hasLeft = (x - 1 > 0) && !WorldGen.SolidOrSlopedTile(x - 1, y);
                bool hasTop = (y + 1 < Main.maxTilesY) && !WorldGen.SolidOrSlopedTile(x, y + 1);
                bool hasBottom = (y - 1 > 0) && !WorldGen.SolidOrSlopedTile(x, y - 1);
                bool hasAny = hasRight || hasLeft || hasTop || hasBottom;

                if (WorldGen.TileIsExposedToAir(x, y) && tile.TileType == TileID.Granite)
                {

                    if (genRand.NextBool(50))
                    {
                        float strength = genRand.Next(7, 11);
                        int steps = genRand.Next(12, 20);
                        ushort tileType = (ushort)ModContent.TileType<SilkTile>();

                        TileID.Sets.CanBeClearedDuringOreRunner[TileID.Granite] = true;
                        WorldGen.OreRunner(x, y,
                           strength,
                            steps, tileType);
                        TileID.Sets.CanBeClearedDuringOreRunner[TileID.Granite] = false;
                        WorldGen.PlaceTile(x, y, ModContent.TileType<MiracleSilkTile>(), mute: true, forced: true);
                        //     SilkManager.GrowSilk(x, y, genRand);
                    }
                }
                if (hasAny && (tile.TileType == TileID.Granite))
                {
                    //WorldGen.PlaceTile(x, y, TileID.Grass, forced: true);
                    Point point = new(x, y);
                    int steps = genRand.Next(1, 4);
                    Vector2 baseDirection = -Vector2.UnitY;
                    int wallCaveWidth = 3;

                    for (int s = 0; s < steps; s++)
                    {
                        if (point.X - wallCaveWidth > 0 && point.X + wallCaveWidth < Main.maxTilesX
                            && point.Y + wallCaveWidth < Main.maxTilesY && point.Y - wallCaveWidth > 0)
                        {
                            WorldUtils.Gen(point, new Shapes.Circle(wallCaveWidth, wallCaveWidth),
                                new Actions.PlaceWall(WallID.GraniteUnsafe));
                        }

                        point += (baseDirection * wallCaveWidth).RotatedByRandom(MathHelper.ToRadians(30)).ToPoint();
                    }
                }
            }
        }

        progress.Set(1D);
    }
}

public class ShimmerFixPass : GenPass
{
    public ShimmerFixPass() : base("Shimmer Fix", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Stay Shimmering";
        Rectangle rec = new(0, ModContent.GetInstance<VeilGen>().DarkspaceStart, Main.maxTilesX, ModContent.GetInstance<VeilGen>().DarkspaceEnd - ModContent.GetInstance<VeilGen>().DarkspaceStart);
        for (int x = rec.Left; x < rec.Right; x++)
        {
            for (int y = rec.Top; y < rec.Bottom; y++)
            {
                Tile tile = Main.tile[x, y];
                if (tile.LiquidType == LiquidID.Lava || tile.LiquidType == LiquidID.Water)
                {
                    tile.LiquidType = LiquidID.Shimmer;
                }
            }
        }
    }
}