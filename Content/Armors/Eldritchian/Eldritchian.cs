using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Armors.Eldritchian
{
    public class EldritchianPlayer : ModPlayer
    {
        private float _attackSpeedBoostCounter;
        private float _attackSpeedBoost;

        public bool hasEldritchian;

        public const float Max_Damage = 200;
        public const float Max_Duration = 600;
        public const float Max_Speed = 3f;

        public override void ResetEffects()
        {
            hasEldritchian = false;

        }

        public override void UpdateDead()
        {
            base.UpdateDead();
            _attackSpeedBoostCounter = 0;
        }

        public override void PostUpdateEquips()
        {
            if (hasEldritchian && _attackSpeedBoostCounter > 0)
            {
                _attackSpeedBoostCounter--;
                float durationMultiplier = _attackSpeedBoostCounter / Max_Duration;
                float boost = durationMultiplier * _attackSpeedBoost;
                Player.GetAttackSpeed(DamageClass.Ranged) += boost;

                if (Main.rand.NextBool(2))
                {
                    int count = Main.rand.Next(6);
                }
            }
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            base.OnHurt(info);
            if (hasEldritchian)
            {
                float damage = info.Damage;
                float multiplier = damage / Max_Damage;
                _attackSpeedBoost = Max_Speed * multiplier;
                _attackSpeedBoostCounter = Max_Duration * multiplier;
                Player.AddBuff(ModContent.BuffType<ShadowBoost>(), (int)_attackSpeedBoostCounter);
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/OverGrowth_TP1"));
            }
        }
    }
    
    public class ShadowBoost : ModBuff
    {
        //This buff doesn't do anything, it just shows you how much time you got left.
        public override void SetStaticDefaults()
        {
            Main.pvpBuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }
    }

    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Body value here will result in TML expecting X_Arms.png, X_Body.png and X_FemaleBody.png sprite-sheet files to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Head)]
    public class EldritchianHood : ModItem
    {
        public override void SetDefaults()
        {
            Item.defense = 14; // The amount of defense the item will give when equipped
        }

        public override void UpdateEquip(Player player)
        {
            player.GetAttackSpeed(DamageClass.Ranged) += 0.1f;
            player.GetDamage(DamageClass.Ranged) += 0.20f;
            player.nightVision = true;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<EldritchianCloak>() && legs.type == ModContent.ItemType<EldritchianLegs>();
        }

        // UpdateArmorSet allows you to give set bonuses to the armor.
        public override void UpdateArmorSet(Player player)
        {
            //Shadow Effect
            if (Main.rand.NextBool(10))
            {
                int count = Main.rand.Next(6);
            }



            player.setBonus = LangText.SetBonus(this);//"Grants immunity to knockback!\n" + "When you take a hit, gain a temporary attack speed boost based on the amount of damage you took!");

            player.noKnockback = true;
            player.GetModPlayer<EldritchianPlayer>().hasEldritchian = true;
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class EldritchianCloak : ModItem
    {
        public override void SetDefaults()
        {
            Item.defense = 12; // The amount of defense the item will give when equipped
        }

        public override void UpdateEquip(Player player)
        {
            player.lifeRegen += 3;
            player.GetAttackSpeed(DamageClass.Ranged) += 0.12f;
            player.GetDamage(DamageClass.Ranged) += 0.16f;
        }

    }

    [AutoloadEquip(EquipType.Legs)]
    public class EldritchianLegs : ModItem
    {
        public override void SetDefaults()
        {
            Item.defense = 8; // The amount of defense the item will give when equipped
        }

        public override void UpdateEquip(Player player)
        {
            player.runAcceleration *= 1.1f;
            player.moveSpeed += 0.2f;
            player.maxRunSpeed += 0.2f; // Increase the movement speed of the player
            player.GetAttackSpeed(DamageClass.Ranged) += 0.08f;
            player.GetDamage(DamageClass.Ranged) += 0.23f;
        }

    }
}