using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Jungle.VanitiesJN;

[AutoloadEquip(EquipType.Head)]
public class FlowerHat : ModItem
{
    public override void SetDefaults()
    {
        Item.vanity = true;
    }
}

[AutoloadEquip(EquipType.Body)]
public class FlowerRobe : ModItem
{
    public override void SetDefaults()
    {
        Item.vanity = true;
    }
}

[AutoloadEquip(EquipType.Legs)]
public class FlowerPants : ModItem
{
    public override void SetDefaults()
    {
        Item.vanity = true;
    }
}