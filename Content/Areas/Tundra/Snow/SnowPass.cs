using ReLogic.Utilities;
using Stellamod.Content.Areas.Tundra.Abyss.TilesAB;
using Stellamod.WorldG;
using System;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Stellamod.Content.Areas.Tundra.Snow;

public class TreasureTrovePass : GenPass
{
    public TreasureTrovePass() : base("Treaure Trove", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Treasure Trove";
        Point caveOrigin = ModContent.GetInstance<VeilGen>().AbyssCenter;

        caveOrigin.Y -= 800;
        GenerationPrefab prefab = ModContent.GetInstance<GenerationTextureManager>().GetPrefab("TreasureTrove");
        Rectangle bounds = prefab.GetBounds(caveOrigin.X, caveOrigin.Y, PrefabPlacementType.FromTopCenter);

        //Fill up area with random tiles fr
        for (int x = bounds.Left; x < bounds.Right; x++)
        {
            for (int y = bounds.Top; y < bounds.Bottom; y++)
            {
                if (!Main.rand.NextBool(16))
                    continue;
                int tileToPlace = TileID.SnowBlock;
                WorldGen.TileRunner(x, y, 16, 32, tileToPlace, addTile: true, 1, 1);
            }
        }

        prefab.PasteErase(caveOrigin, PrefabPlacementType.FromTopCenter);
    }
}

public class ReworkedVanillaIceBiomePass : GenPass
{
    public ReworkedVanillaIceBiomePass() : base("Generate Ice Biome", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        var genRand = WorldGen.genRand;
        progress.Message = Lang.gen[56].Value;
        GenVars.snowTop = (int)Main.worldSurface;
        int num975 = GenVars.lavaLine - genRand.Next(160, 200);
        int bottom = GenVars.lavaLine;

        int left = GenVars.snowOriginLeft;
        int right = GenVars.snowOriginRight;
        int num979 = 10;
        for (int tileY = 0; tileY <= bottom - 140; tileY++)
        {
            progress.Set(tileY / (double)(bottom - 140));
            GenVars.snowMinX[tileY] = left;
            GenVars.snowMaxX[tileY] = right;
            for (int tileX = left; tileX < right; tileX++)
            {
                if (tileY < num975)
                {
                    if (Main.tile[tileX, tileY].WallType == WallID.DirtUnsafe)
                        Main.tile[tileX, tileY].WallType = WallID.SnowWallUnsafe;

                    switch (Main.tile[tileX, tileY].TileType)
                    {
                        case TileID.Dirt:
                        case TileID.Grass:
                        case TileID.CorruptGrass:
                        case TileID.ClayBlock:
                        case TileID.Sand:
                            Main.tile[tileX, tileY].TileType = TileID.SnowBlock;
                            break;
                        case TileID.Stone:
                            Main.tile[tileX, tileY].TileType = TileID.IceBlock;
                            break;
                    }
                }
                else
                {
                    num979 += genRand.Next(-3, 4);
                    if (genRand.Next(3) == 0)
                    {
                        num979 += genRand.Next(-4, 5);
                        if (genRand.Next(3) == 0)
                            num979 += genRand.Next(-6, 7);
                    }

                    if (num979 < 0)
                        num979 = genRand.Next(3);
                    else if (num979 > 50)
                        num979 = 50 - genRand.Next(3);

                    for (int num982 = tileY; num982 < tileY + num979; num982++)
                    {
                        if (Main.tile[tileX, num982].WallType == WallID.DirtUnsafe)
                            Main.tile[tileX, num982].WallType = WallID.SnowWallUnsafe;

                        switch (Main.tile[tileX, num982].TileType)
                        {
                            case TileID.Dirt:
                            case TileID.Grass:
                            case TileID.CorruptGrass:
                            case TileID.ClayBlock:
                            case TileID.Sand:
                                Main.tile[tileX, num982].TileType = TileID.SnowBlock;
                                break;
                            case TileID.Stone:
                                Main.tile[tileX, num982].TileType = TileID.IceBlock;
                                break;
                        }
                    }
                }
            }

            if (GenVars.snowBottom < tileY)
                GenVars.snowBottom = tileY;
        }
    }
}

