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

        private MenuRoot _root;

        public void Register(MenuRoot root)
        {
            _root = root;
        }

        public void NavigateTo<TView>() where TView : BaseView
        {
            if (!EnsureReady("NavigateTo")) return;
            var view = ViewFactory.Create<TView>();
            if (view != null)
                _root.ShowScreen(view);
        }

        public void OpenModal<TModal>() where TModal : BaseView
        {
            if (!EnsureReady("OpenModal")) return;
            var view = ViewFactory.Create<TModal>();
            if (view != null)
                _root.ShowModal(view);
        }

        public void Back()
        {
            if (!EnsureReady("Back")) return;
            _root.BackScreen();
        }

        public void CloseModal()
        {
            if (!EnsureReady("CloseModal")) return;
            _root.CloseModal();
        }

        public void ClearModals()
        {
            if (!EnsureReady("ClearModals")) return;
            _root.ClearModals();
        }

        private bool EnsureReady(string method)
        {
            if (_root) return true;
            Debug.LogError($"UIRouter.{method}: not registered — call Register() first");
            return false;
        }
    }
}
