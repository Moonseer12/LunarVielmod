using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Stellamod.Content.Areas.Desert.WeaponsCL
{
    public class GintzlsSteed : BaseCrossbowItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 6;
        }

        public override void ShootBow(Player player, EntitySource_ItemUse_WithAmmo source, ShootParams shootParams)
        {
            FunctionRepeatHelper.Repeat(() =>
                base.ShootBow(player, source, shootParams), repeats: 3, rate: 7);
        }

        public override void StaminaShootBow(Player player, EntitySource_ItemUse_WithAmmo source, ShootParams shootParams)
        {
            FunctionRepeatHelper.Repeat(() =>
                base.ShootBow(player, source, shootParams), repeats: 7, rate: 7);
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<GintzlMetal, BlankBow>();
        }
    }
}
