using UnityEngine.UIElements;

namespace UI.Records
{
    public record GameVolumeElements(
        Button VoiceButton,
        Slider VoiceSlider,
        Button MusicButton,
        Slider MusicSlider,
        Button SfxButton,
        Slider SfxSlider,
        Button CinematicsButton,
        Slider CinematicsSlider,
        Button MasterButton,
        Slider MasterSlider,
        Button VoipButton,
        Slider VoipSlider
        );
}