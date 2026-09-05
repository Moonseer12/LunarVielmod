using Stellamod.Content.Areas.RoyalCapital.TilesRC;
using Terraria;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Stellamod.Content.Areas.RoyalCapital;

public class RoyalCapitalTerrainPass : GenPass
{
    public RoyalCapitalTerrainPass() : base("Royal Capital Terrain", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Royal Capital Dirt";

        Point capitalSpot = new(666, 1000);
        capitalSpot = TileUtilities.FallToSolidTile(capitalSpot);
        ModContent.GetInstance<VeilGen>().RoyalCapitalLocation = capitalSpot;
        WorldGen.TileRunner(capitalSpot.X + 260, capitalSpot.Y + 10, 350, 2, ModContent.TileType<StarbloomDirt>(), true, 0f, 0f, true, false);
        WorldGen.TileRunner(capitalSpot.X + 260, capitalSpot.Y + 100, 550, 2, ModContent.TileType<StarbloomDirt>(), true, 0f, 0f, true, true);
        WorldGen.TileRunner(capitalSpot.X + 260, capitalSpot.Y + 250, 350, 2, ModContent.TileType<StarbloomDirt>(), true, 0f, 0f, true, true);
        WorldGen.TileRunner(capitalSpot.X + 260, capitalSpot.Y + 400, 550, 2, ModContent.TileType<StarbloomDirt>(), true, 0f, 0f, true, true);
        WorldGen.TileRunner(capitalSpot.X + 260, capitalSpot.Y + 600, 550, 2, ModContent.TileType<StarbloomDirt>(), true, 0f, 0f, true, true);
    }
}

public class RoyalCapitalPass : GenPass
{
    public RoyalCapitalPass() : base("Royal Capital", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        Rectangle rectangle = Structurizer.ReadRectangle("Struct/Alcad/RoyalCapital3");
        progress.Message = "Fighting the Virulent with magic";
        bool placed = false;
        int attempts = 0;
        while (!placed && attempts++ < 10000000)
        {
            Point Loc = ModContent.GetInstance<VeilGen>().RoyalCapitalLocation;
            rectangle.Location = Loc;
            ModContent.GetInstance<VeilGen>().AlcadLocation = Loc;
            Structurizer.ProtectStructure(Loc, "Structures/RoyalCapital");
            placed = true;
        }
    }
}

public class CraftsMenCavesPass : GenPass
{
    public CraftsMenCavesPass() : base("Craftsman Caves", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Craftsman Tunnels";
        Point caveOrigin = ModContent.GetInstance<VeilGen>().RoyalCapitalLocation;
        caveOrigin.X -= 310;
        caveOrigin.Y += 100;
        GenerationPrefab prefab = ModContent.GetInstance<GenerationTextureManager>().GetPrefab("CraftsmanTunnels");
        prefab.PasteErase(caveOrigin, PrefabPlacementType.FromTopCenter);
    }
}