using UnityEngine.UIElements;

namespace UI.Records
{
    public record GameOptionsElements(
        Button LookInversionButton,
        Label LookInversionLabel,
        Button StickLayoutButton,
        Label StickLayoutLabel,
        Button ButtonLayoutButton,
        Label ButtonLayoutLabel,
        Button SensitivityButton,
        Label SensitivityLabel,
        Button TargetAssistButton,
        Label TargetAssistLabel,
        Button PlayerNameButton,
        Label PlayerNameLabel,
        Button GameVolumeButton
        );
}