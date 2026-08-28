using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.MoonlightMagic;
using Stellamod.Content.MoonlightMagic.Elements;
using Stellamod.Content.MoonlightMagic.Forms;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Ishtar.WeaponsIS
{
    public class ChalicemanWand : AbstractMagicWand
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 1800;
            Item.shootSpeed = 12;
            Size = 15;
            TrailLength = 20;
            Form = FormRegistry.Triangle.Value;
            normalSlotCount = 3;
            timedSlotCount = 7;
            Item.mana = 90;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<EreshkinCandle, BlankStaff>();
        }

        public override void ModifyElementPreferences(List<int> elements)
        {
            base.ModifyElementPreferences(elements);
            elements.Add(ModContent.ItemType<PrimeMagicElement>());
            elements.Add(ModContent.ItemType<DeeyaElement>());
            elements.Add(ModContent.ItemType<BloodletElement>());
            elements.Add(ModContent.ItemType<PrimeMagicElement>());
        }
    }
}