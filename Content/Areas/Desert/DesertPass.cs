using Stellamod.Content.Areas.Desert.TilesCL;
using Stellamod.Content.Areas.Desert.WeaponsCL;
using Stellamod.Content.Areas.SpringHills.TilesSH;
using Stellamod.Core.RibbonSystem;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Biomes;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Stellamod.Content.Areas.Desert;

public class LockDesertPass : GenPass
{
    public LockDesertPass() : base("Lock Desert", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Expanding the Desert";
        GenVars.skipDesertTileCheck = true;
        DesertBiome desertBiome = GenVars.configuration.CreateBiome<DesertBiome>();
        var genRand = WorldGen.genRand;

        int desertOffset = -1200;
        int x = Main.maxTilesX / 2 + desertOffset;
        ModContent.GetInstance<VeilGen>().DesertLocation = new Point(x, (int)GenVars.worldSurfaceHigh + genRand.Next(25, 75));
        while (!desertBiome.Place(ModContent.GetInstance<VeilGen>().DesertLocation, GenVars.structures))
        {
            x = Main.maxTilesX / 2 + desertOffset + genRand.Next(-200, 0);
            ModContent.GetInstance<VeilGen>().DesertLocation = new Point(x, (int)GenVars.worldSurfaceHigh + genRand.Next(25, 75));
        }


        //About to give the desert an extension

        int newDesertLeft = GenVars.desertHiveLeft - VeilGen.Desert_Padding;
        int newDesertRight = GenVars.desertHiveRight + VeilGen.Desert_Padding;

        //Adding surface sands
        //This is our desert extension, we just gonna replcae dirt/stone/clay tiles


        //Actually, it should be safe to just replace solid tiles, the colosseum doesn't exist yet
        int maxDesertDepth = 150;
        float steps = newDesertRight - newDesertLeft;
        for (int dx = newDesertLeft; dx < newDesertRight; dx++)
        {
            float marker = dx - newDesertLeft;
            float completionRatio = marker / steps;
            float ease = EasingFunction.QuadraticBump(completionRatio);
            int depth = (int)MathHelper.Lerp(1, maxDesertDepth, ease);
            int tileX = dx;
            int startY = (int)(Main.worldSurface - 100);

            //Move down until we hit a solid tile
            for (int k = 0; k < 300; k++)
            {
                if (!WorldGen.SolidTile(dx, startY))
                {
                    startY++;
                }
                else
                {
                    break;
                }
            }

            //Now we have the position we want to start from
            int bottom = startY + depth;
            for (int dy = startY; dy < bottom; dy++)
            {
                if (WorldGen.SolidTile(tileX, dy))
                {
                    WorldGen.PlaceTile(tileX, dy, TileID.Sand);
                }

                WorldGen.TileRunner(tileX, dy, 3, 10, TileID.Sand);
            }
        }
    }
}

