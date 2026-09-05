using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.VanitiesAB;

[AutoloadEquip(EquipType.Head)]
public class EldritchianHood : ModItem
{
    public override void SetDefaults()
    {
        Item.vanity = true;
    }
}

[AutoloadEquip(EquipType.Body)]
public class EldritchianCloak : ModItem
{
    public override void SetDefaults()
    {
        Item.vanity = true;
    }
}

[AutoloadEquip(EquipType.Legs)]
public class EldritchianLegs : ModItem
{
    public override void SetDefaults()
    {
        Item.vanity = true;
    }
}