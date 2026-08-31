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
            Item.vanity = true;
        }
    }
}