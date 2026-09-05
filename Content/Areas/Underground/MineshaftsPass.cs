using Stellamod.Common.DungeonGeneration;
using Stellamod.Content.CommonMaterials;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.WorldBuilding;

namespace Stellamod.Content.Areas.Underground;

public class MineshaftsPass : GenPass
{
    public MineshaftsPass() : base("Mineshafts", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Enriching the underground...";
        var genRand = WorldGen.genRand;

        //Alright so here's our algorithm
        int padding = 1700;
        float placedShafts = 0;
        float shaftCount = Main.maxTilesX * Main.maxTilesY * 0.0000005f;
        float maxAttemptCount = shaftCount * 10;

        //Generate all mienshafts in advance, generating them late is much slower
        //If you're going to do prepare rooms, have them all at the same time
        Queue<(Rectangle mapBounds, Room[] map)> mineshaftQueue = new Queue<(Rectangle mapBounds, Room[] map)>();
        for (int i = 0; i < shaftCount; i++)
        {
            mineshaftQueue.Enqueue(GenerateMineshaft(genRand));
        }

        (Rectangle mapBounds, Room[] map) = mineshaftQueue.Dequeue();
        for (float n = 0; n < maxAttemptCount; n++)
        {
            int x = genRand.Next(padding, Main.maxTilesX - padding);
            int y = genRand.Next((int)GenVars.rockLayerHigh, ModContent.GetInstance<VeilGen>().DarkspaceStart - 200);
            if (VeilGen.IsTileNearby(x, y, distance: 200, TileSets.BlockMineshafts))
                continue;

            Tile tile = Main.tile[x, y];
            if (Main.tileSolid[tile.TileType] && tile.HasTile && TileID.Sets.Stone[tile.TileType])
            {
                if (PlaceMineshaft(new Point(x, y), mapBounds, map))
                {
                    placedShafts++;
                    if (placedShafts >= shaftCount)
                        break;

                    (mapBounds, map) = mineshaftQueue.Dequeue();
                }
            }

            progress.Set((double)n / placedShafts);
        }
        Console.WriteLine($"{placedShafts} Mineshafts Placed");
    }

