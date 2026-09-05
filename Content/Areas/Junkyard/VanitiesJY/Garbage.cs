using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Junkyard.VanitiesJY;

[AutoloadEquip(EquipType.Head)]
public class GarbageMask : ModItem
{
    public override void SetDefaults()
    {
        Item.vanity = true;
    }
}

[AutoloadEquip(EquipType.Body)]
public class GarbageChestplate : ModItem
{
    public override void SetDefaults()
    {
        Item.vanity = true;
    }
}

[AutoloadEquip(EquipType.Legs)]
public class GarbagePants : ModItem
{
    public override void SetDefaults()
    {
        Item.vanity = true;
    }
}