using UI.Constants;
using UI.Core;
using UI.Factories;
using UI.Interfaces;
using UI.Records;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Views
{
    public sealed class ResumeGameModalView : BaseView, IScreen
    {
        private ResumeGameModalElements _elements;

        public string HeaderName => ScreenNames.ResumeGame;

        public ResumeGameModalView(VisualTreeAsset template) : base(template) { }

        protected override void GetElements() => _elements = ElementsFactory.ResumeGameModal(Root);

        private static void HandleYesClicked()
        {
            Debug.Log("Resume Game confirmed");
            UIRouter.Instance.CloseModal();
        }

        private static void HandleNoClicked()
        {
            Debug.Log("Resume Game cancelled");
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
