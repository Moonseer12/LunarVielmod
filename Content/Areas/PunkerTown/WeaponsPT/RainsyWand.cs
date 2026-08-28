using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.MoonlightMagic;
using Stellamod.Content.MoonlightMagic.Elements;
using Stellamod.Content.MoonlightMagic.Forms;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.WeaponsPT
{
    public class RainsyWand : AbstractMagicWand
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Form = FormRegistry.Aztec.Value;
            Item.damage = 800;
            Item.mana = 70;
            Size = 16;
            TrailLength = 8;
            normalSlotCount = 1;
            timedSlotCount = 5;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<MarshScrap, BlankStaff>();
        }

        public override void ModifyElementPreferences(List<int> elements)
        {
            base.ModifyElementPreferences(elements);
            elements.Add(ModContent.ItemType<RadianceElement>());
            elements.Add(ModContent.ItemType<NaturalElement>());
            elements.Add(ModContent.ItemType<LightningElement>());
            elements.Add(ModContent.ItemType<CheckersElement>());
        }
    }
}