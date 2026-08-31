using Stellamod.Common.XixianFlaskSystem;

namespace Stellamod.Content.Areas.SpringHills.InsourcesSH
{
    public class HealthInsource : InsourceItem
    {
        public override int GetAddedTime()
        {
            return 60 * 30;
        }


        public override void UseInsource(FlaskPlayer flaskPlayer)
        {
            base.UseInsource(flaskPlayer);
            flaskPlayer.Player.Heal(50);
        }
    }
}
