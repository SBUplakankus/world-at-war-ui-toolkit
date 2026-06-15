using UI.Constants;
using UI.Core;
using UI.Factories;
using UI.Interfaces;
using UI.Records;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Views
{
    public class ResumeGameModalView : BaseView, IScreen
    {
        private ResumeGameModalElements _elements;

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

        protected override void Bind()
        {
            _elements.Yes.clicked += HandleYesClicked;
            _elements.No.clicked += HandleNoClicked;
        }

        protected override void UnBind()
        {
            _elements.Yes.clicked -= HandleYesClicked;
            _elements.No.clicked -= HandleNoClicked;
        }

        public string HeaderName => ScreenNames.ResumeGame;
    }
}