public class IceClumpPass : GenPass
{
    public IceClumpPass() : base("Ice Clumping", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Ice biome mounding";
        int smx = 0;
        int smy = 0;
        int contdown = 0;
        int contdownx = 0;

        smx = GenVars.snowOriginLeft + GenVars.snowOriginRight;
        smx /= 2;
        smy = (int)GenVars.worldSurfaceHigh - 600;
        while (!WorldGen.SolidTile(smx, smy) && smy <= Main.UnderworldLayer)
        {
            //seperation
            smy += 1;
        }

        Point Loc7 = new(smx, smy);
        ModContent.GetInstance<VeilGen>().SnowClumpOriginPoint = new Point(smx, smy + 100);

        WorldUtils.Gen(ModContent.GetInstance<VeilGen>().SnowClumpOriginPoint, new Shapes.Mound(450, 150), Actions.Chain(new GenAction[]
            {
                    new Actions.ClearWall(true),
                    new Actions.SetTile(TileID.SnowBlock),
                    new Actions.Smooth(true)
            }));

        // Spawn in Ice Chunks
        WorldGen.TileRunner(Loc7.X, Loc7.Y, 1000, 6, TileID.SnowBlock, false, 0f, 0f, true, true);
        WorldGen.TileRunner(Loc7.X, Loc7.Y + 300, 1200, 7, TileID.IceBlock, false, 0f, 0f, true, true);
        WorldGen.TileRunner(Loc7.X, Loc7.Y + 600, 1000, 2, TileID.IceBlock, false, 0f, 0f, true, true);
        WorldGen.TileRunner(Loc7.X, Loc7.Y + 900, 500, 2, TileID.IceBlock, false, 0f, 0f, true, true);
        WorldGen.TileRunner(Loc7.X, Loc7.Y + 1200, 500, 2, TileID.IceBlock, false, 0f, 0f, true, true);


        WorldUtils.Gen(Loc7, new Shapes.Circle(500, 300), Actions.Chain([
                new Actions.ClearWall(true),
                new Actions.PlaceWall(WallID.SnowWallUnsafe)
        ]));

        for (int daa = 0; daa < 30; daa++)
        {
            contdown -= 10;
            contdownx -= 20;
            // Dig big chasm at top
            WorldGen.digTunnel(smx - Main.rand.Next(10), smy - 250 - contdown, 0, 1, 1, 15, false);

            WorldGen.digTunnel(smx - 300 - contdownx, smy + 1200, 0, 1, 1, Main.rand.Next(40) + 10, true);

            WorldGen.digTunnel(smx - 300 - contdownx, smy + 1500, 0, 1, 1, Main.rand.Next(40) + 10, true);

            WorldGen.digTunnel(smx - 300 - contdownx, smy + 1800, 0, 1, 1, Main.rand.Next(40) + 10, true);
        }
    }
}

