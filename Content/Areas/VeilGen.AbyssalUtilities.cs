using Stellamod.Content.Areas.Tundra.Abyss.EnemiesAB;
using Stellamod.Content.Areas.Tundra.Abyss.TilesAB;
using Stellamod.Core.ZTileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.WorldBuilding;

namespace Stellamod.Content.Areas;

public partial class VeilGen
{
    /// <summary>
    /// Returns the number of tiles that have any liquid within a given area
    /// </summary>
    /// <param name="tileBounds"></param>
    /// <returns></returns>
    public static int CountLiquids(Rectangle tileBounds)
    {
        int count = 0;
        for (int x = tileBounds.Left; x <= tileBounds.Right; x++)
        {
            for (int y = tileBounds.Top; y <= tileBounds.Bottom; y++)
            {
                Tile tile = Main.tile[x, y];
                if (tile.LiquidAmount > 0)
                    count++;
            }
        }
        return count;
    }


    /// <summary>
    /// Returns the percentage of tiles that are filled with any liquid in a given area
    /// </summary>
    /// <param name="tileBounds"></param>
    /// <returns></returns>
    public static float CountLiquidsPercent(Rectangle tileBounds)
    {
        int liquidCount = CountLiquids(tileBounds);
        int maxLiquidCount = tileBounds.Width * tileBounds.Height;


        float pct = (float)liquidCount / (float)maxLiquidCount;
        return pct;
    }

    /// <summary>
    /// Creates half blocks on the edges of water ponds within the given area
    /// </summary>
    /// <param name="tileBounds"></param>
    public static void CreateHalfBlocksOnEdges(Rectangle tileBounds)
    {
        for(int x = tileBounds.Left; x <= tileBounds.Right; x++)
        {
            for(int y = tileBounds.Top; y <= tileBounds.Bottom; y++)
            {
                Tile tile = Main.tile[x, y];
                Tile tileRight = Main.tile[x + 1, y];
                Tile tileAbove = Main.tile[x, y - 1];
                Tile tileLeft = Main.tile[x - 1, y];

                //Right Half Block
                if(tile.HasTile &&
                    tileLeft.LiquidAmount > 0 &&
                    !tileAbove.HasTile)
                {
                    tile.IsHalfBlock = true;
                }

                //Left Half block
                if(tile.HasTile &&
                    tileRight.LiquidAmount > 0 &&
                    !tileAbove.HasTile)
                {
                    tile.IsHalfBlock = true;
                }
            }
        }
    }

    public static void CreateBellFlowerClearing(Vector2 pointToPlaceOn)
    {
        int tileRadius = 30;
        Point tilePoint = pointToPlaceOn.ToPoint();
        ushort abyssDirtTile = (ushort)ModContent.TileType<AbyssalDirt>();


        Rectangle originalBounds = TileUtilities.CenterTileRectangle(tilePoint, tileRadius * 2, tileRadius * 2);
        for (int x = originalBounds.Left; x < originalBounds.Right; x++)
        {
            for (int y = originalBounds.Top; y < originalBounds.Bottom; y++)
            {
                Tile tile = Main.tile[x, y];
                tile.HasTile = true;
                tile.TileType = abyssDirtTile;
                tile.LiquidAmount = 0;
                tile.TileFrameX = -1;
                tile.TileFrameY = -1;
            }
        }

        ClearCircle(tilePoint, tileRadius);


        int islandRadius = 16;
        Point islandPoint = tilePoint;
        islandPoint.Y -= 8;
        WorldUtils.Gen(islandPoint, new Shapes.Circle(islandRadius),
            Actions.Chain(new Actions.SetTile(abyssDirtTile, false, false)));
        WorldUtils.Gen(islandPoint, new Shapes.HalfCircle(islandRadius),
            Actions.Chain(new Actions.ClearTile()));

        int pillarLeft = islandPoint.X - 6;
        int pillarRight = islandPoint.X + 6;
        int pillarTop = islandPoint.Y + 8;
        int pillarBottom = tilePoint.Y + tileRadius + 2;

        for (int x = pillarLeft; x <= pillarRight; x++)
        {
            for (int y = pillarTop; y < pillarBottom; y++)
            {
                Tile tile = Main.tile[x, y];
                tile.HasTile = true;
                tile.TileType = abyssDirtTile;
                tile.LiquidAmount = 0;
                tile.TileFrameX = -1;
                tile.TileFrameY = -1;
            }
        }
        tileRadius += 6;
        int left = tilePoint.X - tileRadius / 2;
        int right = tilePoint.X + tileRadius / 2;
        int top = tilePoint.Y - tileRadius / 2;
        int bottom = tilePoint.Y + tileRadius / 2;

        for(int x = left; x < right; x++)
        {
            for(int y = top; y < bottom; y++)
            {
                Tile tile = Main.tile[x, y];
                if (tile.HasTile)
                    continue;
                tile.LiquidAmount = 255;
                tile.LiquidType = LiquidID.Water;
            }
        }

        Rectangle bounds = TileUtilities.CenterTileRectangle(tilePoint, tileRadius * 2, tileRadius * 2);
        var targetTileTypes = new List<int> { abyssDirtTile };
        VeilGen.DecorateEdgeTilesWithWalls(bounds, targetTileTypes, 2,
            (ushort)ModContent.WallType<AbyssalGrassWallDark>(), 7);
        VeilGen.DecorateEdgeTilesWithWalls(bounds, targetTileTypes, 2,
            (ushort)ModContent.WallType<AbyssalDirtWall>(), 7);
        VeilGen.DecorateWallEdgesWithWallPatches(bounds, new List<int>
        { 
            ModContent.WallType<AbyssalDirtWall>(),
            ModContent.WallType<AbyssalGrassWallDark>() 
        }, 32, (ushort)ModContent.WallType<AbyssalGrassWall>());
        BellFlowerSystem.CreateBellFlower(islandPoint + new Point(0, -5));
    }

