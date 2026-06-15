using UI.Constants;
using UI.Factories;
using UI.Interfaces;
using UI.Records;
using UnityEngine.UIElements;

namespace UI.Views
{
    public class CreditsView : BaseView, IScreen
    {
        private CreditsScreenElements _elements;

        public CreditsView(VisualTreeAsset template) : base(template) { }

        protected override void GetElements() => _elements = ElementsFactory.CreditsScreen(Root);
        protected override void Bind() { }
        protected override void UnBind() { }

        public string HeaderName => ScreenNames.Credits;
    }
}
