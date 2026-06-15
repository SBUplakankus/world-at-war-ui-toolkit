using UI.Interfaces;
using UI.Records;
using UnityEngine.UIElements;

namespace UI.Views
{
    public class GameOptionsView : BaseView, IScreen
    {
        private GameOptionsElements _elements;
        
        public GameOptionsView(VisualTreeAsset template) : base(template)
        {
        }

        protected override void GetElements()
        {
            throw new System.NotImplementedException();
        }

        protected override void Bind()
        {
            throw new System.NotImplementedException();
        }

        protected override void UnBind()
        {
            throw new System.NotImplementedException();
        }

        public string HeaderName { get; }
    }
}