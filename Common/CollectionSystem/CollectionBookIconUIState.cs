using Terraria.UI;

namespace Stellamod.Common.CollectionSystem
{
    public class CollectionBookIconUIState : UIState
    {
        public CollectionBookIconUI bookIconUI;
        public CollectionBookIconUIState() : base()
        {

        }

        public override void OnInitialize()
        {
            bookIconUI = new CollectionBookIconUI();
            Append(bookIconUI);
        }
    }
}
