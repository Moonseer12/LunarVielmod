using ReLogic.Content;
using Stellamod.Common.DungeonGeneration;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;
using Terraria.Utilities;
using Terraria.WorldBuilding;

namespace Stellamod.WorldG;

/// <summary>
/// Collection of helper functions for manipulating textures.
/// </summary>
public static class TextureUtilities
{
    public static int GetPixelIndex(Texture2D texture, int x, int y)
    {
        return x + y * texture.Width;
    }

    public static Color GetPixelColor(Texture2D texture, int x, int y, Color[] pixels)
    {
        return pixels[GetPixelIndex(texture, x, y)];
    }
}

public enum PrefabPlacementType : byte
{
    FromTopLeft,
    FromTopCenter,
    FromCenter,
    FromTopRight
}

/// <summary>
/// Encapsulates a texture for world generation purposes, in most cases we're just going to use the texture as a mask for erasing tiles.
/// </summary>
public class GenerationPrefab : IDisposable
{
    public GenerationPrefab(string name, Asset<Texture2D> textureAsset)
    {
        Name = name;
        TextureAsset = textureAsset;
        Pixels = new Color[Width * Height];
        TextureAsset.Value.GetData(Pixels);
    }

    public string Name { get; private set; }
    public Color[] Pixels { get; private set; }
    public Asset<Texture2D> TextureAsset { get; private set; }
    public int Width => TextureAsset.Width();
    public int Height => TextureAsset.Height();

    public void Dispose()
    {
        TextureAsset = null;
    }

    public Color Sample(int localX, int localY)
    {
        return TextureUtilities.GetPixelColor(TextureAsset.Value, localX, localY, Pixels);
    }


    private void PasteEraseInner(in int originX, in int originY)
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                int tileX = originX + x;
                int tileY = originY + y;
                if (!WorldGen.InWorld(tileX, tileY))
                    continue;

                Color c = Sample(x, y);
                if (c.R > 125)
                {
                    Tile t = Main.tile[tileX, tileY];
                    t.ClearEverything();
                }
            }
        }
    }
    public void PasteErase(int originX, int originY, Point pixelOrigin)
    {
        originX -= pixelOrigin.X;
        originY -= pixelOrigin.Y;
        PasteEraseInner(originX, originY);
    }
    public void PasteErase(Point origin, PrefabPlacementType placementType)
    {
        PasteErase(origin.X, origin.Y, placementType);
    }
    public Rectangle GetBounds(int originX, int originY, PrefabPlacementType placementType)
    {
        switch (placementType)
        {
            case PrefabPlacementType.FromTopLeft:
                break;
            case PrefabPlacementType.FromTopCenter:
                originX -= Width / 2;
                break;
            case PrefabPlacementType.FromCenter:
                originX -= Width / 2;
                originY -= Height / 2;
                break;
            case PrefabPlacementType.FromTopRight:
                originX -= Width;
                break;

        }

        //Clamp to world bounds to prevent index out of bounds exceptions
        Rectangle rectangle = new Rectangle(originX, originY, Width, Height);
        rectangle.X = (int)MathHelper.Clamp(rectangle.X, 0, Main.maxTilesX - 1);
        rectangle.Y = (int)MathHelper.Clamp(rectangle.Y, 0, Main.maxTilesY - 1);

        int maxRight = (int)MathHelper.Clamp(rectangle.X + rectangle.Width, 0, Main.maxTilesX - 1);
        int maxWidth = maxRight - rectangle.Left;
        rectangle.Width = (int)MathHelper.Min(rectangle.Width, maxWidth);

        int maxBottom = (int)MathHelper.Clamp(rectangle.Y + rectangle.Height, 0, Main.maxTilesY - 1);
        int maxHeight = maxBottom - rectangle.Top;
        rectangle.Height = (int)MathHelper.Min(rectangle.Height, maxHeight);
        return rectangle;
    }
    public void PasteErase(int originX, int originY, PrefabPlacementType placementType)
    {
        switch (placementType)
        {
            case PrefabPlacementType.FromTopLeft:
                break;
            case PrefabPlacementType.FromTopCenter:
                originX -= Width / 2;
                break;
            case PrefabPlacementType.FromCenter:
                originX -= Width / 2;
                originY -= Height / 2;
                break;
            case PrefabPlacementType.FromTopRight:
                originX -= Width;
                break;

        }

        PasteEraseInner(originX, originY);
    }


}


[Autoload(Side = ModSide.Client)]
public class GenerationTextureManager : ModSystem
{
    public Dictionary<string, GenerationPrefab> Prefabs { get; private set; }
    public override void Load()
    {
        base.Load();
        Main.QueueMainThreadAction(LoadPrefabAssets);
    }
    public override void Unload()
    {
        base.Unload();
        Main.QueueMainThreadAction(UnloadPrefabAssets);
    }

    private void UnloadPrefabAssets()
    {

    }
    private void LoadPrefabAssets()
    {
        Prefabs = new Dictionary<string, GenerationPrefab>();
        Mod mod = Stellamod.Instance;
        foreach (var file in mod.GetFileNames())
        {
            if (file.Contains("WorldGen/"))
            {
                string path = "Stellamod/" + file;
                path = path.Replace(".rawimg", "");
                Asset<Texture2D> worldGenTexture = ModContent.Request<Texture2D>(path, AssetRequestMode.ImmediateLoad);
                GenerationPrefab prefab = new (Path.GetFileNameWithoutExtension(file), worldGenTexture);
                Console.WriteLine($"Prefab {prefab.Name}");
                Prefabs.Add(prefab.Name, prefab);
            }
        }
    }

    public GenerationPrefab GetPrefab(string name) => Prefabs[name];
}


public static class DungeonLayouter
{

    public static (int, int)[] GenerateLayout(int roomCount, UnifiedRandom rand)
    {
        int size = 16;
        bool[,] map = new bool[size, size];
        int[,] costs = new int[size, size];

        int halfSize = size / 2;
        int x = halfSize;
        int y = halfSize;
        int placedRooms = 0;
        int adjacentIndex = 0;

        (int, int)[] adjacent = new (int, int)[4];
        (int, int)[] roomsOnMap = new (int, int)[roomCount];


        void PlaceRoom(int x, int y)
        {
            if (map[x, y])
                return;

            //Increase costs of neighbouring nodes
            for (int a = 0; a < adjacentIndex; a++)
            {
                (int ax, int ay) = adjacent[a];
                costs[ax, ay] += 1;
            }

            map[x, y] = true;
            roomsOnMap[placedRooms] = (x, y);
            placedRooms++;
        }

        void PushAdjacent(int ax, int ay)
        {
            if (ax < 0 || ax >= size || ay <= 0 || ay >= size)
                return;

            if (costs[ax, ay] >= 2)
                return;

            if (map[ax, ay])
                return;

            adjacent[adjacentIndex++] = (ax, ay);
        }

        void FindAdjacents()
        {
            adjacentIndex = 0;
            PushAdjacent(x - 1, y);
            PushAdjacent(x + 1, y);
            PushAdjacent(x, y - 1);
            PushAdjacent(x, y + 1);
        }

        int snakeLength = 4;

        while (placedRooms < roomCount)
        {
            //Get Adjacent Points to current node
            FindAdjacents();

            //Place at the current position if possible
            PlaceRoom(x, y);

            //Recalculate the adjacent nodes since the costs have changed
            FindAdjacents();

            snakeLength--;

            //We've come to a dead end if the adjacent index = 0
            //In this case we should go to a different room and keep moving around
            if (adjacentIndex <= 0 || snakeLength <= 0)
            {
                snakeLength = rand.Next(4);
                //Just go to a random room we placed
                int positionToMoveTo = rand.Next(placedRooms);
                (x, y) = roomsOnMap[positionToMoveTo];
            }
            else
            {
                int positionToMoveTo = rand.Next(adjacentIndex);
                (int ax, int ay) = adjacent[positionToMoveTo];
                x = ax;
                y = ay;
            }
            //We now have all open spots next to this room
        }
        return roomsOnMap;
    }
}
public record struct CellularAutomataParams(int Steps, float RandomFill, int BirthLimit, int DeathLimit);

