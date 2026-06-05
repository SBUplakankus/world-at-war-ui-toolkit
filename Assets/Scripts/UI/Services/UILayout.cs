using UI.Constants;
using UI.Factories;
using UI.Interfaces;
using UI.Records;
using UI.Views;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Services
{
    [RequireComponent(typeof(UIDocument))]
    public class UILayout : MonoBehaviour
    {
        private UILayer _screenLayer;
        private UILayer _modalLayer;
        
        private ScreenLayoutElements _screenElements;
        private ModalLayoutElements _modalElements;
        
        private BaseView _mainMenu;

        private void GetLayers()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            
            _screenElements = ElementsFactory.ScreenLayout(root);
            _modalElements = ElementsFactory.ModalLayout(root);
            
            _screenLayer = new UILayer(_screenElements.ScreenViewContainer);
            _modalLayer = new UILayer(_modalElements.ModalViewContainer);
        }
        
        private void ToggleBackButton(bool toggle) => _screenElements.BackButton.visible = toggle;

        private void ToggleModal(bool toggle)
        {
            _modalElements.ModalViewContainer.visible = toggle;
            _modalElements.ModalViewContainer.pickingMode = toggle 
                ? PickingMode.Position
                : PickingMode.Ignore;
        }
        
        private static void SetHeaderTitle(Label header, BaseView view)
        {
            header.text = view is IScreen screen
                ? screen.HeaderName
                : ScreenNames.MainMenu;
        }
        
        private void SetUsername()
        {
            // TODO: Fetch Username Test Logic
            _screenElements.Username.text = $"Signed In: {TestData.Username}";
        }

        private void SetVersion()
        {
            // TODO: Fetch Version if at all
            _screenElements.GameVersion.text = TestData.Version;
        }

        private void SetMainMenu()
        {
            _mainMenu = ViewFactory.Create<MainMenuView>();
            if (_mainMenu != null)
                _screenLayer.Push(_mainMenu);
            
            ToggleBackButton(false);
            SetHeaderTitle(_screenElements.Header, _mainMenu);
            SetUsername();
            SetVersion();
        }

        private void Bind() => _screenElements.BackButton.clicked += OnBackClicked;
        
        private void Register() => UIRouter.Instance.Register(this);

        private void OnBackClicked() => UIRouter.Instance.Back();

        public void ShowScreen(BaseView view)
        {
            _screenLayer.Push(view);
            ToggleBackButton(true);
            SetHeaderTitle(_screenElements.Header, view);
        }

        public void BackScreen()
        {
            _screenLayer.Pop();

            if (!_screenLayer.IsEmpty) return;
            
            ToggleBackButton(false);
            SetHeaderTitle(_screenElements.Header, _mainMenu);
        }


        public void ShowModal(BaseView view)
        {
            _modalLayer.Push(view);
            ToggleModal(true);
            SetHeaderTitle(_modalElements.Header, view);
        }

        public void CloseModal()
        {
            _modalLayer.Pop();
            ToggleModal(false);
        }
        
        private void Awake()
        {
            GetLayers();
            SetMainMenu();
            Bind();
            Register();
        }

        private void OnDestroy()
        {
            if (_screenElements.BackButton != null)
                _screenElements.BackButton.clicked -= OnBackClicked;
        }
    }
}
