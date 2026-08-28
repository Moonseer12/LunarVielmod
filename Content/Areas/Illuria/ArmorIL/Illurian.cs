using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.ArmorIL
{
    [AutoloadEquip(EquipType.Head)]
    public class IllurianCrestmask : ModItem
    {
        public override void SetDefaults()
        {
            Item.defense = 17;
        }

        public override void UpdateEquip(Player player)
        {
            player.statLifeMax2 -= 50;
            player.GetDamage(DamageClass.Summon) *= 1.12f;
            player.GetDamage(DamageClass.Magic) *= 1.12f;
            player.GetCritChance(DamageClass.Generic) += 10f;
            player.autoReuseGlove = true;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<IllurianCrestplate>() && legs.type == ModContent.ItemType<IllurianCrestpants>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = LangText.SetBonus(this);
            player.aggro *= 2;
        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class IllurianCrestplate : ModItem
    {
        public override void SetDefaults()
        {
            Item.defense = 25;
        }

        public override void UpdateEquip(Player player)
        {
            player.statManaMax2 += 50;
            player.maxMinions += 3;
            player.statLifeMax2 -= 50;
        }
    }

    [AutoloadEquip(EquipType.Legs)]
    public class IllurianCrestpants : ModItem
    {
        public override void SetDefaults()
        {
            Item.defense = 20;
        }

        public override void UpdateEquip(Player player)
        {
            player.maxMinions += 1;
            player.moveSpeed *= 1.1f;
            player.statLifeMax2 -= 90;
            player.GetArmorPenetration(DamageClass.Generic) += 15f;
            player.GetDamage(DamageClass.Generic) *= 1.2f;
        }
    }
}