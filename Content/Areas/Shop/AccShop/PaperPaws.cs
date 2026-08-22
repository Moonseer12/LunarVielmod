using Stellamod.Common.SummonerSystem;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Shop.AccShop
{
    public class PaperPaws : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
            Item.rare = ModContent.RarityType<ShopRarity>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.maxMinions += 1;
            BellPlayer bellPlayer = player.GetModPlayer<BellPlayer>();
            bellPlayer.standDamageBonus += 0.2f;
        }
    }
}