using Stellamod.Content.CommonMaterials;
using Stellamod.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Ishtar.AccIS
{
    public class SpikeResistPlayer : ModPlayer
    {
        public bool hasPanacea;
        public override void ResetEffects()
        {
            hasPanacea = false;
        }
    }

    public class Panacea : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<SpikeResistPlayer>().hasPanacea = true;
            player.ClearBuff(BuffID.Bleeding);
            player.ClearBuff(BuffID.Poisoned);
            player.ClearBuff(BuffID.Venom);
            player.statLifeMax2 += 40;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<EreshkinCandle, BlankAccessory>();
        }
    }
}
