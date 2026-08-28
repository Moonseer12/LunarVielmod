using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.MoonlightMagic;
using Stellamod.Content.MoonlightMagic.Elements;
using Stellamod.Content.MoonlightMagic.Forms;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Underground.WeaponsUG
{
    public class StoningWand : AbstractMagicWand
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Form = FormRegistry.FourPointedStar.Value;
            Item.damage = 80;
            Item.mana = 50;
            Item.shootSpeed = 10;
            Size = 16;
            TrailLength = 16;
            normalSlotCount = 3;
            timedSlotCount = 1;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<MinersGold, BlankStaff>();
        }

        public override void ModifyElementPreferences(List<int> elements)
        {
            base.ModifyElementPreferences(elements);
            elements.Add(ModContent.ItemType<NaturalElement>());
            elements.Add(ModContent.ItemType<HolinessElement>());
            elements.Add(ModContent.ItemType<RadianceElement>());
        }
    }
}