using UI.Constants;
using UI.Factories;
using UI.Interfaces;
using UI.Records;
using UnityEngine.UIElements;

namespace UI.Views
{
    public sealed class CreditsView : BaseView, IScreen
    {
        private CreditsScreenElements _elements;

        public CreditsView(VisualTreeAsset template) : base(template) { }

        protected override void GetElements() => _elements = ElementsFactory.CreditsScreen(Root);

        public string HeaderName => ScreenNames.Credits;
    }
}
