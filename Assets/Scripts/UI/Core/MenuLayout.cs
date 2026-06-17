using Data;
using UI.Constants;
using UI.Factories;
using UI.Views;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Core
{
    [RequireComponent(typeof(UIDocument))]
    public class MenuLayout : MonoBehaviour
    {
        private UILayer _screenLayer;
        private UILayer _modalLayer;
        private HeaderController _header;
        private ModalController _modal;
        private BaseView _mainMenu;
        private bool _ready;

        private void Awake()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            var screenElements = ElementsFactory.ScreenLayout(root);
            var modalElements = ElementsFactory.ModalLayout(root);

            _screenLayer = new UILayer(screenElements.ScreenViewContainer);
            _modalLayer = new UILayer(modalElements.ModalViewContainer);
            _header = new HeaderController(screenElements);
            _modal = new ModalController(modalElements);

            _mainMenu = ViewFactory.Create<MainMenuView>();
            if (_mainMenu != null)
                _screenLayer.Push(_mainMenu);

            _header.HideBackButton();
            _header.SetTitle(_mainMenu);
            _header.SetUsername(SaveDataManager.CurrentSave.username);
            _header.SetVersion(Application.version);
            _modal.Hide();

            _header.BindBackButton(OnBackClicked);
            UIRouter.Instance.Register(this);

            _ready = true;
        }

        private void OnDestroy()
        {
            _header.UnbindBackButton(OnBackClicked);
        }

        private static void OnBackClicked() => UIRouter.Instance.Back();

        public void ShowScreen(BaseView view)
        {
            if (!_ready) return;

            _screenLayer.Push(view);
            _header.ShowBackButton();
            _header.SetTitle(view);
        }

        public void BackScreen()
        {
            if (!_ready) return;

            _screenLayer.Pop();

            if (_screenLayer.IsEmpty)
            {
                _header.HideBackButton();
                _header.SetTitle(_mainMenu);
            }
            else
            {
                _header.ShowBackButton();
                _header.SetTitle(_screenLayer.Current);
            }
        }

        public void ShowModal(BaseView view)
        {
            if (!_ready) return;

            _modalLayer.Push(view);
            _modal.Show();
            _modal.SetTitle(view);
        }

        public void CloseModal()
        {
            if (!_ready) return;

            _modalLayer.Pop();
            _modal.Hide();
        }
    }
}
