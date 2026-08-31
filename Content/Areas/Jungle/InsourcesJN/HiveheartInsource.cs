using Stellamod.Common.MagicCauldron;
using Stellamod.Common.XixianFlaskSystem;
using Stellamod.Content.CommonMaterials;
using Terraria.ID;

namespace Stellamod.Content.Areas.Jungle.InsourcesJN
{
    public class HiveheartInsource : InsourceItem
    {
        public override int GetAddedTime()
        {
            return 60 * 15;
        }

        public override void UseInsource(FlaskPlayer flaskPlayer)
        {
            base.UseInsource(flaskPlayer);
            flaskPlayer.Player.AddBuff(BuffID.Honey, 60 * 10);
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<RadiantNectar, BlankBrooch>();
        }
    }
}
