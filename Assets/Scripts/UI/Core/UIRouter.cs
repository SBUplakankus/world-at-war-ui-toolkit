using UI.Factories;
using UI.Views;
using UnityEngine;

namespace UI.Core
{
    /// <summary>
    /// Singleton Class that orchestrates menu navigation and modal display
    /// </summary>
    public class UIRouter
    {
        private static UIRouter _instance;
        public static UIRouter Instance => _instance ??= new();

        private MenuLayout _layout;

        public void Register(MenuLayout layout)
        {
            _layout = layout;
        }

        public void NavigateTo<TView>() where TView : BaseView
        {
            if (!EnsureReady("NavigateTo")) return;
            var view = ViewFactory.Create<TView>();
            if (view != null)
                _layout.ShowScreen(view);
        }

        public void OpenModal<TModal>() where TModal : BaseView
        {
            if (!EnsureReady("OpenModal")) return;
            var view = ViewFactory.Create<TModal>();
            if (view != null)
                _layout.ShowModal(view);
        }

        public void Back()
        {
            if (!EnsureReady("Back")) return;
            _layout.BackScreen();
        }

        public void CloseModal()
        {
            if (!EnsureReady("CloseModal")) return;
            _layout.CloseModal();
        }

        public void ClearModals()
        {
            if (!EnsureReady("ClearModals")) return;
            _layout.ClearModals();
        }

        private bool EnsureReady(string method)
        {
            if (_layout) return true;
            Debug.LogError($"UIRouter.{method}: not registered — call Register() first");
            return false;
        }
    }
}
