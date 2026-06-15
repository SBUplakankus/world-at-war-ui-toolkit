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

        public GameVolumeView(VisualTreeAsset template) : base(template) { }

        protected override void GetElements() => _elements = ElementsFactory.GameVolume(Root);

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
            _elements.VoiceButton.clicked += HandleVoiceClicked;
            _elements.MusicButton.clicked += HandleMusicClicked;
            _elements.SfxButton.clicked += HandleSfxClicked;
            _elements.CinematicsButton.clicked += HandleCinematicsClicked;
            _elements.MasterButton.clicked += HandleMasterClicked;
            _elements.VoipButton.clicked += HandleVoipClicked;
        }

        protected override void UnBind()
        {
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
