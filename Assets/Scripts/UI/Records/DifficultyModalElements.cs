using UnityEngine.UIElements;

namespace UI.Records
{
    public sealed record DifficultyModalElements(
        Button Recruit,
        Button Regular,
        Button Hardened,
        Button Veteran,
        Image Icon,
        Label Description
    );
}