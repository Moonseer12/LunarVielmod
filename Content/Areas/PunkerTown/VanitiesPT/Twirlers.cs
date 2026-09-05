using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.VanitiesPT
{
    [AutoloadEquip(EquipType.Head)]
    public class Twirlers : ModItem
    {
        public override void SetDefaults()
        {
            Item.vanity = true;
        }
    }
}