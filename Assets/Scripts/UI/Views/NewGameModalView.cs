using System;
using UI.Core;
using UI.Views;
using UnityEngine.UIElements;

namespace UI.Views
{
    public class NewGameModalView : BaseView
    {
        private Button _closeButton;

        public NewGameModalView(VisualTreeAsset template) : base(template) { }

        protected override void GetElements()
        {
            _closeButton = Root.Q<Button>("close-btn");
        }

        protected override void Bind()
        {
            _closeButton.clicked += OnClose;
        }

        protected override void UnBind()
        {
            _closeButton.clicked -= OnClose;
        }

        private void OnClose() => UIRouter.Instance.CloseModal();
    }
}
