using UI.Constants;
using UI.Interfaces;
using UI.Records;
using UI.Views;
using UnityEngine.UIElements;

namespace UI.Views
{
    public class CoOpView : BaseView, IScreen
    {
        private CoOpScreenElements _elements;
        public CoOpView(VisualTreeAsset template) : base(template) { }

        protected override void GetElements() { }
        protected override void Bind() { }
        protected override void UnBind() { }
        
        public string HeaderName => ScreenNames.CoOp;
    }
}
