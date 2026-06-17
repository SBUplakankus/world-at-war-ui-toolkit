using Data;
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

        private static void HandleOnlineClicked()
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

        protected override void Bind()
        {
            _elements.MessageOfTheDay.text = MotdManager.RandomMessage.body;

            _elements.OnlineButton.clicked += HandleOnlineClicked;
            _elements.SplitScreenButton.clicked += HandleSplitScreenClicked;
            _elements.LanPartyButton.clicked += HandleLanPartyClicked;
            _elements.OptionsButton.clicked += HandleOptionsClicked;
        }

        protected override void UnBind()
        {
            _elements.OnlineButton.clicked -= HandleOnlineClicked;
            _elements.SplitScreenButton.clicked -= HandleSplitScreenClicked;
            _elements.LanPartyButton.clicked -= HandleLanPartyClicked;
            _elements.OptionsButton.clicked -= HandleOptionsClicked;
        }

        public string HeaderName => ScreenNames.Multiplayer;
    }
}
