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
    public sealed class NewGameModalView : BaseView, IScreen
    {
        private NewGameModalElements _elements;

        public string HeaderName => ScreenNames.NewGame;

        public NewGameModalView(VisualTreeAsset template) : base(template) { }

        protected override void GetElements() => _elements = ElementsFactory.NewGameModal(Root);

        private static void HandleYesClicked()
        {
            UIRouter.Instance.OpenModal<ContentWarningModalView>();
        }

        private static void HandleNoClicked()
        {
            Debug.Log("New Game cancelled");
            UIRouter.Instance.CloseModal();
        }

        private void BindButtonClicks()
        {
            _elements.Yes.clicked += HandleYesClicked;
            _elements.No.clicked += HandleNoClicked;
        }

        private void UnBindButtonClicks()
        {
            _elements.Yes.clicked -= HandleYesClicked;
            _elements.No.clicked -= HandleNoClicked;
        }

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
