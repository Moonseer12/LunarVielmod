using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital.AccRC
{
    public class BlackRose : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.HasBuff(BuffID.ManaSickness))
            {
                int combatText = CombatText.NewText(player.getRect(), Color.Red, "10", true);
                CombatText numText = Main.combatText[combatText];
                numText.lifeTime = 60;

                player.ClearBuff(BuffID.ManaSickness);
                player.statLife -= 10;
            }
        }
    }
}