    public static void GenerateMineshaftTunnel(Point tilePoint, Point tileDirection, int tunnelLength)
    {
        var genRand = WorldGen.genRand;
        string GetStructurePath()
        {
            int num = genRand.Next(1, 15);
            string baseStructurePath = $"Structures/Catacombs/CaRoom{num}";
            return baseStructurePath;
        }

        int[] tileBlend = new int[]
        {

        };

        for (int t = 0; t < tunnelLength; t++)
        {
            string structure = GetStructurePath();
            Rectangle rectangle = Structurizer.ReadRectangle(structure);
            rectangle.Location = tilePoint;
            if (VeilGen.TilePercent(tilePoint, rectangle, TileID.Dirt, TileID.Stone) < 0.7f)
            {
                break;
            }

            int[] chestIndices = Structurizer.ReadStruct(tilePoint, structure, null);
            if (chestIndices.Length != 0)
            {
                foreach (int chestIndex in chestIndices)
                {
                    if (chestIndex == -1)
                        continue;
                    Chest chest = Main.chest[chestIndex];
                    var itemsToAdd = new List<(int type, int stack)>();
                    if (genRand.NextBool(2))
                    {
                        switch (genRand.Next(6))
                        {
                            case 0:
                                itemsToAdd.Add((ItemID.MagicMirror, 1));
                                break;
                            case 1:
                                itemsToAdd.Add((ItemID.HermesBoots, 1));
                                break;
                            case 2:
                                itemsToAdd.Add((ItemID.FlareGun, 1));
                                itemsToAdd.Add((ItemID.Flare, genRand.Next(20, 30)));
                                break;
                            case 3:
                                itemsToAdd.Add((ItemID.Mace, 1));
                                break;
                            case 4:
                                itemsToAdd.Add((ItemID.LavaCharm, 1));
                                break;
                            case 5:
                                itemsToAdd.Add((ItemID.Aglet, 1));
                                break;
                        }
                    }

                    itemsToAdd.Add((ModContent.ItemType<MinersGold>(), genRand.Next(3, 5)));
                    if (genRand.NextBool(3))
                    {
                        switch (genRand.Next(0, 2))
                        {
                            case 0:
                                itemsToAdd.Add((ItemID.Bomb, genRand.Next(3, 7)));
                                break;
                            case 1:
                                itemsToAdd.Add((ItemID.Dynamite, genRand.Next(1, 3)));
                                break;
                        }
                    }

                    if (genRand.NextBool(3))
                    {
                        switch (genRand.Next(0, 2))
                        {
                            case 0:
                                itemsToAdd.Add((ItemID.Torch, genRand.Next(3, 7)));
                                break;
                            case 1:
                                itemsToAdd.Add((ItemID.SpelunkerGlowstick, genRand.Next(5, 10)));
                                break;
                        }
                    }

                    if (genRand.NextBool(3))
                    {
                        switch (genRand.Next(0, 2))
                        {
                            case 0:
                                itemsToAdd.Add((ItemID.LesserHealingPotion, genRand.Next(2, 4)));
                                break;
                            case 1:
                                itemsToAdd.Add((ItemID.LesserManaPotion, genRand.Next(1, 3)));
                                break;
                        }
                    }

                    if (genRand.NextBool(3))
                    {
                        switch (genRand.Next(0, 6))
                        {
                            case 0:
                                itemsToAdd.Add((ItemID.SpelunkerPotion, genRand.Next(2, 4)));
                                break;
                            case 1:
                                itemsToAdd.Add((ItemID.PotionOfReturn, genRand.Next(1, 3)));
                                break;
                            case 2:
                                itemsToAdd.Add((ItemID.HunterPotion, genRand.Next(1, 3)));
                                break;
                            case 3:
                                itemsToAdd.Add((ItemID.MiningPotion, genRand.Next(1, 3)));
                                break;
                            case 4:
                                itemsToAdd.Add((ItemID.TrapsightPotion, genRand.Next(1, 3)));
                                break;
                            case 5:
                                itemsToAdd.Add((ItemID.ObsidianSkinPotion, genRand.Next(1, 3)));
                                break;
                        }
                    }
                    for (int n = 0; n < 4; n++)
                    {
                        if (genRand.NextBool(4))
                        {
                            switch (genRand.Next(0, 7))
                            {
                                case 0:
                                    itemsToAdd.Add((ItemID.Amethyst, genRand.Next(3, 10)));
                                    break;
                                case 1:
                                    itemsToAdd.Add((ItemID.Emerald, genRand.Next(3, 10)));
                                    break;
                                case 2:
                                    itemsToAdd.Add((ItemID.Sapphire, genRand.Next(3, 10)));
                                    break;
                                case 3:
                                    itemsToAdd.Add((ItemID.Topaz, genRand.Next(3, 10)));
                                    break;
                                case 4:
                                    itemsToAdd.Add((ItemID.Ruby, genRand.Next(3, 10)));
                                    break;
                                case 5:
                                    itemsToAdd.Add((ItemID.Diamond, genRand.Next(3, 10)));
                                    break;
                                case 6:
                                    itemsToAdd.Add((ItemID.Amber, genRand.Next(3, 10)));
                                    break;
                            }
                        }
                    }

                    for (int n = 0; n < 4; n++)
                    {
                        if (genRand.NextBool(4))
                        {
                            switch (genRand.Next(0, 8))
                            {
                                case 0:
                                    itemsToAdd.Add((ItemID.CopperOre, genRand.Next(3, 10)));
                                    break;
                                case 1:
                                    itemsToAdd.Add((ItemID.TinOre, genRand.Next(3, 10)));
                                    break;
                                case 2:
                                    itemsToAdd.Add((ItemID.IronOre, genRand.Next(3, 10)));
                                    break;
                                case 3:
                                    itemsToAdd.Add((ItemID.LeadOre, genRand.Next(3, 10)));
                                    break;
                                case 4:
                                    itemsToAdd.Add((ItemID.SilverOre, genRand.Next(3, 10)));
                                    break;
                                case 5:
                                    itemsToAdd.Add((ItemID.TungstenOre, genRand.Next(3, 10)));
                                    break;
                                case 6:
                                    itemsToAdd.Add((ItemID.GoldOre, genRand.Next(3, 10)));
                                    break;
                                case 7:
                                    itemsToAdd.Add((ItemID.PlatinumOre, genRand.Next(3, 10)));
                                    break;
                            }
                        }
                    }

                    if (genRand.NextBool(1))
                    {
                        switch (genRand.Next(3))
                        {
                            case 0:
                                itemsToAdd.Add((ItemID.CopperCoin, genRand.Next(45, 100)));
                                break;
                            case 1:
                                itemsToAdd.Add((ItemID.SilverCoin, genRand.Next(45, 100)));
                                break;
                            case 2:
                                itemsToAdd.Add((ItemID.GoldCoin, genRand.Next(1, 3)));
                                break;
                        }
                    }

                    if (genRand.NextBool(100))
                    {
                        itemsToAdd.Add((ItemID.MiningHelmet, 1));
                        itemsToAdd.Add((ItemID.MiningPants, 1));
                        itemsToAdd.Add((ItemID.MiningShirt, 1));
                    }

                    int chestItemIndex = 0;
                    foreach (var itemToAdd in itemsToAdd)
                    {
                        Item item = new Item();
                        item.SetDefaults(itemToAdd.type);
                        item.stack = itemToAdd.stack;
                        chest.item[chestItemIndex] = item;
                        chestItemIndex++;
                        if (chestItemIndex >= 40)
                            break; // Make sure not to exceed the capacity of the chest
                    }
                }

            }


            Structurizer.ProtectStructure(tilePoint, structure);

            if (tileDirection.X != 0)
            {
                tilePoint.X += tileDirection.X * rectangle.Width;
            }
            else if (tileDirection.Y != 0)
            {
                tilePoint.Y += tileDirection.Y * (rectangle.Height + 1);
            }

            if (genRand.NextBool(4) && tileDirection != new Point(0, -1))
            {
                GenerateMineshaftTunnel(tilePoint, new Point(0, -1), tunnelLength / 2);
            }
            else if (genRand.NextBool(2) && tileDirection != new Point(1, 0))
            {
                GenerateMineshaftTunnel(tilePoint, new Point(1, 0), tunnelLength / 2);
            }
        }
    }

