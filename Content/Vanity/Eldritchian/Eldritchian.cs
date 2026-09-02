using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Vanity.Eldritchian;

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