public class IceSpikePass : GenPass
{
    public IceSpikePass() : base("Ice Spikes", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Ice settling in the ground";
        int numSpikes = 40;
        for (int k = 0; k < numSpikes; k++)
        {
            int X = WorldGen.genRand.Next(GenVars.snowOriginLeft, GenVars.snowOriginRight);
            int Y = (int)(Main.worldSurface - 200);
            int yBelow = Y + 1;
            for (int yOffset = 0; yOffset < 1000; yOffset++)
            {
                yBelow++;
                if (WorldGen.SolidTile(X, yBelow))
                    break;
            }

            Vector2 startPoint = new(X, yBelow);
            Vector2D endPoint = new(WorldGen.genRand.Next(-10, 10), WorldGen.genRand.Next(-20, -8));
            if (Main.tile[X, yBelow].TileType == TileID.SnowBlock)
            {
                StructureMap structures = GenVars.structures;
                Rectangle areaToPlaceIn = new(
                    (int)startPoint.X - 5,
                    (int)startPoint.Y - 10,
                    10, 20);
                if (!structures.CanPlace(areaToPlaceIn))
                    continue;

                WorldUtils.Gen(startPoint.ToPoint(), new Shapes.Tail(10, endPoint), Actions.Chain([
                    new Actions.SetTile(TileID.IceBlock),
                ]));
            }
        }

        int numCircles = 12;
        for (int s = 0; s < numCircles; s++)
        {
            int X = WorldGen.genRand.Next(GenVars.snowOriginLeft, GenVars.snowOriginRight);
            int Y = (int)(Main.worldSurface - 100);
            int yBelow = Y + 1;
            Vector2 WallPosition = new(X, yBelow);
            for (int yOffset = 0; yOffset < 1000; yOffset++)
            {
                yBelow++;
                if (WorldGen.SolidTile(X, yBelow))
                {
                    break;
                }
            }

            if (Main.tile[X, yBelow].TileType == TileID.SnowBlock)
            {
                StructureMap structures = GenVars.structures;
                Rectangle areaToPlaceIn = new(
                    (int)WallPosition.X - 3,
                    (int)WallPosition.Y - 3,
                    6, 6);
                if (!structures.CanPlace(areaToPlaceIn))
                    continue;
                WorldUtils.Gen(WallPosition.ToPoint(), new Shapes.Circle(WorldGen.genRand.Next(1, 3)), Actions.Chain([
                        new Actions.SetTile(TileID.IceBlock),
                        new Actions.Smooth(true)
                   ]));
            }
        }
        for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 9.2f) * 6E-03); k++)
        {
            int X = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
            int Y = WorldGen.genRand.Next(0, (int)Main.worldSurface);
            int yBelow = Y + 1;
            Vector2 WallPosition = new(X, yBelow);
            if (!WorldGen.SolidTile(X, yBelow))
                continue;

            if (Main.tile[X, yBelow].TileType == TileID.IceBlock)
            {
                WorldUtils.Gen(WallPosition.ToPoint(), new Shapes.Circle(WorldGen.genRand.Next(1, 3)), Actions.Chain([
                        new Actions.ClearWall(true),
                        new Actions.PlaceWall(WallID.IceEcho),
                        new Actions.Smooth(true)
                   ]));
            }
        }
        for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 8.2f) * 6E-04); k++)
        {
            int X = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
            int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY);
            int yBelow = Y + 1;
            Vector2 WallPosition = new(X, yBelow);
            if (!WorldGen.SolidTile(X, yBelow))
                continue;

            if (Main.tile[X, yBelow].TileType == TileID.SnowBlock)
            {
                WorldUtils.Gen(WallPosition.ToPoint(), new Shapes.Circle(WorldGen.genRand.Next(1, 4)), Actions.Chain([
                        new Actions.SetTile(TileID.IceBlock),
                        new Actions.Smooth(true)
                   ]));
            }
        }
    }
}

public class IceCavernPass : GenPass
{
    public IceCavernPass() : base("Ice Caverns", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Carving out ice-y caverns";
        var genRand = WorldGen.genRand;

        int totalX = 0;
        int numX = 0;
        int minSnowX = 0;
        int maxSnowX = 1;
        for (int x = 0; x < Main.maxTilesX; x++)
        {
            int y = (int)Main.worldSurface - 50;
            while (y < Main.maxTilesY)
            {
                y++;
                if (WorldGen.SolidTile(x, y) &&
                    (Main.tile[x, y].TileType == TileID.SnowBlock ||
                    Main.tile[x, y].TileType == TileID.IceBlock))
                {
                    if (numX == 0)
                    {
                        minSnowX = x;
                    }
                    else
                    {
                        maxSnowX = x;
                    }

                    numX++;
                    totalX += x;
                    break;
                }
            }
        }


        //Place Main Ice Tunnel
        int snowTunnelX = totalX / numX;
        int snowTunnelY = GenVars.snowTop - 100;
        Vector2 cavePosition = new(snowTunnelX, snowTunnelY);
        Vector2 caveVelocity = Vector2.UnitX;
        Vector2 caveStrength = new(20, 30);
        Vector2 pullDirection = Vector2.UnitY;
        int caveWidth = 7;
        int caveSteps = 100;
        GenerateFallingIceCavern(cavePosition, caveVelocity, pullDirection, caveStrength, caveWidth, caveSteps);

        //Place Ice Cavern Layers
        int numIceCaverns = genRand.Next(15, 20);
        int iceCavernY = GenVars.snowTop + 50;
        for (int c = 0; c < numIceCaverns; c++)
        {
            for (int n = 0; n < genRand.Next(1, 3); n++)
            {
                for (int a = 0; a < 1000; a++)
                {
                    //Attempts
                    int iceCavernX = genRand.Next(minSnowX, maxSnowX);

                    //Place the cavern
                    cavePosition = new Vector2(iceCavernX, iceCavernY);
                    Point iceCavernTile = cavePosition.ToPoint();
                    if (!WorldGen.SolidTile(iceCavernTile))
                        continue;
                    if (Main.tile[iceCavernTile.X, iceCavernTile.Y].TileType != TileID.IceBlock &&
                        Main.tile[iceCavernTile.X, iceCavernTile.Y].TileType != TileID.SnowBlock)
                        continue;


                    caveVelocity = Vector2.UnitX;
                    if (cavePosition.X > snowTunnelX)
                        caveVelocity = -Vector2.UnitX;
                    caveStrength = new Vector2(20, 30);
                    caveWidth = genRand.Next(5, 8);
                    caveSteps = genRand.Next(70, 100);
                    GenerateIceCavern(cavePosition, caveVelocity, caveStrength, caveWidth, caveSteps);

                    //Place holes to more
                    int numTunnels = genRand.Next(15, 20);
                    for (int t = 0; t < numTunnels; t++)
                    {
                        cavePosition = new Vector2(iceCavernX, iceCavernY);
                        cavePosition += new Vector2(0, genRand.Next(0, 300));
                        caveVelocity = Vector2.UnitX;
                        if (genRand.NextBool(2))
                        {
                            caveVelocity = -Vector2.UnitX;
                        }
                        caveStrength = new Vector2(5, 10);
                        caveWidth = genRand.Next(5, 8);
                        caveSteps = genRand.Next(15, 30);

                        pullDirection = Vector2.UnitY;
                        GenerateFallingIceCavern(cavePosition, caveVelocity, pullDirection, caveStrength, caveWidth, caveSteps);
                    }
                    break;
                }


            }

            iceCavernY += 50;
        }

        int abyssTunnelX = genRand.Next(GenVars.snowOriginLeft, GenVars.snowOriginRight);
        cavePosition = new Vector2(abyssTunnelX, iceCavernY - 50);
        caveVelocity = Vector2.UnitY;
        caveStrength = new Vector2(15, 20);
        pullDirection = -Vector2.UnitX * 0.2f;
        caveWidth = 7;
        caveSteps = 100;
        GenerateFallingIceCavern(cavePosition, caveVelocity, pullDirection, caveStrength, caveWidth, caveSteps);
    }

