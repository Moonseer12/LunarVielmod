using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.MoonlightMagic;
using Stellamod.Content.MoonlightMagic.Elements;
using Stellamod.Content.MoonlightMagic.Forms;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Hallowrooms.WeaponsHR
{
    public class PieceOfCakeWand : AbstractMagicWand
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 1400;
            Item.shootSpeed = 15;
            Size = 12;
            TrailLength = 48;
            Form = FormRegistry.Swirl.Value;
            normalSlotCount = 4;
            timedSlotCount = 3;
            Item.mana = 70;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<KaleidoscopicInk, BlankStaff>();
        }

        public override void ModifyElementPreferences(List<int> elements)
        {
            base.ModifyElementPreferences(elements);
            elements.Add(ModContent.ItemType<PrismaticElement>());
        }
    }
}