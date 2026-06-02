using UI.Enums;
using UnityEngine;
using Screen = UI.Enums.Screen;

namespace UI.Services
{
    [CreateAssetMenu(menuName = "UI/UI Service")]
    public class UIService : ScriptableObject
    {
        private UIShell _shell;

        public void Register(UIShell shell) => _shell = shell;
        public void NavigateTo(Screen screen) => _shell.ShowScreen(screen);
        public void OpenModal(Modal modal) => _shell.ShowModal(modal);
        public void Back() => _shell.BackScreen();
        public void CloseModal() => _shell.CloseModal();
    }
}