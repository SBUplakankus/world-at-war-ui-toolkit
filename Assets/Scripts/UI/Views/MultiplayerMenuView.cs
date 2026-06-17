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
    public sealed class MultiplayerMenuView : BaseView, IScreen
    {
        private MultiplayerMenuElements _elements;

        public string HeaderName => ScreenNames.Multiplayer;

        public MultiplayerMenuView(VisualTreeAsset template) : base(template) { }

        protected override void GetElements() => _elements = ElementsFactory.MultiplayerMenu(Root);

        private static void HandleOnlineClicked()
        {
            UIRouter.Instance.OpenModal<NoConnectionModalView>();
        }

        private static void HandleSplitScreenClicked()
        {
            UIRouter.Instance.OpenModal<NoConnectionModalView>();
        }

        private static void HandleLanPartyClicked()
        {
            UIRouter.Instance.OpenModal<NoConnectionModalView>();
        }

        private static void HandleOptionsClicked()
        {
            Debug.Log("Multiplayer Options clicked");
            UIRouter.Instance.NavigateTo<GameOptionsView>();
        }

        protected override void Bind()
        {
            _elements.MessageOfTheDay.text = UIResources.MessagesOfTheDay.Random();

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
    }
}