    public static (Rectangle rect, Room[] map) GenerateMineshaft(UnifiedRandom genRand)
    {
        (int, int)[] layout = DungeonLayouter.GenerateLayout(40, genRand);
        Point[] vertices = new Point[layout.Length];
        for (int v = 0; v < vertices.Length; v++)
        {
            vertices[v] = new Point(layout[v].Item1, layout[v].Item2);
        }

        DungeonChart simpleChart = DungeonChart.FromMap(layout);
        Room[] map = Dungeonizer.CreateDungeonFromChart(VeilGen.MineshaftPrefabs, simpleChart, genRand);
        Rectangle rectangle = Dungeonizer.GetDungeonBounds(map);
        return (rectangle, map);
    }

    public static bool PlaceMineshaft(Point startTile, Rectangle rectangle, Room[] map)
    {
        if (Structurizer.CanPlaceStructureHere(rectangle))
            return false;

        Point point = startTile;
        Point vectorToOrigin = point - rectangle.Top().ToPoint();
        rectangle.Location += vectorToOrigin;
        //Main.NewText(map.Length);
        //Just a failsafe
        while (rectangle.Right().X >= Main.maxTilesX)
            rectangle.Location -= new Point(32, 0);

        int width = rectangle.Width;
        width -= 150;
        int height = rectangle.Height;

        for (int r = 0; r < map.Length; r++)
        {
            Room room = map[r];
            Point bottomLeft = room.bounds.BottomLeft().ToPoint();
            Point offset = rectangle.Top().ToPoint();

            int tileX = offset.X;
            int tileY = offset.Y;

            bottomLeft.X += tileX;
            bottomLeft.Y += tileY;
            bottomLeft.Y -= map[0].bounds.Height;

            
            /*
            if (VeilGen.IsTileNearby(bottomLeft.X, bottomLeft.Y, 25, TileSets.BlockMineshafts))
                continue;
            */
            Structurizer.ReadStruct(bottomLeft, room.prefab, Structurizer.DefaultTileBlend);
            Structurizer.ProtectStructure(bottomLeft, room.prefab);
        }
        return true;
    }
}