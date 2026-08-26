using Stellamod.Content.Areas.PunkerTown.TilesPT;
using Stellamod.WorldG;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Stellamod.Content.Areas.PunkerTown;

public class MarshPass : GenPass
{
    public MarshPass() : base("Marsh", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Creating the Marsh";
        Point startTile = ModContent.GetInstance<VeilGen>().MarshLocation;
        int length = 1400;
        Point endTile = startTile + new Point(length, 0);
        int mountainHeight = 200;
        int[] heights = new int[length];
        int grassTileType = ModContent.TileType<RainforestGrass>();
        for (int x = startTile.X; x < endTile.X; x++)
        {
            float localX = x - startTile.X;

            float ratio = localX / length;
            int height = (int)(VeilGen.GetMarshHeight(ratio) * mountainHeight);
            heights[x - startTile.X] = height;
            for (int y = 0; y < height; y++)
            {
                WorldGen.PlaceTile(x, startTile.Y - y, grassTileType);
            }
        }
    }
}

public class MarshTreesPass : GenPass
{
    public MarshTreesPass() : base("Marsh Trees", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
         progress.Message = "Planting the Marshy Trees";
        int marshTileLength = 1400;
        GenerateMarshFoliage(ModContent.GetInstance<VeilGen>().MarshLocation, marshTileLength);

        //Place Gothivia Spot
        Point treeTile = ModContent.GetInstance<VeilGen>().MarshLocation + VeilGen.GothiviaSpawnOffset;
        while (!WorldGen.SolidTile(treeTile))
        {
            treeTile.Y++;
        }
        WorldGen.PlaceWall(treeTile.X, treeTile.Y, ModContent.WallType<TheSeededTree>());
    }

    public static void GenerateMarshFoliage(Point startTile, int length)
    {
        var genRand = WorldGen.genRand;

        //Generate the terrain
        Point endTile = startTile + new Point(length, 0);
        int mountainHeight = 200;
        int[] heights = new int[length];
        int grassTileType = ModContent.TileType<RainforestGrass>();
        for (int x = startTile.X; x < endTile.X; x++)
        {
            float localX = x - startTile.X;
            float ratio = localX / length;
            int height = (int)(VeilGen.GetMarshHeight(ratio) * mountainHeight);
            heights[x - startTile.X] = height;
        }

        ushort uGrassTileType = (ushort)grassTileType;
        //Generate big trees, mangrove trees
        for (int x = startTile.X; x < endTile.X; x++)
        {
            float localX = x - startTile.X;
            float ratio = localX / length;
            int heightIndex = x - startTile.X;
            int height = heights[heightIndex];

            int y = startTile.Y - height;
            Tile tile = Main.tile[x, startTile.Y - height];

            Rectangle scanArea = new(x, y, 5, 2);
            Point point = new(x - scanArea.Width / 2, y);
            Dictionary<ushort, int> dictionary = new Dictionary<ushort, int>();
            WorldUtils.Gen(point, new Shapes.Rectangle(scanArea.Width, scanArea.Height), new Actions.TileScanner(uGrassTileType).Output(dictionary));
            int tileCount = dictionary[uGrassTileType];

            if (tileCount >= 5)
            {
                if (genRand.NextBool(16))
                {
                    int treeHeight = genRand.Next(20, 48);
                    PlaceMangroveTrees(x, y, treeHeight);
                }
            }
        }

        //Now we're going to place acacia trees
        ushort bigTreeTileType = (ushort)ModContent.TileType<MangroveTree>();
        for (int x = startTile.X; x < endTile.X; x++)
        {
            float localX = x - startTile.X;
            float ratio = localX / length;
            int heightIndex = x - startTile.X;
            int height = heights[heightIndex];

            int y = startTile.Y - height;
            Tile tile = Main.tile[x, startTile.Y - height];

            Rectangle scanArea = new(x, y, 5, 2);
            Point point = new(x - scanArea.Width / 2, y);
            Dictionary<ushort, int> dictionary = new Dictionary<ushort, int>();
            WorldUtils.Gen(point, new Shapes.Rectangle(scanArea.Width, scanArea.Height), new Actions.TileScanner(uGrassTileType, bigTreeTileType).Output(dictionary));
            int tileCount = dictionary[uGrassTileType];
            int mangroveTreeCount = dictionary[bigTreeTileType];

            if (tileCount >= 5 && mangroveTreeCount <= 0)
            {
                if (genRand.NextBool(8))
                {
                    int treeHeight = genRand.Next(12, 20);
                    PlaceAcaciaTrees(x, y, treeHeight);
                }
            }
        }

        //Spawn surface waters
        int numWaterBlotches = Main.rand.Next(10, 15);
        for (int n = 0; n < numWaterBlotches; n++)
        {
            int randX = genRand.Next(startTile.X, endTile.X);

            int heightIndex = randX - startTile.X;
            int height = heights[heightIndex];

            int randY = startTile.Y - height - 20;

            int radius = 12;
            Point point = new(randX, randY);
            WorldUtils.Gen(point,
                new Shapes.Circle(radius / 2, radius / 2),
                new Actions.SetLiquid(type: LiquidID.Water));
        }

        //Spawn underground waters
        numWaterBlotches = genRand.Next(60, 80);
        for (int n = 0; n < numWaterBlotches; n++)
        {
            int randX = genRand.Next(startTile.X, endTile.X);

            int heightIndex = randX - startTile.X;
            int height = heights[heightIndex];

            int randY = startTile.Y - height + 10 + genRand.Next(0, 100);
            randY = (int)MathHelper.Clamp(randY, startTile.Y - height, startTile.Y);

            int radius = genRand.Next(8, 20);
            Point point = new(randX, randY);

            WorldUtils.Gen(point,
                new Shapes.Circle(radius / 2, radius / 2),
                new Actions.ClearTile(true));

            WorldUtils.Gen(point,
                new Shapes.Circle(radius / 3, radius / 3),
                new Actions.SetLiquid(type: LiquidID.Water));
        }

        //Grass up the holes we just made
        for (int x = startTile.X; x < endTile.X; x++)
        {
            int heightIndex = x - startTile.X;
            int height = heights[heightIndex];
            for (int y = startTile.Y - height + 7; y < Main.maxTilesY / 2; y++)
            {
                Tile tile = Main.tile[x, y];
                if (!tile.HasTile)
                    continue;

                bool touchingAir = WorldGen.TileIsExposedToAir(x, y);
                if (touchingAir && (tile.TileType == ModContent.TileType<RainforestGrass>()) && genRand.NextBool(2))
                {
                    Point point = new(x, y);
                    int steps = genRand.Next(1, 4);
                    Vector2 baseDirection = -Vector2.UnitY;
                    int caveWidth = 3;

                    for (int s = 0; s < steps; s++)
                    {
                        if (point.X - caveWidth > 0 && point.X + caveWidth < Main.maxTilesX && point.Y + caveWidth < Main.maxTilesY && point.Y - caveWidth > 0)
                        {
                            WorldUtils.Gen(point, new Shapes.Circle(caveWidth, caveWidth),
                                new Actions.PlaceWall(WallID.JungleUnsafe));
                        }

                        point += (baseDirection * caveWidth).RotatedByRandom(MathHelper.ToRadians(30)).ToPoint();
                    }
                }
            }
        }
    }

