namespace Heroes
{
    using Ensage.Common.Menu;

    internal class AbaddonMenu
    {
        #region Fields

        private readonly MenuItem enabled;

        private readonly Menu menu;

        #endregion

        #region Constructors and Destructors

        public AbaddonMenu(string heroName)
        {
            menu = new Menu("Heroes", "abaddon", true, heroName, true);

            menu.AddItem(enabled = new MenuItem("enabled", "Enabled").SetValue(true));
            menu.AddItem(new MenuItem("coilCreep", "Coil Creep").SetValue(new KeyBind('D', KeyBindType.Press)))
                .SetTooltip("Coil nearest dying creep for gold")
                .ValueChanged += (sender, arg) => { CoilCreepPressed = arg.GetNewValue<KeyBind>().Active; };

            menu.AddToMainMenu();
        }

        #endregion

        #region Public Properties

        public bool IsEnabled => enabled.GetValue<bool>();
        public bool CoilCreepPressed { get; private set; }

        #endregion

        #region Public Methods and Operators

        public void OnClose()
        {
            menu.RemoveFromMainMenu();
        }

        #endregion
    }
}