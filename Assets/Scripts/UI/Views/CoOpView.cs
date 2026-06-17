using UI.Constants;
using UI.Core;
using UI.Factories;
using UI.Interfaces;
using UI.Records;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Views
{
    public sealed class CoOpView : BaseView, IScreen
    {
        private CoOpScreenElements _elements;

        public string HeaderName => ScreenNames.CoOp;

        public CoOpView(VisualTreeAsset template) : base(template) { }

        private static void HandleOnlineClicked()
        {
            UIRouter.Instance.OpenModal<NoConnectionModalView>();
        }

        private static void HandleSplitScreenClicked()
        {
            UIRouter.Instance.OpenModal<NoConnectionModalView>();
        }

        private static void HandleLanClicked()
        {
            UIRouter.Instance.OpenModal<NoConnectionModalView>();
        }

        private void BindButtonClicks()
        {
            _elements.OnlineButton.clicked += HandleOnlineClicked;
            _elements.SplitScreenButton.clicked += HandleSplitScreenClicked;
            _elements.LanButton.clicked += HandleLanClicked;
        }

        private void UnBindButtonClicks()
        {
            _elements.OnlineButton.clicked -= HandleOnlineClicked;
            _elements.SplitScreenButton.clicked -= HandleSplitScreenClicked;
            _elements.LanButton.clicked -= HandleLanClicked;
        }

        protected override void GetElements() => _elements = ElementsFactory.CoOpScreen(Root);

        protected override void Bind() =>  BindButtonClicks();

        protected override void UnBind() => UnBindButtonClicks();
    }
}