    public static void GenerateIceSpike(Vector2 cavePosition, double width, Vector2D endOffset, ushort tileId = TileID.IceBlock)
    {
        WorldUtils.Gen(cavePosition.ToPoint(), new Shapes.Tail(width, endOffset), Actions.Chain(new GenAction[]
        {
                new Actions.SetTile(tileId),
        }));
    }

    public static void GenerateFallingIceCavern(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 pullDirection, Vector2 caveStrength, int caveWidth, int caveSteps)
    {
        var genRand = WorldGen.genRand;
        Vector2 caveVelocity = baseCaveDirection;
        ushort[] wallTypes = [
            WallID.SnowWallUnsafe,
            WallID.IceUnsafe
        ];

        Vector2 pullVelocity = pullDirection;
        Vector2 startVelocity = baseCaveDirection;
        float sharpness = 1f;
        int ignoreTile = ModContent.TileType<AbyssalDirt>();
        for (int s = 0; s < caveSteps; s++)
        {
            float degreesToRotate = sharpness;
            float length = caveVelocity.Length();
            float targetAngle = (pullVelocity - startVelocity).ToRotation();
            Vector2 newVelocity = caveVelocity.ToRotation().AngleTowards(targetAngle,
                MathHelper.ToRadians(degreesToRotate)).ToRotationVector2() * length;
            caveVelocity = newVelocity;

            if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
            {
                WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                    genRand.NextFloat(caveStrength.X, caveStrength.Y),
                    genRand.Next(7, 25), -1, ignoreTileType: ignoreTile);
            }

            //Place Walls
            for (int w = 0; w < 5; w++)
            {
                ushort wallType = wallTypes[genRand.Next(0, wallTypes.Length)];
                if (genRand.NextBool(2))
                {
                    wallType = WallID.IceUnsafe;
                }

                Vector2 wallVelocity = genRand.NextVector2Circular(32, 32);
                Vector2 wallPosition = cavePosition + wallVelocity;
                WorldUtils.Gen(wallPosition.ToPoint(), new Shapes.Circle(4, 4), Actions.Chain([
                    new Actions.PlaceWall(wallType),
                    new Actions.Smooth(true)
                ]));
            }


            // Update the cave position.
            cavePosition += caveVelocity * caveWidth * 0.5f;
        }
    }

    public static void GenerateIceCavern(Vector2 cavePosition, Vector2 baseCaveDirection, Vector2 caveStrength, int caveWidth, int caveSteps)
    {
        var genRand = WorldGen.genRand;
        Vector2 caveVelocity = baseCaveDirection;
        ushort[] wallTypes = [
            WallID.SnowWallUnsafe,
            WallID.IceUnsafe
        ];

        int ignoreTile = ModContent.TileType<AbyssalDirt>();
        for (int s = 0; s < caveSteps; s++)
        {
            float radiansOffset = MathF.Sin(s * 0.5f) * MathHelper.ToRadians(45);
            Vector2 shiftedVelocity = baseCaveDirection.RotatedBy(radiansOffset);
            caveVelocity = shiftedVelocity;

            if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
            {
                WorldGen.TileRunner((int)cavePosition.X, (int)cavePosition.Y,
                    genRand.NextFloat(caveStrength.X, caveStrength.Y),
                    genRand.Next(7, 25), -1, ignoreTileType: ignoreTile);
            }

            //Make Stalagtites
            if (genRand.NextBool(2))
            {
                Vector2D endOffset = new(
                    genRand.Next(-10, 10),
                    genRand.Next(-20, -3));
                Vector2 spikePosition = cavePosition;
                spikePosition += new Vector2(0, -10);
                GenerateIceSpike(spikePosition, width: 25, endOffset);
            }

            //Make Stalagmites
            if (genRand.NextBool(4))
            {
                Vector2D endOffset = new(
                    genRand.Next(-10, 10),
                    genRand.Next(3, 7));
                Vector2 spikePosition = cavePosition;
                spikePosition += new Vector2(0, 15);
                GenerateIceSpike(spikePosition, width: 15, endOffset);
            }

            //Place Walls
            for (int w = 0; w < 5; w++)
            {
                ushort wallType = wallTypes[genRand.Next(0, wallTypes.Length)];
                if (genRand.NextBool(2))
                {
                    wallType = WallID.IceUnsafe;
                }

                Vector2 wallVelocity = genRand.NextVector2Circular(32, 32);
                Vector2 wallPosition = cavePosition + wallVelocity;
                WorldUtils.Gen(wallPosition.ToPoint(), new Shapes.Circle(4, 4), Actions.Chain(new GenAction[]
                {
                    new Actions.PlaceWall(wallType),
                    new Actions.Smooth(true)
                }));
            }


            // Update the cave position.
            cavePosition += caveVelocity * caveWidth * 0.5f;
        }
    }
}

