using UnityEngine.UIElements;

namespace UI.Records
{
    public sealed record NoConnectionModalElements(
        Label Notice,
        Image Icon,
        Button Ok
        );
}
