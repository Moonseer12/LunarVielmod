using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.ArmorIL
{
    [AutoloadEquip(EquipType.Head)]
    public class IllurianWarriorHelm : ModItem
    {
        public override void SetDefaults()
        {
            Item.defense = 26;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<IllurianWarriorChestplate>() && legs.type == ModContent.ItemType<IllurianWarriorGreaves>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = LangText.SetBonus(this);
            player.waterWalk = true;
        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class IllurianWarriorChestplate : ModItem
    {
        public override void SetDefaults()
        {
            Item.defense = 27;
        }
    }

    [AutoloadEquip(EquipType.Legs)]
    public class IllurianWarriorGreaves : ModItem
    {
        public override void SetDefaults()
        {
            Item.defense = 26;
        }
    }
}