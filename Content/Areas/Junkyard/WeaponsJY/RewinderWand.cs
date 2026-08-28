using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.MoonlightMagic;
using Stellamod.Content.MoonlightMagic.Forms;

namespace Stellamod.Content.Areas.Junkyard.WeaponsJY
{
    public class RewinderWand : AbstractMagicWand
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Form = FormRegistry.Gear.Value;
            Item.damage = 1200;
            Item.mana = 40;
            Size = 30;
            TrailLength = 30;
            normalSlotCount = 1;
            timedSlotCount = 6;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<MechanizedSoul, BlankStaff>();
        }
    }
}