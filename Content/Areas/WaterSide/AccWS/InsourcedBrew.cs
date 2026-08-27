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
            Item.DefaultToAccessory();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FlaskPlayer>().maxInsourceCount += 1;
        }
    }
}
