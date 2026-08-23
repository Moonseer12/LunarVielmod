using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Vanity.MagiciansCodeHat
{
    [AutoloadEquip(EquipType.Head)]
    public class MagiciansCodeHat : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(gold: 30);
            Item.rare = ItemRarityID.Orange;
            Item.vanity = true;
        }
    }
}