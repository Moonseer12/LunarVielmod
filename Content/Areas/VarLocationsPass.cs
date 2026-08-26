using Stellamod.WorldG;
using Terraria;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Stellamod.Content.Areas;

public class VarLocationsPass : GenPass
{
    public VarLocationsPass() : base("World Gen GenVar Locations", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Locking Snow Biome Location";
        Point marshSpot = new();
        marshSpot.Y = (int)(Main.worldSurface - 2000);
        marshSpot.X = 1850;
        marshSpot = TileUtilities.FallToSolidTile(marshSpot.X, marshSpot.Y);
        marshSpot.Y += 25;
        ModContent.GetInstance<VeilGen>().MarshLocation = marshSpot;
        GenVars.jungleOriginX = marshSpot.X + 700;

        //Set snow biome location
        GenVars.snowOriginLeft = ModContent.GetInstance<VeilGen>().WitchTownLocation.X + 4400;
        GenVars.snowOriginRight = GenVars.snowOriginLeft + 1200;

        //Set dungeon and jungle sides
        GenVars.tLeft = GenVars.jungleOriginX;
        GenVars.tRight = GenVars.jungleOriginX + 100;
        GenVars.tTop = Main.maxTilesY / 2;
        GenVars.tBottom = GenVars.tTop + 100;
        GenVars.dungeonSide = 1;

        //Remove the left beach
        GenVars.leftBeachEnd = 100;
        GenVars.shellStartXLeft = 100;
        GenVars.shellStartYLeft = 100;

        Main.spawnTileX = 16;
        Main.spawnTileY = 16;
    }
}