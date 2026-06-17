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

        private void SetMessageOfTheDay()
        {
            _elements.MessageOfTheDay.text = UIResources.MessagesOfTheDay.Random();
        }

        private void BindButtonClicks()
        {
            _elements.OnlineButton.clicked += HandleOnlineClicked;
            _elements.SplitScreenButton.clicked += HandleSplitScreenClicked;
            _elements.LanPartyButton.clicked += HandleLanPartyClicked;
            _elements.OptionsButton.clicked += HandleOptionsClicked;
        }

        private void UnBindButtonClicks()
        {
            _elements.OnlineButton.clicked -= HandleOnlineClicked;
            _elements.SplitScreenButton.clicked -= HandleSplitScreenClicked;
            _elements.LanPartyButton.clicked -= HandleLanPartyClicked;
            _elements.OptionsButton.clicked -= HandleOptionsClicked;
        }

        protected override void GetElements() => _elements = ElementsFactory.MultiplayerMenu(Root);

        protected override void Bind()
        {
            SetMessageOfTheDay();
            BindButtonClicks();
        }

        protected override void UnBind()
        {
            UnBindButtonClicks();
        }
    }
}