public class AshotiTemplePass : GenPass
{
    public AshotiTemplePass() : base("Ashoti Temple", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Burying Ashoti";

        var genRand = WorldGen.genRand;
        int radius = 80;
        int desertCenterX = (GenVars.desertHiveLeft + GenVars.desertHiveRight) / 2;
        int desertCenterY = GenVars.desertHiveLow - 200;
        Point arenaPoint = new(desertCenterX, desertCenterY);
        Main.tileSolid[TileID.LihzahrdBrick] = true;

        //Building the arena
        WorldUtils.Gen(arenaPoint, new Shapes.Circle(radius, radius), new Actions.SetTile(TileID.LihzahrdBrick));
        WorldUtils.Gen(arenaPoint, new Shapes.Circle(radius - 2, radius - 2), new Actions.SetTile((ushort)ModContent.TileType<ChiseledStone>()));
        WorldUtils.Gen(arenaPoint, new Shapes.Circle(radius - 4, radius - 4), new Actions.SetTile((ushort)ModContent.TileType<NoxianBlock>()));
        WorldUtils.Gen(arenaPoint, new Shapes.Circle(radius - 6, radius - 6), new Actions.ClearTile());
        WorldUtils.Gen(arenaPoint, new Shapes.Circle(radius / 2, radius / 2), new Actions.SetLiquid(type: LiquidID.Lava));
        string structure;


        //Place the center piece where the thing be
        structure = "Structures/AshotiTemple/TempleBottom";
        Rectangle templeBottomRect = Structurizer.ReadRectangle(structure);
        Point templeBottomToPlace = arenaPoint;
        templeBottomToPlace.X -= templeBottomRect.Width / 2;
        templeBottomToPlace.Y += templeBottomRect.Height;
        Structurizer.ReadStruct(templeBottomToPlace, structure);
        Structurizer.ProtectStructure(templeBottomToPlace, structure);


        //Decorate arena with walls
        for (int w = 0; w < 80; w++)
        {
            float progressOnCircle = w / 80f;
            float rot = progressOnCircle * MathHelper.TwoPi;
            Vector2 vel = rot.ToRotationVector2() * radius;
            Point pointToWall = arenaPoint + vel.ToPoint();
            WorldUtils.Gen(pointToWall, new Shapes.Circle(4, 4), new Actions.PlaceWall(type: WallID.LihzahrdBrickUnsafe));
        }

        //Make Middle of the Temple
        int middleLength = 7;

        for (int m = 0; m < middleLength; m++)
        {
            Point offset = new Point(0, m * -43);
            Point tileToPlaceOn = arenaPoint + offset;

            if (m == middleLength - 1)
            {
                structure = "Structures/AshotiTemple/TempleEntrance";
                Rectangle rect = Structurizer.ReadRectangle(structure);
                tileToPlaceOn.X -= rect.Width / 2;
                tileToPlaceOn.Y -= 28;
                Structurizer.ProtectStructure(tileToPlaceOn, structure);
            }
            else
            {
                structure = "Structures/AshotiTemple/TempleMiddle";
                Rectangle rect = Structurizer.ReadRectangle(structure);
                tileToPlaceOn.X -= rect.Width / 2;
                int[] chestIndices = Structurizer.ReadStruct(tileToPlaceOn, structure);
                foreach (int chestIndex in chestIndices)
                {
                    if (chestIndex == -1)
                        continue;
                    Chest chest = Main.chest[chestIndex];
                    var itemsToAdd = new List<(int type, int stack)>();

                    //Golem Drops
                    switch (genRand.Next(8))
                    {
                        case 0:
                            itemsToAdd.Add((ItemID.Stynger, 1));
                            itemsToAdd.Add((ItemID.StyngerBolt, genRand.Next(60, 100)));
                            break;
                        case 1:
                            itemsToAdd.Add((ItemID.PossessedHatchet, 1));
                            break;
                        case 2:
                            itemsToAdd.Add((ItemID.SunStone, 1));
                            break;
                        case 3:
                            itemsToAdd.Add((ItemID.EyeoftheGolem, 1));
                            break;
                        case 4:
                            itemsToAdd.Add((ItemID.EyeoftheGolem, 1));
                            break;
                        case 5:
                            itemsToAdd.Add((ItemID.HeatRay, 1));
                            break;
                        case 6:
                            itemsToAdd.Add((ItemID.StaffofEarth, 1));
                            break;
                        case 7:
                            itemsToAdd.Add((ItemID.GolemFist, 1));
                            break;
                    }

                    if (genRand.NextBool(3))
                    {
                        switch (genRand.Next(2))
                        {
                            case 0:
                                itemsToAdd.Add((ModContent.ItemType<Lihh>(), 1));
                                break;
                            case 1:
                                itemsToAdd.Add((ModContent.ItemType<Relagis>(), 1));
                                break;
                        }
                    }

                    itemsToAdd.Add((ItemID.LihzahrdPowerCell, 1));
                    itemsToAdd.Add((ItemID.LihzahrdFurnace, 1));

                    if (genRand.NextBool(3))
                    {
                        switch (genRand.Next(2))
                        {
                            case 0:
                                itemsToAdd.Add((ItemID.GreaterHealingPotion, genRand.Next(2, 6)));
                                break;
                            case 1:
                                itemsToAdd.Add((ItemID.GreaterManaPotion, genRand.Next(2, 6)));
                                break;
                        }
                    }

                    switch (genRand.Next(2))
                    {
                        case 0:
                            itemsToAdd.Add((ItemID.SolarTablet, 1));
                            break;
                        case 1:
                            itemsToAdd.Add((ItemID.LunarTabletFragment, genRand.Next(3, 8)));
                            break;
                    }


                    int chestItemIndex = 0;
                    foreach (var itemToAdd in itemsToAdd)
                    {
                        Item item = new();
                        item.SetDefaults(itemToAdd.type);
                        item.stack = itemToAdd.stack;
                        chest.item[chestItemIndex] = item;
                        chestItemIndex++;
                        if (chestItemIndex >= 40)
                            break; // Make sure not to exceed the capacity of the chest
                    }
                }
                Structurizer.ProtectStructure(tileToPlaceOn, structure);
            }
        }
    }
}

