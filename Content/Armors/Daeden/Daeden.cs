using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Armors.Daeden
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Body value here will result in TML expecting X_Arms.png, X_Body.png and X_FemaleBody.png sprite-sheet files to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Head)]
    public class DaedenMask : ModItem
    {
        public override void SetDefaults()
        {
            Item.defense = 14; // The amount of defense the item will give when equipped
        }

        public override void UpdateEquip(Player player)
        {

            player.GetDamage(DamageClass.Ranged) *= 1.2f;
            player.GetCritChance(DamageClass.Generic) += 5f;

        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<DaedenChestplate>() && legs.type == ModContent.ItemType<DaedenLegs>();
        }

        // UpdateArmorSet allows you to give set bonuses to the armor.
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = LangText.SetBonus(this);//"This armor is really scuffed..." + "\nGives the ability of a molten quiver!" + "\n-Stuck at 400 max HP, but 20% increased damage for Rangers");  // This is the setbonus tooltip
            player.GetDamage(DamageClass.Ranged) *= 1.20f;
            player.statLifeMax2 = 400;
            player.hasMoltenQuiver = true;

        }


        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.

    }

    [AutoloadEquip(EquipType.Body)]
    public class DaedenChestplate : ModItem
    {
        public override void SetDefaults()
        {
            Item.defense = 18; // The amount of defense the item will give when equipped
        }

        public override void UpdateEquip(Player player)
        {

            player.GetArmorPenetration(DamageClass.Generic) += 20f;
            player.GetDamage(DamageClass.Ranged) *= 1.1f;
            player.GetCritChance(DamageClass.Generic) += 5f;
            player.statLifeMax2 += 55;




        }


        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.

    }
    
    [AutoloadEquip(EquipType.Legs)]
    public class DaedenLegs : ModItem
    {
        public override void SetDefaults()
        {
            Item.defense = 15; // The amount of defense the item will give when equipped
        }

        public override void UpdateEquip(Player player)
        {

            player.moveSpeed += 0.5f;
            player.maxRunSpeed += 0.5f;


        }



        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.

    }
}