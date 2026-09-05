using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Vanity.Appretience;

[AutoloadEquip(EquipType.Head)]
public class AppretienceHat : ModItem
{
    public override void SetDefaults()
    {
        Item.vanity = true;
    }
}

[AutoloadEquip(EquipType.Body)]
public class AppretienceBreastplate : ModItem
{
    public override void SetDefaults()
    {
        Item.vanity = true;
    }
}

[AutoloadEquip(EquipType.Legs)]
public class AppretiencePants : ModItem
{
    public override void SetDefaults()
    {
        Item.vanity = true;
    }
}