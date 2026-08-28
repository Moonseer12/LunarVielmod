using Stellamod.Common.SummonerSystem;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common.XixianFlaskSystem
{
    public class InsourceUnlocker : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.useTime = 2;
            Item.useAnimation = 2;
            Item.useStyle = ItemUseStyleID.Shoot;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
        public override bool? UseItem(Player player)
        {
            int x = (int)Main.MouseWorld.X / 16;
            int y = (int)Main.MouseWorld.Y / 16;
            if (player.altFunctionUse == 2)
            {
                //Right click 
                //Cycle
                player.GetModPlayer<FlaskPlayer>().ResetProgress();
                player.GetModPlayer<BellPlayer>().ResetProgress();
            }
            else
            {
                player.GetModPlayer<FlaskPlayer>().GrantAllProgress();
                player.GetModPlayer<BellPlayer>().GrantAllProgress();

            }
            return true;
        }
    }
}