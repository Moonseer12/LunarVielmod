using Stellamod.WorldG;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Stellamod.Content.Areas.SpringHills;

public class StoneGolemCavePass : GenPass
{
    public StoneGolemCavePass() : base("Stone Golem Cave", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Stone Golem Cave";

        bool placed = false;
        int attempts = 0;
        while (!placed && attempts++ < 10000000)
        {
            // Select a place in the first 6th of the world, avoiding the oceans
            int smx = ModContent.GetInstance<VeilGen>().WitchTownLocation.X;
            smx -= 500;

            //Start at 200 tiles above the surface instead of 0, to exclude floating islands
            int smy = (int)(Main.worldSurface - 300);

            // We go down until we hit a solid tile or go under the world's surface
            while (!WorldGen.SolidTile(smx, smy) && smy <= Main.worldSurface)
            {
                smy++;
            }
            smy += 45;
            Point Loc = new(smx, smy + 15);
            string path = "Structures/Overworld/StoneGolemCave";


            var stoneGolemCaveRectangle = Structurizer.ReadRectangle(path);
            Structurizer.ProtectStructure(Loc, path);
            placed = true;


            //Set the default spawn point of the world
            Point spawnLocation = Loc;
            spawnLocation.X += 92;
            spawnLocation.Y -= 44;
            Main.spawnTileX = spawnLocation.X;
            Main.spawnTileY = spawnLocation.Y;

            //Place the Training Grounds
            Point trainingGroundsSpawnPoint = Loc - new Point(0, stoneGolemCaveRectangle.Height);
            Structurizer.ProtectStructure(trainingGroundsSpawnPoint, path);

            //Place the Jiitas Bridge
            string jiitasPath = "Structures/TrainingbridgeJiitas";
            var jiitasRectangle = Structurizer.ReadRectangle(jiitasPath);


            Point jiitasSpawnPoint = trainingGroundsSpawnPoint - new Point(jiitasRectangle.Width, 0);

            //Offset it down by 10 tiles so it's level with the training ground
            jiitasSpawnPoint.Y += 10;
            Structurizer.ProtectStructure(jiitasSpawnPoint, path);
            GenerateFallingWoodenBeams(jiitasRectangle, jiitasSpawnPoint, TileID.BoneBlock);
        }
    }

    public static void GenerateFallingWoodenBeams(Rectangle structureRectangle, Point Loc, int onTileType)
    {
        //Need to substract the height of the rectangle here because of how we place structures
        //They place from the bottom left.
        structureRectangle.Location = Loc - new Point(0, structureRectangle.Height);
        List<Point> tilesToFallFrom = new();
        for (int x = structureRectangle.Location.X;
          x < structureRectangle.Location.X + structureRectangle.Width; x++)
        {
            for (int y = structureRectangle.Location.Y; y < structureRectangle.Location.Y + structureRectangle.Height; y++)
            {
                Tile tile = Main.tile[x, y];
                if (tile.HasTile && tile.TileType == onTileType)
                {
                    tilesToFallFrom.Add(new Point(x, y));
                }
            }
        }

        foreach (var point in tilesToFallFrom)
        {
            int beamX = point.X;
            int beamY = point.Y;
            int solidCount = 0;
            while (solidCount < 5)
            {
                if (!WorldGen.SolidTile(beamX, beamY))
                {
                    WorldGen.PlaceTile(beamX, beamY, TileID.WoodenBeam);
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