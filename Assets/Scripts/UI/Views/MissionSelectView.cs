using Data;
using UI.Constants;
using UI.Factories;
using UI.Interfaces;
using UI.Records;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Views
{
    public class MissionSelectView : BaseView, IScreen
    {
        private MissionSelectElements _elements;
        private MissionEntry[] _missions;

        public MissionSelectView(VisualTreeAsset template) : base(template) { }

        protected override void GetElements() => _elements = ElementsFactory.MissionSelect(Root);

        private void DisplayMission(MissionEntry entry)
        {
            _elements.Header.text = entry.header;
            _elements.Description.text = entry.description;
        }

        private void HandleSemperFiClicked()
        {
            Debug.Log("Semper Fi selected");
            DisplayMission(_missions[0]);
        }

        private void HandleLittleResistanceClicked()
        {
            Debug.Log("Little Resistance selected");
            DisplayMission(_missions[1]);
        }

        private void HandleHardLandingClicked()
        {
            Debug.Log("Hard Landing selected");
            DisplayMission(_missions[2]);
        }

        private void HandleVendettaClicked()
        {
            Debug.Log("Vendetta selected");
            DisplayMission(_missions[3]);
        }

        private void HandleTheirLandTheirBloodClicked()
        {
            Debug.Log("Their Land Their Blood selected");
            DisplayMission(_missions[4]);
        }

        private void HandleBurnEmOutClicked()
        {
            Debug.Log("Burn Em Out selected");
            DisplayMission(_missions[5]);
        }

        private void HandleRelentlessClicked()
        {
            Debug.Log("Relentless selected");
            DisplayMission(_missions[6]);
        }

        private void HandleBloodAndIronClicked()
        {
            Debug.Log("Blood and Iron selected");
            DisplayMission(_missions[7]);
        }

        private void HandleRingOfSteelClicked()
        {
            Debug.Log("Ring of Steel selected");
            DisplayMission(_missions[8]);
        }

        private void HandleEvictionClicked()
        {
            Debug.Log("Eviction selected");
            DisplayMission(_missions[9]);
        }

        private void HandleBlackCatsClicked()
        {
            Debug.Log("Black Cats selected");
            DisplayMission(_missions[10]);
        }

        private void HandleBlowtorchAndCorkscrewClicked()
        {
            Debug.Log("Blowtorch and Corkscrew selected");
            DisplayMission(_missions[11]);
        }

        private void HandleBreakingPointClicked()
        {
            Debug.Log("Breaking Point selected");
            DisplayMission(_missions[12]);
        }

        private void HandleHeartOfTheReichClicked()
        {
            Debug.Log("Heart of the Reich selected");
            DisplayMission(_missions[13]);
        }

        private void HandleDownfallClicked()
        {
            Debug.Log("Downfall selected");
            DisplayMission(_missions[14]);
        }

        protected override void Bind()
        {
            _missions = MissionDataManager.AllMissions;
            var completed = SaveDataManager.CurrentSave.missionsCompleted;

            _elements.SemperFi.clicked += HandleSemperFiClicked;
            _elements.SemperFi.SetEnabled(0 < completed);

            _elements.LittleResistance.clicked += HandleLittleResistanceClicked;
            _elements.LittleResistance.SetEnabled(1 < completed);

            _elements.HardLanding.clicked += HandleHardLandingClicked;
            _elements.HardLanding.SetEnabled(2 < completed);

            _elements.Vendetta.clicked += HandleVendettaClicked;
            _elements.Vendetta.SetEnabled(3 < completed);

            _elements.TheirLandTheirBlood.clicked += HandleTheirLandTheirBloodClicked;
            _elements.TheirLandTheirBlood.SetEnabled(4 < completed);

            _elements.BurnEmOut.clicked += HandleBurnEmOutClicked;
            _elements.BurnEmOut.SetEnabled(5 < completed);

            _elements.Relentless.clicked += HandleRelentlessClicked;
            _elements.Relentless.SetEnabled(6 < completed);

            _elements.BloodAndIron.clicked += HandleBloodAndIronClicked;
            _elements.BloodAndIron.SetEnabled(7 < completed);

            _elements.RingOfSteel.clicked += HandleRingOfSteelClicked;
            _elements.RingOfSteel.SetEnabled(8 < completed);

            _elements.Eviction.clicked += HandleEvictionClicked;
            _elements.Eviction.SetEnabled(9 < completed);

            _elements.BlackCats.clicked += HandleBlackCatsClicked;
            _elements.BlackCats.SetEnabled(10 < completed);

            _elements.BlowtorchAndCorkscrew.clicked += HandleBlowtorchAndCorkscrewClicked;
            _elements.BlowtorchAndCorkscrew.SetEnabled(11 < completed);

            _elements.BreakingPoint.clicked += HandleBreakingPointClicked;
            _elements.BreakingPoint.SetEnabled(12 < completed);

            _elements.HeartOfTheReich.clicked += HandleHeartOfTheReichClicked;
            _elements.HeartOfTheReich.SetEnabled(13 < completed);

            _elements.Downfall.clicked += HandleDownfallClicked;
            _elements.Downfall.SetEnabled(14 < completed);

            _elements.Thumbnail.image = null;
            _elements.Header.text = "SELECT A MISSION";
            _elements.Description.text = "Choose a mission from the list.";
        }

        protected override void UnBind()
        {
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

        public string HeaderName => ScreenNames.MissionSelect;
    }
}
