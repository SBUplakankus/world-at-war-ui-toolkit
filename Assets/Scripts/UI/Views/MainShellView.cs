using UI.Factories;
using UI.Records;
using UI.Services;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Views
{
    public class MainShellView : BaseView
    {
        private MenuShellElements _elements;
        
        public MainShellView(VisualTreeAsset template, UIService uiService) : base(template, uiService)
        {
        }

        private void SetHeaderName(string headerName) => _elements.MenuName.text = headerName;
        private void SetUsername(string username) => _elements.Username.text = username;
        private void SetGameVersion(string gameVersion) => _elements.GameVersion.text = gameVersion;

        private void HandleBackClicked()
        {
            Debug.Log("Back clicked");
            UIService.Back();
        }
        
        protected override void GetElements() => _elements = ElementsFactory.MenuShell(Root);

        protected override void Bind()
        {
            SetHeaderName("Main Menu");
            SetUsername("Sean");
            SetGameVersion("1.0.1");
            _elements.BackButton.clicked += HandleBackClicked;
        }

        protected override void UnBind()
        {
            _elements.BackButton.clicked -= HandleBackClicked;
        }
    }
}