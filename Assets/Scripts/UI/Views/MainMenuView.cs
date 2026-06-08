using UI.Constants;
using UI.Core;
using UI.Factories;
using UI.Interfaces;
using UI.Records;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Views
{
    public class MainMenuView : BaseView
    {
        private MainMenuElements _elements;

        public string HeaderName => ScreenNames.MainMenu;

        public MainMenuView(VisualTreeAsset template) : base(template)
        {
            
        }

        private static void HandleSoloClicked()
        {
            Debug.Log("SOLO clicked");
            UIRouter.Instance.NavigateTo<SoloView>();
            UIRouter.Instance.OpenModal<NoticeModalView>();
        }

        private static void HandleCoOpClicked()
        {
            Debug.Log("COOP clicked");
            UIRouter.Instance.NavigateTo<CoOpView>();
        }

        private static void HandleMultiplayerClicked()
        {
            Debug.Log("Multiplayer clicked");
            UIRouter.Instance.NavigateTo<MultiplayerView>();
        }

        private static void HandleZombiesClicked()
        {
            Debug.Log("Zombies clicked");
            UIRouter.Instance.NavigateTo<ZombiesView>();
        }

        private static void HandleOptionsClicked()
        {
            Debug.Log("Options clicked");
            UIRouter.Instance.NavigateTo<OptionsView>();
        }

        private static void HandleCreditsClicked()
        {
            Debug.Log("Credits clicked");
            UIRouter.Instance.NavigateTo<CreditsView>();
        }

        private static string FetchMessageOfTheDay()
        {
            return "Message of the Day it is the Message of the Day. Follow @massivemeltmedia on BlueSky and Subscribe on YouTube.";
        }

        protected override void GetElements() => _elements = ElementsFactory.MainMenu(Root);

        protected override void Bind()
        {
            _elements.SoloButton.clicked += HandleSoloClicked;
            _elements.CoOpButton.clicked += HandleCoOpClicked;
            _elements.MultiplayerButton.clicked += HandleMultiplayerClicked;
            _elements.ZombiesButton.clicked += HandleZombiesClicked;
            _elements.OptionsButton.clicked += HandleOptionsClicked;
            _elements.CreditsButton.clicked += HandleCreditsClicked;
            _elements.MessageLabel.text = FetchMessageOfTheDay();
        }

        protected override void UnBind()
        {
            _elements.SoloButton.clicked -= HandleSoloClicked;
            _elements.CoOpButton.clicked -= HandleCoOpClicked;
            _elements.MultiplayerButton.clicked -= HandleMultiplayerClicked;
            _elements.ZombiesButton.clicked -= HandleZombiesClicked;
            _elements.OptionsButton.clicked -= HandleOptionsClicked;
            _elements.CreditsButton.clicked -= HandleCreditsClicked;
        }
    }
}
