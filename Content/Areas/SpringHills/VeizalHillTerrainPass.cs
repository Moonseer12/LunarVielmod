using System;
using Stellamod.WorldG;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Stellamod.Content.Areas.SpringHills;

public class VeizalHillTerrainPass : GenPass
{
    public VeizalHillTerrainPass() : base("Veizal Hill Terrain", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Veizal Hills Terrain";
        Point startHillTile = ModContent.GetInstance<VeilGen>().MarshLocation;
        startHillTile.X += 1400;
        Point endHillTile = startHillTile;
        endHillTile.X += 900;

        startHillTile.Y -= 90;
        while (WorldGen.InWorld(endHillTile.X, endHillTile.Y) && !WorldGen.SolidTile(endHillTile.X, endHillTile.Y))
        {
            endHillTile.Y++;
        }


        //Move the start tile backwards so it connects to the marsh
        while (WorldGen.InWorld(startHillTile.X, startHillTile.Y) && !WorldGen.SolidTile(endHillTile.X, endHillTile.Y))
        {
            startHillTile.X--;
        }


        Point waterLakeStart = new();
        waterLakeStart.X = (int)MathHelper.Lerp(startHillTile.X, endHillTile.X, 0.2f);
        waterLakeStart.Y = (int)(Main.worldSurface - 200);

        Point waterLakeEnd = new();
        waterLakeEnd.X = (int)MathHelper.Lerp(startHillTile.X, endHillTile.X, 0.4f);
        waterLakeEnd.Y = (int)(Main.worldSurface - 200);



        //Move a bit more into the hill so it's more cleanly integrated
        startHillTile.X -= 80;

        ModContent.GetInstance<VeilGen>().VeizalHillStartLcoation = startHillTile;
        ModContent.GetInstance<VeilGen>().VeizalHillEndLocation = endHillTile;
        for (int x = startHillTile.X; x < endHillTile.X; x++)
        {
            //Calculate heights, creating a slowly descending slope
            float width = endHillTile.X - startHillTile.X;
            float ratio = (x - startHillTile.X) / width;

            float tileYHeight = MathHelper.Lerp(startHillTile.Y, endHillTile.Y, ratio);

            //Create some signing motions for variance in the terrain
            tileYHeight += MathF.Sin(ratio * 4.0f) * 16;
            tileYHeight += MathF.Sin(ratio * 8.0f + 0.5f) * 2f;
            tileYHeight += MathF.Sin(ratio * 16.0f + 0.75f) * 5;
            int y = (int)tileYHeight;

            while (WorldGen.InWorld(x, y) && !WorldGen.SolidTile(x, y))
            {
                if (!Main.tileSolid[Main.tile[x, y].TileType])
                    WorldGen.KillTile(x, y);
                WorldGen.PlaceTile(x, y, TileID.Dirt);
                y++;
            }
        }
        VeilGen.GenerateBowlLake(waterLakeStart, waterLakeEnd, maxLakeDepth: 65);
    }
}

public class VeizalHillPass : GenPass
{
    public VeizalHillPass() : base("Veizal Hill", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Hills and Veizal's House";

        //Place Veizal Manor
        StructureMap structures = GenVars.structures;
        string structure = "Structures/Overworld/VeizalManor";
        Rectangle rectangle = Structurizer.ReadRectangle(structure);
        progress.Message = "WE'RE RICH!";
        int[] tileBlend = new int[]
        {
            TileID.RubyGemspark
        };
        int maxAttemptCount = 1000;
        for (int a = 0; a < maxAttemptCount; a++)
        {
            // Select a place in the first 6th of the world, avoiding the oceans
            int x = (int)MathHelper.Lerp(ModContent.GetInstance<VeilGen>().VeizalHillStartLcoation.X, ModContent.GetInstance<VeilGen>().VeizalHillEndLocation.X, 0.7f);
            int y = (int)(Main.worldSurface - 500);
            Point tileToPlaceOn = TileUtilities.FallToSolidTile(x, y);
            int cathedralY = tileToPlaceOn.Y;

            //Start at 200 tiles above the surface instead of 0, to exclude floating islands
            Point Loc = tileToPlaceOn;
            if (!Structurizer.TryPlaceAndProtectStructure(Loc, structure))
                continue;
            Structurizer.ReadStruct(Loc, structure, tileBlend);
            Rectangle structureRectangle = Structurizer.ReadRectangle(structure);
            structureRectangle.Location = Loc;
            for (int beamX = structureRectangle.Location.X;
                beamX < structureRectangle.Location.X + structureRectangle.Width; beamX += 4)
            {
                //Place beams
                int beamY = structureRectangle.Location.Y;
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