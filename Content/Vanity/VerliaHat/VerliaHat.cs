using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Vanity.VerliaHat
{
    [AutoloadEquip(EquipType.Head)]
    public class VerliaHat : ModItem
    {
        public override void SetStaticDefaults()
        {
            ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(gold: 10);
            Item.rare = ItemRarityID.Orange;
            Item.vanity = true;
        }
    }
}