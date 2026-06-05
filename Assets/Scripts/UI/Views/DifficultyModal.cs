using System;
using UI.Views;
using UnityEngine.UIElements;

namespace UI.Views
{
    public class DifficultyModal : BaseView
    {
        private Button _closeButton;

        public DifficultyModal(VisualTreeAsset template) : base(template) { }

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
