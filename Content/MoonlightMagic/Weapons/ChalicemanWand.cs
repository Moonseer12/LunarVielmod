using Stellamod.Content.CommonMaterials;
using Stellamod.Content.MoonlightMagic.Elements;
using Stellamod.Content.MoonlightMagic.Forms;
using Stellamod.Items;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Stellamod.Content.MoonlightMagic.Weapons
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
            this.RegisterBrew(mold: ModContent.ItemType<BlankStaff>(), material: ModContent.ItemType<EreshkinCandle>());
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
