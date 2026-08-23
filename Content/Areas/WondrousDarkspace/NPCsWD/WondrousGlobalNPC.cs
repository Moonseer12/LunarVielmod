using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WondrousDarkspace.NPCsWD
{
    public class WondrousGlobalNPC : GlobalNPC
    {
        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            base.EditSpawnPool(pool, spawnInfo);
            if (!spawnInfo.Player.GetModPlayer<BiomePlayer>().ZoneWonder)
                return;
        }

        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            base.EditSpawnRate(player, ref spawnRate, ref maxSpawns);
            if (!player.GetModPlayer<BiomePlayer>().ZoneWonder)
                return;
            spawnRate = (int)(spawnRate * 0.5f);
            maxSpawns = (int)(maxSpawns * 1.5f);
        }
    }
}