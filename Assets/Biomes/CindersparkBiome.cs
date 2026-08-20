using Microsoft.Xna.Framework;
using Stellamod.Content.Biomes;
using Stellamod.Core.LunarLightingSystem;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Assets.Biomes
{
    public class CindersparkBiome : ModBiome, IBackLightModifier
    {
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/Cinderspark");
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
        public override string BestiaryIcon => base.BestiaryIcon;
        public override string BackgroundPath => MapBackground;
        public override Color? BackgroundColor => base.BackgroundColor;
        public override bool IsBiomeActive(Player player) => BiomeTileCounts.InCinder;
        public override void OnEnter(Player player) => player.GetModPlayer<BiomePlayer>().ZoneCinder = true;
        public override void OnLeave(Player player) => player.GetModPlayer<BiomePlayer>().ZoneCinder = false;
        public void ModifyBackLight(ref Color backLightColor)
        {
            backLightColor = Color.Lerp(backLightColor, Color.White, 0.58f);
        }
    }
}