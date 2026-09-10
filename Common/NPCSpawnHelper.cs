using Stellamod.Content.Areas.Cinderspark;
using Stellamod.Content.Areas.Desert;
using Stellamod.Content.Areas.Desert.NPCsCL;
using Stellamod.Content.Areas.Fable;
using Stellamod.Content.Areas.Ishtar;
using Stellamod.Content.Areas.PunkerTown;
using Stellamod.Content.Areas.RoyalCapital;
using Stellamod.Content.Areas.SpringHills;
using Stellamod.Content.Areas.Terror;
using Stellamod.Content.Areas.Tundra.Abyss;
using Stellamod.Content.Areas.Tundra.Abyss.EnemiesAB;
using Stellamod.Content.Areas.Underground;
using Stellamod.Content.Areas.WaterSide;
using Stellamod.Content.Areas.WondrousDarkspace;
using Stellamod.Core.NPCHelpers;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common;

/// <summary>
/// Classifies all of our NPCs, keep track of where they spawn and weights
/// </summary>
public class SpawnSets : ModSystem
{
    public override void SetupContent()
    {
        SpringEnemy = new List<int>();
        HarmonicEnemy = new List<int>();
        MarshEnemy = new List<int>();
        AegislavSurfaceEnemy = new List<int>();
        HeatedDepthsEnemy = new List<int>();
        FableEnemy = new List<int>();
        AbyssEnemy = new List<int>();
        AbyssWaterEnemy = new List<int>();
        AbyssCritter = new List<int>();
        AbyssTempleEnemy = new List<int>();
        IshtarEnemy = new();
        UndergroundEnemy = new();
        MineshaftEnemy = new();
        DarkspaceEnemy = new();
        CindersparkEnemy = new();
        DesertEnemy = new();
        RoyalCapitalEnemy = new();
        SnowEnemy = new();
        ModifiedWeights = NPCID.Sets.Factory.CreateFloatSet(1f);
        base.SetupContent();
    }

    public override void ResizeArrays()
    {
        base.ResizeArrays();
        ModifiedWeights = NPCID.Sets.Factory.CreateFloatSet(1f);
        TryNotToSpawnOnWater = NPCID.Sets.Factory.CreateBoolSet();
    }
    public static List<int> SpringEnemy;
    public static List<int> HarmonicEnemy;
    public static List<int> MarshEnemy;
    public static List<int> AegislavSurfaceEnemy;
    public static List<int> HeatedDepthsEnemy;
    public static List<int> FableEnemy;
    public static List<int> AbyssEnemy;
    public static List<int> AbyssCritter;
    public static List<int> AbyssWaterEnemy;
    public static List<int> AbyssTempleEnemy;
    public static List<int> IshtarEnemy;
    public static List<int> UndergroundEnemy;
    public static List<int> MineshaftEnemy;
    public static List<int> DarkspaceEnemy;
    public static List<int> CindersparkEnemy;
    public static List<int> DesertEnemy;
    public static List<int> RoyalCapitalEnemy;
    public static List<int> SnowEnemy;
    public static float[] ModifiedWeights;
    public static bool[] TryNotToSpawnOnWater;

}

public static class NPCSpawnExtensions
{
    extension(NPCID.Sets)
    {
        public static bool[] TryNotToSpawnOnWater => SpawnSets.TryNotToSpawnOnWater;
    }

    public static void PreferLand(this ModNPC npc)
    {
        NPCID.Sets.TryNotToSpawnOnWater[npc.Type] = true;
    }


    //Wrapper functions for this functionality just incase we want to change how this works
    public static void AddToSpringHills(this ModNPC npc)
    {
        SpawnSets.SpringEnemy.Add(npc.Type);
    }

    public static void AddToMarsh(this ModNPC npc)
    {
        SpawnSets.MarshEnemy.Add(npc.Type);
    }

