using Stellamod.Common.ArmorRework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.ArmorSH
{
    [AutoloadEquip(EquipType.Head)]
    public class StoniaHat : ModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
            ArmorSetSystem.RegisterArmorSet<StoniaHat, StoniaChestplate, StoniaBoots>(ArmorGroup.Act_I);
        }

        public override void UpdateEquip(Player player)
        {
            ArmorStatsPlayer armorStatsPlayer = player.GetModPlayer<ArmorStatsPlayer>();
            armorStatsPlayer.criticalStrikeDamage += 0.5f;
            armorStatsPlayer.defenseBonus += 2;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<StoniaChestplate>() && legs.type == ModContent.ItemType<StoniaBoots>();
        }

        // UpdateArmorSet allows you to give set bonuses to the armor.
        public override void UpdateArmorSet(Player player)
        {
            player.noFallDmg = true;
            player.pickSpeed -= 0.25f;
            player.maxFallSpeed *= 3f;
        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class StoniaChestplate : ModItem
    {
        public override void UpdateEquip(Player player)
        {
            ArmorStatsPlayer armorStatsPlayer = player.GetModPlayer<ArmorStatsPlayer>();
            armorStatsPlayer.defenseBonus += 3;
            armorStatsPlayer.generalEndurance += 0.05f;
            armorStatsPlayer.accessorySlots += 1;
        }
    } 
  
    [AutoloadEquip(EquipType.Legs)]
    public class StoniaBoots : ModItem
    {
        public override void UpdateEquip(Player player)
        {
            ArmorStatsPlayer armorStatsPlayer = player.GetModPlayer<ArmorStatsPlayer>();
            armorStatsPlayer.criticalStrikeChance += 0.05f;
            armorStatsPlayer.defenseBonus += 1;
        }
    }
}