    public static void PlaceMangroveTrees(int treex, int treey, int height)
    {

        if (treey - height < 1)
            return;

        for (int x = -1; x < 3; x++)
        {
            for (int y = 0; y < (height + 2); y++)
            {
                WorldGen.KillTile(treex + x, treey - y);
            }
        }

        WorldGen.PlaceTile(treex, treey, ModContent.TileType<MangroveTree>(), true, true);
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (y == height - 1 && x == 1)
                {
                    WorldGen.PlaceTile(treex + x, treey - (y), ModContent.TileType<MangroveTreeTop>(), true, true);
                }
                else
                {
                    WorldGen.PlaceTile(treex + x, treey - (y), ModContent.TileType<MangroveTree>(), true, true);
                }
            }
        }
        for (int x = -1; x < 3; x++)
        {
            for (int y = 0; y < (height + 2); y++)
            {
                WorldGen.TileFrame(treex + x, treey + y);
            }
        }
    }

    public static void PlaceAcaciaTrees(int treex, int treey, int height)
    {
        if (treey - height < 1)
            return;

        for (int x = -1; x < 3; x++)
        {
            for (int y = 0; y < (height + 2); y++)
            {
                WorldGen.KillTile(treex + x, treey - y);
            }
        }

        WorldGen.PlaceTile(treex, treey, ModContent.TileType<AcaciaTree>(), true, true);
        for (int y = 0; y < height; y++)
        {
            if (y == height - 1)
            {
                WorldGen.PlaceTile(treex, treey - (y + 1), ModContent.TileType<AcaciaTreeTop>(), true, true);
            }
            else
            {
                WorldGen.PlaceTile(treex, treey - (y + 1), ModContent.TileType<AcaciaTree>(), true, true);

            }

        }

        for (int y = 0; y < (height + 2); y++)
        {
            WorldGen.TileFrame(treex, treey + y);
        }
    }
}

