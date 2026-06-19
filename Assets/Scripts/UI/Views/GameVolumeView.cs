using Data;
using UI.Constants;
using UI.Factories;
using UI.Interfaces;
using UI.Records;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Views
{
    public sealed class GameVolumeView : BaseView, IScreen
    {
        private GameVolumeElements _elements;
        private PlayerSaveData _save;

        public string HeaderName => ScreenNames.GameVolume;

        public GameVolumeView(VisualTreeAsset template) : base(template) { }

        protected override void GetElements() => _elements = ElementsFactory.GameVolume(Root);

        private void SaveVolumeSettings(BlurEvent _)
        {
            SaveDataManager.Save(_save);
        }

        private void OnVoiceChanged(ChangeEvent<float> evt) => _save.settings.voiceVolume = evt.newValue;

        private void OnMusicChanged(ChangeEvent<float> evt) => _save.settings.musicVolume = evt.newValue;

        private void OnSfxChanged(ChangeEvent<float> evt) => _save.settings.sfxVolume = evt.newValue;

        private void OnCinematicsChanged(ChangeEvent<float> evt) => _save.settings.cinematicsVolume = evt.newValue;

        private void OnMasterChanged(ChangeEvent<float> evt) => _save.settings.masterVolume = evt.newValue;

        private void OnVoipChanged(ChangeEvent<float> evt) => _save.settings.voipVolume = evt.newValue;

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
            Debug.Log("Voip clicked");
        }

        private void LoadSliderValues()
        {
            _save = SaveDataManager.CurrentSave;
            _elements.VoiceSlider.value = _save.settings.voiceVolume;
            _elements.MusicSlider.value = _save.settings.musicVolume;
            _elements.SfxSlider.value = _save.settings.sfxVolume;
            _elements.CinematicsSlider.value = _save.settings.cinematicsVolume;
            _elements.MasterSlider.value = _save.settings.masterVolume;
            _elements.VoipSlider.value = _save.settings.voipVolume;
        }

        private void BindSliderHandlers()
        {
            _elements.VoiceSlider.RegisterValueChangedCallback(OnVoiceChanged);
            _elements.MusicSlider.RegisterValueChangedCallback(OnMusicChanged);
            _elements.SfxSlider.RegisterValueChangedCallback(OnSfxChanged);
            _elements.CinematicsSlider.RegisterValueChangedCallback(OnCinematicsChanged);
            _elements.MasterSlider.RegisterValueChangedCallback(OnMasterChanged);
            _elements.VoipSlider.RegisterValueChangedCallback(OnVoipChanged);
        }

        private void BindBlurHandlers()
        {
            _elements.VoiceSlider.RegisterCallback<BlurEvent>(SaveVolumeSettings);
            _elements.MusicSlider.RegisterCallback<BlurEvent>(SaveVolumeSettings);
            _elements.SfxSlider.RegisterCallback<BlurEvent>(SaveVolumeSettings);
            _elements.CinematicsSlider.RegisterCallback<BlurEvent>(SaveVolumeSettings);
            _elements.MasterSlider.RegisterCallback<BlurEvent>(SaveVolumeSettings);
            _elements.VoipSlider.RegisterCallback<BlurEvent>(SaveVolumeSettings);
        }

        private void BindButtonClicks()
        {
            _elements.VoiceButton.clicked += HandleVoiceClicked;
            _elements.MusicButton.clicked += HandleMusicClicked;
            _elements.SfxButton.clicked += HandleSfxClicked;
            _elements.CinematicsButton.clicked += HandleCinematicsClicked;
            _elements.MasterButton.clicked += HandleMasterClicked;
            _elements.VoipButton.clicked += HandleVoipClicked;
        }

        private void UnBindSliderHandlers()
        {
            _elements.VoiceSlider.UnregisterValueChangedCallback(OnVoiceChanged);
            _elements.MusicSlider.UnregisterValueChangedCallback(OnMusicChanged);
            _elements.SfxSlider.UnregisterValueChangedCallback(OnSfxChanged);
            _elements.CinematicsSlider.UnregisterValueChangedCallback(OnCinematicsChanged);
            _elements.MasterSlider.UnregisterValueChangedCallback(OnMasterChanged);
            _elements.VoipSlider.UnregisterValueChangedCallback(OnVoipChanged);
        }

        private void UnBindBlurHandlers()
        {
            _elements.VoiceSlider.UnregisterCallback<BlurEvent>(SaveVolumeSettings);
            _elements.MusicSlider.UnregisterCallback<BlurEvent>(SaveVolumeSettings);
            _elements.SfxSlider.UnregisterCallback<BlurEvent>(SaveVolumeSettings);
            _elements.CinematicsSlider.UnregisterCallback<BlurEvent>(SaveVolumeSettings);
            _elements.MasterSlider.UnregisterCallback<BlurEvent>(SaveVolumeSettings);
            _elements.VoipSlider.UnregisterCallback<BlurEvent>(SaveVolumeSettings);
        }

        private void UnBindButtonClicks()
        {
            _elements.VoiceButton.clicked -= HandleVoiceClicked;
            _elements.MusicButton.clicked -= HandleMusicClicked;
            _elements.SfxButton.clicked -= HandleSfxClicked;
            _elements.CinematicsButton.clicked -= HandleCinematicsClicked;
            _elements.MasterButton.clicked -= HandleMasterClicked;
            _elements.VoipButton.clicked -= HandleVoipClicked;
        }

        protected override void Bind()
        {
            LoadSliderValues();
            BindSliderHandlers();
            BindBlurHandlers();
            BindButtonClicks();
        }

        protected override void UnBind()
        {
            UnBindSliderHandlers();
            UnBindBlurHandlers();
            UnBindButtonClicks();
        }
    }
}
