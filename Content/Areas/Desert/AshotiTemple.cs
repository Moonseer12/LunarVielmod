using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Desert
{
    public class AshotiTemple : BaseUrdveilBiome
    {
        public override int Music => MusicID.Temple;
        public override SceneEffectPriority Priority => SceneEffectPriority.BossLow;
        public override string BestiaryIcon => base.BestiaryIcon;
        public override string BackgroundPath => MapBackground;
        public override Color? BackgroundColor => base.BackgroundColor;
        public override bool IsBiomeActive(Player player) => BiomeTileCounts.InAshotiTemple;
        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            player.GetModPlayer<BiomePlayer>().ZoneAshotiTemple = true;
        }
        public override void OnLeave(Player player) => player.GetModPlayer<BiomePlayer>().ZoneAshotiTemple = false;
    }

    public class AshotiTempleNPC : GlobalNPC
    {
        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            base.EditSpawnPool(pool, spawnInfo);
            if (spawnInfo.Player.GetModPlayer<BiomePlayer>().ZoneAshotiTemple)
            {
                pool[NPCID.Lihzahrd] = 3;
                pool[NPCID.FlyingSnake] = 3;
            }
        }
    }
}