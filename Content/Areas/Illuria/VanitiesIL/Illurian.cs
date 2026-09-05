using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.VanitiesIL;

[AutoloadEquip(EquipType.Head)]
public class IllurianCrestmask : ModItem
{
    public override void SetDefaults()
    {
        Item.vanity = true;
    }
}

[AutoloadEquip(EquipType.Body)]
public class IllurianCrestplate : ModItem
{
    public override void SetDefaults()
    {
        Item.vanity = true;
    }
}


[AutoloadEquip(EquipType.Legs)]
public class IllurianCrestpants : ModItem
{
    public override void SetDefaults()
    {
        Item.vanity = true;
    }
}