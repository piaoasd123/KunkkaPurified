namespace Heroes
{
    using System;

    using Ensage;
    using Ensage.Common;

    internal class Bootstrap
    {
        #region Fields

        private IHero hero;
        #endregion

        #region Public Methods and Operators

        public void Initialize()
        {
            Events.OnLoad += OnLoad;
        }

        #endregion

        #region Methods

        private void Drawing_OnDraw(EventArgs args)
        {
            hero.OnDraw();
        }

        private void Game_OnUpdate(EventArgs args)
        {
            hero.OnUpdate();
        }

        private void OnClose(object sender, EventArgs e)
        {
            Events.OnClose -= OnClose;
            Game.OnIngameUpdate -= Game_OnUpdate;
            Player.OnExecuteOrder -= Player_OnExecuteAction;
            Drawing.OnDraw -= Drawing_OnDraw;
            hero.OnClose();
        }

        private void OnLoad(object sender, EventArgs e)
        {
            if (ObjectManager.LocalHero.ClassID == ClassID.CDOTA_Unit_Hero_Kunkka)
            {
                hero = new Kunkka();
            }
            else if (ObjectManager.LocalHero.ClassID == ClassID.CDOTA_Unit_Hero_Abaddon)
            {
                hero = new Abaddon();
            }
            else
            {
                return;
            }

            hero.OnLoad();

            Events.OnClose += OnClose;
            Game.OnIngameUpdate += Game_OnUpdate;
            Player.OnExecuteOrder += Player_OnExecuteAction;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private void Player_OnExecuteAction(Player sender, ExecuteOrderEventArgs args)
        {
            hero.OnExecuteAbilitiy(sender, args);
        }

        #endregion
    }
}