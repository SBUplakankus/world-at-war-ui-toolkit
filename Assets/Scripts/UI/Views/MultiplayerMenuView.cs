using UI.Constants;
using UI.Core;
using UI.Factories;
using UI.Interfaces;
using UI.Records;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Views
{
    public class MultiplayerMenuView : BaseView, IScreen
    {
        private MultiplayerMenuElements _elements;

        public MultiplayerMenuView(VisualTreeAsset template) : base(template) { }

        protected override void GetElements() => _elements = ElementsFactory.MultiplayerMenu(Root);

        private static void HandlePlayerOnlineClicked()
        {
            Debug.Log("Player Online clicked");
        }

        private static void HandleSplitScreenClicked()
        {
            Debug.Log("Split Screen clicked");
        }

        private static void HandleLanPartyClicked()
        {
            Debug.Log("LAN Party clicked");
        }

        private static void HandleOptionsClicked()
        {
            Debug.Log("Multiplayer Options clicked");
            UIRouter.Instance.NavigateTo<GameOptionsView>();
        }

        private static void HandleMainMenuClicked()
        {
            Debug.Log("Back to Main Menu clicked");
            UIRouter.Instance.NavigateTo<MainMenuView>();
        }

        protected override void Bind()
        {
            _elements.PlayerOnline.clicked += HandlePlayerOnlineClicked;
            _elements.SplitScreen.clicked += HandleSplitScreenClicked;
            _elements.LanParty.clicked += HandleLanPartyClicked;
            _elements.Options.clicked += HandleOptionsClicked;
            _elements.MainMenu.clicked += HandleMainMenuClicked;
        }

        protected override void UnBind()
        {
            _elements.PlayerOnline.clicked -= HandlePlayerOnlineClicked;
            _elements.SplitScreen.clicked -= HandleSplitScreenClicked;
            _elements.LanParty.clicked -= HandleLanPartyClicked;
            _elements.Options.clicked -= HandleOptionsClicked;
            _elements.MainMenu.clicked -= HandleMainMenuClicked;
        }

        public string HeaderName => ScreenNames.Multiplayer;
    }
}