public class ColosseumPass : GenPass
{
    public ColosseumPass() : base("Colosseum", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        var genRand = WorldGen.genRand;
        progress.Message = "Gintzing all over the desert";
        int desertCenterX = (GenVars.desertHiveLeft + GenVars.desertHiveRight) / 2;
        int desertSurfaceY = 0;
        int colosseumX = desertCenterX - 71;
        colosseumX += 35;

        int colosseumY = (int)Main.worldSurface - 50;
        while (!WorldGen.SolidTile(colosseumX, colosseumY))
        {
            colosseumY++;
        }

        desertSurfaceY = colosseumY;
        colosseumY += 40;
        Point colosseumPoint = new(colosseumX, colosseumY);

        //Place the colosseum
        StructureMap desertStructures = new();
        GenerateColosseum(colosseumPoint, desertStructures);

        //Basically we're just gonna get random points on the colosseum and palce ribbons
        //This should look aight?
        //Hopefully lol
        int ribbonPlacementRange = 50;
        int numColosseumRibbons = 18;
        int ribbons = 0;
        for (int attempts = 0; attempts < 100000; attempts++)
        {
            //Just get some random points tbh, I forgot how big the colosseum is
            int randX = desertCenterX + genRand.Next(-ribbonPlacementRange, ribbonPlacementRange);

            //We want to use where the desert surface was cause the colosseum is in the gorund
            int randY = desertSurfaceY + genRand.Next(-100, -10);
            Point placementPoint = new(randX, randY);
            if (WorldGen.SolidTile(placementPoint))
            {
                int dir = Math.Sign(randX - desertCenterX);
                PlaceRibbon(placementPoint, dir, genRand.Next(8, 15));
                ribbons++;
                if (ribbons >= numColosseumRibbons)
                {
                    break;
                }
            }
            else
            {
                continue;
            }
        }

        //Ok, since the desert hive is a protected structure, we need to make a local structure map to safely place things on it
        //This is a bit annoying but it'll work


        //Generate the desert hide out

        int[] tileBlend = [
            TileID.RubyGemspark
        ];

        //Place List House
        void RandomlyPlaceStructureInSurfaceDesert(string structure)
        {
            for (int attempts = 0; attempts < 10000; attempts++)
            {
                int randDesertX = genRand.Next(GenVars.desertHiveLeft, GenVars.desertHiveRight);
                int y = (int)(Main.worldSurface - 300);
                for (int m = 0; m < 1000; m++)
                {
                    y++;
                    if (WorldGen.SolidTile(randDesertX, y))
                    {


                        break;
                    }
                }

                Point tilePoint = new(randDesertX, y);
                if (Structurizer.SafePlaceAndProtectStructure(tilePoint, structure, desertStructures, tileBlend, out int[] chestIndices))
                {
                    Rectangle structureRect = Structurizer.ReadRectangle(structure);
                    PlaceRibbonsandBeams(structureRect, tilePoint);
                    break;
                }
            }
        }

        RandomlyPlaceStructureInSurfaceDesert("Structures/ListsHouse");
        RandomlyPlaceStructureInSurfaceDesert("Structures/DesertOrgan");
        RandomlyPlaceStructureInSurfaceDesert("Structures/DesertEresh");

        int newDesertLeft = GenVars.desertHiveLeft - VeilGen.Desert_Padding;
        int newDesertRight = GenVars.desertHiveRight + VeilGen.Desert_Padding;

        //Place Houses
        int numHouses = genRand.Next(12, 15);
        int houseCount = 0;
        for (int attempts = 0; attempts < 10000; attempts++)
        {
            int randX = genRand.Next(newDesertLeft, newDesertRight);
            int y = (int)(Main.worldSurface - 200);
            for (int yOffset = 0; yOffset < 500; yOffset++)
            {
                y++;
                if (!WorldGen.SolidTile(randX, y))
                    continue;

                Tile tile = Main.tile[randX, y];
                if (Main.tile[randX, y - 1].LiquidAmount > 0)
                    continue;

                if (tile.TileType == TileID.Sand)
                    break;

            }

            if (TryPlaceDesertHouse(new Point(randX, y), desertStructures))
            {
                houseCount++;
            }
            if (houseCount >= numHouses)
            {
                break;
            }
        }

        //Place sand decorations
        int numSandDecorations = genRand.Next(40, 60);
        int[] wallTypesToPlace = [
            ModContent.WallType<SandCastle1>(),
            ModContent.WallType<SandCastle2>(),
            ModContent.WallType<SandCastle3>(),
            ModContent.WallType<SandCastle4>(),
            ModContent.WallType<SandCastle5>(),
            ModContent.WallType<SandCastle6>(),
            ModContent.WallType<SandCastle7>()
        ];


        for (int n = 0; n < numSandDecorations; n++)
        {
            int randX = genRand.Next(newDesertLeft, newDesertRight);
            int y = (int)(Main.worldSurface - 200);
            for (int yOffset = 0; yOffset < 500; yOffset++)
            {
                y++;
                if (!WorldGen.SolidTile(randX, y))
                    continue;
                Tile tile = Main.tile[randX, y];
                if (tile.TileType == TileID.Sand)
                    break;
            }

            int randSandCastle = genRand.Next(0, wallTypesToPlace.Length);
            int sandCastleType = wallTypesToPlace[randSandCastle];
            WorldGen.PlaceWall(randX, y, sandCastleType);
        }
    }

