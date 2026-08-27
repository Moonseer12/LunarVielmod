using Stellamod.Content.Areas;
using Stellamod.Content.Areas.Cinderspark;
using Stellamod.Content.Areas.Desert;
using Stellamod.Content.Areas.Dungeon;
using Stellamod.Content.Areas.Fable;
using Stellamod.Content.Areas.Illuria;
using Stellamod.Content.Areas.Junkyard;
using Stellamod.Content.Areas.PunkerTown;
using Stellamod.Content.Areas.RoyalCapital;
using Stellamod.Content.Areas.SpringHills;
using Stellamod.Content.Areas.Terror;
using Stellamod.Content.Areas.Tundra.Abyss;
using Stellamod.Content.Areas.Tundra.MoonspiralTower;
using Stellamod.Content.Areas.Tundra.Snow;
using Stellamod.Content.Areas.Underground;
using Stellamod.Content.Areas.WaterSide;
using Stellamod.Content.Areas.WondrousDarkspace;
using Stellamod.Content.Areas.WorldsEnd;
using System.Collections.Generic;
using Terraria.GameContent.Biomes.Desert;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Stellamod.WorldG;

public class PassWriter
{
    private int _insertionIndex;
    public PassWriter(List<GenPass> tasks)
    {
        this.Tasks = tasks;
    }
    public readonly List<GenPass> Tasks;

    public void SetInsertionIndex(int index)
    {
        _insertionIndex = index;
    }
    public void SetInsertionIndex(string passName) => SetInsertionIndex(Tasks.FindIndex(genpass => genpass.Name.Equals(passName)));
    public void NextPass(GenPass genPass)
    {
        _insertionIndex++;
        Tasks.Insert(_insertionIndex, genPass);

    }
    public void DisablePass(string passName)
    {
        Tasks[Tasks.FindIndex(genpass => genpass.Name.Equals(passName))].Disable();

    }
    public void ReplacePass(GenPass genPass)
    {
        Tasks[_insertionIndex] = genPass;
    }
}


public partial class StellaWorld : ModSystem
{
    public override void Load()
    {
        base.Load();
        On_DesertDescription.CreateFromPlacement += ClampHive;
    }

    private DesertDescription ClampHive(On_DesertDescription.orig_CreateFromPlacement orig, Point origin)
    {
        var description = orig(origin);

        //TODO:
        /*
        Rectangle hiveRect = description.Hive;
        hiveRect.Height = DarkspaceStart - (int)Main.worldSurface;
        hiveRect.Height -= 32;
        description.Hive = hiveRect;*/
        return description;
    }

    public static void DisableGenTask(List<GenPass> tasks, string passName)
    {
        tasks.Find(x => x.Name.Equals(passName)).Disable();
    }

