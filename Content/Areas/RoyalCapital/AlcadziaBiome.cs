using Terraria;
using Terraria.Graphics.Capture;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital
{
    public class AlcadziaBiome : BaseUrdveilBiome
    {
        public bool IsPrimaryBiome = true;
        public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("Stellamod/StarbloomWaterStyle");
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.Find<ModSurfaceBackgroundStyle>("Stellamod/AlcadziaBackgroundStyle");
        public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Normal;
        public override SceneEffectPriority Priority => SceneEffectPriority.BossLow;
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/AlcadizHurricane");
        public override void SpecialVisuals(Player player, bool isActive)
        {
            string name = "LunarVeil:RoyalCapitalSky";
            if (!SkyManager.Instance[name].IsActive() && isActive)
                SkyManager.Instance.Activate(name, player.Center);
            if (SkyManager.Instance[name].IsActive() && !isActive)
                SkyManager.Instance.Deactivate(name);
        }
        public override bool IsBiomeActive(Player player) => BiomeTileCounts.InRoyalCapital;
        public override string BestiaryIcon => base.BestiaryIcon;
        public override string BackgroundPath => base.BackgroundPath;
        public override Color? BackgroundColor => base.BackgroundColor;
        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            player.GetModPlayer<BiomePlayer>().ZoneAlcadzia = true;
        }
        public override void OnLeave(Player player) => player.GetModPlayer<BiomePlayer>().ZoneAlcadzia = false;
    }
}