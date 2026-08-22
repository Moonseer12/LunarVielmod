using Stellamod.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Armors.Garbage
{
    [AutoloadEquip(EquipType.Head)]
    public class GarbageMask : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 28; // Width of the item
            Item.height = 26; // Height of the item
            Item.value = Item.sellPrice(gold: 10); // How many coins the item is worth
            Item.rare = ItemRarityID.Pink; // The rarity of the item
            Item.defense = 17; // The amount of defense the item will give when equipped
        }

        public override void UpdateEquip(Player player)
        {
            player.GetAttackSpeed(DamageClass.Ranged) += 0.3f;
            player.GetDamage(DamageClass.Ranged) += 0.15f;
            player.GetDamage(DamageClass.Summon) += 0.15f;
            player.maxMinions += 2;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<GarbageChestplate>() && legs.type == ModContent.ItemType<GarbagePants>();
        }

        // UpdateArmorSet allows you to give set bonuses to the armor.
        public override void UpdateArmorSet(Player player)
        {
            //Shadow Effect
            if (Main.rand.NextBool(10))
            {
                int count = Main.rand.Next(3);
                for (int iz = 0; iz < count; iz++)
                {
                    for (int i = 0; i < 1; i++)
                    {
                        Dust.NewDustPerfect(player.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 3)).RotatedByRandom(19.0), 0, Color.PaleVioletRed, 0.5f).noGravity = true;
                    }
                    for (int i = 0; i < 1; i++)
                    {
                        Dust.NewDustPerfect(player.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 3)).RotatedByRandom(19.0), 0, Color.Green, 0.5f).noGravity = true;
                    }
                }
            }



            player.setBonus = LangText.SetBonus(this);//"Grants immunity to knockback!\n" + "+2 Summons");
            player.noKnockback = true;
            player.maxMinions += 2;

        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class GarbageChestplate : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 34; // Width of the item
            Item.height = 24; // Height of the item
            Item.value = Item.sellPrice(gold: 10); // How many coins the item is worth
            Item.rare = ItemRarityID.Pink; // The rarity of the item
            Item.defense = 17; // The amount of defense the item will give when equipped
        }

        public override void UpdateEquip(Player player)
        {
            player.statLifeMax2 += 40;
            player.lifeRegen += 1;
            player.GetAttackSpeed(DamageClass.Ranged) += 0.10f;
            player.GetDamage(DamageClass.Ranged) += 0.25f;
            player.GetDamage(DamageClass.Summon) += 0.25f;
        }

    }

    [AutoloadEquip(EquipType.Legs)]
    public class GarbagePants : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 22; // Width of the item
            Item.height = 12; // Height of the item
            Item.value = Item.sellPrice(gold: 10); // How many coins the item is worth
            Item.rare = ItemRarityID.Pink; // The rarity of the item
            Item.defense = 12; // The amount of defense the item will give when equipped
        }

        public override void UpdateEquip(Player player)
        {
            player.runAcceleration *= 1.05f;
            player.moveSpeed += 0.1f;
            player.maxRunSpeed += 0.1f; // Increase the movement speed of the player
            player.maxMinions += 1;
        }

    }
}