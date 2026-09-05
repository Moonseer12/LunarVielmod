using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.VanitiesSH;

[AutoloadEquip(EquipType.Head)]
public class WitchenHat : ModItem
{
    public override void SetDefaults()
    {
        Item.vanity = true;
    }
}

[AutoloadEquip(EquipType.Body)]
public class WitchenRobe : ModItem
{
    public override void SetDefaults()
    {
        Item.vanity = true;
    }
}

[AutoloadEquip(EquipType.Legs)]
public class WitchenPants : ModItem
{
    public override void SetDefaults()
    {
        Item.vanity = true;
    }
}