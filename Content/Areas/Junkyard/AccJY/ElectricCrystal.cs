using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Junkyard.AccJY
{
    public class ElectricCrystal : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Generic) *= 1.06f;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<MechanizedSoul, BlankAccessory>();
        }
    }
}