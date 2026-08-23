using Stellamod.Content.Areas.RoyalCapital;
using Terraria;
using Terraria.Graphics.Capture;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Terror
{
    public class CrimsonBridewellBiome : BaseUrdveilBiome
    {
        public override ModWaterStyle WaterStyle => ModContent.GetInstance<StarbloomWaterStyle>();
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<NoBackgroundStyle>();
        public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Normal;

        // Select Music
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeLow;

        public override int Music
        {
            get
            {
                return MusicLoader.GetMusicSlot(Mod, "Assets/Music/Aegislav");
            }
        }


        public override string BestiaryIcon => base.BestiaryIcon;
        public override string BackgroundPath => base.BackgroundPath;
        public override Color? BackgroundColor => base.BackgroundColor;

        public override bool IsBiomeActive(Player player) => BiomeTileCounts.InCrimsonBridewell;
        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            player.GetModPlayer<BiomePlayer>().ZoneCrimsonBridewell = true;
            if (Main.netMode == NetmodeID.Server)
                return;
            return;
        }
        public override void OnLeave(Player player)
        {
            player.GetModPlayer<BiomePlayer>().ZoneCrimsonBridewell = false;
            if (Main.netMode == NetmodeID.Server)
                return;
            return;
        }
    }
}
