using Stellamod.Common.IgnitersNPowders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Items;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Ishtar.AccIS
{
    public class GrailedExtenderPowder : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);
            player.GetModPlayer<IgniterPlayer>().boomerang = true;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<EreshkinCandle, BlankAccessory>();
        }
    }
}