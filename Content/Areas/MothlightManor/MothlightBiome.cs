using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.MothlightManor
{
    public class MothlightBiome : BaseUrdveilBiome
    {
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/MothlightManor");
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
        public override string BestiaryIcon => base.BestiaryIcon;
        public override string BackgroundPath => MapBackground;
        public override Color? BackgroundColor => base.BackgroundColor;
        public override bool IsBiomeActive(Player player) => BiomeTileCounts.InMothlight;
        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            player.GetModPlayer<BiomePlayer>().ZoneMothlight = true;
        }
        public override void OnLeave(Player player) => player.GetModPlayer<BiomePlayer>().ZoneMothlight = false;
    }
}