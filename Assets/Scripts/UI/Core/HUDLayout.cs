using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Core
{
    [RequireComponent(typeof(UIDocument))]
    public class HUDLayout : MonoBehaviour
    {
        private UILayer _hudLayer;
        private UILayer _pauseLayer;
    }
}