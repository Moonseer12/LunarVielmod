using Stellamod.Common.ArmorRework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WondrousDarkspace.ArmorWD
{
    public class ShadeWraithPlayer : ModPlayer
    {
        public bool hasSetBonus;
        public override void ResetEffects()
        {
            hasSetBonus = false;
        }
        public override void OnHurt(Player.HurtInfo info)
        {
            if (!hasSetBonus)
                return;

            //Nuh uh
            if (Player.HasBuff<ShadeWrathCooldown>())
                return;


            float percentOfLife = (float)Player.statLife / (float)Player.statLifeMax;
            if (percentOfLife <= 0.4f)
            {
                //Trigger the buff
                int time = 300;
                Player.AddBuff(ModContent.BuffType<ShadeWrath>(), time);

                int cooldownTime = 55 * 60;
                Player.AddBuff(ModContent.BuffType<ShadeWrathCooldown>(), cooldownTime);

                //Idk some effects here or something
                //Some sounds
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/OverGrowth_TP1"));
            }
        }
    }

    [AutoloadEquip(EquipType.Head)]
    public class ShadeWraithHead : ModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ArmorSetSystem.RegisterArmorSet<ShadeWraithHead, ShadeWraithBody, ShadeWraithLegs>(ArmorGroup.Act_I);
        }

        public override void UpdateEquip(Player player)
        {
            var stats = player.GetStats();
            stats.meleeAttackSpeed += 0.25f;
            stats.defenseBonus += 4;
            stats.accessorySlots++;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<ShadeWraithBody>() && legs.type == ModContent.ItemType<ShadeWraithLegs>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.GetModPlayer<ShadeWraithPlayer>().hasSetBonus = true;
        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class ShadeWraithBody : ModItem
    {
        public override void UpdateEquip(Player player)
        {
            var stats = player.GetStats();
            stats.defenseBonus += 5;
            stats.meleeDamage += 0.05f;
            stats.accessorySlots++;
        }
    }
    [AutoloadEquip(EquipType.Legs)]
    public class ShadeWraithLegs : ModItem
    {
        public override void UpdateEquip(Player player)
        {
            var stats = player.GetStats();
            stats.meleeArmorPenetration += 3;
            stats.defenseBonus += 4;
            stats.accessorySlots++;
        }
    }
}
