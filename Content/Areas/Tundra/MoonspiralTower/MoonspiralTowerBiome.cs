using Terraria;
using Terraria.Graphics.Capture;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.MoonspiralTower
{
    public class MoonspiralTowerBiome : BaseUrdveilBiome
    {
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<StarbloomBackgroundStyle>();
        public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Normal;
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
        public override int Music
        {
            get
            {
                return MusicLoader.GetMusicSlot(Mod, "Assets/Music/MoonspiralTower");
            }
        }
        public override string BestiaryIcon => base.BestiaryIcon;
        public override string BackgroundPath => base.BackgroundPath;
        public override Color? BackgroundColor => base.BackgroundColor;
        public override bool IsBiomeActive(Player player) => BiomeTileCounts.InMoonspiralTower;
        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            player.GetModPlayer<BiomePlayer>().ZoneMoonspiralTower = true;
        }
        public override void OnLeave(Player player) => player.GetModPlayer<BiomePlayer>().ZoneMoonspiralTower = false;
    }
}