    public static void ClearCircle(Point tilePoint, int tileRadius)
    {
        WorldUtils.Gen(tilePoint, new Shapes.Circle(tileRadius, tileRadius),
           Actions.Chain(new Actions.ClearTile()));
        /*
        for (int x = tilePoint.X - tileRadius; x <= tilePoint.X + tileRadius; x++)
        {
            for (int y = tilePoint.Y - tileRadius; y <= tilePoint.Y + tileRadius; y++)
            {
                Point point = new Point(x, y);
                int dx = Math.Abs(point.X - tilePoint.X);
                int dy = Math.Abs(point.Y - tilePoint.Y);
                int diff = dx + dy;
                if (diff > tileRadius)
                    continue;
                Tile tile = Main.tile[point];
                tile.ClearTile();
            }
        }*/
    }
    public static void GrowKelpArea<KelpTile>(Rectangle tileBounds, int minHeight, int maxHeight, int denom)
        where KelpTile : ModTile
    {
        var genRand = WorldGen.genRand;
        for (int x = tileBounds.Left; x < tileBounds.Right; x++)
        {
            for(int y= tileBounds.Top; y < tileBounds.Bottom; y++)
            {
                Tile tileBelow = Main.tile[x, y + 1];
                Tile tile = Main.tile[x, y];
                if (!tileBelow.HasTile)
                    continue;
                if (tile.LiquidAmount <= 0)
                    continue;
                if (!genRand.NextBool(denom))
                    continue;
                GrowKelp<KelpTile>(x, y, minHeight, maxHeight);
            }
        }
    }
    /// <summary>
    /// Places a line of tiles
    /// </summary>
    /// <typeparam name="KelpTile"></typeparam>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="minHeight"></param>
    /// <param name="maxHeight"></param>
    public static void GrowKelp<KelpTile>(int x, int y, int minHeight, int maxHeight) 
        where KelpTile : ModTile
    {
        var genRand = WorldGen.genRand;
        int height = genRand.Next(minHeight, maxHeight);
        int endHeight = y - height;
        int startHeight = y;
        for(int j = endHeight; j <= startHeight; j++)
        {
            Tile tile = Main.tile[x, j];
            if (tile.HasTile)
                break;
            WorldGen.PlaceTile(x, j, ModContent.TileType<KelpTile>());
        }
    }

