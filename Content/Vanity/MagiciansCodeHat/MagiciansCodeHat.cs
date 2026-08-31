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
            Item.vanity = true;
        }
    }
}