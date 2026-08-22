using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.Vanity.Verlia
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Head value here will result in TML expecting a X_Head.png file to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Head)]
    public class MagiciansCodeHat : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 18; // Width of the item
            Item.height = 18; // Height of the item
            Item.value = Item.sellPrice(gold: 30); // How many coins the item is worth
            Item.rare = ItemRarityID.Orange; // The rarity of the item
            Item.defense = 15; // The amount of defense the item will give when equipped
        }
        public override void UpdateEquip(Player player)
        {
            player.statLifeMax2 += 25;
            player.GetDamage(DamageClass.Melee) *= 1.20f;
            player.GetDamage(DamageClass.Summon) *= 1.20f;
            player.maxMinions += 2;
            player.endurance += 0.1f;



        }
        // IsArmorSet determines what armor pieces are needed for the setbonus to take effect


        // UpdateArmorSet allows you to give set bonuses to the armor.

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
    }
}