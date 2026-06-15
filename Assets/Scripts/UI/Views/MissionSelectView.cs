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

        public MissionSelectView(VisualTreeAsset template) : base(template) { }

        protected override void GetElements() => _elements = ElementsFactory.MissionSelect(Root);

        private void DisplayMission(string header, string description)
        {
            _elements.Header.text = header;
            _elements.Description.text = description;
        }

        private void HandleSemperFiClicked()
        {
            Debug.Log("Semper Fi selected");
            DisplayMission("SEMPER FI", "The Marines assault the beaches of Betio Island.");
        }

        private void HandleLittleResistanceClicked()
        {
            Debug.Log("Little Resistance selected");
            DisplayMission("LITTLE RESISTANCE", "Advance through the jungles of Peleliu.");
        }

        private void HandleHardLandingClicked()
        {
            Debug.Log("Hard Landing selected");
            DisplayMission("HARD LANDING", "Secure the airfield on Peleliu.");
        }

        private void HandleVendettaClicked()
        {
            Debug.Log("Vendetta selected");
            DisplayMission("VENDETTA", "Fight alongside Reznov in the ruins of Stalingrad.");
        }

        private void HandleTheirLandTheirBloodClicked()
        {
            Debug.Log("Their Land Their Blood selected");
            DisplayMission("THEIR LAND, THEIR BLOOD", "Push through the Russian countryside.");
        }

        private void HandleBurnEmOutClicked()
        {
            Debug.Log("Burn Em Out selected");
            DisplayMission("BURN 'EM OUT", "Use flamethrowers to clear the jungle.");
        }

        private void HandleRelentlessClicked()
        {
            Debug.Log("Relentless selected");
            DisplayMission("RELENTLESS", "Fight through the ruins of Stalingrad.");
        }

        private void HandleBloodAndIronClicked()
        {
            Debug.Log("Blood and Iron selected");
            DisplayMission("BLOOD & IRON", "Command a tank through enemy lines.");
        }

        private void HandleRingOfSteelClicked()
        {
            Debug.Log("Ring of Steel selected");
            DisplayMission("RING OF STEEL", "Defend the outskirts of Berlin.");
        }

        private void HandleEvictionClicked()
        {
            Debug.Log("Eviction selected");
            DisplayMission("EVICTION", "Clear buildings in Berlin.");
        }

        private void HandleBlackCatsClicked()
        {
            Debug.Log("Black Cats selected");
            DisplayMission("BLACK CATS", "Fly a PBY Catalina on a rescue mission.");
        }

        private void HandleBlowtorchAndCorkscrewClicked()
        {
            Debug.Log("Blowtorch and Corkscrew selected");
            DisplayMission("BLOWTORCH & CORNSCREW", "Assault Okinawa's beaches and caves.");
        }

        private void HandleBreakingPointClicked()
        {
            Debug.Log("Breaking Point selected");
            DisplayMission("BREAKING POINT", "Fight through the caves of Okinawa.");
        }

        private void HandleHeartOfTheReichClicked()
        {
            Debug.Log("Heart of the Reich selected");
            DisplayMission("HEART OF THE REICH", "Storm the Reichstag.");
        }

        private void HandleDownfallClicked()
        {
            Debug.Log("Downfall selected");
            DisplayMission("DOWNFALL", "Final assault on Berlin.");
        }

        protected override void Bind()
        {
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

            _elements.Thumbnail.image = null;
            DisplayMission("SELECT A MISSION", "Choose a mission from the list.");
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
