using Data;
using UI.Constants;
using UI.Factories;
using UI.Views;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Core
{
    /// <summary>
    /// Core MonoBehaviour Root Class for the Main Menu. Stores the UI Layers, Chrome & Modal Controllers.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class MenuRoot : MonoBehaviour
    {
        private UILayer _screenLayer;
        private UILayer _modalLayer;
        private ChromeController _chrome;
        private ModalController _modal;
        private BaseView _mainMenu;
        private bool _ready;

        private void Awake()
        {
            SaveDataManager.Delete();

            var root = GetComponent<UIDocument>().rootVisualElement;

            var screenElements = ElementsFactory.ScreenLayout(root);
            var modalElements = ElementsFactory.ModalLayout(root);

            _screenLayer = new UILayer(screenElements.ScreenViewContainer);
            _modalLayer = new UILayer(modalElements.ModalViewContainer);
            _chrome = new ChromeController(screenElements);
            _modal = new ModalController(modalElements);

            _mainMenu = ViewFactory.Create<MainMenuView>();

            _chrome.HideBackButton();
            _chrome.SetTitle(_mainMenu);
            _chrome.SetUsername(SaveDataManager.CurrentSave.username);
            _chrome.SetVersion(Application.version);
            _modal.Hide();

            _chrome.BindBackButton(OnBackClicked);
            UIRouter.Instance.Register(this);
            _ready = true;

            if (_mainMenu != null)
                _screenLayer.Push(_mainMenu);
        }

        private void Start()
        {
            if (!SaveDataManager.SaveFileExists)
                UIRouter.Instance.OpenModal<SaveNoticeView>();
        }

        private void OnDestroy()
        {
            _chrome.UnbindBackButton(OnBackClicked);
            _screenLayer?.Clear();
            _modalLayer?.Clear();
        }

        private static void OnBackClicked() => UIRouter.Instance.Back();

        public void ShowScreen(BaseView view)
        {
            if (!_ready)
            {
                Debug.LogWarning("MenuRoot.ShowScreen: not ready");
                return;
            }

            _screenLayer.Push(view);
            _chrome.ShowBackButton();
            _chrome.SetTitle(view);
        }

        public void BackScreen()
        {
            if (!_ready)
            {
                Debug.LogWarning("MenuRoot.BackScreen: not ready");
                return;
            }

            _screenLayer.Pop();

            if (_screenLayer.IsEmpty)
            {
                _chrome.HideBackButton();
                _chrome.SetTitle(_mainMenu);
            }
            else
            {
                _chrome.ShowBackButton();
                _chrome.SetTitle(_screenLayer.Current);
            }
        }

        public void ShowModal(BaseView view)
        {
            if (!_ready)
            {
                Debug.LogWarning("MenuRoot.ShowModal: not ready");
                return;
            }

            _modalLayer.Push(view);
            _modal.Show();
            _modal.SetTitle(view);
        }

        public void CloseModal()
        {
            if (!_ready)
            {
                Debug.LogWarning("MenuRoot.CloseModal: not ready");
                return;
            }

            _modalLayer.Pop();
            _modal.Hide();
        }

        public void ClearModals()
        {
            if (!_ready)
            {
                Debug.LogWarning("MenuRoot.ClearModals: not ready");
                return;
            }

            _modalLayer.Clear();
            _modal.Hide();
        }
    }
}
