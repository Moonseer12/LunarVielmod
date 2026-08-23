using Stellamod.Content.MoonlightMagic.Enchantments;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Stellamod.Content.MoonlightMagic
{
    public static class EnchantmentHelper
    {
        public static List<int> SpecialEnchantments = new List<int>
        {
            ModContent.ItemType<ChaosChaosEnchantment>(),
            ModContent.ItemType<SuperEnchantment>()
        };
    }
}
