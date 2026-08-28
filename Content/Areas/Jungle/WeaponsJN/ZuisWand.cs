using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.MoonlightMagic;
using Stellamod.Content.MoonlightMagic.Forms;

namespace Stellamod.Content.Areas.Jungle.WeaponsJN
{
    public class ZuisWand : AbstractMagicWand
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 1800;
            Item.shootSpeed = 20;
            Size = 12;
            TrailLength = 40;
            Form = FormRegistry.SmallKnife.Value;
            normalSlotCount = 5;
            timedSlotCount = 5;
            Item.mana = 50;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<RadiantNectar, BlankStaff>();
        }
    }
}