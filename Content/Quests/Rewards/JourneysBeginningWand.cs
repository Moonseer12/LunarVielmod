using Stellamod.Content.MoonlightMagic;
using Stellamod.Content.MoonlightMagic.Elements;
using Stellamod.Content.MoonlightMagic.Forms;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Stellamod.Content.Quests.Rewards
{

    public class JourneysBeginningWand : AbstractMagicWand
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Form = FormRegistry.SmallKnife.Value;
            Item.damage = 160;
            Item.mana = 34;
            normalSlotCount = 3;
            timedSlotCount = 2;
        }

        public override void ModifyElementPreferences(List<int> elements)
        {
            base.ModifyElementPreferences(elements);
            elements.Add(ModContent.ItemType<HexElement>());
            elements.Add(ModContent.ItemType<RadianceElement>());
            elements.Add(ModContent.ItemType<PhantasmalElement>());
        }
    }
}
