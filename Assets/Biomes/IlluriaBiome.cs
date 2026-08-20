using Microsoft.Xna.Framework;
using Stellamod.Content.Biomes;
using Stellamod.NPCs.Bosses.Niivi;
using Terraria;
using Terraria.Graphics.Capture;
using Terraria.ModLoader;

namespace Stellamod.Assets.Biomes
{
    public class IlluriaBiome : ModBiome
    {
        public bool IsPrimaryBiome = true;
        public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("Stellamod/StarbloomWaterStyle");
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.Find<ModSurfaceBackgroundStyle>("Stellamod/StarbloomBackgroundStyle");
        public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Normal;
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/TheGreatIlluria");
        public override SceneEffectPriority Priority => SceneEffectPriority.BossLow;
        public override string BestiaryIcon => base.BestiaryIcon;
        public override string BackgroundPath => base.BackgroundPath;
        public override Color? BackgroundColor => base.BackgroundColor;
        public override bool IsBiomeActive(Player player) => BiomeTileCounts.InIlluria || NPC.AnyNPCs(ModContent.NPCType<Niivi>());
        public override void OnEnter(Player player) => player.GetModPlayer<BiomePlayer>().ZoneIlluria = true;
        public override void OnLeave(Player player) => player.GetModPlayer<BiomePlayer>().ZoneIlluria = false;
    }
}