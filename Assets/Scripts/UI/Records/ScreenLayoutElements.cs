using UnityEngine.UIElements;

namespace UI.Records
{
    public sealed record ScreenLayoutElements(
        Label Header,
        Label Username,
        Label GameVersion,
        Button BackButton,
        VisualElement ScreenViewContainer
    );
}