    public static void AddToHarmonicCoralways(this ModNPC npc)
    {
        SpawnSets.HarmonicEnemy.Add(npc.Type);
    }

    public static void AddToHeatedDepths(this ModNPC npc)
    {
        SpawnSets.HeatedDepthsEnemy.Add(npc.Type);
    }

    public static void AddToFable(this ModNPC npc)
    {
        SpawnSets.FableEnemy.Add(npc.Type);
    }

    public static void AddToAbyss(this ModNPC npc)
    {
        SpawnSets.AbyssEnemy.Add(npc.Type);
    }

    public static void AddToAbyssCritter(this ModNPC npc)
    {
        SpawnSets.AbyssCritter.Add(npc.Type);
    }

    public static void AddToAbyssTemple(this ModNPC npc)
    {
        SpawnSets.AbyssTempleEnemy.Add(npc.Type);
    }

    public static void AddToIshtar(this ModNPC npc)
    {
        SpawnSets.IshtarEnemy.Add(npc.Type);
    }

    public static void AddToUnderground(this ModNPC npc)
    {
        SpawnSets.UndergroundEnemy.Add(npc.Type);
    }

    public static void AddToMineshaft(this ModNPC npc)
    {
        SpawnSets.MineshaftEnemy.Add(npc.Type);
    }

    public static void AddToDarkspace(this ModNPC npc)
    {
        SpawnSets.DarkspaceEnemy.Add(npc.Type);
    }

    public static void AddToCinderspark(this ModNPC npc)
    {
        SpawnSets.CindersparkEnemy.Add(npc.Type);
    }

    public static void AddToDesert(this ModNPC npc)
    {
        SpawnSets.DesertEnemy.Add(npc.Type);
    }

    public static void AddToRoyalCapital(this ModNPC npc)
    {
        SpawnSets.RoyalCapitalEnemy.Add(npc.Type);
    }

    public static void AddToSnow(this ModNPC npc)
    {
        SpawnSets.SnowEnemy.Add(npc.Type);
    }

    public static void ModifySpawnWeight(this ModNPC npc, float multiplier)
    {
        SpawnSets.ModifiedWeights[npc.Type] = multiplier;
    }
}

