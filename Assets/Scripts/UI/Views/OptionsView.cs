using UI.Constants;
using UI.Interfaces;
using UI.Views;
using UnityEngine.UIElements;

namespace UI.Views
{
    public class OptionsView : BaseView, IScreen
    {
        public OptionsView(VisualTreeAsset template) : base(template) { }

        protected override void GetElements() { }
        protected override void Bind() { }
        protected override void UnBind() { }
        public string HeaderName => ScreenNames.Options;
    }
}
