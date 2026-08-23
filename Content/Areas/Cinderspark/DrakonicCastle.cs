using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark
{
    public class DrakonicManor : BaseUrdveilBiome
    {
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/Cinderspark");
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
        public override string BestiaryIcon => base.BestiaryIcon;
        public override string BackgroundPath => MapBackground;
        public override Color? BackgroundColor => base.BackgroundColor;
        public override bool IsBiomeActive(Player player) => BiomeTileCounts.InManor;
        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            player.GetModPlayer<BiomePlayer>().ZoneDrakonic = true;
        }
        public override void OnLeave(Player player) => player.GetModPlayer<BiomePlayer>().ZoneDrakonic = false;
    }
}