public class NPCSpawnHelper : GlobalNPC
{
    private void AddEnemiesFromSpawnSet(List<int> set, IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
    {
        for (int i = 0; i < set.Count; i++)
        {
            int enemyType = set[i];
            if (spawnInfo.Water && NPCID.Sets.TryNotToSpawnOnWater[enemyType])
                continue;

            float totalWeight = 1f;
            float weight = totalWeight / (float)set.Count;

            //If we want to make an enemy rarer we'd do it here
            weight *= SpawnSets.ModifiedWeights[enemyType];
            pool.TryAdd(enemyType, weight);
        }
    }

    public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
    {
        base.EditSpawnRate(player, ref spawnRate, ref maxSpawns);
        //More towns people
        if (Main.dayTime && player.InModBiome<DesertTownBiome>())
        {
            float spRate = spawnRate;
            spawnRate = (int)(spRate * 0.3f);
            maxSpawns *= 2;
        }
        if (player.InModBiome<AbyssBiome>())
        {
            float sp = (float)spawnRate;
            sp *= 0.6f;
       //     spawnRate = (int)sp;


            float ms = (float)maxSpawns;
            ms *= 1.4f;
//maxSpawns = (int)ms;
        }
    }

    public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
    {
        base.EditSpawnPool(pool, spawnInfo);
        if (spawnInfo.Player.ZoneDesert)
        {
            if (Main.dayTime)
            {
                pool.Clear();
                pool.TryAdd(ModContent.NPCType<DesertPerson>(), 0.3f);
            }
            else
                AddEnemiesFromSpawnSet(SpawnSets.DesertEnemy, pool, spawnInfo);
        }
        if (spawnInfo.Player.ZoneForest || spawnInfo.Player.ZonePurity || spawnInfo.Player.InModBiome<SpringHillsBiome>())
        {
            AddEnemiesFromSpawnSet(SpawnSets.SpringEnemy, pool, spawnInfo);
        }
        if (spawnInfo.Player.InModBiome<BiomeMarsh>())
        {
            AddEnemiesFromSpawnSet(SpawnSets.MarshEnemy, pool, spawnInfo);
        }
        if (spawnInfo.Player.InModBiome<HarmonicCoralwaysBiome>())
        {
            pool.Clear();
            AddEnemiesFromSpawnSet(SpawnSets.HarmonicEnemy, pool, spawnInfo);
            pool.TryAdd(NPCID.Piranha, 0.1f);
            pool.TryAdd(NPCID.Shark, 0.1f);
            pool.TryAdd(NPCID.BlueJellyfish, 0.1f);
            pool.TryAdd(NPCID.PinkJellyfish, 0.1f);
            pool.TryAdd(NPCID.Squid, 0.1f);
            pool.TryAdd(NPCID.Crab, 0.1f);
        }
        if (spawnInfo.Player.InModBiome<AegislavBiome>())
        {
            pool.Clear();
            AddEnemiesFromSpawnSet(SpawnSets.AegislavSurfaceEnemy, pool, spawnInfo);
            pool.TryAdd(NPCID.BloodCrawler, 0.1f);
            pool.TryAdd(NPCID.FaceMonster, 0.1f);
            pool.TryAdd(NPCID.Crimera, 0.1f);
        }
        if (spawnInfo.Player.InModBiome<HeatedDepthsBiome>())
        {
            AddEnemiesFromSpawnSet(SpawnSets.HeatedDepthsEnemy, pool, spawnInfo);
        }
        if (spawnInfo.Player.InModBiome<FableBiome>())
        {
            AddEnemiesFromSpawnSet(SpawnSets.FableEnemy, pool, spawnInfo);
        }
        if (spawnInfo.Player.InModBiome<AbyssBiome>())
        {
            pool.Clear();
       
            if(!BellFlowerSystem.Whispering)
                AddEnemiesFromSpawnSet(SpawnSets.AbyssEnemy, pool, spawnInfo);
            AddEnemiesFromSpawnSet(SpawnSets.AbyssCritter, pool, spawnInfo);
        }
        if (spawnInfo.Player.InModBiome<AurelusBiome>())
        {
            pool.Clear();
            AddEnemiesFromSpawnSet(SpawnSets.AbyssTempleEnemy, pool, spawnInfo);
        }
        if (spawnInfo.Player.InModBiome<IshtarBiome>())
        {
            AddEnemiesFromSpawnSet(SpawnSets.IshtarEnemy, pool, spawnInfo);
        }
        if (spawnInfo.Player.ZoneNormalCaverns)
        {
            AddEnemiesFromSpawnSet(SpawnSets.UndergroundEnemy, pool, spawnInfo);
        }
        if (spawnInfo.Player.InModBiome<MineshaftBiome>())
        {
            AddEnemiesFromSpawnSet(SpawnSets.MineshaftEnemy, pool, spawnInfo);
        }
        if (spawnInfo.Player.InModBiome<WonderousDarkspaceBiome>())
        {
            AddEnemiesFromSpawnSet(SpawnSets.DarkspaceEnemy, pool, spawnInfo);
        }
        if (spawnInfo.Player.InModBiome<CindersparkBiome>())
        {
            AddEnemiesFromSpawnSet(SpawnSets.CindersparkEnemy, pool, spawnInfo);
        }
        if (spawnInfo.Player.InModBiome<AlcadziaBiome>())
        {
            AddEnemiesFromSpawnSet(SpawnSets.RoyalCapitalEnemy, pool, spawnInfo);
        }
        if (spawnInfo.Player.ZoneSnow)
        {
            AddEnemiesFromSpawnSet(SpawnSets.SnowEnemy, pool, spawnInfo);
        }
    }
}