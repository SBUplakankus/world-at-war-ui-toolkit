using Data;
using UI.Constants;
using UI.Factories;
using UI.Interfaces;
using UI.Records;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Views
{
    public class GameVolumeView : BaseView, IScreen
    {
        private GameVolumeElements _elements;
        private PlayerSaveData _save;

        public GameVolumeView(VisualTreeAsset template) : base(template) { }

        protected override void GetElements() => _elements = ElementsFactory.GameVolume(Root);

        private void OnVoiceChanged(ChangeEvent<float> evt)
        {
            _save.settings.voiceVolume = evt.newValue;
            SaveDataManager.Save(_save);
        }

        private void OnMusicChanged(ChangeEvent<float> evt)
        {
            _save.settings.musicVolume = evt.newValue;
            SaveDataManager.Save(_save);
        }

        private void OnSfxChanged(ChangeEvent<float> evt)
        {
            _save.settings.sfxVolume = evt.newValue;
            SaveDataManager.Save(_save);
        }

        private void OnCinematicsChanged(ChangeEvent<float> evt)
        {
            _save.settings.cinematicsVolume = evt.newValue;
            SaveDataManager.Save(_save);
        }

        private void OnMasterChanged(ChangeEvent<float> evt)
        {
            _save.settings.masterVolume = evt.newValue;
            SaveDataManager.Save(_save);
        }

        private void OnVoipChanged(ChangeEvent<float> evt)
        {
            _save.settings.voipVolume = evt.newValue;
            SaveDataManager.Save(_save);
        }

        private static void HandleVoiceClicked()
        {
            Debug.Log("Voice clicked");
        }

        private static void HandleMusicClicked()
        {
            Debug.Log("Music clicked");
        }

        private static void HandleSfxClicked()
        {
            Debug.Log("SFX clicked");
        }

        private static void HandleCinematicsClicked()
        {
            Debug.Log("Cinematics clicked");
        }

        private static void HandleMasterClicked()
        {
            Debug.Log("Master clicked");
        }

        private static void HandleVoipClicked()
        {
            Debug.Log("VoIP clicked");
        }

        protected override void Bind()
        {
            _save = SaveDataManager.CurrentSave;
            _elements.VoiceSlider.value = _save.settings.voiceVolume;
            _elements.MusicSlider.value = _save.settings.musicVolume;
            _elements.SfxSlider.value = _save.settings.sfxVolume;
            _elements.CinematicsSlider.value = _save.settings.cinematicsVolume;
            _elements.MasterSlider.value = _save.settings.masterVolume;
            _elements.VoipSlider.value = _save.settings.voipVolume;

            _elements.VoiceSlider.RegisterValueChangedCallback(OnVoiceChanged);
            _elements.MusicSlider.RegisterValueChangedCallback(OnMusicChanged);
            _elements.SfxSlider.RegisterValueChangedCallback(OnSfxChanged);
            _elements.CinematicsSlider.RegisterValueChangedCallback(OnCinematicsChanged);
            _elements.MasterSlider.RegisterValueChangedCallback(OnMasterChanged);
            _elements.VoipSlider.RegisterValueChangedCallback(OnVoipChanged);

            _elements.VoiceButton.clicked += HandleVoiceClicked;
            _elements.MusicButton.clicked += HandleMusicClicked;
            _elements.SfxButton.clicked += HandleSfxClicked;
            _elements.CinematicsButton.clicked += HandleCinematicsClicked;
            _elements.MasterButton.clicked += HandleMasterClicked;
            _elements.VoipButton.clicked += HandleVoipClicked;
        }

        protected override void UnBind()
        {
            _elements.VoiceSlider.UnregisterValueChangedCallback(OnVoiceChanged);
            _elements.MusicSlider.UnregisterValueChangedCallback(OnMusicChanged);
            _elements.SfxSlider.UnregisterValueChangedCallback(OnSfxChanged);
            _elements.CinematicsSlider.UnregisterValueChangedCallback(OnCinematicsChanged);
            _elements.MasterSlider.UnregisterValueChangedCallback(OnMasterChanged);
            _elements.VoipSlider.UnregisterValueChangedCallback(OnVoipChanged);

            _elements.VoiceButton.clicked -= HandleVoiceClicked;
            _elements.MusicButton.clicked -= HandleMusicClicked;
            _elements.SfxButton.clicked -= HandleSfxClicked;
            _elements.CinematicsButton.clicked -= HandleCinematicsClicked;
            _elements.MasterButton.clicked -= HandleMasterClicked;
            _elements.VoipButton.clicked -= HandleVoipClicked;
        }

        public string HeaderName => ScreenNames.GameVolume;
    }
}
