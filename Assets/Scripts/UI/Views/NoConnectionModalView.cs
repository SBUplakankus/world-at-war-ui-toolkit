using UI.Constants;
using UI.Core;
using UI.Factories;
using UI.Interfaces;
using UI.Records;
using UnityEngine.UIElements;

namespace UI.Views
{
    public sealed class NoConnectionModalView : BaseView, IScreen
    {
        private NoConnectionModalElements _elements;

        public string HeaderName => ScreenNames.Notice;

        public NoConnectionModalView(VisualTreeAsset template) : base(template)
        {
        }

        protected override void GetElements() => _elements = ElementsFactory.NoConnectionModal(Root);

        private static void HandleOkClicked() => UIRouter.Instance.CloseModal();

        private void BindButtonClicks()
        {
            _elements.Ok.clicked += HandleOkClicked;
        }

        private void UnBindButtonClicks()
        {
            _elements.Ok.clicked -= HandleOkClicked;
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