    public static void GenerateColosseum(Point tilePoint, StructureMap structureMap = null)
    {
        var genRand = WorldGen.genRand;
        string GetMiniStructurePath()
        {
            int num = genRand.Next(1, 3);
            string baseStructurePath = $"Structures/Colosseum/SquareHouse{num}";
            return baseStructurePath;
        }

        string GetStructurePath()
        {
            int num = genRand.Next(1, 5);
            string baseStructurePath = $"Structures/Colosseum/House{num}";
            return baseStructurePath;
        }

        int[] tileBlend = [
            TileID.RubyGemspark
        ];

        void Arena(Point tilePoint)
        {
            var structure = "Structures/Colosseum/TheColosseum";
            Rectangle rectangle = Structurizer.ReadRectangle(structure);
            rectangle.Location = tilePoint;
            Structurizer.ReadStruct(tilePoint, structure, tileBlend);
            Structurizer.ProtectStructure(tilePoint, structure, structureMap);
            for (int beamX = rectangle.Location.X;
             beamX < rectangle.Location.X + rectangle.Width; beamX += 8)
            {
                //Place beams
                int beamY = rectangle.Location.Y;
                Tile tile = Main.tile[beamX, beamY];
                if (!tile.HasTile)
                    continue;

                int solidCount = 0;
                while (solidCount < 5)
                {
                    tile = Main.tile[beamX, beamY];
                    if (!tile.HasTile)
                    {
                        WorldGen.PlaceTile(beamX, beamY, TileID.SandstoneColumn);
                    }
                    else
                    {
                        solidCount++;
                    }
                    beamY++;
                }
            }
        }
        void PlaceAir(Point tilePoint)
        {
            string structure = "Structures/Colosseum/Elevator";
            Rectangle rectangle = Structurizer.ReadRectangle(structure);
            rectangle.Location = tilePoint;
            Structurizer.ProtectStructure(tilePoint, structure, structureMap);
        }

        void PlaceBigStructure(Point tilePoint)
        {
            string structure = GetStructurePath();
            Rectangle rectangle = Structurizer.ReadRectangle(structure);
            rectangle.Location = tilePoint;
            var chestIndices = Structurizer.ReadStruct(tilePoint, structure, tileBlend);
            if (chestIndices.Length != 0)
            {
                foreach (int chestIndex in chestIndices)
                {
                    if (chestIndex == -1)
                        continue;
                    Chest chest = Main.chest[chestIndex];
                    var itemsToAdd = new List<(int type, int stack)>();

                    int chestItemIndex = 0;
                    foreach (var itemToAdd in itemsToAdd)
                    {
                        Item item = new();
                        item.SetDefaults(itemToAdd.type);
                        item.stack = itemToAdd.stack;
                        chest.item[chestItemIndex] = item;
                        chestItemIndex++;
                        if (chestItemIndex >= 40)
                            break; // Make sure not to exceed the capacity of the chest
                    }
                }
            }

            for (int beamX = rectangle.Location.X;
                beamX < rectangle.Location.X + rectangle.Width; beamX += 8)
            {
                //Place beams
                int beamY = rectangle.Location.Y;
                Tile tile = Main.tile[beamX, beamY];
                if (!tile.HasTile)
                    continue;
                int solidCount = 0;
                while (solidCount < 5)
                {
                    tile = Main.tile[beamX, beamY];
                    if (!tile.HasTile)
                    {
                        WorldGen.PlaceTile(beamX, beamY, TileID.SandstoneColumn);
                    }
                    else
                    {
                        solidCount++;
                    }
                    beamY++;
                }
            }
            Structurizer.ProtectStructure(tilePoint, structure, structureMap);
        }
        void PlaceSmallStructure(Point tilePoint)
        {
            string structure = GetMiniStructurePath();
            Rectangle rectangle = Structurizer.ReadRectangle(structure);
            rectangle.Location = tilePoint;
            var chestIndices = Structurizer.ReadStruct(tilePoint, structure, tileBlend);
            if (chestIndices.Length != 0)
            {
                foreach (int chestIndex in chestIndices)
                {
                    if (chestIndex == -1)
                        continue;
                    Chest chest = Main.chest[chestIndex];
                    var itemsToAdd = new List<(int type, int stack)>();

                    int chestItemIndex = 0;
                    foreach (var itemToAdd in itemsToAdd)
                    {
                        Item item = new();
                        item.SetDefaults(itemToAdd.type);
                        item.stack = itemToAdd.stack;
                        chest.item[chestItemIndex] = item;
                        chestItemIndex++;
                        if (chestItemIndex >= 40)
                            break; // Make sure not to exceed the capacity of the chest
                    }
                }
            }
            Structurizer.ProtectStructure(tilePoint, structure, structureMap);

            for (int beamX = rectangle.Location.X;
                beamX < rectangle.Location.X + rectangle.Width; beamX += 8)
            {
                //Place beams
                int beamY = rectangle.Location.Y;
                Tile tile = Main.tile[beamX, beamY];
                if (!tile.HasTile)
                    continue;
                int solidCount = 0;
                while (solidCount < 5)
                {
                    tile = Main.tile[beamX, beamY];
                    if (!tile.HasTile)
                    {
                        WorldGen.PlaceTile(beamX, beamY, TileID.SandstoneColumn);
                    }
                    else
                    {
                        solidCount++;
                    }
                    beamY++;
                }
            }
        }
        PlaceAir(tilePoint + new Point(48, 100));
        PlaceAir(tilePoint + new Point(50, 100));
        int upOffset = 18;
        PlaceBigStructure(tilePoint);
        PlaceBigStructure(tilePoint + new Point(24, 0));
        PlaceBigStructure(tilePoint + new Point(24 + 32, 0));
        PlaceBigStructure(tilePoint + new Point(24 + 32 + 24, 0));

        tilePoint.Y -= upOffset;
        PlaceBigStructure(tilePoint);
        PlaceBigStructure(tilePoint + new Point(24, 0));
        PlaceBigStructure(tilePoint + new Point(24 + 32, 0));
        PlaceBigStructure(tilePoint + new Point(24 + 32 + 24, 0));


        tilePoint.Y -= upOffset;
        PlaceBigStructure(tilePoint + new Point(4, 0));
        PlaceBigStructure(tilePoint + new Point(24 + 4, 0));
        PlaceBigStructure(tilePoint + new Point(24 + 32 - 4, 0));
        PlaceBigStructure(tilePoint + new Point(24 + 32 + 24 - 4, 0));

        tilePoint.Y -= upOffset;
        PlaceSmallStructure(tilePoint + new Point(34, 0));
        PlaceSmallStructure(tilePoint + new Point(52, 0));

        tilePoint.Y -= upOffset;
        PlaceSmallStructure(tilePoint + new Point(16, 1));
        PlaceSmallStructure(tilePoint + new Point(34, 1));
        PlaceSmallStructure(tilePoint + new Point(52, 1));
        PlaceSmallStructure(tilePoint + new Point(70, 1));

        tilePoint.Y -= upOffset;
        Arena(tilePoint + new Point(-21, -1));

        /*
        //Layer 6
      
        */
    }

