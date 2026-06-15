using UI.Constants;
using UI.Core;
using UI.Factories;
using UI.Interfaces;
using UI.Records;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Views
{
    public class GameOptionsView : BaseView, IScreen
    {
        private GameOptionsElements _elements;

        private string[] _inversionOptions = { "Off", "On" };
        private int _inversionIndex;

        private string[] _layoutOptions = { "Default", "Southpaw", "Legacy", "Legacy Southpaw" };
        private int _stickLayoutIndex;
        private int _buttonLayoutIndex;

        private string[] _sensitivityOptions = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" };
        private int _sensitivityIndex = 2;

        private string[] _assistOptions = { "Off", "On" };
        private int _assistIndex = 1;

        public GameOptionsView(VisualTreeAsset template) : base(template) { }

        protected override void GetElements() => _elements = ElementsFactory.GameOptions(Root);

        private static void CycleOption(ref int index, string[] options, Label label)
        {
            index = (index + 1) % options.Length;
            label.text = options[index];
        }

        private void HandleLookInversionClicked()
        {
            CycleOption(ref _inversionIndex, _inversionOptions, _elements.LookInversionLabel);
        }

        private void HandleStickLayoutClicked()
        {
            CycleOption(ref _stickLayoutIndex, _layoutOptions, _elements.StickLayoutLabel);
        }

        private void HandleButtonLayoutClicked()
        {
            CycleOption(ref _buttonLayoutIndex, _layoutOptions, _elements.ButtonLayoutLabel);
        }

        private void HandleSensitivityClicked()
        {
            CycleOption(ref _sensitivityIndex, _sensitivityOptions, _elements.SensitivityLabel);
        }

        private void HandleTargetAssistClicked()
        {
            CycleOption(ref _assistIndex, _assistOptions, _elements.TargetAssistLabel);
        }

        private static void HandlePlayerNameClicked()
        {
            Debug.Log("Player Name clicked");
        }

        private static void HandleGameVolumeClicked()
        {
            Debug.Log("Game Volume clicked");
            UIRouter.Instance.NavigateTo<GameVolumeView>();
        }

        protected override void Bind()
        {
            _elements.LookInversionButton.clicked += HandleLookInversionClicked;
            _elements.StickLayoutButton.clicked += HandleStickLayoutClicked;
            _elements.ButtonLayoutButton.clicked += HandleButtonLayoutClicked;
            _elements.SensitivityButton.clicked += HandleSensitivityClicked;
            _elements.TargetAssistButton.clicked += HandleTargetAssistClicked;
            _elements.PlayerNameButton.clicked += HandlePlayerNameClicked;
            _elements.GameVolumeButton.clicked += HandleGameVolumeClicked;
        }

        protected override void UnBind()
        {
            _elements.LookInversionButton.clicked -= HandleLookInversionClicked;
            _elements.StickLayoutButton.clicked -= HandleStickLayoutClicked;
            _elements.ButtonLayoutButton.clicked -= HandleButtonLayoutClicked;
            _elements.SensitivityButton.clicked -= HandleSensitivityClicked;
            _elements.TargetAssistButton.clicked -= HandleTargetAssistClicked;
            _elements.PlayerNameButton.clicked -= HandlePlayerNameClicked;
            _elements.GameVolumeButton.clicked -= HandleGameVolumeClicked;
        }

        public string HeaderName => ScreenNames.Options;
    }
}
