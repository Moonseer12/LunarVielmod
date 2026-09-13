using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.TODO.PikpikGlove
{
    public class OnionOfUselessness : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<OnionPlayer>().Onion2 = true;
            player.GetModPlayer<OnionPlayer>().OnionDamage = 11;
        }
    }
}