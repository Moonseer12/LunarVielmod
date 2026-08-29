using Stellamod.Common.DashSystem;
using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.MoonspiralTower.AccMT
{
    [AutoloadEquip(EquipType.Waist)] // Load the spritesheet you create as a shield for the player when it is equipped.
    public class ShadeScarf : AbstractDashItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<PearlescentScrap, BlankAccessory>();
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);
            DashPlayer dashPlayer = player.GetModPlayer<DashPlayer>();
            dashPlayer.DashVelocity += 15;
            dashPlayer.extraStaminaCost++;
            dashPlayer.ExtraImmunityFramesBonus += 1;
            player.moveSpeed *= 1.3f;
            player.maxRunSpeed *= 1.3f;
            player.statLifeMax2 += 10;
     
        }
    }
}