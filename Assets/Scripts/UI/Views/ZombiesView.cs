using UI.Constants;
using UI.Interfaces;
using UI.Views;
using UnityEngine.UIElements;

namespace UI.Views
{
    public class ZombiesView : BaseView, IScreen
    {
        public ZombiesView(VisualTreeAsset template) : base(template) { }

        protected override void GetElements() { }
        protected override void Bind() { }
        protected override void UnBind() { }
        public string HeaderName => ScreenNames.Zombies;
    }
}
