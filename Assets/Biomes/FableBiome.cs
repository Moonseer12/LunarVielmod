using Microsoft.Xna.Framework;
using Stellamod.Backgrounds;
using Stellamod.Content.Areas.Ishtar;
using Stellamod.Content.Biomes;
using Stellamod.Core.Biomes;
using Terraria;
using Terraria.Graphics.Capture;
using Terraria.ModLoader;

namespace Stellamod.Assets.Biomes
{
    public class FableBiome : BaseUrdveilBiome
    {
        public bool IsPrimaryBiome = true;
        public override ModWaterStyle WaterStyle => ModContent.GetInstance<IshtarWaterStyle>();
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<FabledBackgroundStyle>();
        public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Normal;
        public override int Music
        {
            get
            {
                if (Main.dayTime)
                {
                    return MusicLoader.GetMusicSlot(Mod, "Assets/Music/OggFabledWilds");
                }
                else
                {
                    return MusicLoader.GetMusicSlot(Mod, "Assets/Music/LightedFable");
                }
            }
        }
        public override SceneEffectPriority Priority => SceneEffectPriority.BossLow;
        public override string BestiaryIcon => base.BestiaryIcon;
        public override string BackgroundPath => base.BackgroundPath;
        public override Color? BackgroundColor => base.BackgroundColor;
        public override bool IsBiomeActive(Player player) => BiomeTileCounts.InFable;
        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            player.GetModPlayer<BiomePlayer>().ZoneFable = true;
        }
        public override void OnLeave(Player player)
        {
            base.OnLeave(player);
            player.GetModPlayer<BiomePlayer>().ZoneFable = false;
        }
    }
}