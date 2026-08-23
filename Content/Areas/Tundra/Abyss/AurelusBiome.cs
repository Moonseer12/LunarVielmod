using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss
{
    public class AurelusBiome : BaseUrdveilBiome
    {
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/AurelusTemple");
        public override SceneEffectPriority Priority => SceneEffectPriority.BossLow;
        public override string BestiaryIcon => base.BestiaryIcon;
        public override string BackgroundPath => MapBackground;
        public override Color? BackgroundColor => base.BackgroundColor;
        public override bool IsBiomeActive(Player player) => BiomeTileCounts.InAurelus;
        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            player.GetModPlayer<BiomePlayer>().ZoneAurelus = true;
        }
        public override void OnLeave(Player player) => player.GetModPlayer<BiomePlayer>().ZoneAurelus = false;
    }
}