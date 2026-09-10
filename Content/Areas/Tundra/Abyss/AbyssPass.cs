using Stellamod.Content.Areas.Tundra.Abyss.TilesAB;
using Stellamod.Core.ZTileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Stellamod.Content.Areas.Tundra.Abyss;

public static class SavedGenerationParameters
{
    public static int SnowLeft;
    public static int SnowRight;
    public static int SnowTop;
    public static int SnowBottom;
    public static double RockLayerHigh;
}

public class AbyssPass : GenPass
{
    public AbyssPass() : base("Abyss", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        int left = SavedGenerationParameters.SnowLeft;
        int right = SavedGenerationParameters.SnowRight;
        int top = SavedGenerationParameters.SnowTop;
        int bottom = ModContent.GetInstance<StellaWorld>().DarkspaceStart;

        //Calculate center of the abyss
        ModContent.GetInstance<VeilGen>().AbyssCenter.X = left + right;
        ModContent.GetInstance<VeilGen>().AbyssCenter.X /= 2;
        ModContent.GetInstance<VeilGen>().AbyssCenter.Y = (int)(SavedGenerationParameters.RockLayerHigh + Main.maxTilesY * 0.15);
        ModContent.GetInstance<VeilGen>().AbyssCenter.Y -= 20;
        //Place the center like a circle

        ushort abyssTile = (ushort)ModContent.TileType<AbyssalDirt>();

        int abyssHigh = ModContent.GetInstance<VeilGen>().AbyssCenter.Y - 500;

        int abyssLow = bottom;

        Rectangle rect = new Rectangle(left, abyssHigh, right - left, abyssLow - abyssHigh);
        Rectangle wallRect = rect.CenterPad(64);

        VeilGen.ClearWallsArea(wallRect);

        //Fill the entire area with abyss dirt tiles
        for (int x = left; x < right; x++)
        {
            for (int y = abyssHigh; y < abyssLow; y++)
            {
                Tile tile = Main.tile[x, y];
                tile.TileFrameX = -1;
                tile.TileFrameY = -1;
                tile.HasTile = true;
                tile.TileType = abyssTile;
                tile.IsHalfBlock = false;
            }
        }
        //var genRand = WorldGen.genRand;
        FastRandom fastRandom = new FastRandom(WorldGen.genRand.Next(0, 3000));
        for (int x = left; x < right; x++)
        {
            if (x > left && x < right - 1)
                continue;

            for (int y = abyssHigh; y < abyssLow; y += 8)
            {
                WorldGen.TileRunner(x, y,
                    strength: 48,
                    125, abyssTile, addTile: true);
            }
        }

        for (int x = left; x < right; x += 8)
        {
            int y = abyssHigh;
            WorldGen.TileRunner(x, y,
                strength: 48,
                125, abyssTile, addTile: true);
            y = abyssLow;
            WorldGen.TileRunner(x, y,
                strength: 48,
                125, abyssTile, addTile: true);
        }

        TileID.Sets.CanBeClearedDuringGeneration[abyssTile] = true;
        TileID.Sets.CanBeClearedDuringOreRunner[abyssTile] = true;

        Span<ushort> pool = new ushort[1].AsSpan();
        pool[0] = (ushort)ModContent.TileType<AbyssalCoarseDirt>();

        FastNoiseLite fnl = new FastNoiseLite();
        for (int i = 0; i < 1; i++)
        {
            fnl.SetSeed(fastRandom.Next(0, 20000));
            fnl.SetFrequency(0.05f);
            fnl.SetDomainWarpType(FastNoiseLite.DomainWarpType.OpenSimplex2);
            fnl.SetDomainWarpAmp(65);
            for (int x = left; x < right; x++)
            {
                for (int y = abyssHigh; y < abyssLow; y++)
                {
                    float noise = fnl.GetNoise(x, y);
                    if (noise > 0.1f)
                    {
                        Tile tile = Main.tile[x, y];
                        tile.TileType = pool[i];
                    }
                }
            }
        }

        Dictionary<int, List<Vector2>> caveConnectPoints = new Dictionary<int, List<Vector2>>();
        bool CreateAbyssCavernCave(int index, Vector2 originPoint, Vector2 velocity, Rectangle scanArea)
        {
            Vector2 cavernPoint = originPoint;
            int failSafe = 0;
            float strength = fastRandom.Next(12, 18);
            float cavingSteps = fastRandom.Next(24, 64);
            float down = fastRandom.Next(-64, -12);
            int connectPointCounter = 5;
            bool success = false;
            while (scanArea.Contains(cavernPoint.ToPoint()) && failSafe < 500)
            {
                connectPointCounter--;
                if (cavingSteps > 0)
                {
                    if (connectPointCounter <= 0)
                    {
                        caveConnectPoints[index].Add(cavernPoint);
                    }
                    WorldGen.TileRunner((int)cavernPoint.X, (int)cavernPoint.Y,
                          strength: strength,
                          fastRandom.Next(7, 27), -1);
                    success = true;
                }
                cavingSteps--;
                if (cavingSteps < down)
                {
                    down = fastRandom.Next(-64, -12);
                    strength = fastRandom.Next(12, 20);
                    cavingSteps = fastRandom.Next(24, 96);
                }
                cavernPoint += velocity * 7;
                failSafe++;
            }
            return success;
        }

        bool CreateAbyssClearing(int index, Vector2 originPoint, Vector2 velocity, Rectangle scanArea)
        {
            Vector2 cavernPoint = originPoint;
            int failSafe = 0;
            float strength = fastRandom.Next(12, 18);
            float cavingSteps = fastRandom.Next(24, 64);
            float down = fastRandom.Next(-64, -12);
            int connectPointCounter = 5;
            bool success = false;
            while (scanArea.Contains(cavernPoint.ToPoint()) && failSafe < 500)
            {
                connectPointCounter--;
                if (cavingSteps > 0)
                {
                    if (connectPointCounter <= 0)
                    {
                        caveConnectPoints[index].Add(cavernPoint);
                    }
                    WorldGen.TileRunner((int)cavernPoint.X, (int)cavernPoint.Y,
                          strength: strength,
                          fastRandom.Next(27, 32), -1);
                    success = true;
                }
                cavingSteps--;
                if (cavingSteps < down)
                {
                    down = fastRandom.Next(-64, -12);
                    strength = fastRandom.Next(35, 45);
                    cavingSteps = fastRandom.Next(56, 100);
                }
                cavernPoint += velocity * 7;
                failSafe++;
            }
            return success;
        }

        List<Vector2> FindPointsICanConnectTo(int index, Vector2 referencePoint)
        {
            float connectRadius = 150;
            float maxConnectionRadiusSquared = connectRadius * connectRadius;
            List<Vector2> otherPoints = new List<Vector2>(16);
            foreach (var kvp in caveConnectPoints)
            {
                if (kvp.Key == index)
                    continue;
                foreach (Vector2 cavePoint in kvp.Value)
                {
                    float distanceSquared = Vector2.DistanceSquared(referencePoint, cavePoint);
                    if (distanceSquared <= maxConnectionRadiusSquared)
                    {
                        otherPoints.Add(cavePoint);
                    }
                }
            }
            return otherPoints;
        }
        List<Vector2> FindAnyPointsICanConnectTo(Vector2 referencePoint, float connectRadius = 150)
        {
            float maxConnectionRadiusSquared = connectRadius * connectRadius;
            List<Vector2> otherPoints = new List<Vector2>(16);
            foreach (var kvp in caveConnectPoints)
            {
                foreach (Vector2 cavePoint in kvp.Value)
                {
                    float distanceSquared = Vector2.DistanceSquared(referencePoint, cavePoint);
                    if (distanceSquared <= maxConnectionRadiusSquared)
                    {
                        otherPoints.Add(cavePoint);
                    }
                }
            }
            return otherPoints;
        }


        //Sprinkle several long caves throughout the biome
        int numCaves = 18;
        Rectangle operationRectangle = new Rectangle(left, abyssHigh, right - left, abyssLow - abyssHigh);
        operationRectangle = operationRectangle.CenterPad(25);


        for (int n = 0; n < numCaves; n++)
        {
            caveConnectPoints.TryAdd(n, new List<Vector2>());
            int dir = 1;
            if (fastRandom.Next(2) == 0)
                dir = -1;
            Vector2 p = new Vector2();
            p.X = fastRandom.Next(left - 25, left + 25);
            if (dir == -1)
                p.X = fastRandom.Next(right - 25, right);
            p.X += fastRandom.Next(-250, 250);
            p.Y = (int)MathHelper.Lerp(abyssHigh, abyssLow, n / (float)numCaves);

            //All caves should be moving to the right
            Vector2 initialDirection = Vector2.UnitX;
            if (dir == -1)
                initialDirection *= -1;

            bool success = false;
            if(n % 4 == 0)
            {
                success = CreateAbyssClearing(n, p, initialDirection, operationRectangle); 
            }
            else
            {
                success = CreateAbyssCavernCave(n, p, initialDirection, operationRectangle);
            }
          
            if (!success)
            {
                n--;
            }
        }


        //Create numerous clearings in the abyss
        int numClearings = 8;

        /*
        for (int n = 0; n < numClearings; n++)
        {
            int dir = 1;
            if (fastRandom.Next(2) == 0)
                dir = -1;
            Vector2 p = new Vector2();
            p.X = fastRandom.Next(left - 25, left + 25);
            if (dir == -1)
                p.X = fastRandom.Next(right - 25, right);
            p.X += fastRandom.Next(-250, 250);
            p.Y = (int)MathHelper.Lerp(abyssHigh, abyssLow, n / (float)numClearings);

            //All caves should be moving to the right
            Vector2 initialDirection = Vector2.UnitX;
            if (dir == -1)
                initialDirection *= -1;

            bool success = CreateAbyssClearing(n, p, initialDirection, operationRectangle);
            if (!success)
            {
                n--;
            }
        }
        */
        //NOW WE CONNECT CAVES
        //Let's make two connections per layer
        //or atleast try to

        for (int n = 0; n < numCaves; n++)
        {
            int attempts = 0;
            for (int k = 0; k < 3; k++)
            {
                if (attempts >= 100)
                {
                    break;
                }
                List<Vector2> points = caveConnectPoints[n];
                if (points.Count <= 0)
                    break;

                Vector2 referencePoint = points[fastRandom.Next(0, points.Count)];
                List<Vector2> pointsICanConnectTo = FindPointsICanConnectTo(n, referencePoint);
                //So by distance to point
                pointsICanConnectTo = pointsICanConnectTo.OrderBy(x => Vector2.Distance(referencePoint, x)).ToList();

                if (pointsICanConnectTo.Count <= 0)
                {
                    k--;
                    attempts++;
                    continue;
                }
                int min = (int)MathF.Min(6, pointsICanConnectTo.Count);
                VeilGen.CreateAbyssConnectionCave(referencePoint, pointsICanConnectTo[fastRandom.Next(0, min)]);
            }
        }

        List<Point> validPointsForFlowers = new List<Point>();
        int skip = 8;
        int xPadding = 50;
        int innerLeft = left + xPadding;
        int innerRight = right - xPadding;
        int innerHigh = abyssHigh + 150;
        int innerLow = abyssLow - 125;
        for(int x = innerLeft; x < innerRight; x+= skip)
        {
            for(int y = innerHigh; y < innerLow; y+= skip)
            {
                Rectangle tileBounds = TileUtilities.CenterTileRectangle(new Point(x, y), 100, 100);
                if (!VeilGen.IsFilledEnough(tileBounds, 0.95f))
                {
                    continue;
                }
                validPointsForFlowers.Add(new Point(x, y));
            }
        }

        //Place Clearings
        //How do we palce these uhhhh
        //Yeahs
        int numBellFlowers = 5;
        List<Point> placedFlowers = new List<Point>();
        List<Vector2> allPoints = new List<Vector2>();
        foreach (var kvp in caveConnectPoints)
            allPoints.AddRange(kvp.Value);
        BellFlowerSystem.ClearBellFlowers();
        void GenerateBellFlowers()
        {
            for (int n = 0; n < numBellFlowers; n++)
            {

                bool TooCloseToAnotherPlacedFlower(Point p)
                {
                    foreach (Point placed in placedFlowers)
                    {
                        if (TileUtilities.TooCloseToTilePoint(p, placed, proximity: 200))
                            return true;
                    }
                    return false;
                }

                Vector2 GetRandomConnectionPoint(Vector2 referencePoint, float checkDistance)
                {
                    float checkDistanceSquared = checkDistance * checkDistance;
                    for (int a = 0; a < 50; a++)
                    {
                        Vector2 p = allPoints[fastRandom.Next(0, allPoints.Count)];
                        float distanceSquare = Vector2.DistanceSquared(referencePoint, p);
                        if (distanceSquare <= checkDistanceSquared)
                            return p;
                    }
                    return allPoints[fastRandom.Next(0, allPoints.Count)];
                }
                int maxAttempts = 100;
                int a = 0;


                while (a < maxAttempts)
                {
                    Point randPoint = validPointsForFlowers[fastRandom.Next(0, validPointsForFlowers.Count)];
                    if (TooCloseToAnotherPlacedFlower(randPoint))
                    {
                        a++;
                        continue;
                    }


                    //List<Vector2> pointsICanConnectTo = FindAnyPointsICanConnectTo(randPoint.ToVector2(), connectRadius: 250);
                    //pointsICanConnectTo = pointsICanConnectTo.OrderBy(x => Vector2.Distance(randPoint.ToVector2(), x)).ToList();
                    Vector2 pointToConnectTo = GetRandomConnectionPoint(randPoint.ToVector2(), checkDistance: 250);
                    if (pointToConnectTo.Y > randPoint.Y)
                        continue;


                    VeilGen.CreateBellFlowerClearing(randPoint.ToVector2());

                    CreateAbyssConnectionCaveMini(randPoint.ToVector2() + new Vector2(0, -16), pointToConnectTo);
                    placedFlowers.Add(randPoint);
                    break;
                }
                if (a >= maxAttempts)
                {
                    Main.NewText("FAIL TO PLACED BELL FLOWER", Color.Red);
                }
            }

        }

        for(int i = 0; i < 3; i++)
            VeilGen.PruneLonelyTiles(rect);
        VeilGen.GenerateWaterBowls(rect, 512, new Point(5, 12), new Point(5, 12));
        VeilGen.GenerateWaterBlobs(rect, 4, new Point(64, 100));
        GenerateBellFlowers();

        var types = new ushort[]
        {
            ModContent.ZTileType<AbyssalFlower>(),
            ModContent.ZTileType<AbyssalFlower>(),
            ModContent.ZTileType<AbyssalFlower>(),
            ModContent.ZTileType<AbyssalWhiteFlower>()
        };
        var types2 = new ushort[]
        {
            ModContent.ZTileType<AbyssalOrbFlower>()
        };
        var wetTypes = new ushort[]
        {
            ModContent.ZTileType<AbyssalReed>()
        };

        VeilGen.KillZTilesInArea(rect);

        int[] multiTileFlowers = new int[]
        {
            ModContent.TileType<BlueFlower>(),
            ModContent.TileType<BlueFlower2>(),
            ModContent.TileType<TealBulb>(),
            ModContent.TileType<TealBulb2>(),
            ModContent.TileType<TealBulb3>()
        };

        var groundTiles = new List<int>
        {
            ModContent.TileType<AbyssalDirt>(),
            ModContent.TileType<AbyssalCoarseDirt>()
        };

        //No need to settle liquids anymore, water just places in the correct spot
        VeilGen.SettleLiquids();
        VeilGen.CreateHalfBlocksOnEdges(rect);
        VeilGen.CreateHalfBlocksOnEdges(rect);
        VeilGen.DecorateSurfaceEdgesWithZTile(new()
        {
            denom = 8,
            renderLayer = ZRenderLayer.InFrontOfWalls,
            targetTileTypes = groundTiles,
            tileBounds = rect,
            zLayer = 0,
            zTileTypes = types,
            value = 175
        });
        VeilGen.DecorateSurfaceEdgesWithZTile(new()
        {
            denom = 128,
            renderLayer = ZRenderLayer.InFrontOfWalls,
            targetTileTypes = groundTiles,
            tileBounds = rect,
            zLayer = 0,
            zTileTypes = types2,
            value = 175
        });
        VeilGen.DecorateWetAreasWithZTile(new()
        {
            denom = 24,
            renderLayer = ZRenderLayer.InFrontOfWalls,
            targetTileTypes = groundTiles,
            tileBounds = rect,
            zLayer = 0,
            zTileTypes = wetTypes,
            value = 175
        });


        VeilGen.DecorateEdgeTilesWithWalls(rect, groundTiles,
            (ushort)ModContent.WallType<AbyssalDirtWall>(), 1);

        VeilGen.DecorateEdgeTilesWithWalls(rect, groundTiles,
             (ushort)ModContent.WallType<AbyssalGrassWallDark>(), 1);
        VeilGen.GrowKelpArea<AbyssalKelp>(rect, minHeight: 5, maxHeight: 9, denom: 7);

        //Extra kelp around flowers
        foreach(Point p in placedFlowers)
        {
            Rectangle kelpRect = TileUtilities.CenterTileRectangle(p, 50, 50);
            VeilGen.GrowKelpArea<AbyssalKelp>(kelpRect, minHeight: 20, maxHeight: 35, denom: 4);
        }

        //This code down here only runs if not in world gen
        if (WorldGen.SkipFramingBecauseOfGen)
            return;
        for (int x = left; x < right; x++)
        {
            for (int y = abyssHigh; y < abyssLow; y++)
            {
                WorldGen.SquareTileFrame(x, y, resetFrame: true);
                WorldGen.SquareWallFrame(x, y, resetFrame: true);
            }
        }

        TileUtilities.UpdateMap(rect, 255);
    }
}

public class AurelusTemplePass : GenPass
{
    public AurelusTemplePass() : base("Aurelus Temple", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        Rectangle rectangle = StructureLoader.ReadRectangle("Structures/Aurelus/AurelusTemple");
        progress.Message = "Singularities Singing!";
        bool placed = false;
        int attempts = 0;
        while (!placed && attempts++ < 1000000)
        {
            Point Loc = ModContent.GetInstance<VeilGen>().AbyssCenter;
            Loc.X -= rectangle.Width / 2;
            Loc.Y += rectangle.Height / 2;
            rectangle.Location = Loc;
            StructureLoader.ProtectStructure(Loc, "Structures/Aurelus/AurelusTemple");
            placed = true;
        }
    }
}