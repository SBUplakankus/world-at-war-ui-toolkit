using UI.Factories;
using UI.Records;
using UI.Services;
using UnityEngine;
using UnityEngine.UIElements;
using Screen = UI.Enums.Screen;

namespace UI.Views
{
    public class MainMenuView : BaseView
    {
        private MainMenuElements _elements;

        public MainMenuView(VisualTreeAsset template, UIService uiService) : base(template, uiService)
        {
        }

        private void HandleSoloClicked()
        {
            Debug.Log("SOLO clicked");
            UIService.NavigateTo(Screen.Solo);
        }

        private void HandleCoOpClicked()
        {
            Debug.Log("COOP clicked");
            UIService.NavigateTo(Screen.CoOp);
        }
        

        private void HandleMultiplayerClicked()
        {
            Debug.Log("Multiplayer clicked");
            UIService.NavigateTo(Screen.Multiplayer);
        }

        private void HandleZombiesClicked()
        {
            Debug.Log("Zombies clicked");
            UIService.NavigateTo(Screen.Zombies);
        }

        private void HandleOptionsClicked()
        {
            Debug.Log("Options clicked");
            UIService.NavigateTo(Screen.Options);
        }

        private void HandleCreditsClicked()
        {
            Debug.Log("Credits clicked");
            UIService.NavigateTo(Screen.Credits);
        }

        private string FetchMessageOfTheDay()
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