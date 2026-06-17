using Data;
using UI.Constants;
using UI.Core;
using UI.Enums;
using UI.Factories;
using UI.Interfaces;
using UI.Records;
using UnityEngine;
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

        protected override void Bind()
        {
            _elements.Ok.clicked += HandleOkClicked;
        }

        protected override void UnBind()
        {
            _elements.Ok.clicked -= HandleOkClicked;
        }
    }
}
