using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.VanitiesPT;

[AutoloadEquip(EquipType.Head)]
public class DaedenMask : ModItem
{
    public override void SetDefaults()
    {
        Item.vanity = true;
    }
}

[AutoloadEquip(EquipType.Body)]
public class DaedenChestplate : ModItem
{
    public override void SetDefaults()
    {
        Item.vanity = true;
    }
}  

[AutoloadEquip(EquipType.Legs)]
public class DaedenLegs : ModItem
{
    public override void SetDefaults()
    {
        Item.vanity = true;
    }
}