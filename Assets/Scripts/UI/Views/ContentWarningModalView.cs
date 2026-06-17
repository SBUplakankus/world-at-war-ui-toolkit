using UI.Constants;
using UI.Core;
using UI.Factories;
using UI.Interfaces;
using UI.Records;
using UnityEngine.UIElements;

namespace UI.Views
{
    public sealed class ContentWarningModalView : BaseView, IScreen
    {
        private NoticeModalElements _elements;

        public string HeaderName => ScreenNames.ContentWarning;

        public ContentWarningModalView(VisualTreeAsset template) : base(template)
        {
        }

        protected override void GetElements() => _elements = ElementsFactory.NoticeModal(Root);

        private static void HandleOkClicked()
        {
            UIRouter.Instance.CloseModal();
            UIRouter.Instance.OpenModal<DifficultyModalView>();
        }

        private void BindButtonClicks() => _elements.Ok.clicked += HandleOkClicked;

        private void UnBindButtonClicks() => _elements.Ok.clicked -= HandleOkClicked;

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
