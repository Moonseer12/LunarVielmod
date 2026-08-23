using Stellamod.Content.Areas.Fable;
using Stellamod.Content.Areas.RoyalCapital;
using Stellamod.Content.Areas.Terror;
using Stellamod.Content.Areas.WorldsEnd;
using Terraria;
using Terraria.Graphics.Capture;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills
{
    public class SpringHillsBiome : BaseUrdveilBiome
    {
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<NoBackgroundStyle>();
        public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Normal;
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
        public override int Music
        {
            get
            {
                int music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/SpringFields");
                if (!BiomeTileCounts.InSpringHills)
                    return -1;
                return music;
            }
        }
        public override bool IsBiomeActive(Player player)
        {
            bool isaActive = BiomeTileCounts.InForest && player.ZoneOverworldHeight || BiomeTileCounts.InSpringHills;
            if (!isaActive)
                return false;
            if (player.InModBiome<FableBiome>())
                return false;
            if (player.InModBiome<AlcadziaBiome>())
                return false;
            if (player.InModBiome<WorldsEndBiome>())
                return false;
            if (player.InModBiome<AegislavBiome>())
                return false;
            if (player.InModBiome<XixVillageBiome>())
                return false;
            if (player.ZoneBeach)
                return false;
            if (player.ZoneSnow)
                return false;
            if (player.InZonePurity())
                return true;
            return true;
        }
        public override string BestiaryIcon => base.BestiaryIcon;
        public override string BackgroundPath => base.BackgroundPath;
        public override Color? BackgroundColor => base.BackgroundColor;
        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            player.GetModPlayer<BiomePlayer>().ZoneSpringHills = true;
        }
        public override void OnLeave(Player player) => player.GetModPlayer<BiomePlayer>().ZoneSpringHills = false;
    }
}