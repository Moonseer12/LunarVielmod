using Stellamod.Core.Tooltips;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common.MagicCauldron
{
    public class MoldExpandableTooltip : AbstractExpandingTooltip
    {
        public override void ModifyExpandableTooltips(Item item, List<TooltipLine> lines)
        {
            if (item.GetGlobalItem<MoldGlobalItem>().isMold)
            {
                TooltipLine moldLine = new(Mod, "MoldHelpingText", LangText.Common("CauldronMoldHelp"));
                lines.Add(moldLine);
            }
        }
    }

    public class BrewingMaterialLabelTooltip : GlobalItem
    {
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(item, tooltips);
            Cauldron cauldron = ModContent.GetInstance<Cauldron>();
            if (cauldron.IsMaterial(item.type))
            {
                TooltipLine materialLine = new(Mod, "BrewingMaterialLabel", LangText.Common("CauldronMaterialLabel"));
                tooltips.Add(materialLine);
            }
        }
    }

    public class BrewingMaterialExpandableTooltip : AbstractExpandingTooltip
    {
        public override void ModifyExpandableTooltips(Item item, List<TooltipLine> lines)
        {
            Cauldron cauldron = ModContent.GetInstance<Cauldron>();
            if (cauldron.IsMaterial(item.type))
            {
                TooltipLine materialLine = new(Mod, "BrewingMaterialHelpingText", LangText.Common("CauldronMaterialHelp"));
                lines.Add(materialLine);
            }
        }
    }
}
