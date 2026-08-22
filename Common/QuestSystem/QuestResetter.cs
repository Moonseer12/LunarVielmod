using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common.QuestSystem
{
    public class QuestResetter : ModItem
    {
        private int _useIndex;

        public override void SetDefaults()
        {
            Item.width = 62;
            Item.height = 32;
            Item.scale = 0.9f;
            Item.rare = ItemRarityID.Green;
            Item.useTime = 2;
            Item.useAnimation = 2;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = false;
            Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/Balls");
        }

        public override bool? UseItem(Player player)
        {
            QuestPlayer questPlayer = player.GetModPlayer<QuestPlayer>();
            questPlayer.ActiveQuests.Clear();
            questPlayer.CompletedQuests.Clear();
            questPlayer.RewardQuests.Clear();
            questPlayer.RecalculateUI = true;
            return true;
        }
    }
}
