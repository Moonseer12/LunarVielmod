using Terraria;
using Terraria.Graphics.Capture;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.EveroseVillage;

public class EveroseVillageBiome : BaseUrdveilBiome
{
    public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<NoBackgroundStyle>();
    public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Normal;
    public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
    public override int Music
    {
        get
        {
            if(!Main.dayTime)
                return MusicLoader.GetMusicSlot(Mod, "Assets/Music/ObservatorOfTheStars");
            return MusicLoader.GetMusicSlot(Mod, "Assets/Music/JustAnotherDay");
        }
    }
    public override string BestiaryIcon => base.BestiaryIcon;
    public override string BackgroundPath => base.BackgroundPath;
    public override Color? BackgroundColor => base.BackgroundColor;
    public override bool IsBiomeActive(Player player) => BiomeTileCounts.InEveroseVillage;
    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        player.GetModPlayer<BiomePlayer>().ZoneEveroseVillage = true;
    }
    public override void OnLeave(Player player) => player.GetModPlayer<BiomePlayer>().ZoneEveroseVillage = false;
}