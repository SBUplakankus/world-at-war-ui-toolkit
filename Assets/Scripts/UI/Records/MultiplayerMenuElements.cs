using UnityEngine.UIElements;

namespace UI.Records
{
    public sealed record MultiplayerMenuElements(
        Button PlayerOnline,
        Button SplitScreen,
        Button LanParty,
        Button Options,
        Button MainMenu,
        Label MessageOfTheDay
        );
}