using Microsoft.Xna.Framework;
using Stellamod.Content.Biomes;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Assets.Biomes
{
    public class BloodCathedral : ModBiome
    {
        public override int Music
        {
            get
            {
                if (!Main.dayTime)
                {
                    return MusicLoader.GetMusicSlot(Mod, "Assets/Music/BloodCathedral");
                }
                else
                {
                    return -1;
                }
            }
        }
        public override SceneEffectPriority Priority => SceneEffectPriority.BossLow;
        public override string BestiaryIcon => base.BestiaryIcon;
        public override string BackgroundPath => MapBackground;
        public override Color? BackgroundColor => base.BackgroundColor;
        public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("Stellamod/IshtarWaterStyle");
        public override bool IsBiomeActive(Player player) => BiomeTileCounts.InBloodCathedral;
        public override void OnEnter(Player player) => player.GetModPlayer<BiomePlayer>().ZoneBloodCathedral = true;
        public override void OnLeave(Player player) => player.GetModPlayer<BiomePlayer>().ZoneBloodCathedral = false;
    }
}