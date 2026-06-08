using UI.Factories;
using UI.Views;

namespace UI.Core
{
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
            var view = ViewFactory.Create<TView>();
            if (view != null)
                _layout.ShowScreen(view);
        }

        public void OpenModal<TModal>() where TModal : BaseView
        {
            var view = ViewFactory.Create<TModal>();
            if (view != null)
                _layout.ShowModal(view);
        }

        public void Back()
        {
            _layout.BackScreen();
        }

        public void CloseModal()
        {
            _layout.CloseModal();
        }
    }
}
