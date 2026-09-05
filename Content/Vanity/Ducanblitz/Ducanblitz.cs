using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Vanity.Ducanblitz;

[AutoloadEquip(EquipType.Head)]
public class DucanblitzCap : ModItem
{
    public override void SetDefaults()
    {
        Item.vanity = true;
    }
}

[AutoloadEquip(EquipType.Body)]
public class DucanblitzBreastplate : ModItem
{
    public override void SetDefaults()
    {
        Item.vanity = true;
    }
}

[AutoloadEquip(EquipType.Legs)]
public class DucanblitzThighs : ModItem
{    
    public override void SetDefaults()
    {
        Item.vanity = true;
    }
}