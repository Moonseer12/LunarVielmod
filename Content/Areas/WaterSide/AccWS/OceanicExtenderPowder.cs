using Stellamod.Common.IgnitersNPowders;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WaterSide.AccWS
{
    public class OceanicExtenderPowder : ModItem
    {   
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            // Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
            var line = new TooltipLine(Mod, "ADBPau", Helpers.LangText.Common("NoStack"))
            {
                OverrideColor = new Color(110, 187, 24)

            };
            tooltips.Add(line);
        }

        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            IgniterPlayer igniterPlayer = player.GetModPlayer<IgniterPlayer>();
            igniterPlayer.extenderBonus += 0.5f;
        }
    }
}