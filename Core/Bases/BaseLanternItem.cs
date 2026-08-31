using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.Bases
{
    public abstract class BaseLanternItem : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 0;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item2;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.noMelee = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            TooltipLine line = new TooltipLine(Mod, "Lantern", LangText.Common("Lantern"));
            line.OverrideColor = Color.LightGoldenrodYellow;
            tooltips.Add(line);

            line = new TooltipLine(Mod, "LanternHelp", LangText.Common("LanternHelp"));
            line.OverrideColor = Color.Lerp(Color.LightGoldenrodYellow, Color.Black, 0.15f);
            tooltips.Add(line);
        }
        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(Item.buffType, 3600);
            }
        }
    }
}
