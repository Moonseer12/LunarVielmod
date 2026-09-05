using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.VanitiesIL;

[AutoloadEquip(EquipType.Head)]
public class IllurianWarriorHelm : ModItem
{
    public override void SetDefaults()
    {
        Item.vanity = true;
    }
}

[AutoloadEquip(EquipType.Body)]
public class IllurianWarriorChestplate : ModItem
{
    public override void SetDefaults()
    {
        Item.vanity = true;
    }
}


[AutoloadEquip(EquipType.Legs)]
public class IllurianWarriorGreaves : ModItem
{
    public override void SetDefaults()
    {
        Item.vanity = true;
    }
}