    public static void CreateAbyssConnectionCave(Vector2 start, Vector2 end)
    {
        var genRand = WorldGen.genRand;
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
    public static void CreateAbyssConnectionCaveMini(Vector2 start, Vector2 end)
    {
        var genRand = WorldGen.genRand;
        float strength = genRand.NextFloat(6, 9);
        float steps = Vector2.Distance(start, end) / 2f;
        for (float f = 0; f < steps; f++)
        {
            float lerp = f / steps;
            Vector2 pos = Vector2.Lerp(start, end, lerp);
            WorldGen.TileRunner((int)pos.X, (int)pos.Y,
                 strength: strength,
                 genRand.Next(5, 12), -1);
        }
    }
    public static void DecorateSurfaceEdgesWithZTile(in EdgeDecorationParameters parameters)
    {
        int left = parameters.tileBounds.Left;
        int right = parameters.tileBounds.Right;
        int top = parameters.tileBounds.Top;
        int bottom = parameters.tileBounds.Bottom;
        var genRand = WorldGen.genRand;
        ZTileMap zTileMap = ModContent.GetInstance<ZTileMap>();
        ZTileLoader zTileLoader = ModContent.GetInstance<ZTileLoader>();
        for (int x = left; x < right; x++)
        {
            for (int y = top; y < bottom; y++)
            {
                Tile tile = Main.tile[x, y];
                Tile tileAbove = Main.tile[x, y - 1];
                if (tileAbove.LiquidAmount > 0)
                    continue;
                if (!tile.HasTile)
                    continue;
                if (!parameters.targetTileTypes.Contains(tile.TileType))
                    continue;

                bool hasTop = (y + 1 < Main.maxTilesY) && !WorldGen.SolidOrSlopedTile(x, y + 1);
                bool hasBottom = (y - 1 > 0) && !WorldGen.SolidOrSlopedTile(x, y - 1);
                if (!hasTop && hasBottom)
                {
                    if (genRand.NextBool(parameters.denom))
                    {
                        var zTileType = parameters.zTileTypes.NextElement(genRand);

                        ZTileInstanceData instanceData = zTileLoader.InstanceTileData(zTileLoader.GetTile(zTileType));
                        instanceData.frameNumber = (ushort)genRand.Next(0, zTileLoader.GetTile(zTileType).frameCount);
                        instanceData.value = parameters.value;
                        Vector2 worldPos = new Point(x, y).ToWorldCoordinates();
                        zTileMap.CreateTile(
                            parameters.renderLayer,
                            worldPos,
                            parameters.zLayer,
                            instanceData);
                    }
                }
            }
        }
    }
    public static void DecorateWetAreasWithZTile(in EdgeDecorationParameters parameters)
    {
        int left = parameters.tileBounds.Left;
        int right = parameters.tileBounds.Right;
        int top = parameters.tileBounds.Top;
        int bottom = parameters.tileBounds.Bottom;
        var genRand = WorldGen.genRand;
        ZTileMap zTileMap = ModContent.GetInstance<ZTileMap>();
        ZTileLoader zTileLoader = ModContent.GetInstance<ZTileLoader>();
        for (int x = left; x < right; x++)
        {
            for (int y = top; y < bottom; y++)
            {
                Tile tile = Main.tile[x, y];
                Tile tileAbove = Main.tile[x, y - 1];
                if (!tile.HasTile)
                    continue;
                if (tileAbove.LiquidAmount <= 0)
                    continue;
                if (!parameters.targetTileTypes.Contains(tile.TileType))
                    continue;

                bool hasTop = (y + 1 < Main.maxTilesY) && !WorldGen.SolidOrSlopedTile(x, y + 1);
                bool hasBottom = (y - 1 > 0) && !WorldGen.SolidOrSlopedTile(x, y - 1);
                if (!hasTop && hasBottom)
                {
                    if (genRand.NextBool(parameters.denom))
                    {
                        var zTileType = parameters.zTileTypes.NextElement(genRand);

                        ZTileInstanceData instanceData = zTileLoader.InstanceTileData(zTileLoader.GetTile(zTileType));
                        instanceData.frameNumber = (ushort)genRand.Next(0, zTileLoader.GetTile(zTileType).frameCount);
                        instanceData.value = parameters.value;
                        Vector2 worldPos = new Point(x, y).ToWorldCoordinates();
                        zTileMap.CreateTile(
                            parameters.renderLayer,
                            worldPos,
                            parameters.zLayer,
                            instanceData);
                    }
                }
            }
        }
    }
    public static void DecorateEdgesWithZTile(in EdgeDecorationParameters parameters)
    {
        int left = parameters.tileBounds.Left;
        int right = parameters.tileBounds.Right;
        int top = parameters.tileBounds.Top;
        int bottom = parameters.tileBounds.Bottom;
        var genRand = WorldGen.genRand;
        ZTileMap zTileMap = ModContent.GetInstance<ZTileMap>();
        ZTileLoader zTileLoader = ModContent.GetInstance<ZTileLoader>();
        for (int x = left; x < right; x++)
        {
            for (int y = top; y < bottom; y++)
            {
                Tile tile = Main.tile[x, y];
                if (!tile.HasTile)
                    continue;
                if (!parameters.targetTileTypes.Contains(tile.TileType))
                    continue;

                bool hasRight = (x + 1 < Main.maxTilesX) && !WorldGen.SolidOrSlopedTile(x + 1, y);
                bool hasLeft = (x - 1 > 0) && !WorldGen.SolidOrSlopedTile(x - 1, y);
                bool hasTop = (y + 1 < Main.maxTilesY) && !WorldGen.SolidOrSlopedTile(x, y + 1);
                bool hasBottom = (y - 1 > 0) && !WorldGen.SolidOrSlopedTile(x, y - 1);
                bool hasAny = hasRight || hasLeft || hasTop || hasBottom;
                if (hasAny)
                {
                    if (genRand.NextBool(parameters.denom))
                    {
                        var zTileType = parameters.zTileTypes.NextElement(genRand);

                        ZTileInstanceData instanceData = zTileLoader.InstanceTileData(zTileLoader.GetTile(zTileType));
                        Vector2 worldPos = new Point(x, y).ToWorldCoordinates();
                        zTileMap.CreateTile(
                            parameters.renderLayer,
                            worldPos,
                            parameters.zLayer,
                            instanceData);
                    }
                }
            }
        }
    }

    public static void ClearWallsArea(Rectangle tileBounds)
    {
        int left = tileBounds.Left;
        int right = tileBounds.Right;
        int top = tileBounds.Top;
        int bottom = tileBounds.Bottom;
        for (int x = left; x < right; x++)
        {
            for (int y = top; y < bottom; y++)
            {
                Tile tile = Main.tile[x, y];
                tile.WallType = WallID.None;
            
            }
        }
    }
    public static void DecorateEdgeTilesWithWalls(Rectangle tileBounds, List<int> targetTileTypes, ushort wallType, int maxWallCaveWidth = 2)
    {
        int left = tileBounds.Left;
        int right = tileBounds.Right;
        int top = tileBounds.Top;
        int bottom = tileBounds.Bottom;
        var genRand = WorldGen.genRand;
        int wallCaveWidth = maxWallCaveWidth;
        Vector2 baseDirection = -Vector2.UnitY;

        //Here we're placing walls and silk tiles, this is a bit slow, so maybe optimize it a bit later.
        for (int x = left; x < right; x++)
        {
            for (int y = top; y < bottom; y++)
            {
                Tile tile = Main.tile[x, y];
                if (!tile.HasTile)
                    continue;
                if (!targetTileTypes.Contains(tile.TileType))
                    continue;
                bool hasRight = (x + 1 < Main.maxTilesX) && !WorldGen.SolidOrSlopedTile(x + 1, y);
                bool hasLeft = (x - 1 > 0) && !WorldGen.SolidOrSlopedTile(x - 1, y);
                bool hasTop = (y + 1 < Main.maxTilesY) && !WorldGen.SolidOrSlopedTile(x, y + 1);
                bool hasBottom = (y - 1 > 0) && !WorldGen.SolidOrSlopedTile(x, y - 1);
                bool hasAny = hasRight || hasLeft || hasTop || hasBottom;
                if (hasAny)
                {
                    //WorldGen.PlaceTile(x, y, TileID.Grass, forced: true);
                    Point point = new Point(x, y);
                    int steps = genRand.Next(0, 2);


                    for (int s = 0; s < steps; s++)
                    {
                        if (point.X - wallCaveWidth > 0 && point.X + wallCaveWidth < Main.maxTilesX
                            && point.Y + wallCaveWidth < Main.maxTilesY && point.Y - wallCaveWidth > 0)
                        {
                            WorldUtils.Gen(point, new Shapes.Circle(wallCaveWidth, wallCaveWidth),
                                new Actions.PlaceWall(wallType));
                        }

                        point += (baseDirection * wallCaveWidth).RotatedByRandom(MathHelper.ToRadians(30)).ToPoint();
                    }
                }
            }
        }
    }
    public static void DecorateEdgeTilesWithWalls(Rectangle tileBounds, List<int> targetTileTypes, int steps, ushort wallType, int maxWallCaveWidth = 2)
    {
        int left = tileBounds.Left;
        int right = tileBounds.Right;
        int top = tileBounds.Top;
        int bottom = tileBounds.Bottom;
        var genRand = WorldGen.genRand;
        int wallCaveWidth = maxWallCaveWidth;
        Vector2 baseDirection = -Vector2.UnitY;
        //Here we're placing walls and silk tiles, this is a bit slow, so maybe optimize it a bit later.
        for (int x = left; x < right; x++)
        {
            for (int y = top; y < bottom; y++)
            {
                Tile tile = Main.tile[x, y];
                if (!tile.HasTile)
                    continue;
                if (!targetTileTypes.Contains(tile.TileType))
                    continue;
                bool hasRight = (x + 1 < Main.maxTilesX) && !WorldGen.SolidOrSlopedTile(x + 1, y);
                bool hasLeft = (x - 1 > 0) && !WorldGen.SolidOrSlopedTile(x - 1, y);
                bool hasTop = (y + 1 < Main.maxTilesY) && !WorldGen.SolidOrSlopedTile(x, y + 1);
                bool hasBottom = (y - 1 > 0) && !WorldGen.SolidOrSlopedTile(x, y - 1);
                bool hasAny = hasRight || hasLeft || hasTop || hasBottom;
                if (hasAny)
                {
                    //WorldGen.PlaceTile(x, y, TileID.Grass, forced: true);
                    Point point = new Point(x, y);

                    for (int s = 0; s < steps; s++)
                    {
                        if (point.X - wallCaveWidth > 0 && point.X + wallCaveWidth < Main.maxTilesX
                            && point.Y + wallCaveWidth < Main.maxTilesY && point.Y - wallCaveWidth > 0)
                        {
                            WorldUtils.Gen(point, new Shapes.Circle(1, 1),
                                new Actions.PlaceWall(wallType));
                        }

                        point += (baseDirection * 4).RotatedByRandom(MathHelper.ToRadians(360)).ToPoint();
                    }
                }
            }
        }
    }
    public static void DecorateWallEdgesWithWallPatches(Rectangle tileBounds, List<int> targetTileTypes, int steps, ushort wallType, int maxWallCaveWidth = 2)
    {
        int left = tileBounds.Left;
        int right = tileBounds.Right;
        int top = tileBounds.Top;
        int bottom = tileBounds.Bottom;
        var genRand = WorldGen.genRand;
        int wallCaveWidth = maxWallCaveWidth;
        Vector2 baseDirection = -Vector2.UnitY;

        //Here we're placing walls and silk tiles, this is a bit slow, so maybe optimize it a bit later.
        for (int x = left; x < right; x++)
        {
            for (int y = top; y < bottom; y++)
            {
                Tile tile = Main.tile[x, y];
                if (!targetTileTypes.Contains(tile.WallType))
                    continue;
                Tile tileLeft = Main.tile[x - 1, y];
                Tile tileRight = Main.tile[x + 1, y];
                Tile tileBottom = Main.tile[x, y + 1];
                Tile tileTop = Main.tile[x, y - 1];
                bool hasAny = 
                    tileLeft.WallType == WallID.None ||
                    tileRight.WallType == WallID.None || 
                    tileTop.WallType == WallID.None || 
                    tileBottom.WallType == WallID.None;
                
                if (hasAny && genRand.NextBool(16))
                {
                    //WorldGen.PlaceTile(x, y, TileID.Grass, forced: true);
      
                    Vector2 worldPos = new Vector2(x, y).ToWorldCoordinates();
                    for (int s = 0; s < steps; s++)
                    {
                        Point tilePoint = worldPos.ToTileCoordinates();
                        Tile placeTile = Main.tile[tilePoint];
                        placeTile.WallType = wallType;
                        placeTile.WallFrameX = -1;
                        placeTile.WallFrameY = -1;
                     
                        worldPos += (baseDirection * 8).RotatedByRandom(MathHelper.ToRadians(360));
                    }
                }
            }
        }
    }
    public static void DecorateSurfaceEdgesWithMultiTile(Rectangle tileBounds, int denom, List<int> targetGroundTileTypes, params int[] tileTypes)
    {
        int left = tileBounds.Left;
        int right = tileBounds.Right;
        int top = tileBounds.Top;
        int bottom = tileBounds.Bottom;
        var genRand = WorldGen.genRand;

        for (int x = left; x < right; x++)
        {
            for (int y = top; y < bottom; y++)
            {
                Tile tileBelow = Main.tile[x, y + 1];
                if (!tileBelow.HasTile)
                    continue;
                if (!targetGroundTileTypes.Contains(tileBelow.TileType))
                    continue;
                if (genRand.NextBool(denom))
                    WorldGen.PlaceObject(x, y, tileTypes.NextElement(genRand));
            }
        }
    }

    public static void GenerateWaterBlobs(Rectangle area, float numWaterBlocks, Point squareRange)
    {
        var genRand = WorldGen.genRand;
        List<Point> validPoints = new();
        for (int x = area.Left; x < area.Right; x++)
        {
            for (int y = area.Top; y < area.Bottom; y++)
            {
                Tile tile = Main.tile[x, y];
                if (tile.HasTile)
                    continue;
                int sy = y;
                int sx = x;
                while (!tile.HasTile && sy < Main.UnderworldLayer)
                {
                    sy++;
                    tile = Main.tile[sx, sy];
                }
                validPoints.Add(new Point(sx, sy));
            }
        }
        if (validPoints.Count <= 0)
            return;

        for (float f = 0; f < numWaterBlocks; f++)
        {
            //Reset the seed for each cave
            Point p = validPoints.NextElement(genRand);
            //Dimensions of the lava bowl
            int width = genRand.Next(squareRange.X, squareRange.Y);
            int left = p.X - width / 2;
            int right = p.X + width / 2;
            int top = p.Y - width / 2;
            int bottom = p.Y + width / 2;
            for (int x = left; x < right; x++)
            {
                for (int y = top; y < bottom; y++)
                {
                    Tile tile = Main.tile[x, y];
                    if (tile.HasTile)
                        continue;
                    //Fall down until hitting a solid tile or another water source
                    int newY = TileUtilities.FallToSolidOrWaterTile(x, y, maxSteps: 255);

                    //We're placing at 1 tile above the actual thing
                    newY--;
                    tile = Main.tile[x, newY];
                    tile.LiquidAmount = 255;
                    tile.LiquidType = LiquidID.Water;
                }
            }
        }
    }
    public static void GenerateWaterBowls(Rectangle area, float numLavaBowls, Point widthRange, Point depthRange)
    {
        var genRand = WorldGen.genRand;
        List<Point> validPoints = new();
        for (int x = area.Left; x < area.Right; x++)
        {
            for (int y = area.Top; y < area.Bottom; y++)
            {
                Tile tile = Main.tile[x, y];
                if (tile.HasTile)
                    continue;
                int sy = y;
                int sx = x;
                while (!tile.HasTile && sy < Main.UnderworldLayer)
                {
                    sy++;
                    tile = Main.tile[sx, sy];
                }
                validPoints.Add(new Point(sx, sy));
            }
        }
        if (validPoints.Count <= 0)
            return;

        for (float f = 0; f < numLavaBowls; f++)
        {
            //Reset the seed for each cave
            Point p = validPoints.NextElement(genRand);
            Tile startTile = Main.tile[p.X, p.Y];

            //Dimensions of the lava bowl
            int width = genRand.Next(widthRange.X, widthRange.Y);
            int depth = genRand.Next(depthRange.X, depthRange.Y);
            int left = p.X - width / 2;
            int right = p.X + width / 2;
            for (int x = left; x < right; x++)
            {
                float numSteps = right - left;
                int d = (int)MathHelper.Lerp(0, depth, EasingFunction.QuadraticBump((x - left) / numSteps));
                for (int y = p.Y; y < p.Y + d; y++)
                {
                    Tile tile = Main.tile[x, y];
                    tile.ClearTile();
                    tile.LiquidAmount = 255;
                    tile.LiquidType = LiquidID.Water;
                }
            }
        }
    }
    public static void GenerateLavaBowls(Rectangle area, float numLavaBowls, Point widthRange, Point depthRange)
    {
        var genRand = WorldGen.genRand;
        for (float f = 0; f < numLavaBowls; f++)
        {
            //Reset the seed for each cave
            int sx = genRand.Next(area.Left, area.Right);
            int sy = genRand.Next(area.Top, area.Bottom);
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
            int width = genRand.Next(widthRange.X, widthRange.Y);
            int depth = genRand.Next(depthRange.X, depthRange.Y);
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
    }
}
