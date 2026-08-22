using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Armors.Witchen
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Body value here will result in TML expecting X_Arms.png, X_Body.png and X_FemaleBody.png sprite-sheet files to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Head)]
    public class WitchenHat : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 18; // Width of the item
            Item.height = 18; // Height of the item
            Item.value = Item.buyPrice(gold: 10); // How many coins the item is worth
            Item.rare = ItemRarityID.Pink; // The rarity of the item
            Item.defense = 9; // The amount of defense the item will give when equipped
        }

        public override void UpdateEquip(Player player)
        {

            player.GetDamage(DamageClass.Magic) *= 1.03f;
            player.GetCritChance(DamageClass.Magic) += 10f;

            player.statLifeMax2 += 30;



        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<WitchenRobe>() && legs.type == ModContent.ItemType<WitchenPants>();
        }

        // UpdateArmorSet allows you to give set bonuses to the armor.
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = LangText.SetBonus(this);//"Truly a giver of society! The witches respect you :P" + "\nGain the effects of a mana flower, magic cuffs" + "\nExtreme mana regeneration" + "\nMana costs are reduced by 50%" + "\nFlowery Rhythm!");  // This is the setbonus tooltip
            player.manaCost *= 0.5f;
            player.manaRegen += 70;
            player.magicCuffs = true;
            player.manaFlower = true;
            player.flowerBoots = true;


        }


        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.

    }

    [AutoloadEquip(EquipType.Body)]
    public class WitchenRobe : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 18; // Width of the item
            Item.height = 18; // Height of the item
            Item.value = Item.buyPrice(gold: 10); // How many coins the item is worth
            Item.rare = ItemRarityID.Pink; // The rarity of the item
            Item.defense = 12; // The amount of defense the item will give when equipped
        }

        public override void UpdateEquip(Player player)
        {
            player.GetCritChance(DamageClass.Magic) += 5f;
            player.GetDamage(DamageClass.Magic) *= 1.2f;
            player.statLifeMax2 += 5;
        }


        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.

    }
    
    [AutoloadEquip(EquipType.Legs)]
    public class WitchenPants : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 18; // Width of the item
            Item.height = 18; // Height of the item
            Item.value = Item.buyPrice(gold: 10); // How many coins the item is worth
            Item.rare = ItemRarityID.Pink; // The rarity of the item
            Item.defense = 9; // The amount of defense the item will give when equipped
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.3f;
            player.maxRunSpeed += 0.3f;
            player.statLifeMax2 += 20;
        }



        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.

    }
}