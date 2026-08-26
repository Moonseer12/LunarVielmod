using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace Stellamod.Content.Areas.SpringHills;

public class DockPass : GenPass
{
    public DockPass() : base("Dock", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Fishing for femboys";
        int dockX = Main.maxTilesX - 1;
        int dockY = (int)Main.worldSurface - 1000;

        //Get the edge of the right ocean
        Tile dockTile = Main.tile[dockX, dockY];
        while (dockTile.LiquidAmount <= 0)
        {
            dockY++;
            dockTile = Main.tile[dockX, dockY];
        }

        while (dockTile.LiquidAmount > 0)
        {
            dockX--;
            dockTile = Main.tile[dockX, dockY];
        }

        //Place the structure
        Point dockLoc = new(dockX, dockY + 1);
        dockLoc.Y -= 7;

        string structure = "Structures/Overworld/TheDock";

        dockLoc.X += 300;
        Rectangle structureRectangle = Structurizer.ReadRectangle(structure);
        structureRectangle.Location = dockLoc;
        for (int beamX = structureRectangle.Location.X;
            beamX < structureRectangle.Location.X + structureRectangle.Width; beamX++)
        {
            //Place beams
            int beamY = structureRectangle.Location.Y;
            if (beamX < Main.maxTilesX && beamY < Main.maxTilesY)
            {

                Tile tile = Main.tile[beamX, beamY];
                if (tile.TileType != TileID.Sunplate)
                    continue;
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
}