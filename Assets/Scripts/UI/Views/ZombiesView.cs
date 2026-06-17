using UI.Constants;
using UI.Core;
using UI.Factories;
using UI.Interfaces;
using UI.Records;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Views
{
    public sealed class ZombiesView : BaseView, IScreen
    {
        private ZombiesScreenElements _elements;

        public string HeaderName => ScreenNames.Zombies;

        public ZombiesView(VisualTreeAsset template) : base(template) { }

        private static void HandleResumeClicked()
        {
            UIRouter.Instance.OpenModal<NoConnectionModalView>();
        }

        private static void HandleNewGameClicked()
        {
            Debug.Log("Zombies New Game clicked");
            UIRouter.Instance.OpenModal<NoConnectionModalView>();
        }

        private void BindButtonClicks()
        {
            _elements.ResumeButton.clicked += HandleResumeClicked;
            _elements.NewGameButton.clicked += HandleNewGameClicked;
        }

        private void UnBindButtonClicks()
        {
            _elements.ResumeButton.clicked -= HandleResumeClicked;
            _elements.NewGameButton.clicked -= HandleNewGameClicked;
        }

        protected override void GetElements() => _elements = ElementsFactory.ZombiesScreen(Root);

        protected override void Bind()
        {
            BindButtonClicks();
        }

        protected override void UnBind()
        {
            UnBindButtonClicks();
        }
    }
}
