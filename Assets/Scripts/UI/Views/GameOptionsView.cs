using Data;
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
        private PlayerSaveData _save;

        public GameOptionsView(VisualTreeAsset template) : base(template) { }

        protected override void GetElements() => _elements = ElementsFactory.GameOptions(Root);

        private void SyncLabels()
        {
            var s = _save.settings;
            _elements.LookInversionLabel.text = s.invertLook;
            _elements.StickLayoutLabel.text = s.stickLayout;
            _elements.ButtonLayoutLabel.text = s.buttonLayout;
            _elements.SensitivityLabel.text = s.sensitivity;
            _elements.TargetAssistLabel.text = s.targetAssist;
            _elements.PlayerNameLabel.text = s.playerName;
        }

        private void CycleSetting(ref string field, string[] options)
        {
            var index = System.Array.IndexOf(options, field);
            field = options[(index + 1) % options.Length];
            SaveDataManager.Save(_save);
            SyncLabels();
        }

        private void HandleLookInversionClicked()
        {
            CycleSetting(ref _save.settings.invertLook, new[] { "Off", "On" });
        }

        private void HandleStickLayoutClicked()
        {
            CycleSetting(ref _save.settings.stickLayout, new[] { "Default", "Southpaw", "Legacy", "Legacy Southpaw" });
        }

        private void HandleButtonLayoutClicked()
        {
            CycleSetting(ref _save.settings.buttonLayout, new[] { "Default", "Southpaw", "Legacy", "Legacy Southpaw" });
        }

        private void HandleSensitivityClicked()
        {
            CycleSetting(ref _save.settings.sensitivity, new[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" });
        }

        private void HandleTargetAssistClicked()
        {
            CycleSetting(ref _save.settings.targetAssist, new[] { "Off", "On" });
        }

        private static void HandlePlayerNameClicked()
        {
            Debug.Log("Player Name clicked");
        }

        private static void HandleGameVolumeClicked()
        {
            UIRouter.Instance.NavigateTo<GameVolumeView>();
        }

        protected override void Bind()
        {
            _save = SaveDataManager.CurrentSave;
            SyncLabels();

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
