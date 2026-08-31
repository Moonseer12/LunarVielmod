using Stellamod.Common.MagicCauldron;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Bar.Drinks
{
    public class CrystalStar : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/StarFlower1");
            soundStyle.PitchVariance = 0.15f;
            Item.UseSound = soundStyle;
            Item.useAnimation = 16;
            Item.useTime = 16;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = true;
            Item.shopSpecialCurrency = Stellamod.MedalCurrencyID;
            Item.shopCustomPrice = 10;
        }

        public override bool? UseItem(Player player)
        {
            player.GetModPlayer<CauldronPlayer>().CrystalStarCount++;
            for (int i = 0; i < 32; i++)
            {
                float progress = i / 32f;
                float rot = progress * MathHelper.ToRadians(360);
                Vector2 velocity = rot.ToRotationVector2() * 2;
                Dust.NewDustPerfect(player.Center, DustID.BoneTorch, velocity);
            }
            return true;
        }
    }

    public class CrystalLuck : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }
    }
}