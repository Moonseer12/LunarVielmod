using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Ishtar;

public class IshtarBiome : BaseUrdveilBiome
{
    public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<IshtarBackgroundStyle>();
    public override ModWaterStyle WaterStyle => ModContent.GetInstance<IshtarWaterStyle>();
    public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/Ishtar");
    public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
    public override string BestiaryIcon => base.BestiaryIcon;
    public override string BackgroundPath => MapBackground;
    public override Color? BackgroundColor => base.BackgroundColor;
    public override bool IsBiomeActive(Player player) => BiomeTileCounts.InIshtar;
    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        player.GetModPlayer<BiomePlayer>().ZoneIshtar = true;
    }
    public override void OnLeave(Player player) => player.GetModPlayer<BiomePlayer>().ZoneIshtar = true;
}