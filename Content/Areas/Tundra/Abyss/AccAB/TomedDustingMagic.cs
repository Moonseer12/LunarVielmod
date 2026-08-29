using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.AccAB
{
    public class TomedDustingMagic : ModItem
    {
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            // Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
            var line = new TooltipLine(Mod, "ADBPau", "Creates a very good voidal explosion on dust explosions and constants!")
            {
                OverrideColor = new Color(80, 187, 124)

            };
            tooltips.Add(line);
        }

        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
        }
    }
}