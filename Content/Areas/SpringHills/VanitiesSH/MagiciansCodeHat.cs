using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.VanitiesSH
{
    [AutoloadEquip(EquipType.Head)]
    public class MagiciansCodeHat : ModItem
    {
        public override void SetDefaults()
        {
            Item.vanity = true;
        }
    }
}