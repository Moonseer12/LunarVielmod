using Stellamod.WorldG;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Stellamod.Content.Areas.Fable;

public class FableTerrainPass : GenPass
{
    public FableTerrainPass() : base("FableTerrain", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Fable Terrain";
        //Calculate the starting location
        Point startHillTile = ModContent.GetInstance<VeilGen>().WitchTownLocation;
        startHillTile.X += 300;
        startHillTile.Y -= 200;
        startHillTile = TileUtilities.FallToSolidTile(startHillTile.X, startHillTile.Y);
        startHillTile.Y += 36;
        ModContent.GetInstance<VeilGen>().FableHillStartLocation = startHillTile;

        //Calculate the ending location
        Point endHillTile = startHillTile;
        endHillTile.X += 1000;
        endHillTile.Y -= 200;
        endHillTile = TileUtilities.FallToSolidTile(endHillTile.X, endHillTile.Y);
        endHillTile.Y += 10;
        ModContent.GetInstance<VeilGen>().FableHillEndLocation = endHillTile;

        float hillHeight = 200;
        float width = endHillTile.X - startHillTile.X;
        for (int x = startHillTile.X; x < endHillTile.X; x++)
        {
            float ratio = (x - startHillTile.X) / width;
            float height = (int)(VeilGen.GetFableHillHeight(ratio) * hillHeight);
            for (int y = 0; y < height; y++)
            {
                WorldGen.PlaceTile(x, startHillTile.Y - y, TileID.Dirt);
            }
        }
        //  WorldGen

        //Place the fable
        Point placementTile = new();
        placementTile.X = (int)MathHelper.Lerp(startHillTile.X, endHillTile.X, 0.6f);
        placementTile.Y = (int)(Main.worldSurface - 400);
        placementTile = TileUtilities.FallToSolidTile(placementTile.X, placementTile.Y);
        placementTile += new Point(10, 53);

        ModContent.GetInstance<VeilGen>().FableLocation = placementTile;




        //Placing a falling off slope at the end of the structure
        Rectangle fableRect = Structurizer.ReadRectangle(StructureAssets.Fable);
        Point fableFalloffStart = ModContent.GetInstance<VeilGen>().FableLocation + new Point(fableRect.Width, 0);
        fableFalloffStart.Y -= 54;
        fableFalloffStart.X -= 20;

        Point fableFalloffEnd = fableFalloffStart;
        fableFalloffEnd.X += 150;
        fableFalloffEnd = TileUtilities.FallToSolidTile(fableFalloffEnd.X, fableFalloffEnd.Y);
        fableFalloffEnd.Y += 10;

        width = fableFalloffEnd.X - fableFalloffStart.X;
        for (int x = fableFalloffStart.X; x < fableFalloffEnd.X; x++)
        {
            float ratio = (x - fableFalloffStart.X) / width;
            int startY = (int)MathHelper.SmoothStep(fableFalloffStart.Y, fableFalloffEnd.Y, ratio);
            Point tilePlace = new Point(x, startY);
            for (int y = startY; y < fableFalloffEnd.Y; y++)
            {
                WorldGen.PlaceTile(tilePlace.X, y, TileID.Dirt);
            }
        }

        ModContent.GetInstance<VeilGen>().FableFarEdgeLocation = fableFalloffEnd;
    }
}

public class FablePass : GenPass
{
    public FablePass() : base("HillsnFable", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Daedus is Reading Books...";
        Structurizer.PlaceAndProtect(new StructurePlacementParams
        {
            tile = ModContent.GetInstance<VeilGen>().FableLocation,
            structurePath = StructureAssets.Fable,
            tileBlend = Structurizer.DefaultTileBlend
        });

        //Placing a falling off slope at the end of the structure
        Rectangle fableRect = Structurizer.ReadRectangle(StructureAssets.Fable);
        Point fableFalloffStart = ModContent.GetInstance<VeilGen>().FableLocation + new Point(fableRect.Width, 0);
        fableFalloffStart.Y -= 54;
        fableFalloffStart.X -= 20;

        Point fableFalloffEnd = fableFalloffStart;
        fableFalloffEnd.X += 150;
        fableFalloffEnd = TileUtilities.FallToSolidTile(fableFalloffEnd.X, fableFalloffEnd.Y);
        fableFalloffEnd.Y += 10;

        float width = fableFalloffEnd.X - fableFalloffStart.X;
        for (int x = fableFalloffStart.X; x < fableFalloffEnd.X; x++)
        {
            float ratio = (x - fableFalloffStart.X) / width;
            int startY = (int)MathHelper.SmoothStep(fableFalloffStart.Y, fableFalloffEnd.Y, ratio);
            Point tilePlace = new(x, startY);
            for (int y = startY; y < fableFalloffEnd.Y; y++)
            {
                WorldGen.PlaceTile(tilePlace.X, y, TileID.Dirt);
            }
        }

        ModContent.GetInstance<VeilGen>().FableFarEdgeLocation = fableFalloffEnd;
    }
}