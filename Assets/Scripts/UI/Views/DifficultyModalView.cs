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

        private static void CloseModal()
        {
            UIRouter.Instance.CloseModal();
        }

        private void OnRecruitEnter(PointerEnterEvent _)
        {
            _elements.Icon.image = UIResources.DifficultyIcons[Difficulty.Recruit];
            _elements.Description.text = UIResources.DifficultyDescriptions[Difficulty.Recruit];
        }

        private void OnRegularEnter(PointerEnterEvent _)
        {
            _elements.Icon.image = UIResources.DifficultyIcons[Difficulty.Regular];
            _elements.Description.text = UIResources.DifficultyDescriptions[Difficulty.Regular];
        }

        private void OnHardenedEnter(PointerEnterEvent _)
        {
            _elements.Icon.image = UIResources.DifficultyIcons[Difficulty.Hardened];
            _elements.Description.text = UIResources.DifficultyDescriptions[Difficulty.Hardened];
        }

        private void OnVeteranEnter(PointerEnterEvent _)
        {
            _elements.Icon.image = UIResources.DifficultyIcons[Difficulty.Veteran];
            _elements.Description.text = UIResources.DifficultyDescriptions[Difficulty.Veteran];
        }

        private void OnPointerLeave(PointerLeaveEvent _)
        {
            _elements.Icon.image = UIResources.DifficultyIcons[Difficulty.Hardened];
            _elements.Description.text = "Label";
        }

        private void HandleRecruitClicked()
        {
            Debug.Log("Difficulty set to: Recruit");
            CloseModal();
        }

        private void HandleRegularClicked()
        {
            Debug.Log("Difficulty set to: Regular");
            CloseModal();
        }

        private void HandleHardenedClicked()
        {
            Debug.Log("Difficulty set to: Hardened");
            CloseModal();
        }

        private void HandleVeteranClicked()
        {
            Debug.Log("Difficulty set to: Veteran");
            CloseModal();
        }

        protected override void Bind()
        {
            _elements.Recruit.RegisterCallback<PointerEnterEvent>(OnRecruitEnter);
            _elements.Regular.RegisterCallback<PointerEnterEvent>(OnRegularEnter);
            _elements.Hardened.RegisterCallback<PointerEnterEvent>(OnHardenedEnter);
            _elements.Veteran.RegisterCallback<PointerEnterEvent>(OnVeteranEnter);

            _elements.Recruit.RegisterCallback<PointerLeaveEvent>(OnPointerLeave);
            _elements.Regular.RegisterCallback<PointerLeaveEvent>(OnPointerLeave);
            _elements.Hardened.RegisterCallback<PointerLeaveEvent>(OnPointerLeave);
            _elements.Veteran.RegisterCallback<PointerLeaveEvent>(OnPointerLeave);

            _elements.Recruit.clicked += HandleRecruitClicked;
            _elements.Regular.clicked += HandleRegularClicked;
            _elements.Hardened.clicked += HandleHardenedClicked;
            _elements.Veteran.clicked += HandleVeteranClicked;
        }

        protected override void UnBind()
        {
            _elements.Recruit.UnregisterCallback<PointerEnterEvent>(OnRecruitEnter);
            _elements.Regular.UnregisterCallback<PointerEnterEvent>(OnRegularEnter);
            _elements.Hardened.UnregisterCallback<PointerEnterEvent>(OnHardenedEnter);
            _elements.Veteran.UnregisterCallback<PointerEnterEvent>(OnVeteranEnter);

            _elements.Recruit.UnregisterCallback<PointerLeaveEvent>(OnPointerLeave);
            _elements.Regular.UnregisterCallback<PointerLeaveEvent>(OnPointerLeave);
            _elements.Hardened.UnregisterCallback<PointerLeaveEvent>(OnPointerLeave);
            _elements.Veteran.UnregisterCallback<PointerLeaveEvent>(OnPointerLeave);

            _elements.Recruit.clicked -= HandleRecruitClicked;
            _elements.Regular.clicked -= HandleRegularClicked;
            _elements.Hardened.clicked -= HandleHardenedClicked;
            _elements.Veteran.clicked -= HandleVeteranClicked;
        }
    }
}
