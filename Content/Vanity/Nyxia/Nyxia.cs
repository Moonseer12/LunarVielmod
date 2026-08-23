using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Vanity.Nyxia
{
    [AutoloadEquip(EquipType.Head)]
    public class NyxiaHat : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.buyPrice(gold: 1);
            Item.rare = ItemRarityID.Green;
            Item.vanity = true;
        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class NyxiaRobe : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.buyPrice(gold: 1);
            Item.rare = ItemRarityID.Green;
            Item.vanity = true;
        }
    }

    [AutoloadEquip(EquipType.Legs)]
    public class NyxiaThighs : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Green;
            Item.vanity = true;
        }
    }
}