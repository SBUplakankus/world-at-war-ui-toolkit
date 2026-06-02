using System.Collections.Generic;
using UI.Constants;
using UI.Enums;
using UI.Views;
using UnityEngine;
using UnityEngine.UIElements;
using Screen = UI.Enums.Screen;

namespace UI.Services
{
    [RequireComponent(typeof(UIDocument))]
    public class UIShell : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private UIService service;
        [SerializeField] private UILibrary library;

        private Dictionary<Screen, VisualTreeAsset> _screens;
        private Dictionary<Modal, VisualTreeAsset> _modals;
        
        private UILayer _screenLayer;
        private UILayer _modalLayer;

        private void Awake()
        {
            _screens = library.ScreenMap();
            _modals = library.ModalMap();
            
            var root = GetComponent<UIDocument>().rootVisualElement;
            _screenLayer = new UILayer(root.Q(UIElements.MainContainer));
            _modalLayer  = new UILayer(root.Q(UIElements.ModalContainer));

            service.Register(this);
        }

        // public void ShowScreen(Screen screen) => _screenLayer.Push(_screens[screen]);
        // public void ShowModal(Modal modal) => _modalLayer.Push(_modals[modal]);
        public void BackScreen() => _screenLayer.Pop();
        public void CloseModal() => _modalLayer.Pop();
    }
}