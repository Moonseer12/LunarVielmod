using Stellamod.Content.Areas.Tundra.Abyss.TilesAB;
using Stellamod.Content.Areas.Tundra.Snow.TilesSN;
using Stellamod.Core.ZTileSystem;
using Stellamod.WorldG;
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
        progress.Message = "Shifting Shadows deep in the Ice";
        int left = SavedGenerationParameters.SnowLeft;
        int right = SavedGenerationParameters.SnowRight;
        int top = SavedGenerationParameters.SnowTop;
        int bottom = ModContent.GetInstance<VeilGen>().DarkspaceStart;

        //Calculate center of the abyss
        Point AbyssCenter = new Point();
        AbyssCenter.X = left + right;
        AbyssCenter.X /= 2;
        AbyssCenter.Y = (int)(SavedGenerationParameters.RockLayerHigh + Main.maxTilesY * 0.15);
        AbyssCenter.Y -= 20;
        //Place the center like a circle

        ushort abyssTile = (ushort)ModContent.TileType<AbyssalDirt>();

        int abyssHigh = AbyssCenter.Y - 500;

        int abyssLow = bottom;

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
            }
        }
        var genRand = WorldGen.genRand;
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

        Span<ushort> pool = new ushort[3].AsSpan();
        pool[0] = (ushort)ModContent.TileType<ThickSnowTile>();
        pool[1] = TileID.SnowBlock;
        pool[2] = TileID.IceBlock;


        FastNoiseLite fnl = new FastNoiseLite();
        for (int i = 0; i < 3; i++)
        {
            fnl.SetSeed(genRand.Next(0, 20000));
            fnl.SetFrequency(0.05f);
            fnl.SetDomainWarpType(FastNoiseLite.DomainWarpType.OpenSimplex2);
            fnl.SetDomainWarpAmp(65);
            for (int x = left; x < right; x++)
            {
                for (int y = abyssHigh; y < abyssLow; y++)
                {
                    float noise = fnl.GetNoise(x, y);
                    if (noise > 0.85f)
                    {
                        Tile tile = Main.tile[x, y];
                        tile.TileType = pool[i];
                    }
                }
            }
        }



        void CreateCave(Vector2 originPoint, in Vector2 initialVelocity)
        {
            //The way this cave style will work, is it will start form the origin point
            //and it will go until it hits the edge of the biome or if it['s traveled enoiugh steps
            //After each segment it generates, it randomizes the velocity again in 30 degree angles from the starting direction
            //Which should create nice little lines/caverns
            Vector2 cavernPoint = originPoint;
            int failsafe = 0;
            float strength = genRand.NextFloat(12, 18);
            int maxSteps = genRand.Next(50, 100);
            while (cavernPoint.X < right && failsafe < 300 && maxSteps > 0)
            {
                int remainingSteps = 4;
                Vector2 velocity = initialVelocity.RotatedBy(genRand.NextFloat(-MathHelper.PiOver4 * 0.25f, MathHelper.PiOver4 * 0.25f));
                while (remainingSteps > 0 && maxSteps > 0)
                {
                    cavernPoint += velocity * 7;
                    if (cavernPoint.X < right)
                    {

                        //Cut away at the terrain
                        WorldGen.TileRunner((int)cavernPoint.X, (int)cavernPoint.Y,
                            strength: strength,
                            genRand.Next(7, 25), -1);
                    }

                    remainingSteps--;
                    maxSteps--;
                }
                failsafe++;
            }
        }
        Dictionary<int, List<Vector2>> caveConnectPoints = new Dictionary<int, List<Vector2>>();
        bool CreateAbyssCavernCave(int index, Vector2 originPoint, Vector2 velocity, Rectangle scanArea)
        {
            Vector2 cavernPoint = originPoint;
            int failSafe = 0;
            float strength = genRand.NextFloat(12, 18);
            float cavingSteps = genRand.Next(24, 64);
            float down = genRand.Next(-64, -12);
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
                          genRand.Next(7, 27), -1);
                    success = true;
                }
                cavingSteps--;
                if (cavingSteps < down)
                {
                    down = genRand.Next(-64, -12);
                    strength = genRand.NextFloat(12, 20);
                    cavingSteps = genRand.Next(24, 96);
                }
                cavernPoint += velocity * 7;
                failSafe++;
            }
            return success;
        }
        bool CreateAbyssCavernCaveMini(int index, Vector2 originPoint, Vector2 velocity, Rectangle scanArea)
        {
            Vector2 cavernPoint = originPoint;
            int failSafe = 0;
            float strength = genRand.NextFloat(12, 18);
            float cavingSteps = genRand.Next(24, 64);
            float down = genRand.Next(-64, -12);
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
                          genRand.Next(7, 27), -1);
                    success = true;
                }
                cavingSteps--;
                if (cavingSteps < down)
                {
                    down = genRand.Next(-64, -12);
                    strength = genRand.NextFloat(10, 14);
                    cavingSteps = genRand.Next(24, 96);
                }
                cavernPoint += velocity * 7;
                failSafe++;
            }
            return success;
        }

        void CreateAbyssConnectionCave(Vector2 start, Vector2 end)
        {
            float strength = genRand.NextFloat(12, 18);
            float steps = Vector2.Distance(start, end) / 4f;
            for (float f = 0; f < steps; f++)
            {
                float lerp = f / steps;
                Vector2 pos = Vector2.Lerp(start, end, lerp);
                WorldGen.TileRunner((int)pos.X, (int)pos.Y,
                     strength: strength,
                     genRand.Next(5, 12), -1);
            }
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
        bool IsValidSpotToPlaceCave(Point tilePoint)
        {
            int tileCount = 0;
            //I want this to be centered
            int height = 36;
            tilePoint.Y -= height / 2;
            Rectangle scanArea = new Rectangle(tilePoint.X, tilePoint.Y, 252, height);
            for (int x = scanArea.Left; x < scanArea.Right; x++)
            {
                for (int y = scanArea.Top; y < scanArea.Bottom; y++)
                {
                    Tile tile = Main.tile[x, y];
                    if (tile.HasTile)
                        tileCount++;
                }
            }

            int maxTileCount = scanArea.Width * scanArea.Height;
            float ratio = tileCount / (float)maxTileCount;
            return ratio > 0.65f;

        }
        //Sprinkle several long caves throughout the biome
        int numCaves = 18;
        Rectangle operationRectangle = new Rectangle(left, abyssHigh, right - left, abyssLow - abyssHigh);
        operationRectangle = operationRectangle.CenterPad(25);

        for (int n = 0; n < numCaves; n++)
        {
            caveConnectPoints.TryAdd(n, new List<Vector2>());
            int dir = 1;
            if (genRand.NextBool(2))
                dir = -1;
            Vector2 p = new Vector2();
            p.X = genRand.Next(left - 25, left + 25);
            if (dir == -1)
                p.X = genRand.Next(right - 25, right);
            p.X += genRand.Next(-250, 250);
            p.Y = (int)MathHelper.Lerp(abyssHigh, abyssLow, n / (float)numCaves);

            //All caves should be moving to the right
            Vector2 initialDirection = Vector2.UnitX;
            if (dir == -1)
                initialDirection *= -1;

            bool success = CreateAbyssCavernCave(n, p, initialDirection, operationRectangle);
            if (!success)
            {
                n--;
            }
        }



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

                Vector2 referencePoint = points[genRand.Next(0, points.Count)];
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
                CreateAbyssConnectionCave(referencePoint, pointsICanConnectTo[genRand.Next(0, min)]);
            }
        }

        Rectangle rect = new Rectangle(left, abyssHigh, right - left, abyssLow - abyssHigh);
        VeilGen.PruneLonelyTiles(rect);
        VeilGen.GenerateWaterBowls(rect, 512, new Point(5, 12), new Point(5, 12));
        VeilGen.GenerateWaterBlobs(rect, 4, new Point(64, 100));


        var types = new ushort[]
        {
            ModContent.GetInstance<AbyssalFlower>().type
        };

        VeilGen.ClearWallsArea(rect);
        VeilGen.KillZTilesInArea(rect);

        int[] multiTileFlowers = new int[]
        {
            ModContent.TileType<BlueFlower>(),
            ModContent.TileType<BlueFlower2>(),
            ModContent.TileType<TealBulb>(),
            ModContent.TileType<TealBulb2>(),
            ModContent.TileType<TealBulb3>()
        };

        VeilGen.DecorateSurfaceEdgesWithMultiTile(rect, denom: 8, ModContent.TileType<AbyssalDirt>(), multiTileFlowers);
        VeilGen.DecorateSurfaceEdgesWithZTile(new()
        {
            denom = 8,
            renderLayer = ZRenderLayer.Midground,
            targetTileType = ModContent.TileType<AbyssalDirt>(),
            tileBounds = rect,
            zLayer = 0,
            zTileTypes = types
        });

        VeilGen.DecorateEdgeTilesWithWalls(rect,
            ModContent.TileType<AbyssalDirt>(), 
            (ushort)ModContent.WallType<AbyssalDirtWall>());
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
        /*Rectangle rectangle = StructureLoader.ReadRectangle("Structures/Aurelus/AurelusTemple");
        progress.Message = "Singularities Singing!";
        bool placed = false;
        int attempts = 0;
        while (!placed && attempts++ < 1000000)
        {
            Point Loc = AbyssCenter;
            Loc.X -= rectangle.Width / 2;
            Loc.Y += rectangle.Height / 2;
            rectangle.Location = Loc;
            StructureLoader.ProtectStructure(Loc, "Structures/Aurelus/AurelusTemple");
            placed = true;
        }*/
    }
}