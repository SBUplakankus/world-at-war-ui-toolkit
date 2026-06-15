using UI.Constants;
using UI.Interfaces;
using UI.Records;
using UI.Views;
using UnityEngine.UIElements;

namespace UI.Views
{
    public class MultiplayerMenuView : BaseView, IScreen
    {
        private MultiplayerMenuElements _elements;
        
        public MultiplayerMenuView(VisualTreeAsset template) : base(template) { }

        protected override void GetElements() { }
        protected override void Bind() { }
        protected override void UnBind() { }
        public string HeaderName => ScreenNames.Multiplayer;
    }
}
