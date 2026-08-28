using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.MoonlightMagic;
using Stellamod.Content.MoonlightMagic.Elements;
using Stellamod.Content.MoonlightMagic.Forms;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Terror.WeaponsTR
{
    public class BloodletWand : AbstractMagicWand
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Form = FormRegistry.Squid.Value;
            Item.damage = 150;
            Item.mana = 100;
            Item.shootSpeed = 10;
            Size = 8;
            TrailLength = 16;
            normalSlotCount = 4;
            timedSlotCount = 0;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<TerrorFragments, BlankStaff>();
        }

        public override void ModifyElementPreferences(List<int> elements)
        {
            base.ModifyElementPreferences(elements);
            elements.Add(ModContent.ItemType<BloodletElement>());
            elements.Add(ModContent.ItemType<NaturalElement>());
            elements.Add(ModContent.ItemType<GuutElement>());
        }
    }
}