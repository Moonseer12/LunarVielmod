using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.VanitiesSH
{
    [AutoloadEquip(EquipType.Head)]
    public class SolarianHat : ModItem
    {
        public override void SetDefaults()
        {
            Item.vanity = true;
        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class SolarianChestplate : ModItem
    {
        public override void SetDefaults()
        {
            Item.vanity = true;
        }
    }

    [AutoloadEquip(EquipType.Legs)]
    public class SolarianPants : ModItem
    {
        public override void SetDefaults()
        {
            Item.vanity = true;
        }
    }
}