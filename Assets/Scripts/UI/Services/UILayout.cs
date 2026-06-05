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
        private LayoutElements _elements;
        private BaseView _mainMenu;

        private void GetLayers()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            _screenLayer = new UILayer(root.Q(UIElements.MainContainer));
            _modalLayer = new UILayer(root.Q(UIElements.ModalContainer), true);
            
            _elements = ElementsFactory.Layout(root);
        }
        
        private void ToggleBackButton(bool toggle) => _elements.BackButton.visible = toggle;
        
        private void SetHeaderTitle(BaseView view)
        {
            _elements.MenuName.text = view is IScreen screen
                ? screen.HeaderName
                : ScreenNames.MainMenu;
        }
        
        private void SetUsername()
        {
            // TODO: Fetch Username Test Logic
            _elements.Username.text = $"Signed In: {TestData.Username}";
        }

        private void SetVersion()
        {
            // TODO: Fetch Version if at all
            _elements.GameVersion.text = TestData.Version;
        }

        private void SetMainMenu()
        {
            _mainMenu = ViewFactory.Create<MainMenuView>();
            if (_mainMenu != null)
                _screenLayer.Push(_mainMenu);
            
            ToggleBackButton(false);
            SetHeaderTitle(_mainMenu);
            SetUsername();
            SetVersion();
        }

        private void Bind() => _elements.BackButton.clicked += OnBackClicked;
        
        private void Register() => UIRouter.Instance.Register(this);

        private void OnBackClicked() => UIRouter.Instance.Back();

        public void ShowScreen(BaseView view)
        {
            _screenLayer.Push(view);
            ToggleBackButton(true);
            SetHeaderTitle(view);
        } 
        public void ShowModal(BaseView view) => _modalLayer.Push(view);

        public void BackScreen()
        {
            _screenLayer.Pop();

            if (!_screenLayer.IsEmpty) return;
            
            ToggleBackButton(false);
            SetHeaderTitle(_mainMenu);
        }
        
        public void CloseModal() => _modalLayer.Pop();
        
        private void Awake()
        {
            GetLayers();
            SetMainMenu();
            Bind();
            Register();
        }

        private void OnDestroy()
        {
            if (_elements.BackButton != null)
                _elements.BackButton.clicked -= OnBackClicked;
        }
    }
}