public class VeilGen : ModSystem
{
    public Point AbyssCenter;
    public Point RoyalCapitalLocation;
    public Point VeizalHillStartLcoation;
    public Point VeizalHillEndLocation;
    public Point MistyHillStartLocation;
    public Point MistyHillEndLocation;
    public Point MistyDungeonLocation;
    public Point FableFarEdgeLocation;
    public Point FableLocation;
    public Point FableHillStartLocation;
    public Point FableHillEndLocation;
    public Point DesertLocation;
    public Point WitchTownLocation;
    public Point ManorLocation;
    public Point MarshLocation;
    public Point AlcadLocation;
    public Point CoralwaysLocation;
    public Point SnowClumpOriginPoint;
    public static Point GothiviaSpawnOffset => new(246, -99);

    public int CindersparkStart;
    public int CindersparkEnd;
    public int DarkspaceStart;
    public int DarkspaceEnd;
    public int HeatedDepthsStart;
    public int HeatedDepthsEnd;
    public const int Desert_Padding = 200;

    public static readonly Room[] MineshaftPrefabs = DungeonSaveUtility.GetDungeonPrefabs("Mineshafts");

    public static float GetFableHillHeight(float x)
    {
        float bump = x * (4 - x * 4);
        float mountains = MathF.Sin(x * 1) * 0.5f + 0.5f;
        float mountains2 = MathF.Sin(x * 1) * 0.5f + 0.7f;
        float dips = MathF.Sin(x * 16) * 0.1f;
        float roughness = MathF.Sin(x * 76) * 0.01f;
        float roughness2 = MathF.Sin(x * 101) * 0.005f;
        float y = bump * mountains * mountains2 - dips - roughness - roughness2;
        return y + 0.1f;
    }