    public static void AddNewGenerationPasses(List<GenPass> tasks)
    {
        PassWriter passWriter = new(tasks);
        passWriter.DisablePass("Traps");

        passWriter.SetInsertionIndex("Ocean Sand");
        passWriter.NextPass(new ReworkedOceanSandPass());

        passWriter.SetInsertionIndex("Beaches");
        passWriter.NextPass(new ReworkedBeachesPass());

        passWriter.SetInsertionIndex("Reset");
        passWriter.NextPass(new ForceCrimsonPass());

        passWriter.SetInsertionIndex("Terrain");
        passWriter.NextPass(new VanillaTerrainPass());
        passWriter.NextPass(new InitializePyrPass());
        passWriter.NextPass(new XixVillageLocPass());
        passWriter.NextPass(new VarLocationsPass());
        passWriter.NextPass(new FableTerrainPass());
        passWriter.NextPass(new MarshPass());
        passWriter.NextPass(new VeizalHillTerrainPass());
        passWriter.NextPass(new MistyDungeonHillPass());
        passWriter.NextPass(new RoyalCapitalTerrainPass());
        passWriter.NextPass(new CindersparkPass());
        passWriter.NextPass(new CindersparkCavesPass());
        passWriter.NextPass(new TreeCavesPass());

        passWriter.SetInsertionIndex("Shimmer");
        passWriter.NextPass(new ShimmerSpotPass());

        passWriter.SetInsertionIndex("Planting Trees");
        passWriter.NextPass(new MarshTreesPass());

        passWriter.SetInsertionIndex("Micro Biomes");
        passWriter.DisablePass("Micro Biomes");
        passWriter.NextPass(new WorldsEndPass());
        passWriter.NextPass(new OresPass());
        passWriter.NextPass(new IlluriaPass());
        passWriter.NextPass(new RoyalCapitalPass());
        passWriter.NextPass(new VeizalHillPass());
        passWriter.NextPass(new FablePass());
        passWriter.NextPass(new RysaHousePass());
        passWriter.NextPass(new MistyDungeonPass());
        passWriter.NextPass(new MarshHousingPass());
        passWriter.NextPass(new AegislavPass());
        passWriter.NextPass(new WaterWobbleCavePass());
        passWriter.NextPass(new CraftsMenCavesPass());
        passWriter.NextPass(new TreasureTrovePass());
        passWriter.NextPass(new MoonspiralTowerPass());

        passWriter.SetInsertionIndex("Generate Ice Biome");
        passWriter.NextPass(new ReworkedVanillaIceBiomePass());
        passWriter.NextPass(new IceClumpPass());
        passWriter.NextPass(new IceSpikePass());
        passWriter.NextPass(new AbyssPass());
        passWriter.NextPass(new IceCavernPass());
        passWriter.NextPass(new IceHousePass());

        passWriter.SetInsertionIndex("Jungle");
        passWriter.NextPass(new MarshJungleMudPass());
        passWriter.NextPass(new JungleSurfaceCavePass());
        passWriter.NextPass(new DarkspacePass());
        passWriter.NextPass(new HardRocksPass());
        passWriter.NextPass(new RavineCavesPass());
        passWriter.NextPass(new DeepCavesPass());
        passWriter.NextPass(new MineshaftsPass());
        passWriter.NextPass(new ExtraCavesPass());
        passWriter.NextPass(new CavernWatersPass());
        passWriter.NextPass(new DarkstonePass());

        passWriter.SetInsertionIndex("Full Desert");
        passWriter.ReplacePass(new LockDesertPass());

        passWriter.SetInsertionIndex("Final Cleanup");
        passWriter.NextPass(new ShimmerFixPass());
        passWriter.NextPass(new RunicaUnderwaterPass());
        passWriter.NextPass(new JunkyardCavesPass());
        passWriter.NextPass(new ManorPass());
        passWriter.NextPass(new SkullrunnerPass());
        passWriter.NextPass(new DockPass());
        passWriter.NextPass(new AshotiTemplePass());
        passWriter.NextPass(new AurelusTemplePass());
        passWriter.NextPass(new WindmillsPass());
        passWriter.NextPass(new ColosseumPass());
        passWriter.NextPass(new XixVillagePass());
        passWriter.NextPass(new StoneGolemCavePass());
        passWriter.NextPass(new HardWallsPass());
        passWriter.NextPass(new GrassPass());
    }

    public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
    {
        DisableGenTask(tasks, "Terrain");
        DisableGenTask(tasks, "Tunnels");
        DisableGenTask(tasks, "Mount Caves");
        DisableGenTask(tasks, "Surface Caves");
        DisableGenTask(tasks, "Mountain Caves");
        DisableGenTask(tasks, "Generate Ice Biome");
        DisableGenTask(tasks, "Dungeon");
        DisableGenTask(tasks, "Wavy Caves");
        DisableGenTask(tasks, "Living Trees");
        DisableGenTask(tasks, "Dirt Layer Caves");
        DisableGenTask(tasks, "Rock Layer Caves");
        DisableGenTask(tasks, "Small Holes");
        DisableGenTask(tasks, "Corruption");
        DisableGenTask(tasks, "Floating Islands");
        DisableGenTask(tasks, "Shimmer");
        DisableGenTask(tasks, "Jungle Temple");
        DisableGenTask(tasks, "Temple");
        DisableGenTask(tasks, "Lihzahrd Altars");
        DisableGenTask(tasks, "Sand Patches");
        DisableGenTask(tasks, "Dunes");
        DisableGenTask(tasks, "Marble");
        DisableGenTask(tasks, "Granite");
        DisableGenTask(tasks, "Jungle");
        DisableGenTask(tasks, "Wall Variety");
        DisableGenTask(tasks, "Mushroom Patches");
        AddNewGenerationPasses(tasks);
    }
}