using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Vanity.Ulven;

[AutoloadEquip(EquipType.Head)]
public class UlvenHelmet : ModItem
{
    public override void SetDefaults()
    {
        Item.vanity = true;
    }
}   

[AutoloadEquip(EquipType.Body)]
public class UlvenChestplate : ModItem
{
    public override void SetDefaults()
    {
        Item.vanity = true;
    }
}  

[AutoloadEquip(EquipType.Legs)]
public class UlvenGreaves : ModItem
{
    public override void SetDefaults()
    {
        Item.vanity = true;
    }
}