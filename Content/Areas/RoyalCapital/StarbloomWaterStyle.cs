using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital
{
    public class StarbloomWaterStyle : ModWaterStyle
    {
        public override int ChooseWaterfallStyle() => ModContent.Find<ModWaterfallStyle>("Stellamod/StarbloomWaterfallStyle").Slot;
        public override int GetSplashDust() => DustID.PinkCrystalShard;
        public override int GetDropletGore() => GoreID.WaterDripHallow;
        public override Color BiomeHairColor() => Color.LightGoldenrodYellow;
    }
}