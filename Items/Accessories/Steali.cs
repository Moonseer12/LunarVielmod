using Stellamod.Common.DashSystem;
using Stellamod.Content.CommonMaterials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
    [AutoloadEquip(EquipType.Waist)] // Load the spritesheet you create as a shield for the player when it is equipped.
    public class Steali : AbstractDashItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);
            DashPlayer dashPlayer = player.GetModPlayer<DashPlayer>();
            dashPlayer.DashVelocity += 7;
            dashPlayer.DashRegenerationBonus += 0.05f;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<WinterbornShard, BlankAccessory>();
        }
    }
}