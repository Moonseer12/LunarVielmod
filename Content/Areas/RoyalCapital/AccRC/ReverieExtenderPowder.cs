using Stellamod.Common.IgnitersNPowders;
using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital.AccRC
{
    public class ReverieExtenderPowder : ModItem
    {

        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
        }



        public override void UpdateAccessory(Player player, bool hideVisual)
        {

            player.GetModPlayer<IgniterPlayer>().extenderBonus += 1.0f;
            player.GetModPlayer<IgniterPlayer>().reverie = true;

        }


        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<AlcaricMush, BlankAccessory>();
        }


    }
}