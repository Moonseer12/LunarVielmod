using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Vanity.Twirlers
{
    [AutoloadEquip(EquipType.Head)]
    public class Twirlers : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(gold: 10);
            Item.rare = ItemRarityID.Green;
            Item.vanity = true;
        }
    }
}