using Stellamod.Common.XixianFlaskSystem;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WaterSide.AccWS
{
    public class InsourcedBrew : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 28;
            Item.value = Item.sellPrice(silver: 25);
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FlaskPlayer>().maxInsourceCount += 1;
        }
    }
}
