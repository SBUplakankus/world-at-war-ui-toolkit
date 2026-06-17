using UnityEngine.UIElements;

namespace UI.Records
{
    public sealed record MultiplayerMenuElements(
        Button OnlineButton,
        Button SplitScreenButton,
        Button LanPartyButton,
        Button OptionsButton,
        Label MessageOfTheDay
        );
}