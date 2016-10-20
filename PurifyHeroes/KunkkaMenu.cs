﻿namespace Heroes
{
    using Ensage.Common.Menu;

    internal class KunkkaMenu
    {
        #region Fields

        private readonly MenuItem autoReturn;

        private readonly MenuItem enabled;

        private readonly Menu menu;

        #endregion

        #region Constructors and Destructors

        public KunkkaMenu(string heroName)
        {
            menu = new Menu("Heroes", "kunkka", true, heroName, true);

            menu.AddItem(enabled = new MenuItem("enabled", "Enabled").SetValue(true));
            menu.AddItem(autoReturn = new MenuItem("autoReturn", "Auto return").SetValue(true))
                .SetTooltip("Will auto return enemy on Torrent, Ship, Arrow or Hook");
            menu.AddItem(new MenuItem("combo", "Combo").SetValue(new KeyBind('E', KeyBindType.Press)))
                .SetTooltip("X Mark => Torrent => Return")
                .ValueChanged += (sender, arg) => { ComboEnabled = arg.GetNewValue<KeyBind>().Active; };
            menu.AddItem(new MenuItem("fullCombo", "Full combo").SetValue(new KeyBind('F', KeyBindType.Press)))
                .SetTooltip("X Mark => Ghost Ship => Torrent => Return")
                .ValueChanged +=
                (sender, arg) => { ComboEnabled = FullComboEnabled = arg.GetNewValue<KeyBind>().Active; };
            menu.AddItem(new MenuItem("tpHome", "X home").SetValue(new KeyBind('G', KeyBindType.Press)))
                .SetTooltip("X Mark on self => Teleport to base")
                .ValueChanged += (sender, arg) => { TpHomePressed = arg.GetNewValue<KeyBind>().Active; };
            menu.AddItem(new MenuItem("hitRun", "Hit & run").SetValue(new KeyBind('H', KeyBindType.Press)))
                .SetTooltip("X Mark on self => Dagger => Hit => Return")
                .ValueChanged += (sender, arg) => { HitAndRunEnabled = arg.GetNewValue<KeyBind>().Active; };
            menu.AddItem(new MenuItem("torrentRune", "Torrent on rune").SetValue(new KeyBind('J', KeyBindType.Press)))
                .SetTooltip("Will cast torrent at rune's position, right before spawn")
                .ValueChanged += (sender, arg) => { TorrentOnRuneEnabled = arg.GetNewValue<KeyBind>().Active; };

            menu.AddToMainMenu();
        }

        #endregion

        #region Public Properties

        public bool AutoReturnEnabled => autoReturn.GetValue<bool>();

        public bool ComboEnabled { get; private set; }

        public bool FullComboEnabled { get; private set; }

        public bool HitAndRunEnabled { get; private set; }

        public bool IsEnabled => enabled.GetValue<bool>();

        public bool TorrentOnRuneEnabled { get; private set; }

        public bool TpHomePressed { get; private set; }

        #endregion

        #region Public Methods and Operators

        public void OnClose()
        {
            menu.RemoveFromMainMenu();
        }

        #endregion
    }
}