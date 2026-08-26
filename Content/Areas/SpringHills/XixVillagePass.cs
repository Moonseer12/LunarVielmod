using Stellamod.WorldG;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Stellamod.Content.Areas.SpringHills;

public class XixVillageLocPass : GenPass
{
    public XixVillageLocPass() : base("Set Xix Village Location", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        Stopwatch sw = Stopwatch.StartNew();
        progress.Message = "Set Xix Village";
        string path = "Structures/WitchTown";
        var rectangle = Structurizer.ReadRectangle(path);
        // int yOffset = Structurizer.OffsetToGround(path);
        //  Mod.Logger.Debug($"Witch Town Offset to Ground {yOffset}");

        bool placed = false;
        int attempts = 0;
        while (!placed && attempts++ < 10000)
        {
            int centerX = Main.maxTilesX / 2;
            int maxRange = attempts;
            maxRange = Math.Min(1000, maxRange);
            int smx = WorldGen.genRand.Next(centerX - maxRange, centerX + maxRange);
            int smy = (int)(Main.worldSurface - 200);

            // We go down until we hit a solid tile or go under the world's surface
            while (!WorldGen.SolidTile(smx, smy) && smy <= Main.worldSurface)
            {
                smy++;
            }

            // If we went under the world's surface, try again
            if (smy > Main.worldSurface - 20)
            {
                continue;
            }

            //We're checking for surrounding dirt and grass so it doesn't place near ice or desert biomes
            //Rectangles are placed from the bottom left, so subtract half the width to check tiles evenly on both sides
            int width = rectangle.Width * 2;
            Point point = new(smx - width / 2, smy + 50);
            Dictionary<ushort, int> dictionary = new Dictionary<ushort, int>();
            WorldUtils.Gen(point, new Shapes.Rectangle(width, rectangle.Height), new Actions.TileScanner(TileID.Dirt, TileID.Stone).Output(dictionary));
            int stoneAndDirtCount = dictionary[TileID.Dirt] + dictionary[TileID.Stone];
            // 20 * 10 == 200. This is checking that at least 75% of the area is Stone or Dirt
            if (stoneAndDirtCount < 10000)
                continue;

            //Check if sand or snow
            width = rectangle.Width * 4;
            point = new Point(smx - width / 2, smy + 50);
            Dictionary<ushort, int> dictionary2 = new Dictionary<ushort, int>();
            WorldUtils.Gen(point, new Shapes.Rectangle(width, rectangle.Height), new Actions.TileScanner(TileID.Sand, TileID.SnowBlock).Output(dictionary2));
            int sandAndSnow = dictionary2[TileID.Sand] + dictionary2[TileID.SnowBlock];
            if (sandAndSnow >= 1)
                continue;


            Point Loc = new(smx, smy + 57);
            ModContent.GetInstance<VeilGen>().WitchTownLocation = Loc;
            break;
        }
        sw.Stop();
        Console.WriteLine($"Witch Town Location Generation Time {sw.ElapsedMilliseconds}ms");
        Console.WriteLine($"Witch Town Location: {ModContent.GetInstance<VeilGen>().WitchTownLocation}");
    }
}

public class XixVillagePass : GenPass
{
    public XixVillagePass() : base("Xix Village", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Witches spreading love all inside you!";
        string path = "Structures/WitchTown";
        var rectangle = Structurizer.ReadRectangle(path);
        var tileBlend = new int[]
        {
            TileID.RubyGemspark
        };
        Structurizer.ProtectStructure(ModContent.GetInstance<VeilGen>().WitchTownLocation, path);
        for (int x = ModContent.GetInstance<VeilGen>().WitchTownLocation.X; x < ModContent.GetInstance<VeilGen>().WitchTownLocation.X + rectangle.Width; x++)
        {
            for (int y = ModContent.GetInstance<VeilGen>().WitchTownLocation.Y; y < ModContent.GetInstance<VeilGen>().WitchTownLocation.Y + 40; y++)
            {
                if (!WorldGen.SolidTile(x, y))
                {
                    WorldGen.PlaceTile(x, y, TileID.Dirt);
                }
            }
        }

        path = "Structures/DelgrimHill";
        Point delgrimHillPoint = ModContent.GetInstance<VeilGen>().FableHillStartLocation;
        delgrimHillPoint.X += 130;
        delgrimHillPoint.Y -= 10;
        Structurizer.ReadStruct(delgrimHillPoint, path, tileBlend);
        Structurizer.ProtectStructure(delgrimHillPoint, path);

        path = "Structures/EveroseVillage";
        Point everosePoint = ModContent.GetInstance<VeilGen>().FableHillEndLocation;
        everosePoint.X += 8;
        everosePoint = TileUtilities.FallToSolidTile(everosePoint);
        everosePoint.Y += 19;
        Structurizer.ReadStruct(everosePoint, path, tileBlend);
        Structurizer.ProtectStructure(everosePoint, path);
    }
}