using UI.Constants;
using UI.Factories;
using UI.Interfaces;
using UI.Records;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Views
{
    public class CoOpView : BaseView, IScreen
    {
        private CoOpScreenElements _elements;

        public CoOpView(VisualTreeAsset template) : base(template) { }

        protected override void GetElements() => _elements = ElementsFactory.CoOpScreen(Root);

        private void HandleOnlineClicked()
        {
            Debug.Log("Online Co-Op clicked");
        }

        private void HandleSplitScreenClicked()
        {
            Debug.Log("Split Screen Co-Op clicked");
        }

        private void HandleLanClicked()
        {
            Debug.Log("LAN Co-Op clicked");
        }

        protected override void Bind()
        {
            _elements.Online.clicked += HandleOnlineClicked;
            _elements.SplitScreen.clicked += HandleSplitScreenClicked;
            _elements.Lan.clicked += HandleLanClicked;
        }

        protected override void UnBind()
        {
            _elements.Online.clicked -= HandleOnlineClicked;
            _elements.SplitScreen.clicked -= HandleSplitScreenClicked;
            _elements.Lan.clicked -= HandleLanClicked;
        }

        public string HeaderName => ScreenNames.CoOp;
    }
}
