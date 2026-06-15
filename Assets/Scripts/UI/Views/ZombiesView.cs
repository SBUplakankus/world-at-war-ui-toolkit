using UI.Constants;
using UI.Core;
using UI.Factories;
using UI.Interfaces;
using UI.Records;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Views
{
    public class ZombiesView : BaseView, IScreen
    {
        private ZombiesScreenElements _elements;

        public ZombiesView(VisualTreeAsset template) : base(template) { }

        protected override void GetElements() => _elements = ElementsFactory.ZombiesScreen(Root);

        private void HandleResumeClicked()
        {
            Debug.Log("Zombies Resume Game clicked");
        }

        private void HandleNewGameClicked()
        {
            Debug.Log("Zombies New Game clicked");
            UIRouter.Instance.OpenModal<NewGameModalView>();
        }

        protected override void Bind()
        {
            _elements.ResumeButton.clicked += HandleResumeClicked;
            _elements.NewGameButton.clicked += HandleNewGameClicked;
        }

        protected override void UnBind()
        {
            _elements.ResumeButton.clicked -= HandleResumeClicked;
            _elements.NewGameButton.clicked -= HandleNewGameClicked;
        }

        public string HeaderName => ScreenNames.Zombies;
    }
}
