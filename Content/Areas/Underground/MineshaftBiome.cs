using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Underground
{
    public class MineshaftBiome : BaseUrdveilBiome
    {
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/Catacombs");
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeLow;
        public override string BestiaryIcon => base.BestiaryIcon;
        public override string BackgroundPath => MapBackground;
        public override Color? BackgroundColor => base.BackgroundColor;
        public override bool IsBiomeActive(Player player) => (player.ZoneRockLayerHeight || player.ZoneDirtLayerHeight) && BiomeTileCounts.InMineshaft;
        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            player.GetModPlayer<BiomePlayer>().ZoneMineshaft = true;
        }
        public override void OnLeave(Player player) => player.GetModPlayer<BiomePlayer>().ZoneMineshaft = false;
    }
}