using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown
{
    public class PunkerTown : BaseUrdveilBiome
    {
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/PunkerTown");
        public override SceneEffectPriority Priority => SceneEffectPriority.BossLow;
        public override string BestiaryIcon => base.BestiaryIcon;
        public override string BackgroundPath => MapBackground;
        public override Color? BackgroundColor => base.BackgroundColor;
        public override bool IsBiomeActive(Player player) => BiomeTileCounts.InGovheil;
        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            player.GetModPlayer<BiomePlayer>().ZonePunkerTown = true;
        }
        public override void OnLeave(Player player) => player.GetModPlayer<BiomePlayer>().ZonePunkerTown = false;
    }
}