    public static void PlaceRibbon(Point tilePoint, int dir, int xLength)
    {
        Point highPoint = tilePoint;
        highPoint.X += dir * xLength;
        for (int i = 0; i < 100; i++)
        {
            if (WorldGen.SolidTile(highPoint))
            {
                break;
            }
            else
            {
                highPoint.Y++;
            }
        }

        //Now that we have the ribbons we can yeah
        RibbonRenderer ribbonRenderer = ModContent.GetInstance<RibbonRenderer>();
        RibbonWandType style = (RibbonWandType)WorldGen.genRand.Next(0, 5);
        ribbonRenderer.PlaceRibbon(tilePoint.ToWorldCoordinates(), highPoint.ToWorldCoordinates(), style);
    }

    public static void PlaceRibbonsandBeams(Rectangle structureRect, Point tilePoint)
    {
        var genRand = WorldGen.genRand;
        //Get top left tile
        Point leftRibbon = tilePoint;


        PlaceDesertBeams(structureRect, tilePoint);
        //Structures place from the bottom left, so we need to subtract theheight to convert them
        leftRibbon.Y -= structureRect.Height;
        leftRibbon.X += 1;

        //Set the right ribbon to the left ribbon and offset it
        Point rightRibbon = leftRibbon;
        rightRibbon.X += structureRect.Width;
        rightRibbon.X -= 1;

        for (int i = 0; i < 1000; i++)
        {
            if (WorldGen.SolidTile(leftRibbon.X, leftRibbon.Y))
            {
                break;
            }
            else
            {
                leftRibbon.Y++;
            }
        }

        for (int i = 0; i < 1000; i++)
        {
            if (WorldGen.SolidTile(rightRibbon.X, rightRibbon.Y))
            {
                break;
            }
            else
            {
                rightRibbon.Y++;
            }
        }


        PlaceRibbon(leftRibbon, -1, genRand.Next(8, 15));
        PlaceRibbon(rightRibbon, 1, genRand.Next(8, 15));
    }

