using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.BloodCathedral
{
    public class BloodCathedral : BaseUrdveilBiome
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
        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            player.GetModPlayer<BiomePlayer>().ZoneBloodCathedral = true;
        }
        public override void OnLeave(Player player) => player.GetModPlayer<BiomePlayer>().ZoneBloodCathedral = false;
    }
}