using UI.Constants;
using UI.Core;
using UI.Factories;
using UI.Interfaces;
using UI.Records;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Views
{
    public class DifficultyModalView : BaseView, IScreen
    {
        private DifficultyModalElements _elements;

        public DifficultyModalView(VisualTreeAsset template) : base(template) { }

        protected override void GetElements() => _elements = ElementsFactory.DifficultyModal(Root);

        private static void SetDifficulty(string difficulty)
        {
            Debug.Log($"Difficulty set to: {difficulty}");
            UIRouter.Instance.CloseModal();
        }

        private void HandleRecruitClicked() => SetDifficulty("Recruit");
        private void HandleRegularClicked() => SetDifficulty("Regular");
        private void HandleHardenedClicked() => SetDifficulty("Hardened");
        private void HandleVeteranClicked() => SetDifficulty("Veteran");

        protected override void Bind()
        {
            _elements.Recruit.clicked += HandleRecruitClicked;
            _elements.Regular.clicked += HandleRegularClicked;
            _elements.Hardened.clicked += HandleHardenedClicked;
            _elements.Veteran.clicked += HandleVeteranClicked;
        }

        protected override void UnBind()
        {
            _elements.Recruit.clicked -= HandleRecruitClicked;
            _elements.Regular.clicked -= HandleRegularClicked;
            _elements.Hardened.clicked -= HandleHardenedClicked;
            _elements.Veteran.clicked -= HandleVeteranClicked;
        }

        public string HeaderName => ScreenNames.Difficulty;
    }
}