    public static void QuickOrePatch(int x, int y, int tileType)
    {
        Walker(x, y, WorldGen.genRand.Next(50, 90), tileType, maxDist: 3);
    }
    public static void Walker(int x, int y, int steps, int tileType, int maxDist)
    {
        Point walkerPoint = new(x,y);
        Point originalPoint = walkerPoint;
        var genRand = WorldGen.genRand;
        for (int s = 0; s < steps; s++)
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
            tile.HasTile = true;
            tile.TileFrameX = -1;
            tile.TileFrameY = -1;
            tile.TileType = (ushort)tileType;

            //Reset if walking too far
            int dx = Math.Abs(walkerPoint.X - originalPoint.X);
            int dy = Math.Abs(walkerPoint.Y - originalPoint.Y);
            if (dx > maxDist || dy > maxDist)
            {
                walkerPoint = originalPoint;
            }
        }

    }


    public static void PruneLonelyTiles(Rectangle areaRectangle)
    {
        for(int x = areaRectangle.Left; x < areaRectangle.Right; x++)
        {
            for(int y = areaRectangle.Top; y < areaRectangle.Bottom; y++)
            {
                Tile tile = Main.tile[x, y];
                Tile tileAbove = Main.tile[x, y - 1];
                Tile tileBelow = Main.tile[x, y + 1];
                Tile tileLeft = Main.tile[x - 1, y];
                Tile tileRight = Main.tile[x + 1, y];

                int count = 0;
                if (tileAbove.HasTile)
                    count++;
                if (tileBelow.HasTile)
                    count++;
                if (tileLeft.HasTile)
                    count++;
                if (tileRight.HasTile)
                    count++;


                if (count <= 1 && tile.HasTile)
                    tile.ClearTile();
            }
        }
    }


    /// <summary>
    /// Checks if a tile is exposed to air only on cardinal directions, it will not check diagonals
    /// This function assumes that it will not have an out of bounds exception, clamp boundaries before using it
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static bool IsTileExposedToAirCardinal(int x, int y)
    {
        return  !Main.tile[x - 1, y].HasTile ||
                !Main.tile[x + 1, y].HasTile ||
                !Main.tile[x, y - 1].HasTile ||
                !Main.tile[x, y + 1].HasTile;
    }

    public static void WallWalker(int x, int y, int steps, int wallType, int maxDist, byte paint = 0)
    {
        Point walkerPoint = new(x, y);
        Point originalPoint = walkerPoint;
        var genRand = WorldGen.genRand;
        for (int s = 0; s < steps; s++)
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
            //not sure if i have to do framing manually
            //we'll find out
            tile.WallType = (ushort)wallType;
            tile.WallFrameX = -1;
            tile.WallFrameY = -1;
            tile.WallColor = paint;

            //Reset if walking too far
            int dx = Math.Abs(walkerPoint.X - originalPoint.X);
            int dy = Math.Abs(walkerPoint.Y - originalPoint.Y);
            if (dx > maxDist || dy > maxDist)
            {
                walkerPoint = originalPoint;
            }
        }
    }


    public static bool IsTileNearby(int x, int y, int distance, bool[] tileSet)
    {
        int left = x - distance;
        int top = y - distance;
        Rectangle rect = new Rectangle(left, top, distance * 2, distance * 2);
        rect = TileUtilities.Clamp(rect);
        for(int i = rect.Left; i < rect.Right; i++)
        {
            for(int j = rect.Top; j < rect.Bottom; j++)
            {
                Tile tile = Main.tile[i, j];
                if (!tile.HasTile)
                    continue;
                if (tileSet[tile.TileType])
                    return true;
            }
        }


        return false;
    }

    public static int CountAliveNeighbours(int x, int y, bool[,] map)
    {
        int width = map.GetLength(0);
        int height = map.GetLength(1);
        int count = 0;
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (i == 0 && j == 0)
                    continue;

                int dx = x + i;
                int dy = y + j;
                if(dx < 0 || dy < 0 || dx >= width || dy >= height)
                {
                    count++;
                } else if (map[dx, dy])
                {
                    count++;
                }
            }
        }
        return count;
    }
    public static bool[,] Step(bool[,] oldMap, in CellularAutomataParams @params)
    {
        int width = oldMap.GetLength(0);
        int height = oldMap.GetLength(1);
        bool[,] newMap = new bool[width, height];
        for(int x = 0; x < width; x++)
        {
            for(int y =0;y < height; y++)
            {
                int neighbours = CountAliveNeighbours(x, y, oldMap);
                if(neighbours > @params.BirthLimit)
                {
                    newMap[x, y] = true;
                } else if (neighbours <= @params.DeathLimit)
                {
                    newMap[x, y] = false;

                } else
                {
                    newMap[x, y] = oldMap[x, y];
                }
            }
        }
        return newMap;
    }

    public static bool PlaceCavePrefab(int x, int y, UnifiedRandom genRand)
    {
        if (VeilGen.IsTileNearby(x, y, 50, TileSets.BlockMineshafts))
            return false;

        int maxCaveCount = 9;
        string caveToPlace = $"CavernCave_{genRand.Next(maxCaveCount) + 1}";
        GenerationPrefab prefab = ModContent.GetInstance<GenerationTextureManager>().GetPrefab(caveToPlace);
        prefab.PasteErase(x, y, PrefabPlacementType.FromCenter);

        //Basically we're just sprinkling blotches everywhere and then smoothing it out with automata to create variation within the same room type
        //Honestly it's genius
        int left = x - prefab.Width / 2;
        int top = y - prefab.Height / 2;
        Rectangle rect = new Rectangle(left, top, prefab.Width, prefab.Height);
        rect = TileUtilities.Clamp(rect);
        int numBlotches = prefab.Width / 3;
        for(int n = 0; n < numBlotches; n++)
        {
            int randX = genRand.Next(rect.Left, rect.Right);
            int randY = genRand.Next(rect.Top, rect.Bottom);
            Walker(randX, randY, genRand.Next(60, 120), TileID.Stone, 5);
        }
        CellularAutomataParams @params = new CellularAutomataParams() with { Steps = 3, RandomFill = 55, BirthLimit = 4, DeathLimit = 4 };
        AutomataSmoothErase(rect, in @params);
        return true;
    }
    public static void PlaceDeepCuttingCave(Vector2 position, Vector2 initialDirection, int caveSteps, int walkerSteps, int walkerWidth, UnifiedRandom genRand, FastNoiseLite fnl)
    {
        void Carve(int x, int y)
        {
            Point walkerPoint = new Point(x, y);
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
        //ALGO:
        //Pick a random point on the world
        //Use that as a starting coordinate
        //Move the tunnel in an initial direction, for us likely diagonally down
        //After each step, the tunnel turns its direction by a  small amount based on noise
        //At each step, do a walker algorithm to cut away at the terrain
        bool placedCave = false;
        for(int s = 0; s < caveSteps; s++)
        {
            Point tile = position.ToTileCoordinates();
            Carve(tile.X, tile.Y);
            position += initialDirection * walkerWidth * 2;
            float noise = fnl.GetNoise(s, 0);
            initialDirection = initialDirection.RotatedBy(noise * 0.1D);
            if (genRand.NextBool(caveSteps) && !placedCave)
            {
                placedCave = PlaceCavePrefab(tile.X, tile.Y, genRand);
            }
        }
    }

    public static void AutomataSmoothErase(Rectangle rectangle, in CellularAutomataParams @params)
    {
        bool[,] map = new bool[rectangle.Width, rectangle.Height];
        for(int x = rectangle.Left; x < rectangle.Right; x++)
        {
            for(int y = rectangle.Top; y < rectangle.Bottom; y++)
            {
                int lx = x - rectangle.Left;
                int ly = y - rectangle.Top;
                map[lx, ly] = Main.tile[x, y].HasTile;
            }
        }
        map = AutomataSmooth(map, in @params);
        Erase(new Point(rectangle.X, rectangle.Y), map);
    }

    public static bool[,] AutomataSmooth(bool[,] map, in CellularAutomataParams @params)
    {
        int width = map.GetLength(0);
        int height = map.GetLength(1);
        for (int s = 0; s < @params.Steps; s++)
        {
            map = Step(map, in @params);
        }

        //Remove tiles with only 1 neighbour
        bool[,] lessLonelyMap = new bool[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighbourCount = 0;
                for (int dx = -1; dx < 2; dx++)
                {
                    for (int dy = -1; dy < 2; dy++)
                    {
                        if (dx != 0 && dy != 0)
                            continue;
                        if (dx == 0 && dy == 0)
                            continue;

                        int newX = x + dx;
                        int newY = y + dy;
                        if (newX < 0 || newY < 0 || newX >= width || newY >= height)
                            neighbourCount++;
                        else if (map[newX, newY])
                            neighbourCount++;
                    }
                }

                if (neighbourCount <= 1)
                {
                    lessLonelyMap[x, y] = false;
                }
                else
                {
                    lessLonelyMap[x, y] = map[x, y];
                }
            }
        }

        return lessLonelyMap;
    }
    public static bool[,] CellularAutomataMap(int width, int height, in CellularAutomataParams @params, UnifiedRandom genRand)
    {
        bool[,] map = new bool[width, height];

        //First initialize the map with random values
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                map[x, y] = genRand.Next(0, 100) < @params.RandomFill;
            }
        }

        for(int s = 0; s < @params.Steps; s++)
        {
            map = Step(map, in @params);
        }


        //Remove tiles with only 1 neighbour
        bool[,] lessLonelyMap = new bool[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighbourCount = 0;
                for (int dx = -1; dx < 2; dx++)
                {
                    for (int dy = -1; dy < 2; dy++)
                    {
                        if (dx != 0 && dy != 0)
                            continue;
                        if (dx == 0 && dy == 0)
                            continue;

                        int newX = x + dx;
                        int newY = y + dy;
                        if (newX < 0 || newY < 0 || newX >= width || newY >= height)
                            neighbourCount++;
                        else if (map[newX, newY])
                            neighbourCount++;
                    }
                }

                if (neighbourCount <= 1)
                {
                    lessLonelyMap[x, y] = false;
                }
                else
                {
                    lessLonelyMap[x, y] = map[x, y];
                }
            }
        }

        return lessLonelyMap;
    }

    public static void Erase(Point topLeft, bool[,] map)
    {
        int width = map.GetLength(0);
        int height = map.GetLength(1);
        Rectangle rect = new Rectangle(topLeft.X, topLeft.Y, width, height);
        rect = TileUtilities.Clamp(rect);
        for(int x = rect.Left; x < rect.Right; x++)
        {
            for(int y = rect.Top; y < rect.Bottom; y++)
            {
                Tile tile = Main.tile[x, y];
                if (!map[x - rect.Left, y - rect.Top])
                {
                    tile.ClearTile();
                }
        
            }
        }
    }

    public static void SettleLiquids()
    {
        Liquid.QuickWater(3);
        WorldGen.WaterCheck();
        int num = 0;
        Liquid.quickSettle = true;
        int num2 = 10;
        while (num < num2)
        {
            int num3 = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;
            num++;
            double num4 = 0.0;
            int num5 = num3 * 5;
            while (Liquid.numLiquid > 0)
            {
                num5--;
                if (num5 < 0)
                {
                    break;
                }

                double num6 = (double)(num3 - (Liquid.numLiquid + LiquidBuffer.numLiquidBuffer)) / (double)num3;
                if (Liquid.numLiquid + LiquidBuffer.numLiquidBuffer > num3)
                {
                    num3 = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;
                }

                if (num6 > num4)
                {
                    num4 = num6;
                }
                else
                {
                    num6 = num4;
                }

                int num7 = 10;
                if (num > num7)
                {
                    num7 = num;
                }

                Liquid.UpdateLiquid();
            }

            WorldGen.WaterCheck();
        }

        Liquid.quickSettle = false;
    }
    public static float GetMarshHeight(float x)
    {
        float bump = x * (4 - x * 4);
        float mountains = MathF.Sin(x * 2) * 0.5f + 0.5f;
        float mountains2 = MathF.Sin(x * 2) * 0.5f + 0.7f;
        float dips = MathF.Sin(x * 32) * 0.1f;
        float roughness = MathF.Sin(x * 120) * 0.01f;
        float roughness2 = MathF.Sin(x * 200) * 0.005f;
        float y = bump * mountains * mountains2 - dips - roughness - roughness2;
        return y + 0.1f;
    }

    public static bool IsAir(int x, int y, int w)
    {
        for (int k = 0; k < w; k++)
        {
            Tile tile = Framing.GetTileSafely(x + k, y);
            if (tile.HasTile && Main.tileSolid[tile.TileType])
                return false;
        }

        return true;
    }

    public static void PlaceMultitile(Point16 position, int type, int style = 0)
    {
        var data = TileObjectData.GetTileData(type, style); //magic numbers and uneccisary params begone!

        if (position.X + data.Width > Main.maxTilesX || position.X < 0)
            return; //make sure we dont spawn outside of the world!

        if (position.Y + data.Height > Main.maxTilesY || position.Y < 0)
            return;

        int xVariants = 0;
        int yVariants = 0;

        if (data.StyleHorizontal)
            xVariants = Main.rand.Next(data.RandomStyleRange);
        else
            yVariants = Main.rand.Next(data.RandomStyleRange);

        for (int x = 0; x < data.Width; x++) //generate each column
        {
            for (int y = 0; y < data.Height; y++) //generate each row
            {
                Tile tile = Framing.GetTileSafely(position.X + x, position.Y + y); //get the targeted tile
                tile.TileType = (ushort)type; //set the type of the tile to our multitile

                int yHeight = 0;
                for (int k = 0; k < data.CoordinateHeights.Length; k++)
                {
                    yHeight += data.CoordinateHeights[k] + data.CoordinatePadding;
                }

                tile.TileFrameX = (short)((x + data.Width * xVariants) * (data.CoordinateWidth + data.CoordinatePadding)); //set the X frame appropriately
                tile.TileFrameY = (short)(y * (data.CoordinateHeights[y > 0 ? y - 1 : y] + data.CoordinatePadding) + yVariants * yHeight); //set the Y frame appropriately
                tile.HasTile = true; //activate the tile
            }
        }
    }
    public static void PlaceBigTrees<TreeTrunk, TreeTop>(int treex, int treey, int height)
        where TreeTrunk : ModTile
        where TreeTop : ModTile
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

        WorldGen.PlaceTile(treex, treey, ModContent.TileType<TreeTrunk>(), true, true);
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (y == height - 1 && x == 1)
                {
                    WorldGen.PlaceTile(treex + x, treey - (y), ModContent.TileType<TreeTop>(), true, true);

                }
                else
                {
                    WorldGen.PlaceTile(treex + x, treey - (y), ModContent.TileType<TreeTrunk>(), true, true);
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
    public static void PlaceTrees<TreeTrunk, TreeTop>(int treex, int treey, int height)
        where TreeTrunk : ModTile
        where TreeTop : ModTile
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

        WorldGen.PlaceTile(treex, treey, ModContent.TileType<TreeTrunk>(), true, true);
        for (int y = 0; y < height; y++)
        {
            if (y == height - 1)
            {
                WorldGen.PlaceTile(treex, treey - (y + 1), ModContent.TileType<TreeTop>(), true, true);
            }
            else
            {
                WorldGen.PlaceTile(treex, treey - (y + 1), ModContent.TileType<TreeTrunk>(), true, true);

            }

        }

        for (int y = 0; y < (height + 2); y++)
        {
            WorldGen.TileFrame(treex, treey + y);
        }
    }

    public static void ClearTrees(Rectangle rectangle)
    {
        int startX = rectangle.Location.X;
        int endX = startX + rectangle.Width;
        int startY = rectangle.Location.Y;
        int endY = rectangle.Location.Y + rectangle.Height;

        startX = Math.Clamp(startX, 0, Main.maxTilesX - 1);
        endX = Math.Clamp(endX, 0, Main.maxTilesX - 1);
        startY = Math.Clamp(startY, 0, Main.maxTilesY - 1);
        endY = Math.Clamp(endY, 0, Main.maxTilesY - 1);

        for (int x = startX; x < endX; x++)
        {
            for (int y = startY; y < endY; y++)
            {
                Tile tile = Main.tile[x, y];
                if (TileID.Sets.IsATreeTrunk[tile.TileType])
                {
                    tile.ClearEverything();
                }
            }
        }
    }

    public static void GenerateBowlLake(Point waterStart, Point waterEnd, int maxLakeDepth)
    {
        //Generate water bowl
        while (!WorldGen.SolidTile(waterStart))
            waterStart.Y++;

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
                WorldGen.KillTile(lakeX, lakeY);
                WorldGen.KillWall(lakeX, lakeY);
                d++;
                if (d > 10)
                {
                    WorldGen.PlaceLiquid(lakeX, lakeY, (byte)LiquidID.Water, byte.MaxValue);
                }
            }
        }
    }

    public static void ClearLonelyTiles(Rectangle rectangle)
    {
        int startX = rectangle.Location.X;
        int endX = startX + rectangle.Width;
        int startY = rectangle.Location.Y;
        int endY = rectangle.Location.Y + rectangle.Height;

        //Add 1 extra tile of fluff since we're checking adjacent tiles
        startX = Math.Clamp(startX, 1, Main.maxTilesX - 2);
        endX = Math.Clamp(endX, 1, Main.maxTilesX - 2);
        startY = Math.Clamp(startY, 1, Main.maxTilesY - 2);
        endY = Math.Clamp(endY, 1, Main.maxTilesY - 2);

        for (int x = startX; x < endX; x++)
        {
            for (int y = startY; y < endY; y++)
            {
                Tile tile = Main.tile[x, y];
                if (!tile.HasTile)
                    continue;

                int adjacentCount = 0;
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        //Ignore diagonals
                        if (i != 0 && j != 0)
                            continue;
                        if (i == 0 && j == 0)
                            continue;
                        Tile adjacentTile = Main.tile[x + i, y + j];
                        if (adjacentTile.HasTile && Main.tileSolid[adjacentTile.TileType])
                            adjacentCount++;
                    }
                }

                if (adjacentCount <= 1)
                    tile.ClearEverything();
            }
        }
    }

    public static void GenerateFigure8Cave(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, int caveWidth, int caveSteps)
    {
        var genRand = WorldGen.genRand;
        int caveSeed = genRand.Next();

        //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
        float i = cavePosition.X;
        Vector2 caveVelocity = baseCaveDirection;
        Vector2 pullDirection;
        pullDirection.X = -baseCaveDirection.X;
        pullDirection.Y = 1;

        Vector2 targetPosition = caveVelocity + pullDirection;
        Vector2 startPullDirection = pullDirection;
        float sharpness = 3f;
        float counter = 0;
        float target = 100;
        for (int j = 0; j < caveSteps; j++)
        {
            //Homing
            float degreesToRotate = sharpness;
            float length = caveVelocity.Length();
            float targetAngle = (targetPosition - cavePosition).ToRotation();
            Vector2 newVelocity = caveVelocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(degreesToRotate)).ToRotationVector2() * length;
            caveVelocity = newVelocity;
            if (counter < 20f)
            {
                pullDirection = Vector2.Lerp(startPullDirection, Vector2.Zero, counter / 20f);
                targetPosition = cavePosition + pullDirection;
                counter++;
            }

            if (counter > target)
            {
                target = genRand.Next(100, 150);
                targetPosition.X = -targetPosition.X;
                startPullDirection = pullDirection;
                counter = 0;
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

    public static void GenerateSimpleCaveWall(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, Vector2 pullDirection, int caveWidth, int caveSteps, ushort tileToPlace)
    {
        var genRand = WorldGen.genRand;
        int caveSeed = genRand.Next();

        //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
        float i = cavePosition.X;
        Vector2 caveVelocity = baseCaveDirection;
        Vector2 breakStrength = caveStrength;

        Vector2 startVelocity = caveVelocity;
        Vector2 pullVelocity = pullDirection;

        float sharpness = 1;
        float counter = 0;
        bool shouldBreak = false;
        for (int j = 0; j < caveSteps; j++)
        {

            counter++;
            breakStrength *= 0.9995f;
            float degreesToRotate = sharpness;
            float length = caveVelocity.Length();
            float targetAngle = (pullVelocity - startVelocity).ToRotation();
            Vector2 newVelocity = caveVelocity.ToRotation().AngleTowards(targetAngle,
                MathHelper.ToRadians(degreesToRotate)).ToRotationVector2() * length;
            caveVelocity = newVelocity;


            if (shouldBreak)
            {
                break;
            }

            if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
            {
                Point wallPoint = cavePosition.ToPoint();
                WorldUtils.Gen(wallPoint, new Shapes.Circle(8, 8), Actions.Chain(new GenAction[]
                {
                    new Actions.PlaceWall(tileToPlace),
                    new Actions.Smooth(true)
                }));
            }

            float tilePercent = VeilGen.TilePercentNoAir(cavePosition.ToPoint(), new Rectangle((int)cavePosition.X, (int)cavePosition.Y, 20, 20), TileID.Dirt, TileID.Stone);
            if (tilePercent < 0.5f && j > caveSteps / 2)
            {
                shouldBreak = true;
            }
            // Update the cave position.
            cavePosition += caveVelocity * caveWidth * 0.5f;
            //  caveStrength *= 0.99f;
        }
    }

    public static void GenerateSimpleCave(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, Vector2 pullDirection, int caveWidth, int caveSteps, int tileToPlace = -1, bool addTile = false)
    {
        var genRand = WorldGen.genRand;

        //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
        float i = cavePosition.X;
        Vector2 caveVelocity = baseCaveDirection;
        Vector2 breakStrength = caveStrength;

        Vector2 startVelocity = caveVelocity;
        Vector2 pullVelocity = pullDirection;

        float sharpness = 1;
        float counter = 0;
        for (int j = 0; j < caveSteps; j++)
        {

            counter++;
            breakStrength *= 0.9995f;
            float degreesToRotate = sharpness;
            float length = caveVelocity.Length();
            float targetAngle = (pullVelocity - startVelocity).ToRotation();
            Vector2 newVelocity = caveVelocity.ToRotation().AngleTowards(targetAngle,
                MathHelper.ToRadians(degreesToRotate)).ToRotationVector2() * length;
            caveVelocity = newVelocity;

            if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
            {
                WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                    genRand.NextFloat(breakStrength.X, breakStrength.Y),
                    genRand.Next(4, 5), tileToPlace, addTile);
            }

            // Update the cave position.
            cavePosition += caveVelocity * caveWidth * 0.5f;
            //  caveStrength *= 0.99f;
        }
    }

    public static void GenerateStraightCaveWall(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, Vector2 pullDirection, int caveWidth, int caveSteps, ushort tileToPlace)
    {
        var genRand = WorldGen.genRand;
        int caveSeed = genRand.Next();

        //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
        float i = cavePosition.X;
        Vector2 caveVelocity = baseCaveDirection;
        Vector2 breakStrength = caveStrength;

        Vector2 startVelocity = caveVelocity;
        Vector2 pullVelocity = pullDirection;
        float counter = 0;
        bool shouldBreak = false;
        for (int j = 0; j < caveSteps; j++)
        {

            counter++;
            breakStrength *= 0.9995f;


            if (shouldBreak)
            {
                break;
            }

            if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
            {
                Point wallPoint = cavePosition.ToPoint();
                WorldUtils.Gen(wallPoint, new Shapes.Circle(8, 8), Actions.Chain(new GenAction[]
                {
                    new Actions.PlaceWall(tileToPlace),
                    new Actions.Smooth(true)
                }));
            }

            float tilePercent = VeilGen.TilePercentNoAir(cavePosition.ToPoint(), new Rectangle((int)cavePosition.X, (int)cavePosition.Y, 20, 20), TileID.Dirt, TileID.Stone);
            if (tilePercent < 0.5f && j > caveSteps / 2)
            {
                shouldBreak = true;
            }
            // Update the cave position.
            cavePosition += caveVelocity * caveWidth * 0.5f;
            //  caveStrength *= 0.99f;
        }
    }

    public static void GenerateStraightCave(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, Vector2 pullDirection, int caveWidth, int caveSteps, int tileToPlace = -1)
    {
        var genRand = WorldGen.genRand;
        int caveSeed = genRand.Next();

        //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
        float i = cavePosition.X;
        Vector2 caveVelocity = baseCaveDirection;
        Vector2 breakStrength = caveStrength;

        Vector2 startVelocity = caveVelocity;
        Vector2 pullVelocity = pullDirection;
        float counter = 0;
        bool shouldBreak = false;
        for (int j = 0; j < caveSteps; j++)
        {

            counter++;
            breakStrength *= 0.9995f;
            float tilePercent = VeilGen.TilePercentNoAir(cavePosition.ToPoint(), new Rectangle((int)cavePosition.X, (int)cavePosition.Y, 20, 20), TileID.Dirt, TileID.Stone);

            if (shouldBreak)
                break;

            if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
            {
                WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                    genRand.NextFloat(breakStrength.X, breakStrength.Y),
                    genRand.Next(4, 5), tileToPlace);
            }

            // Update the cave position.
            cavePosition += caveVelocity * caveWidth * 0.5f;


            if (tilePercent < 0.5f && j > caveSteps / 2)
            {
                shouldBreak = true;
            }
        }
    }

    public static void GenerateSimpleCave(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, Vector2 pullDirection, int caveWidth, int caveSteps, int tileToPlace = -1)
    {
        var genRand = WorldGen.genRand;
        int caveSeed = genRand.Next();

        //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
        float i = cavePosition.X;
        Vector2 caveVelocity = baseCaveDirection;
        Vector2 breakStrength = caveStrength;

        Vector2 startVelocity = caveVelocity;
        Vector2 pullVelocity = pullDirection;

        float sharpness = 1;
        float counter = 0;
        bool shouldBreak = false;
        for (int j = 0; j < caveSteps; j++)
        {

            counter++;
            breakStrength *= 0.9995f;
            float degreesToRotate = sharpness;
            float length = caveVelocity.Length();
            float targetAngle = (pullVelocity - startVelocity).ToRotation();
            Vector2 newVelocity = caveVelocity.ToRotation().AngleTowards(targetAngle,
                MathHelper.ToRadians(degreesToRotate)).ToRotationVector2() * length;
            caveVelocity = newVelocity;

            float tilePercent = VeilGen.TilePercentNoAir(cavePosition.ToPoint(), new Rectangle((int)cavePosition.X, (int)cavePosition.Y, 20, 20), TileID.Dirt, TileID.Stone);

            if (shouldBreak)
                break;

            if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
            {
                WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                    genRand.NextFloat(breakStrength.X, breakStrength.Y),
                    genRand.Next(4, 5), tileToPlace);
            }

            // Update the cave position.
            cavePosition += caveVelocity * caveWidth * 0.5f;


            if (tilePercent < 0.5f && j > caveSteps / 2)
            {
                shouldBreak = true;
            }
        }
    }

    public static void GenerateSimpleCave(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, Vector2 pullDirection, int caveWidth, int caveSteps)
    {
        var genRand = WorldGen.genRand;
        int caveSeed = genRand.Next();

        //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
        float i = cavePosition.X;
        Vector2 caveVelocity = baseCaveDirection;
        Vector2 breakStrength = caveStrength;

        Vector2 startVelocity = caveVelocity;
        Vector2 pullVelocity = pullDirection;

        float sharpness = 1;
        float counter = 0;
        for (int j = 0; j < caveSteps; j++)
        {

            counter++;
            breakStrength *= 0.9995f;
            float degreesToRotate = sharpness;
            float length = caveVelocity.Length();
            float targetAngle = (pullVelocity - startVelocity).ToRotation();
            Vector2 newVelocity = caveVelocity.ToRotation().AngleTowards(targetAngle,
                MathHelper.ToRadians(degreesToRotate)).ToRotationVector2() * length;
            caveVelocity = newVelocity;

            if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
            {
                WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                    genRand.NextFloat(breakStrength.X, breakStrength.Y),
                    genRand.Next(4, 5), -1);
            }

            // Update the cave position.
            cavePosition += caveVelocity * caveWidth * 0.5f;
            //  caveStrength *= 0.99f;
        }
    }
    public static void GenerateSquiggleCave(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, int caveWidth, int caveSteps)
    {
        var genRand = WorldGen.genRand;

    }

    public static void GenerateCavernousCave1(Vector2 caveOrigin, Vector2 caveInitialDirection, int caveWidth, int caveSteps)
    {
        FastNoiseLite fastNoiseLite = new FastNoiseLite();
        fastNoiseLite.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
        float maxRadianOffset = MathHelper.ToRadians(40);
        Vector2 cavePosition = caveOrigin;

        var genRand = WorldGen.genRand;
        Vector2 breakStrength = new Vector2(10, 25);
        float strength = genRand.NextFloat(breakStrength.X, breakStrength.Y);
        float offset = genRand.NextFloat(0, 1000);
        for (int n = 0; n < caveSteps; n++)
        {
            float noiseSample = fastNoiseLite.GetNoise(n * 4f + offset, 0);
            float rotation = noiseSample * maxRadianOffset;
            Vector2 caveDirection = caveInitialDirection.RotatedBy(rotation);
            cavePosition += caveDirection * 4;


            float ratio = n / (float)caveSteps;
            float extraWidth = MathHelper.Lerp(0, caveWidth, EasingFunction.QuadraticBump(ratio));
            if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
            {
                /*
                WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                    strength + genRand.NextFloat(-2f, 2f) + extraWidth,
                    genRand.Next(4, 5), -1);*/
            }

            //      Main.NewText(noiseSample);
        }

        cavePosition = caveOrigin;
        for (int n = 0; n < caveSteps; n++)
        {
            float noiseSample = fastNoiseLite.GetNoise(n * 4f + offset, 0);
            float rotation = noiseSample * maxRadianOffset;
            Vector2 caveDirection = caveInitialDirection.RotatedBy(rotation);
            cavePosition += caveDirection * 3;

            float ratio = n / (float)caveSteps;
            float extraWidth = MathHelper.Lerp(0, caveWidth, EasingFunction.QuadraticBump(ratio));
            if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
            {
                WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                 extraWidth,
                genRand.Next(4, 5), TileID.Stone);
            }
        }
    }

    public static void GenerateLongCurveCave(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, int caveWidth, int caveSteps)
    {
        var genRand = WorldGen.genRand;
        int caveSeed = genRand.Next();

        //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
        float i = cavePosition.X;
        Vector2 caveVelocity = baseCaveDirection;
        Vector2 breakStrength = caveStrength;
        Vector2 pullDirection;
        pullDirection.X = -baseCaveDirection.X;
        pullDirection.Y = 1;

        Vector2 startVelocity = caveVelocity;
        Vector2 pullVelocity = caveVelocity;

        float sharpness = 10;
        float counter = 0;
        float target = genRand.Next(50, 200);
        float direction = 1;
        for (int j = 0; j < caveSteps; j++)
        {

            counter++;
            breakStrength *= 0.9995f;
            float degreesToRotate = sharpness;
            float length = caveVelocity.Length();
            float targetAngle = (pullVelocity - startVelocity).ToRotation();
            Vector2 newVelocity = caveVelocity.ToRotation().AngleTowards(targetAngle,
                MathHelper.ToRadians(degreesToRotate)).ToRotationVector2() * length;
            caveVelocity = newVelocity;


            if (counter > target)
            {
                target = genRand.Next(50, 200);
                float mult = direction % 2 == 0 ? 1 : 0;
                pullVelocity = startVelocity.RotatedBy(MathHelper.ToRadians(-180 * mult));
                direction++;
                counter = 0;
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
                    genRand.NextFloat(breakStrength.X, breakStrength.Y),
                    genRand.Next(4, 5), -1);
            }

            // Update the cave position.
            cavePosition += caveVelocity * caveWidth * 0.5f;
            //  caveStrength *= 0.99f;
        }
    }

    public static void GenerateFishCave(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, int caveWidth, int caveSteps)
    {
        var genRand = WorldGen.genRand;
        int caveSeed = genRand.Next();

        //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
        FastNoiseLite fastNoiseLite = new FastNoiseLite();
        fastNoiseLite.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
        fastNoiseLite.SetSeed(caveSeed);

        float i = cavePosition.X;
        for (int j = 0; j < caveSteps; j++)
        {

            //1. Have Position

            //The default direction

            Vector2 caveDirection = baseCaveDirection;


            //Sample the noise
            float sample = fastNoiseLite.GetNoise(cavePosition.X, j / 50f);
            float caveOffsetAngleAtStep = sample * MathHelper.ToRadians(90);


            //Rotate based on the noise
            caveDirection = caveDirection.RotatedBy(caveOffsetAngleAtStep);

            // Carve out at the current position.
            if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
            {
                //digging 
                ShapeData shapeData = new ShapeData();
                Point point = new Point((int)cavePosition.X, (int)cavePosition.Y);
                WorldUtils.Gen(point, new Shapes.Circle(3, 3), new Actions.ClearTile());

                /*WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                    genRand.NextFloat(caveStrength.X, caveStrength.Y),
                    genRand.Next(4, 5), -1);*/
            }

            // Update the cave position.
            cavePosition += caveDirection * caveWidth * 0.5f;
        }
    }

    public static void GenerateStraightCaves(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength,
        int caveWidth,
        int caveSteps)
    {
        var genRand = WorldGen.genRand;
        int caveSeed = genRand.Next();

        //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
        float i = cavePosition.X;
        Vector2 caveVelocity = baseCaveDirection;
        Vector2 pullDirection = genRand.NextVector2Circular(1, 1);
        Vector2 targetPosition = caveVelocity + pullDirection;
        float sharpness = 1;
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
                pullDirection = genRand.NextVector2Circular(1, 1);
                targetPosition = -targetPosition;
            }

            if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
            {
                /*
                //digging 
                ShapeData shapeData = new ShapeData();
            
                */
                WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                    genRand.NextFloat(caveStrength.X, caveStrength.Y),
                    genRand.Next(4, 5), -1);
            }

            // Update the cave position.
            cavePosition += caveVelocity * caveWidth * 0.5f;
        }
    }

    public static void GenerateHighCaves(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength,
        int caveWidth,
        int caveSteps,
        int clearingDenominator)
    {
        var genRand = WorldGen.genRand;
        int caveSeed = genRand.Next();

        //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
        float i = cavePosition.X;
        Vector2 caveVelocity = baseCaveDirection;
        Vector2 pullDirection = genRand.NextVector2Circular(1, 1);
        Vector2 targetPosition = caveVelocity + pullDirection;
        float sharpness = 9;

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
                pullDirection = genRand.NextVector2Circular(1, 1);
                targetPosition = -targetPosition;
            }

            if (genRand.NextBool(clearingDenominator) && j > caveSteps / 2)
            {
                int clearingCaveWidth = 15;
                int clearingCaveSteps = 500;

                //Cave position in tiles
                Vector2 clearingPosition = new Vector2((int)cavePosition.X, (int)cavePosition.Y);

                //Starting cave direction
                Vector2 clearingCaveDirection = caveVelocity;//.RotatedBy(WorldGen.genRand.NextFloatDirection() * 0.54f);

                //How much the tile runner is gonna carve out
                Vector2 clearingCaveStrength = new Vector2(20, 25);

                VeilGen.GenerateOpenCaveClearing(clearingPosition,
                    clearingCaveDirection,
                    clearingCaveStrength,
                    clearingCaveWidth,
                    clearingCaveSteps);
            }

            if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
            {
                /*
                //digging 
                ShapeData shapeData = new ShapeData();
            
                */
                WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                    genRand.NextFloat(caveStrength.X, caveStrength.Y),
                    genRand.Next(4, 5), -1);

            }

            // Update the cave position.
            cavePosition += caveVelocity * caveWidth * 0.5f;
        }
    }


    public static void GenerateOpenCaveClearing(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, int caveWidth, int caveSteps)
    {
        var genRand = WorldGen.genRand;
        int caveSeed = genRand.Next();

        //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
        FastNoiseLite fastNoiseLite = new FastNoiseLite();
        fastNoiseLite.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
        fastNoiseLite.SetSeed(caveSeed);

        Vector2 caveVelocity = baseCaveDirection;
        Vector2 baseCavePosition = cavePosition;
        for (int j = 0; j < caveSteps; j++)
        {
            if (genRand.NextBool(4))
            {
                caveVelocity = Main.rand.NextVector2Circular(1, 1);
            }
            if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
            {
                //digging 
                // ShapeData shapeData = new ShapeData();
                // Point point = new Point((int)cavePosition.X, (int)cavePosition.Y);
                // WorldUtils.Gen(point, new Shapes.Circle(3, 3), new Actions.ClearTile());

                WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                    genRand.NextFloat(caveStrength.X, caveStrength.Y),
                    genRand.Next(8, 9), -1);
            }

            // Update the cave position.
            cavePosition = baseCavePosition + caveVelocity * caveWidth;
        }
    }

    public static void GenerateLongNoodleCave(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, int caveWidth, int caveSteps)
    {
        var genRand = WorldGen.genRand;
        int caveSeed = genRand.Next();

        //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
        FastNoiseLite fastNoiseLite = new FastNoiseLite();
        fastNoiseLite.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
        fastNoiseLite.SetSeed(caveSeed);

        float i = cavePosition.X;
        Vector2 caveVelocity = baseCaveDirection;
        Vector2 pullDirection = genRand.NextVector2Circular(1, 1);
        Vector2 targetPosition = caveVelocity + pullDirection;
        float sharpness = 9;
        for (int j = 0; j < caveSteps; j++)
        {

            //1. Have Position
            //  caveDirection = Vector2.Lerp(caveDirection, pullDirection, 0.05f);


            //Homing
            float degreesToRotate = sharpness;
            float length = caveVelocity.Length();
            float targetAngle = (targetPosition - caveVelocity).ToRotation();
            Vector2 newVelocity = caveVelocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(degreesToRotate)).ToRotationVector2() * length;
            caveVelocity = newVelocity;


            if (genRand.NextBool(3))
            {
                pullDirection = genRand.NextVector2Circular(1, 1);
                targetPosition = -targetPosition;
            }

            if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
            {
                //digging 
                ShapeData shapeData = new ShapeData();
                Point point = new Point((int)cavePosition.X, (int)cavePosition.Y);
                WorldUtils.Gen(point, new Shapes.Circle(3, 3), new Actions.ClearTile());

                /*WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                    genRand.NextFloat(caveStrength.X, caveStrength.Y),
                    genRand.Next(4, 5), -1);*/
            }

            // Update the cave position.
            cavePosition += caveVelocity * caveWidth * 0.5f;
        }
    }


    public static void GenerateVeinyCaves(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, int caveWidth, int caveSteps)
    {
        var genRand = WorldGen.genRand;
        int caveSeed = genRand.Next();

        //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
        FastNoiseLite fastNoiseLite = new FastNoiseLite();
        fastNoiseLite.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
        fastNoiseLite.SetSeed(caveSeed);

        for (int j = 0; j < caveSteps; j++)
        {
            float divisor = 2f;
            float sample = fastNoiseLite.GetNoise(cavePosition.X / divisor, cavePosition.Y / divisor);
            float caveOffsetAngleAtStep = sample * MathHelper.TwoPi * 1.9f;
            Vector2 caveDirection = baseCaveDirection.RotatedBy(caveOffsetAngleAtStep);

            // Carve out at the current position.
            if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
            {
                //digging 
                WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y, MathF.Sin(j * 0.05f) * 10 +
                    genRand.NextFloat(2, 5),
                    genRand.Next(5, 10), -1);
            }

            // Update the cave position.
            cavePosition += caveDirection * caveWidth * 0.5f;
        }
    }
    public static void GenerateLinearCave(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, int caveWidth, int caveSteps)
    {
        var genRand = WorldGen.genRand;
        int caveSeed = genRand.Next();

        //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
        FastNoiseLite fastNoiseLite = new FastNoiseLite();
        fastNoiseLite.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
        fastNoiseLite.SetSeed(caveSeed);

        for (int j = 0; j < caveSteps; j++)
        {
            float divisor = 50f;
            float sample = fastNoiseLite.GetNoise(cavePosition.X / divisor, cavePosition.Y / divisor);
            float caveOffsetAngleAtStep = sample * MathHelper.TwoPi * 1.9f;
            Vector2 caveDirection = baseCaveDirection.RotatedBy(caveOffsetAngleAtStep);

            // Carve out at the current position.
            if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
            {
                //digging 
                WorldGen.TileRunner(
                    (int)cavePosition.X,
                    (int)cavePosition.Y,
                    genRand.NextFloat(caveStrength.X, caveStrength.Y),
                    genRand.Next((int)caveStrength.X, (int)caveStrength.Y),
                    type: -1);
            }

            // Update the cave position.
            cavePosition += caveDirection * caveWidth * 0.5f;
        }
    }
    public static float TilePercentNoAir(Point tilePoint, Rectangle size, params ushort[] tileIDs)
    {
        int count = 0;
        int width = size.Width;
        int height = size.Height;
        for (int x = tilePoint.X; x < tilePoint.X + width; x++)
        {
            if (x < 0)
                continue;
            if (x >= Main.maxTilesX)
                continue;

            for (int y = tilePoint.Y; y > tilePoint.Y - height; y--)
            {

                if (y < 0)
                    continue;
                if (y >= Main.maxTilesY)
                    continue;

                Tile tile = Main.tile[x, y];
                for (int t = 0; t < tileIDs.Length; t++)
                {
                    int tileID = tileIDs[t];
                    if (tile.HasTile)
                    {
                        count++;
                    }
                }
            }
        }

        int tileM = width * height;
        float tilePercent = count / (float)tileM;
        return tilePercent;
    }

    public static float TilePercent(Point tilePoint, Rectangle size, params ushort[] tileIDs)
    {
        int count = 0;
        int width = size.Width;
        int height = size.Height;
        for (int x = tilePoint.X; x < tilePoint.X + width; x++)
        {
            if (x < 0)
                continue;
            if (x >= Main.maxTilesX)
                continue;

            for (int y = tilePoint.Y; y > tilePoint.Y - height; y--)
            {

                if (y < 0)
                    continue;
                if (y >= Main.maxTilesY)
                    continue;

                Tile tile = Main.tile[x, y];
                for (int t = 0; t < tileIDs.Length; t++)
                {
                    int tileID = tileIDs[t];
                    if (!WorldGen.SolidTile(x, y))
                    {
                        count++;
                    }

                    if (tile.HasTile && tile.TileType == tileID)
                    {
                        count++;
                    }
                }
            }
        }

        int tileM = width * height;
        float tilePercent = count / (float)tileM;
        return tilePercent;
    }

    public static void GenerateWiggleCave(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, int caveWidth, int caveSteps)
    {
        var genRand = WorldGen.genRand;
        int caveSeed = genRand.Next();

        //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
        FastNoiseLite fastNoiseLite = new FastNoiseLite();
        fastNoiseLite.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
        fastNoiseLite.SetSeed(caveSeed);

        for (int j = 0; j < caveSteps; j++)
        {
            float divisor = 2f;
            float sample = fastNoiseLite.GetNoise(cavePosition.X / divisor, cavePosition.Y / divisor);
            sample = MathF.Sin(sample * 8);
            float caveOffsetAngleAtStep = sample * MathHelper.TwoPi * 1.9f;
            Vector2 caveDirection = baseCaveDirection.RotatedBy(caveOffsetAngleAtStep);

            // Carve out at the current position.
            if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15 && sample > 0)
            {
                //digging 
                WorldGen.TileRunner(
                    (int)cavePosition.X,
                    (int)cavePosition.Y,
                    strength: genRand.NextFloat(caveStrength.X, caveStrength.Y),
                    genRand.Next(5, 10),
                    type: -1);
            }

            // Update the cave position.
            cavePosition += caveDirection * caveWidth * 0.5f;
        }
    }

    public static void GenerateNoodleCave(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, int caveWidth, int caveSteps)
    {
        var genRand = WorldGen.genRand;
        int caveSeed = genRand.Next();

        //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
        FastNoiseLite fastNoiseLite = new FastNoiseLite();
        fastNoiseLite.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
        fastNoiseLite.SetSeed(caveSeed);

        for (int j = 0; j < caveSteps; j++)
        {
            float divisor = 2f;
            float sample = fastNoiseLite.GetNoise(cavePosition.X / divisor, cavePosition.Y / divisor);
            sample = MathF.Sin(sample * 4);
            float caveOffsetAngleAtStep = sample * MathHelper.TwoPi * 1.9f;
            Vector2 caveDirection = baseCaveDirection.RotatedBy(caveOffsetAngleAtStep);

            // Carve out at the current position.
            if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15 && sample > 0)
            {
                //digging 
                WorldGen.TileRunner(
                    (int)cavePosition.X,
                    (int)cavePosition.Y,
                    strength: genRand.NextFloat(caveStrength.X, caveStrength.Y),
                    genRand.Next(5, 10),
                    type: -1);
            }

            // Update the cave position.
            cavePosition += caveDirection * caveWidth * 0.5f;
        }
    }

    public static void GenerateWormCave(Vector2 cavePosition,
        Vector2 baseCaveDirection, Vector2 caveStrength, int caveWidth, int caveSteps)
    {
        var genRand = WorldGen.genRand;
        int caveSeed = genRand.Next();

        //Why make my own noise functions when I can just use this?!?!?1 Hhahahaha
        FastNoiseLite fastNoiseLite = new FastNoiseLite();
        fastNoiseLite.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
        fastNoiseLite.SetSeed(caveSeed);

        //Vector2 baseCaveDirection = Vector2.UnitY.RotatedBy(WorldGen.genRand.NextFloatDirection() * 0.54f);
        //Vector2 cavePosition = new Vector2(Main.maxTilesX / 2, (int)Main.worldSurface);

        for (int j = 0; j < caveSteps; j++)
        {
            float divisor = 1f;
            float sample = fastNoiseLite.GetNoise(cavePosition.X / divisor, cavePosition.Y / divisor);

            float angleOffset = sample * MathHelper.Pi;
            Vector2 caveDirection = baseCaveDirection.RotatedBy(angleOffset);

            // Carve out at the current position.
            if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15 && sample > 0f)
            {
                //digging 
                WorldGen.TileRunner(
                    (int)cavePosition.X,
                    (int)cavePosition.Y,
                    strength: genRand.NextFloat(caveStrength.X, caveStrength.Y),
                    genRand.Next(5, 10),
                    type: -1);
            }

            // Update the cave position.
            cavePosition += caveDirection * caveWidth * 0.5f;
        }
    }

    public override void NetSend(BinaryWriter writer)
    {
        base.NetSend(writer);
        writer.Write(MarshLocation.X);
        writer.Write(MarshLocation.Y);
        writer.Write(CoralwaysLocation.X);
        writer.Write(CoralwaysLocation.Y);
    }
    public override void NetReceive(BinaryReader reader)
    {
        base.NetReceive(reader);
        Point marshLocation = new();
        marshLocation.X = reader.ReadInt32();
        marshLocation.Y = reader.ReadInt32();
        MarshLocation = marshLocation;

        Point coralwaysLocation = new();
        coralwaysLocation.X = reader.ReadInt32();
        coralwaysLocation.Y = reader.ReadInt32();
        CoralwaysLocation = coralwaysLocation;
    }

    public override void SaveWorldData(TagCompound tag)
    {
        tag["MarshLocation"] = MarshLocation;
        tag["FableHillLocation"] = FableHillStartLocation;
        tag["CoralwaysLocation"] = CoralwaysLocation;
        tag["CindersparkStart"] = CindersparkStart;
        tag["CindersparkEnd"] = CindersparkEnd;
        tag["DarkspaceStart"] = DarkspaceStart;
        tag["DarkspaceEnd"] = DarkspaceEnd;
        tag["HeatedDepthsStart"] = HeatedDepthsStart;
        tag["HeatedDepthsEnd"] = HeatedDepthsEnd;
    }

    public override void LoadWorldData(TagCompound tag)
    {
        MarshLocation = tag.Get<Point>("MarshLocation");
        FableHillStartLocation = tag.Get<Point>("FableHillLocation");
        CoralwaysLocation = tag.Get<Point>("CoralwaysLocation");
        CindersparkStart = tag.Get<int>("CindersparkStart");
        CindersparkEnd = tag.Get<int>("CindersparkEnd");
        DarkspaceStart = tag.Get<int>("DarkspaceStart");
        DarkspaceEnd = tag.Get<int>("DarkspaceEnd");
        HeatedDepthsStart = tag.Get<int>("HeatedDepthsStart");
        HeatedDepthsEnd = tag.Get<int>("HeatedDepthsEnd");
    }
}