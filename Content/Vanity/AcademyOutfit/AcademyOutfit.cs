using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Vanity.AcademyOutfit;

[AutoloadEquip(EquipType.Head)]
public class AcademyOutfitHead : ModItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
    }

    public override void SetDefaults()
    {
        Item.vanity = true;
    }
}

[AutoloadEquip(EquipType.Body)]
public class AcademyOutfitRobe : ModItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }

    public override void SetDefaults()
    {
        Item.vanity = true;
    }
}

[AutoloadEquip(EquipType.Legs)]
public class AcademyOutfitLegs : ModItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }

    public override void SetDefaults()
    {
        Item.vanity = true;
    }
}

