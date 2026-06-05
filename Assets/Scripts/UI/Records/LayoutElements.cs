using UnityEngine.UIElements;

namespace UI.Records
{
    public sealed record LayoutElements(
        Label MenuName,
        Label Username,
        Label GameVersion,
        Button BackButton
    );
}