using Stellamod.Common.XixianFlaskSystem;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.AccSH
{
    public class KindCompass : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);
            FlaskPlayer flaskPlayer = player.GetModPlayer<FlaskPlayer>();
            flaskPlayer.maxInsourceCount += 1;
        }
    }
}
