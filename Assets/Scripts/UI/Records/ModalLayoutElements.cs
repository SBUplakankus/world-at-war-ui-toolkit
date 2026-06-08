using UnityEngine.UIElements;

namespace UI.Records
{
    public sealed record ModalLayoutElements(
        Label Header,
        VisualElement ModalViewContainer,
        VisualElement ModalContainer
        );
}