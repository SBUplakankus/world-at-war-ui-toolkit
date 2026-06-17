using Data;
using UI.Constants;
using UI.Core;
using UI.Factories;
using UI.Interfaces;
using UI.Records;
using UnityEngine.UIElements;

namespace UI.Views
{
    public sealed class GameOptionsView : BaseView, IScreen
    {
        private GameOptionsElements _elements;
        private PlayerSaveData _save;

        public string HeaderName => ScreenNames.Options;

        public GameOptionsView(VisualTreeAsset template) : base(template) { }

        protected override void GetElements() => _elements = ElementsFactory.GameOptions(Root);

        private void CycleSetting(ref string field, string[] options, TextElement label)
        {
            var index = System.Array.IndexOf(options, field);
            field = options[(index + 1) % options.Length];
            label.text = field;
            SaveDataManager.Save(_save);
        }

        private void HandleLookInversionClicked()
            => CycleSetting(ref _save.settings.invertLook, UIResources.OptionInversion, _elements.LookInversionLabel);

        private void HandleStickLayoutClicked()
            => CycleSetting(ref _save.settings.stickLayout, UIResources.OptionStickLayout, _elements.StickLayoutLabel);

        private void HandleButtonLayoutClicked()
            => CycleSetting(ref _save.settings.buttonLayout, UIResources.OptionButtonLayout, _elements.ButtonLayoutLabel);

        private void HandleSensitivityClicked()
            => CycleSetting(ref _save.settings.sensitivity, UIResources.OptionSensitivity, _elements.SensitivityLabel);

        private void HandleTargetAssistClicked()
            => CycleSetting(ref _save.settings.targetAssist, UIResources.OptionTargetAssist, _elements.TargetAssistLabel);

        private static void HandlePlayerNameClicked() { }

        private static void HandleGameVolumeClicked()
            => UIRouter.Instance.NavigateTo<GameVolumeView>();

        private void SyncLabels()
        {
            _save = SaveDataManager.CurrentSave;
            _elements.LookInversionLabel.text = _save.settings.invertLook;
            _elements.StickLayoutLabel.text = _save.settings.stickLayout;
            _elements.ButtonLayoutLabel.text = _save.settings.buttonLayout;
            _elements.SensitivityLabel.text = _save.settings.sensitivity;
            _elements.TargetAssistLabel.text = _save.settings.targetAssist;
            _elements.PlayerNameLabel.text = _save.settings.playerName;
        }

        private void BindButtonClicks()
        {
            _elements.LookInversionButton.clicked += HandleLookInversionClicked;
            _elements.StickLayoutButton.clicked += HandleStickLayoutClicked;
            _elements.ButtonLayoutButton.clicked += HandleButtonLayoutClicked;
            _elements.SensitivityButton.clicked += HandleSensitivityClicked;
            _elements.TargetAssistButton.clicked += HandleTargetAssistClicked;
            _elements.PlayerNameButton.clicked += HandlePlayerNameClicked;
            _elements.GameVolumeButton.clicked += HandleGameVolumeClicked;
        }

        private void UnBindButtonClicks()
        {
            _elements.LookInversionButton.clicked -= HandleLookInversionClicked;
            _elements.StickLayoutButton.clicked -= HandleStickLayoutClicked;
            _elements.ButtonLayoutButton.clicked -= HandleButtonLayoutClicked;
            _elements.SensitivityButton.clicked -= HandleSensitivityClicked;
            _elements.TargetAssistButton.clicked -= HandleTargetAssistClicked;
            _elements.PlayerNameButton.clicked -= HandlePlayerNameClicked;
            _elements.GameVolumeButton.clicked -= HandleGameVolumeClicked;
        }

        protected override void Bind()
        {
            SyncLabels();
            BindButtonClicks();
        }

        protected override void UnBind()
        {
            UnBindButtonClicks();
        }
    }
}
