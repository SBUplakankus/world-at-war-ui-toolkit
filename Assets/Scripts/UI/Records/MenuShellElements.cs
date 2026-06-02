using UnityEngine.UIElements;

namespace UI.Records
{
    public sealed record MenuShellElements(
        Label MenuName,
        Label Username,
        Label GameVersion,
        Button BackButton
    );
}