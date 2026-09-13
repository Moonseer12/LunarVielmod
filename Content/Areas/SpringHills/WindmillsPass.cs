using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Stellamod.Content.Areas.SpringHills;

public class WindmillsPass : GenPass
{
    public WindmillsPass() : base("Windmills", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Adding life to the world!";
        bool placed = false;
        int attempts = 0;
        Point windmillPlacementTile = new();
        windmillPlacementTile.X = (int)MathHelper.Lerp(ModContent.GetInstance<VeilGen>().MistyHillStartLocation.X, ModContent.GetInstance<VeilGen>().MistyHillEndLocation.X, 0.45f);
        windmillPlacementTile.Y = (int)(Main.worldSurface - 1200);
        windmillPlacementTile = TileUtilities.FallToSolidTile(windmillPlacementTile.X, windmillPlacementTile.Y);
        while (!placed && attempts++ < 10000000)
        {
            string structure = "Structures/Overworld/Windmill";
            Rectangle structureRectangle = Structurizer.ReadRectangle(structure);
            structureRectangle.Location = windmillPlacementTile;
            for (int beamX = structureRectangle.Location.X;
                beamX < structureRectangle.Location.X + structureRectangle.Width; beamX += 4)
            {
                int beamY = structureRectangle.Location.Y;
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

            placed = true;
        }
    }
}