/// <summary>
/// Creates the mud layout for the marshy jungle, our jungle is a lot more uniform in how it spawns, so we need to redo the vanilla jungle generation
/// </summary>
public class MarshJungleMudPass : GenPass
{
    public MarshJungleMudPass() : base("Marsh Jungle Mud", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Growing Jungle Mud";
        int width = 1850;

        int halfWidth = width / 2;
        GenVars.jungleMinX = GenVars.jungleOriginX - halfWidth;
        GenVars.jungleMaxX = GenVars.jungleOriginX + halfWidth;

        int minY = (int)Main.worldSurface - 100;

        int darkspaceMaxY = Main.UnderworldLayer - (Main.maxTilesY / 6);
        darkspaceMaxY -= 400;
        int darkspaceMinY = darkspaceMaxY - 12;

        int minMaxY = darkspaceMinY - 700;
        int maxMaxY = darkspaceMinY;

        for (int x = GenVars.jungleMinX; x < GenVars.jungleMaxX; x ++)
        {
            int jungleRange = GenVars.jungleMaxX - GenVars.jungleMinX;
            float xRatio = (x - GenVars.jungleMinX) / (float)jungleRange;
            float bump = EasingFunction.QuadraticBump(xRatio);
            int maxY = (int)MathHelper.Lerp(minMaxY, maxMaxY, bump);

            for (int y = minY; y < maxY; y ++)
            {
                Tile tile = Main.tile[x, y];
                if (tile.HasTile)
                {
                    tile.TileType = TileID.Mud;
                }
            }
        }
    }
}

public class MarshHousingPass : GenPass
{
    public MarshHousingPass() : base("Marsh Housing", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Placing Marshy Outposts";
        PatternManager<int> houses = new PatternManager<int>(
            new Tuple<int, float>(0, 1.0f),
            new Tuple<int, float>(1, 1.0f),
            new Tuple<int, float>(2, 1.0f),
            new Tuple<int, float>(3, 1.0f));

        string GetStruturePath(int index)
        {
            return $"Structures/MarshOutpost{index + 1}";
        }

        //Place ravager first
        string ravagerArena = "Structures/RavagerArena";
        Point ravagerPlacementPoint = ModContent.GetInstance<VeilGen>().MarshLocation;
        ravagerPlacementPoint.X += 550;
        ravagerPlacementPoint.Y -= 500;
        ravagerPlacementPoint = TileUtilities.FallToSolidTile(ravagerPlacementPoint);
        Structurizer.ProtectStructure(ravagerPlacementPoint, ravagerArena);

        int[] tileBlend = [
            TileID.RubyGemspark
        ];
        Structurizer.ReadStruct(ravagerPlacementPoint, ravagerArena, tileBlend);

        int numHouses = 5;
        for (int i = 0; i < numHouses; i++)
        {
            int houseIndex = houses.NextPattern();
            string structure = GetStruturePath(houseIndex);

            for (int a = 0; a < 100000; a++)
            {
                Point houseFallingPoint = ModContent.GetInstance<VeilGen>().MarshLocation;
                houseFallingPoint.Y -= 1000;

                int dir = Main.rand.NextBool(2) ? 1 : -1;

                //Need to avoid the center point
                houseFallingPoint.X = GenVars.jungleOriginX + Main.rand.Next(200, 500) * dir;
                houseFallingPoint = TileUtilities.FallToSolidTile(houseFallingPoint);

                if (!Structurizer.TryPlaceAndProtectStructure(houseFallingPoint, structure))
                    continue;
                Rectangle structureRectangle = Structurizer.ReadRectangle(structure);
                structureRectangle.Location = houseFallingPoint;
                for (int beamX = structureRectangle.Location.X;
                    beamX < structureRectangle.Location.X + structureRectangle.Width; beamX += 4)
                {
                    //Place beams
                    int beamY = structureRectangle.Location.Y;
                    Tile tile = Main.tile[beamX, beamY];
                    int solidCount = 0;
                    while (solidCount < 5)
                    {
                        if (!WorldGen.SolidTile(beamX, beamY))
                        {
                            WorldGen.PlaceTile(beamX, beamY, TileID.BorealBeam);
                        }
                        else
                        {
                            solidCount++;
                        }
                        beamY++;
                    }
                }
                break;
            }
        }
    }
}

public class JungleSurfaceCavePass : GenPass
{
    public JungleSurfaceCavePass() : base("Jungle Surface Caves", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Jungle Surface Caves";
        int caveOriginX = GenVars.jungleOriginX;
        int caveOriginY = ModContent.GetInstance<VeilGen>().MarshLocation.Y;
        caveOriginY -= 35;
        GenerationPrefab prefab = ModContent.GetInstance<GenerationTextureManager>().GetPrefab("JungleTop");
        prefab.PasteErase(caveOriginX, caveOriginY, PrefabPlacementType.FromTopCenter);
    }
}