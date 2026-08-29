using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Special.PikpikGlove
{
    public class OnionOfStrength : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<OnionPlayer>().Onion4 = true;
            player.GetModPlayer<OnionPlayer>().OnionDamage = 70;
        }
    }
}