public class IceHousePass : GenPass
{
    public IceHousePass() : base("Ice Houses", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "The frozen folk making village homes";

        StructureMap circleStructures = new();
        for (int k = 0; k < 5; k++)
        {
            int attempts = 0;
            while (attempts++ < 10000000)
            {
                // Select a place in the first 6th of the world, avoiding the oceans
                int smx = WorldGen.genRand.Next(GenVars.snowOriginLeft, GenVars.snowOriginRight);
                //Start at 200 tiles above the surface instead of 0, to exclude floating islands
                int smy = (int)GenVars.worldSurfaceHigh - 700;

                // We go down until we hit a solid tile or go under the world's surface
                Tile tile = Main.tile[smx, smy];
                while (!WorldGen.SolidTile(smx, smy))
                {
                    smy++;
                    tile = Main.tile[smx, smy];
                }

                // If we went under the world's surface, try again
                if (smy > Main.worldSurface + 500)
                {
                    continue;
                }

                Vector2 WallPosition = new(smx + 8, smy + 11);

                Rectangle areaToPlaceIn = new(
                    (int)WallPosition.X - 12,
                    (int)WallPosition.Y - 12,
                    24, 24);
                bool success = circleStructures.CanPlace(areaToPlaceIn);
                if (!success)
                    continue;

                //Place snow underneath of the house structure
                WorldUtils.Gen(WallPosition.ToPoint(), new Shapes.Circle(12), Actions.Chain([
                    new Actions.SetTile(TileID.SnowBlock)
                ]));

                circleStructures.AddProtectedStructure(areaToPlaceIn);

                switch (Main.rand.Next(2))
                {
                    case 0:
                        for (int da = 0; da < 1; da++)
                        {
                            Point Loc = new(smx, smy + 5);
                            string path = "Struct/IceStruct/HouseSurfaceIce1";
                            Structurizer.ProtectStructure(Loc, path);
                        }
                        break;
                    case 1:
                        for (int da = 0; da < 1; da++)
                        {
                            Point Loc = new(smx, smy + 5);
                            string path = "Struct/IceStruct/HouseSurfaceIce2";
                            Structurizer.ProtectStructure(Loc, path);
                        }
                        break;
                }
                break;
            }
        }
    }
}