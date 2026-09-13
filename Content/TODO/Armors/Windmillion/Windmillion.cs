using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.ArmorSH
{
    [AutoloadEquip(EquipType.Head)]
    public class WindmillionHat : ModItem
    {
        public override void SetDefaults()
        {
            Item.defense = 1;
        }

        public override void UpdateEquip(Player player)
        {

            player.GetCritChance(DamageClass.Ranged) += 10f;
            player.GetDamage(DamageClass.Ranged) *= 1.1f;
            player.statLifeMax2 += 10;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<WindmillionRobe>() && legs.type == ModContent.ItemType<WindmillionBoots>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void UpdateArmorSet(Player player)
        {//30% Increased throwing attack speed!
            //Highly increased throwing weapon knowckback
            player.setBonus = LangText.SetBonus(this);//"I am wind in a million" + "\n30% Increased throwing attack speed!" + "\nHighly increased throwing weapon knowckback");
            player.GetAttackSpeed(DamageClass.Ranged) += 0.3f;
            player.GetKnockback(DamageClass.Ranged) += 0.3f;
        }


    }

    [AutoloadEquip(EquipType.Body)]
    public class WindmillionRobe : ModItem
    {
        public override void SetDefaults()
        {
            Item.defense = 3;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetCritChance(DamageClass.Ranged) += 2f;
        }

    }
    
    [AutoloadEquip(EquipType.Legs)]
    public class WindmillionBoots : ModItem
    {
        public override void SetDefaults()
        {
            Item.defense = 1;
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.1f;
            player.GetDamage(DamageClass.Ranged) *= 1.10f;
        }


    }
}