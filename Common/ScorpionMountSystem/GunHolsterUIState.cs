using Terraria.UI;

namespace Stellamod.Common.ScorpionMountSystem
{
    public class GunHolsterUIState : UIState
    {
        public GunHolsterUI gunHolsterUI;
        public GunHolsterUIState() : base()
        {

        }

        public override void OnInitialize()
        {
            gunHolsterUI = new GunHolsterUI();
            Append(gunHolsterUI);
        }
    }
}
