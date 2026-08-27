using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
    public class OnionOfSight : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
        }



        public override void UpdateAccessory(Player player, bool hideVisual)
        {

            player.GetModPlayer<MyPlayer>().Onion3 = true;
            player.GetModPlayer<MyPlayer>().OnionDamage = 30;
        }




    }
}