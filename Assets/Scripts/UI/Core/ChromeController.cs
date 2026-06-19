using System;
using UI.Constants;
using UI.Interfaces;
using UI.Records;
using UI.Views;

namespace UI.Core
{
    /// <summary>
    /// Manages the menu chrome — title, username, version labels and the back button.
    /// </summary>
    public class ChromeController
    {
        private readonly ScreenLayoutElements _elements;

        public ChromeController(ScreenLayoutElements elements)
        {
            _elements = elements;
        }

        public void SetTitle(BaseView view)
        {
            _elements.Header.text = view is IScreen screen
                ? screen.HeaderName
                : ScreenNames.MainMenu;
        }

        public void SetUsername(string username) => _elements.Username.text = $"Signed In: {username}";
        public void SetVersion(string version) => _elements.GameVersion.text = version;
        public void ShowBackButton() => _elements.BackButton.visible = true;
        public void HideBackButton() => _elements.BackButton.visible = false;
        public void BindBackButton(Action onBack) => _elements.BackButton.clicked += onBack;
        public void UnbindBackButton(Action onBack) => _elements.BackButton.clicked -= onBack;
    }
}
