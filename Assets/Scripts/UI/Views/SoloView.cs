using UI.Constants;
using UI.Core;
using UI.Factories;
using UI.Interfaces;
using UI.Records;
using UI.Views;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Views
{
    public class SoloView : BaseView, IScreen
    {
        private SoloScreenElements _elements;
        
        public string HeaderName => ScreenNames.Solo;
        
        public SoloView(VisualTreeAsset template) : base(template) { }

        private bool SaveExists()
        {
            // TODO: Proper Save Check Logic
            return TestData.SaveExists;
        }

        private void DisplayButtonOptions()
        {
            if (!SaveExists())
            {
                _elements.NewGameButton.visible = true;
                _elements.ResumeButton.visible = false;
                _elements.MissionSelectButton.visible = false;
            }
            else
            {
                _elements.NewGameButton.visible = true;
                _elements.ResumeButton.visible = true;
                _elements.MissionSelectButton.visible = true;
            }
        }

        private void HandleNewGameClicked()
        {
            UIRouter.Instance.OpenModal<NewGameModalView>();
        }

        private void HandleResumeGameClicked()
        {
            // TODO: Loading Last Save Scene Logic
            Debug.Log("Resume Game Clicked");
        }

        private void HandleMissionSelectClicked()
        {
            UIRouter.Instance.NavigateTo<MissionSelectView>();
        }
        
        protected override void GetElements() => _elements = ElementsFactory.SoloScreen(Root);

        protected override void Bind()
        {
            _elements.NewGameButton.clicked += HandleNewGameClicked;
            _elements.ResumeButton.clicked += HandleResumeGameClicked;
            _elements.MissionSelectButton.clicked += HandleMissionSelectClicked;
            DisplayButtonOptions();
        }
        
        protected override void UnBind() 
        {
            _elements.NewGameButton.clicked -= HandleNewGameClicked;
            _elements.ResumeButton.clicked -= HandleResumeGameClicked;
            _elements.MissionSelectButton.clicked -= HandleMissionSelectClicked;
        }
        
    }
}
