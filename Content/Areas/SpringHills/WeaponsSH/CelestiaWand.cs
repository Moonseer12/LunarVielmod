using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.MoonlightMagic;
using Stellamod.Content.MoonlightMagic.Elements;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.WeaponsSH
{
    public class CelestiaWand : AbstractMagicWand
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 24;
            Item.mana = 50;
            Item.shootSpeed = 12;
            Size = 12;
            TrailLength = 18;
            normalSlotCount = 0;
            timedSlotCount = 2;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<Ivythorn, BlankStaff>();
        }

        public override void ModifyElementPreferences(List<int> elements)
        {
            base.ModifyElementPreferences(elements);
            elements.Add(ModContent.ItemType<RadianceElement>());
            elements.Add(ModContent.ItemType<PhantasmalElement>());
            elements.Add(ModContent.ItemType<BasicElement>());
        }
    }
}