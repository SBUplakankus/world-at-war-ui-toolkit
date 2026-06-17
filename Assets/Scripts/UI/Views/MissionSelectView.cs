using Data;
using UI.Constants;
using UI.Enums;
using UI.Factories;
using UI.Interfaces;
using UI.Records;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Views
{
    public sealed class MissionSelectView : BaseView, IScreen
    {
        private MissionSelectElements _elements;

        public string HeaderName => ScreenNames.MissionSelect;

        public MissionSelectView(VisualTreeAsset template) : base(template) { }

        protected override void GetElements() => _elements = ElementsFactory.MissionSelect(Root);

        private void DisplayMission(Missions mission)
        {
            _elements.Header.text = UIResources.MissionTitles[mission];
            _elements.Description.text = UIResources.MissionDescriptions[mission];
        }

        private void HandleSemperFiClicked()
        {
            Debug.Log("Semper Fi selected");
        }

        private void HandleLittleResistanceClicked()
        {
            Debug.Log("Little Resistance selected");
        }

        private void HandleHardLandingClicked()
        {
            Debug.Log("Hard Landing selected");
        }

        private void HandleVendettaClicked()
        {
            Debug.Log("Vendetta selected");
        }

        private void HandleTheirLandTheirBloodClicked()
        {
            Debug.Log("Their Land Their Blood selected");
        }

        private void HandleBurnEmOutClicked()
        {
            Debug.Log("Burn Em Out selected");
        }

        private void HandleRelentlessClicked()
        {
            Debug.Log("Relentless selected");
        }

        private void HandleBloodAndIronClicked()
        {
            Debug.Log("Blood and Iron selected");
        }

        private void HandleRingOfSteelClicked()
        {
            Debug.Log("Ring of Steel selected");
        }

        private void HandleEvictionClicked()
        {
            Debug.Log("Eviction selected");
        }

        private void HandleBlackCatsClicked()
        {
            Debug.Log("Black Cats selected");
        }

        private void HandleBlowtorchAndCorkscrewClicked()
        {
            Debug.Log("Blowtorch and Corkscrew selected");
        }

        private void HandleBreakingPointClicked()
        {
            Debug.Log("Breaking Point selected");
        }

        private void HandleHeartOfTheReichClicked()
        {
            Debug.Log("Heart of the Reich selected");
        }

        private void HandleDownfallClicked()
        {
            Debug.Log("Downfall selected");
        }

        private void OnSemperFiHover(PointerEnterEvent _) => DisplayMission(Missions.SemperFi);
        private void OnLittleResistanceHover(PointerEnterEvent _) => DisplayMission(Missions.LittleResistance);
        private void OnHardLandingHover(PointerEnterEvent _) => DisplayMission(Missions.HardLanding);
        private void OnVendettaHover(PointerEnterEvent _) => DisplayMission(Missions.Vendetta);
        private void OnTheirLandTheirBloodHover(PointerEnterEvent _) => DisplayMission(Missions.TheirLandTheirBlood);
        private void OnBurnEmOutHover(PointerEnterEvent _) => DisplayMission(Missions.BurnEmOut);
        private void OnRelentlessHover(PointerEnterEvent _) => DisplayMission(Missions.Relentless);
        private void OnBloodAndIronHover(PointerEnterEvent _) => DisplayMission(Missions.BloodAndIron);
        private void OnRingOfSteelHover(PointerEnterEvent _) => DisplayMission(Missions.RingOfSteel);
        private void OnEvictionHover(PointerEnterEvent _) => DisplayMission(Missions.Eviction);
        private void OnBlackCatsHover(PointerEnterEvent _) => DisplayMission(Missions.BlackCats);
        private void OnBlowtorchAndCorkscrewHover(PointerEnterEvent _) => DisplayMission(Missions.BlowtorchAndCorkscrew);
        private void OnBreakingPointHover(PointerEnterEvent _) => DisplayMission(Missions.BreakingPoint);
        private void OnHeartOfTheReichHover(PointerEnterEvent _) => DisplayMission(Missions.HeartOfTheReich);
        private void OnDownfallHover(PointerEnterEvent _) => DisplayMission(Missions.Downfall);

        protected override void Bind()
        {
            var completed = SaveDataManager.CurrentSave.missionsCompleted;

            _elements.SemperFi.RegisterCallback<PointerEnterEvent>(OnSemperFiHover);
            _elements.LittleResistance.RegisterCallback<PointerEnterEvent>(OnLittleResistanceHover);
            _elements.HardLanding.RegisterCallback<PointerEnterEvent>(OnHardLandingHover);
            _elements.Vendetta.RegisterCallback<PointerEnterEvent>(OnVendettaHover);
            _elements.TheirLandTheirBlood.RegisterCallback<PointerEnterEvent>(OnTheirLandTheirBloodHover);
            _elements.BurnEmOut.RegisterCallback<PointerEnterEvent>(OnBurnEmOutHover);
            _elements.Relentless.RegisterCallback<PointerEnterEvent>(OnRelentlessHover);
            _elements.BloodAndIron.RegisterCallback<PointerEnterEvent>(OnBloodAndIronHover);
            _elements.RingOfSteel.RegisterCallback<PointerEnterEvent>(OnRingOfSteelHover);
            _elements.Eviction.RegisterCallback<PointerEnterEvent>(OnEvictionHover);
            _elements.BlackCats.RegisterCallback<PointerEnterEvent>(OnBlackCatsHover);
            _elements.BlowtorchAndCorkscrew.RegisterCallback<PointerEnterEvent>(OnBlowtorchAndCorkscrewHover);
            _elements.BreakingPoint.RegisterCallback<PointerEnterEvent>(OnBreakingPointHover);
            _elements.HeartOfTheReich.RegisterCallback<PointerEnterEvent>(OnHeartOfTheReichHover);
            _elements.Downfall.RegisterCallback<PointerEnterEvent>(OnDownfallHover);

            _elements.SemperFi.clicked += HandleSemperFiClicked;
            _elements.LittleResistance.clicked += HandleLittleResistanceClicked;
            _elements.HardLanding.clicked += HandleHardLandingClicked;
            _elements.Vendetta.clicked += HandleVendettaClicked;
            _elements.TheirLandTheirBlood.clicked += HandleTheirLandTheirBloodClicked;
            _elements.BurnEmOut.clicked += HandleBurnEmOutClicked;
            _elements.Relentless.clicked += HandleRelentlessClicked;
            _elements.BloodAndIron.clicked += HandleBloodAndIronClicked;
            _elements.RingOfSteel.clicked += HandleRingOfSteelClicked;
            _elements.Eviction.clicked += HandleEvictionClicked;
            _elements.BlackCats.clicked += HandleBlackCatsClicked;
            _elements.BlowtorchAndCorkscrew.clicked += HandleBlowtorchAndCorkscrewClicked;
            _elements.BreakingPoint.clicked += HandleBreakingPointClicked;
            _elements.HeartOfTheReich.clicked += HandleHeartOfTheReichClicked;
            _elements.Downfall.clicked += HandleDownfallClicked;

            _elements.SemperFi.SetEnabled(true);
            _elements.LittleResistance.SetEnabled(1 < completed);
            _elements.HardLanding.SetEnabled(2 < completed);
            _elements.Vendetta.SetEnabled(3 < completed);
            _elements.TheirLandTheirBlood.SetEnabled(4 < completed);
            _elements.BurnEmOut.SetEnabled(5 < completed);
            _elements.Relentless.SetEnabled(6 < completed);
            _elements.BloodAndIron.SetEnabled(7 < completed);
            _elements.RingOfSteel.SetEnabled(8 < completed);
            _elements.Eviction.SetEnabled(9 < completed);
            _elements.BlackCats.SetEnabled(10 < completed);
            _elements.BlowtorchAndCorkscrew.SetEnabled(11 < completed);
            _elements.BreakingPoint.SetEnabled(12 < completed);
            _elements.HeartOfTheReich.SetEnabled(13 < completed);
            _elements.Downfall.SetEnabled(14 < completed);

            _elements.Header.text = UIResources.MissionTitles[Missions.SemperFi];
            _elements.Description.text = UIResources.MissionDescriptions[Missions.SemperFi];
        }

        protected override void UnBind()
        {
            _elements.SemperFi.UnregisterCallback<PointerEnterEvent>(OnSemperFiHover);
            _elements.LittleResistance.UnregisterCallback<PointerEnterEvent>(OnLittleResistanceHover);
            _elements.HardLanding.UnregisterCallback<PointerEnterEvent>(OnHardLandingHover);
            _elements.Vendetta.UnregisterCallback<PointerEnterEvent>(OnVendettaHover);
            _elements.TheirLandTheirBlood.UnregisterCallback<PointerEnterEvent>(OnTheirLandTheirBloodHover);
            _elements.BurnEmOut.UnregisterCallback<PointerEnterEvent>(OnBurnEmOutHover);
            _elements.Relentless.UnregisterCallback<PointerEnterEvent>(OnRelentlessHover);
            _elements.BloodAndIron.UnregisterCallback<PointerEnterEvent>(OnBloodAndIronHover);
            _elements.RingOfSteel.UnregisterCallback<PointerEnterEvent>(OnRingOfSteelHover);
            _elements.Eviction.UnregisterCallback<PointerEnterEvent>(OnEvictionHover);
            _elements.BlackCats.UnregisterCallback<PointerEnterEvent>(OnBlackCatsHover);
            _elements.BlowtorchAndCorkscrew.UnregisterCallback<PointerEnterEvent>(OnBlowtorchAndCorkscrewHover);
            _elements.BreakingPoint.UnregisterCallback<PointerEnterEvent>(OnBreakingPointHover);
            _elements.HeartOfTheReich.UnregisterCallback<PointerEnterEvent>(OnHeartOfTheReichHover);
            _elements.Downfall.UnregisterCallback<PointerEnterEvent>(OnDownfallHover);

            _elements.SemperFi.clicked -= HandleSemperFiClicked;
            _elements.LittleResistance.clicked -= HandleLittleResistanceClicked;
            _elements.HardLanding.clicked -= HandleHardLandingClicked;
            _elements.Vendetta.clicked -= HandleVendettaClicked;
            _elements.TheirLandTheirBlood.clicked -= HandleTheirLandTheirBloodClicked;
            _elements.BurnEmOut.clicked -= HandleBurnEmOutClicked;
            _elements.Relentless.clicked -= HandleRelentlessClicked;
            _elements.BloodAndIron.clicked -= HandleBloodAndIronClicked;
            _elements.RingOfSteel.clicked -= HandleRingOfSteelClicked;
            _elements.Eviction.clicked -= HandleEvictionClicked;
            _elements.BlackCats.clicked -= HandleBlackCatsClicked;
            _elements.BlowtorchAndCorkscrew.clicked -= HandleBlowtorchAndCorkscrewClicked;
            _elements.BreakingPoint.clicked -= HandleBreakingPointClicked;
            _elements.HeartOfTheReich.clicked -= HandleHeartOfTheReichClicked;
            _elements.Downfall.clicked -= HandleDownfallClicked;
        }
    }
}
