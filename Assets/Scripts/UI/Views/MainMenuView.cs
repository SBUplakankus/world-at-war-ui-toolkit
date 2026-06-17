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
    public sealed class MainMenuView : BaseView
    {
        private MainMenuElements _elements;

        public string HeaderName => ScreenNames.MainMenu;

        public MainMenuView(VisualTreeAsset template) : base(template) { }

        protected override void GetElements() => _elements = ElementsFactory.MainMenu(Root);

        private static void CheckSaveFile()
        {
            try
            {
                if (!SaveDataManager.SaveFileExists)
                    UIRouter.Instance.OpenModal<SaveNoticeView>();
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"MainMenuView: failed to check save file — {e.Message}");
            }
        }

        private void SetMessageOfTheDay()
        {
            _elements.MessageLabel.text = UIResources.MessagesOfTheDay.Random();
        }

        private static void HandleSoloClicked()
        {
            Debug.Log("SOLO clicked");
            UIRouter.Instance.NavigateTo<SoloView>();
        }

        private static void HandleCoOpClicked()
        {
            Debug.Log("COOP clicked");
            UIRouter.Instance.NavigateTo<CoOpView>();
        }

        private static void HandleMultiplayerClicked()
        {
            Debug.Log("Multiplayer clicked");
            UIRouter.Instance.NavigateTo<MultiplayerMenuView>();
        }

        private static void HandleZombiesClicked()
        {
            Debug.Log("Zombies clicked");
            UIRouter.Instance.NavigateTo<ZombiesView>();
        }

        private static void HandleOptionsClicked()
        {
            Debug.Log("Options clicked");
            UIRouter.Instance.NavigateTo<GameOptionsView>();
        }

        private static void HandleCreditsClicked()
        {
            Debug.Log("Credits clicked");
            UIRouter.Instance.NavigateTo<CreditsView>();
        }

        private void BindButtonClicks()
        {
            _elements.SoloButton.clicked += HandleSoloClicked;
            _elements.CoOpButton.clicked += HandleCoOpClicked;
            _elements.MultiplayerButton.clicked += HandleMultiplayerClicked;
            _elements.ZombiesButton.clicked += HandleZombiesClicked;
            _elements.OptionsButton.clicked += HandleOptionsClicked;
            _elements.CreditsButton.clicked += HandleCreditsClicked;
        }

        private void UnBindButtonClicks()
        {
            _elements.SoloButton.clicked -= HandleSoloClicked;
            _elements.CoOpButton.clicked -= HandleCoOpClicked;
            _elements.MultiplayerButton.clicked -= HandleMultiplayerClicked;
            _elements.ZombiesButton.clicked -= HandleZombiesClicked;
            _elements.OptionsButton.clicked -= HandleOptionsClicked;
            _elements.CreditsButton.clicked -= HandleCreditsClicked;
        }

        protected override void Bind()
        {
            CheckSaveFile();
            BindButtonClicks();
            SetMessageOfTheDay();
        }

        protected override void UnBind()
        {
            UnBindButtonClicks();
        }
    }
}
