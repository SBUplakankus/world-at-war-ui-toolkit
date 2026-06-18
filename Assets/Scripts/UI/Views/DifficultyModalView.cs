using Data;
using UI.Constants;
using UI.Core;
using UI.Enums;
using UI.Factories;
using UI.Interfaces;
using UI.Records;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Views
{
    public sealed class DifficultyModalView : BaseView, IScreen
    {
        private DifficultyModalElements _elements;

        public string HeaderName => ScreenNames.Difficulty;

        public DifficultyModalView(VisualTreeAsset template) : base(template) { }

        protected override void GetElements() => _elements = ElementsFactory.DifficultyModal(Root);

        private static void DifficultySelected(Difficulty difficulty)
        {
            var save = SaveDataManager.CurrentSave;
            save.campaignStarted = true;
            save.missionsCompleted = 0;
            SaveDataManager.Save(save);
            Debug.Log("Difficulty set to: " + difficulty);
            UIRouter.Instance.CloseModal();
            UIRouter.Instance.Back();
        }

        private void SetPreview(Difficulty difficulty)
        {
            _elements.Icon.vectorImage = UIResources.DifficultyIcons[difficulty];
            _elements.Description.text = UIResources.DifficultyDescriptions[difficulty];
        }

        private void OnRecruitEnter(PointerEnterEvent _) => SetPreview(Difficulty.Recruit);
        private void OnRegularEnter(PointerEnterEvent _) => SetPreview(Difficulty.Regular);
        private void OnHardenedEnter(PointerEnterEvent _) => SetPreview(Difficulty.Hardened);
        private void OnVeteranEnter(PointerEnterEvent _) => SetPreview(Difficulty.Veteran);

        private void HandleRecruitClicked() => DifficultySelected(Difficulty.Recruit);

        private void HandleRegularClicked() => DifficultySelected(Difficulty.Regular);

        private void HandleHardenedClicked() => DifficultySelected(Difficulty.Hardened);

        private void HandleVeteranClicked() => DifficultySelected(Difficulty.Veteran);

        private void BindButtonHovers()
        {
            _elements.Recruit.RegisterCallback<PointerEnterEvent>(OnRecruitEnter);
            _elements.Regular.RegisterCallback<PointerEnterEvent>(OnRegularEnter);
            _elements.Hardened.RegisterCallback<PointerEnterEvent>(OnHardenedEnter);
            _elements.Veteran.RegisterCallback<PointerEnterEvent>(OnVeteranEnter);
        }

        private void BindButtonClicks()
        {
            _elements.Recruit.clicked += HandleRecruitClicked;
            _elements.Regular.clicked += HandleRegularClicked;
            _elements.Hardened.clicked += HandleHardenedClicked;
            _elements.Veteran.clicked += HandleVeteranClicked;
        }

        private void UnBindButtonHovers()
        {
            _elements.Recruit.UnregisterCallback<PointerEnterEvent>(OnRecruitEnter);
            _elements.Regular.UnregisterCallback<PointerEnterEvent>(OnRegularEnter);
            _elements.Hardened.UnregisterCallback<PointerEnterEvent>(OnHardenedEnter);
            _elements.Veteran.UnregisterCallback<PointerEnterEvent>(OnVeteranEnter);
        }

        private void UnBindButtonClicks()
        {
            _elements.Recruit.clicked -= HandleRecruitClicked;
            _elements.Regular.clicked -= HandleRegularClicked;
            _elements.Hardened.clicked -= HandleHardenedClicked;
            _elements.Veteran.clicked -= HandleVeteranClicked;
        }

        protected override void Bind()
        {
            BindButtonHovers();
            BindButtonClicks();
        }

        protected override void UnBind()
        {
            UnBindButtonHovers();
            UnBindButtonClicks();
        }
    }
}
