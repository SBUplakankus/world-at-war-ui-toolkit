using UI.Constants;
using UI.Core;
using UI.Factories;
using UI.Interfaces;
using UI.Records;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Views
{
    public sealed class SoloView : BaseView, IScreen
    {
        private SoloScreenElements _elements;
        
        public string HeaderName => ScreenNames.Solo;
        
        public SoloView(VisualTreeAsset template) : base(template) { }
        
        protected override void GetElements() => _elements = ElementsFactory.SoloScreen(Root);

        private static bool CampaignStarted()
        {
            return Data.SaveDataManager.CurrentSave.campaignStarted;
        }

        private static void HandleNewGameClicked()
        {
            UIRouter.Instance.OpenModal<NewGameModalView>();
        }

        private static void HandleResumeGameClicked()
        {
            // TODO: Loading Last Save Scene Logic
            UIRouter.Instance.OpenModal<ResumeGameModalView>();
        }

        private static void HandleMissionSelectClicked()
        {
            UIRouter.Instance.NavigateTo<MissionSelectView>();
        }

        private void DisplayButtonOptions()
        {
            if (!CampaignStarted())
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

        private void BindButtonClicks()
        {
            _elements.NewGameButton.clicked += HandleNewGameClicked;
            _elements.ResumeButton.clicked += HandleResumeGameClicked;
            _elements.MissionSelectButton.clicked += HandleMissionSelectClicked;
        }

        private void UnBindButtonClicks()
        {
            _elements.NewGameButton.clicked -= HandleNewGameClicked;
            _elements.ResumeButton.clicked -= HandleResumeGameClicked;
            _elements.MissionSelectButton.clicked -= HandleMissionSelectClicked;
        }

        protected override void Bind()
        {
            BindButtonClicks();
            DisplayButtonOptions();
        }
        
        protected override void UnBind() 
        {
            UnBindButtonClicks();
        }
        
    }
}
