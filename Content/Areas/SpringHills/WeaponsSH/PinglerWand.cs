using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.MoonlightMagic;
using Stellamod.Content.MoonlightMagic.Forms;

namespace Stellamod.Content.Areas.SpringHills.WeaponsSH
{
    public class PinglerWand : AbstractMagicWand
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Form = FormRegistry.Arrow.Value;
            Item.damage = 18;
            Item.mana = 35;
            Size = 16;
            TrailLength = 8;
            normalSlotCount = 1;
            timedSlotCount = 0;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<Mushroom, BlankStaff>();
        }
    }
}