    public static void PlaceDesertBeams(Rectangle rectangle, Point location)
    {
        rectangle.Location = location;
        for (int beamX = rectangle.Location.X; beamX < rectangle.Location.X + rectangle.Width; beamX += 2)
        {
            //Place beams
            int beamY = rectangle.Location.Y;
            if (beamX < Main.maxTilesX && beamY < Main.maxTilesY)
            {
                int solidCount = 0;
                while (solidCount < 5)
                {
                    if (!WorldGen.SolidTile(beamX, beamY))
                    {
                        WorldGen.PlaceTile(beamX, beamY, TileID.SandstoneColumn);
                    }
                    else
                    {
                        solidCount++;
                    }
                    beamY++;
                }
            }
        }
    }

    public static bool TryPlaceDesertHouse(Point tilePoint, StructureMap structures)
    {
        string[] houseStructureFiles = [
            "Structures/DesertSurhouse1",
            "Structures/DesertSurhouse2",
            "Structures/DesertSurhouse3"
        ];

        var genRand = WorldGen.genRand;
        string structureFile = houseStructureFiles[genRand.Next(0, houseStructureFiles.Length)];
        int[] tileBlend = [
            TileID.RubyGemspark
        ];
        if (Structurizer.SafePlaceAndProtectStructure(tilePoint, structureFile, structures, tileBlend, out int[] chestIndices))
        {
            Rectangle structureRect = Structurizer.ReadRectangle(structureFile);
            PlaceRibbonsandBeams(structureRect, tilePoint);
            return true;
        }
        return false;
    }
}