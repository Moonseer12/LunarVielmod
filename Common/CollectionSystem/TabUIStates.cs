using Terraria.UI;

namespace Stellamod.Common.CollectionSystem
{
    public class LevelingTabStatsPanelUIState : UIState
    {
        public LevelTabStatsPanel ui;
        public LevelingTabStatsPanelUIState() : base()
        {

        }

        public override void OnInitialize()
        {
            ui = new LevelTabStatsPanel();
            Append(ui);
        }
    }

    public class LevelingLeftPanelUIState : UIState
    {
        public LevelingTabLeftPanel ui;
        public LevelingLeftPanelUIState() : base()
        {

        }

        public override void OnInitialize()
        {
            ui = new LevelingTabLeftPanel();
            Append(ui);
        }
    }

    public class CollectionArmorUIState : UIState
    {
        public CollectionArmorInfoUI ui;
        public CollectionArmorUIState() : base()
        {

        }

        public override void OnInitialize()
        {
            ui = new CollectionArmorInfoUI();
            Append(ui);
        }
    }

    public class CollectionItemTabRecipeUIState : UIState
    {
        public CollectionItemRecipesUI ui;
        public CollectionItemTabRecipeUIState() : base()
        {

        }

        public override void OnInitialize()
        {
            ui = new CollectionItemRecipesUI();
            Append(ui);
        }
    }

    public class CollectionItemTabUIState : UIState
    {
        public CollectionItemTabUI ui;
        public CollectionItemTabUIState() : base()
        {

        }

        public override void OnInitialize()
        {
            ui = new CollectionItemTabUI();
            Append(ui);
        }
    }
}