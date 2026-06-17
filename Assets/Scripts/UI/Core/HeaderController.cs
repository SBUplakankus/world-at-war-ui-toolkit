using UI.Constants;
using UI.Interfaces;
using UI.Records;
using UI.Views;
using UnityEngine.UIElements;

namespace UI.Core
{
    public class HeaderController
    {
        private readonly ScreenLayoutElements _elements;

        public HeaderController(ScreenLayoutElements elements)
        {
            _elements = elements;
        }

        public void SetTitle(BaseView view)
        {
            _elements.Header.text = view is IScreen screen
                ? screen.HeaderName
                : ScreenNames.MainMenu;
        }

        public void SetUsername(string username)
        {
            _elements.Username.text = $"Signed In: {username}";
        }

        public void SetVersion(string version)
        {
            _elements.GameVersion.text = version;
        }

        public void ShowBackButton()
        {
            _elements.BackButton.visible = true;
        }

        public void HideBackButton()
        {
            _elements.BackButton.visible = false;
        }

        public void BindBackButton(System.Action onBack)
        {
            _elements.BackButton.clicked += onBack;
        }

        public void UnbindBackButton(System.Action onBack)
        {
            _elements.BackButton.clicked -= onBack;
        }
    }
}
