using Terraria.UI;

namespace Stellamod.Common.CellConverterSystem
{
    public class ConverterUIState : UIState
    {
        public ConverterUI converterUI;
        public ConverterUIState() : base()
        {

        }

        public override void OnInitialize()
        {
            converterUI = new ConverterUI();
            Append(converterUI);
        }
    }
}
