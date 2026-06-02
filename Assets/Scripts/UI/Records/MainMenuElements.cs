using UnityEngine.UIElements;

namespace UI.Records
{
    public sealed record MainMenuElements(
        Button SoloButton,
        Button CoOpButton,
        Button ZombiesButton,
        Button MultiplayerButton,
        Button OptionsButton,
        Button CreditsButton,
        Label MessageLabel
        );
}