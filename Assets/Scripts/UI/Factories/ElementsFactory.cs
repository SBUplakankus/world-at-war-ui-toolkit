using UI.Constants;
using UI.Records;
using UnityEngine.UIElements;

namespace UI.Factories
{
    /// <summary>
    /// Queries the Given Visual Element for the View Elements
    /// </summary>
    public static class ElementsFactory
    {
        public static ScreenLayoutElements ScreenLayout(VisualElement root)
        {
            return new ScreenLayoutElements(
                Header: root.Q<Label>(UIElements.MenuName),
                Username: root.Q<Label>(UIElements.UsernameLabel),
                GameVersion: root.Q<Label>(UIElements.VersionLabel),
                BackButton: root.Q<Button>(UIElements.BackBtn),
                ScreenViewContainer: root.Q<VisualElement>(UIElements.ScreenViewContainer)
            );
        }

        public static ModalLayoutElements ModalLayout(VisualElement root)
        {
            return new ModalLayoutElements(
                Header: root.Q<Label>(UIElements.ModalHeader),
                ModalViewContainer: root.Q<VisualElement>(UIElements.ModalViewContainer),
                ModalContainer: root.Q<VisualElement>(UIElements.ModalContainer)
            );
        }

        public static MainMenuElements MainMenu(VisualElement root)
        {
            return new MainMenuElements(
                SoloButton: root.Q<TemplateContainer>(UIElements.SoloBtn).Q<Button>(),
                CoOpButton: root.Q<TemplateContainer>(UIElements.CoopBtn).Q<Button>(),
                ZombiesButton: root.Q<TemplateContainer>(UIElements.ZombiesBtn).Q<Button>(),
                MultiplayerButton: root.Q<TemplateContainer>(UIElements.MultiBtn).Q<Button>(),
                OptionsButton: root.Q<TemplateContainer>(UIElements.OptionsBtn).Q<Button>(),
                CreditsButton: root.Q<TemplateContainer>(UIElements.CreditsBtn).Q<Button>(),
                MessageLabel: root.Q<Label>(UIElements.MotdContent)
            );
        }

        public static NoticeModalElements NoticeModal(VisualElement root)
        {
            return new NoticeModalElements(
                Notice: root.Q<Label>(UIElements.Notice),
                Ok: root.Q<TemplateContainer>(UIElements.OkBtn).Q<Button>()
            );
        }

        public static SoloScreenElements SoloScreen(VisualElement root)
        {
            return new SoloScreenElements(
                ResumeButton: root.Q<TemplateContainer>(UIElements.ResumeBtn).Q<Button>(),
                NewGameButton: root.Q<TemplateContainer>(UIElements.NewGameBtn).Q<Button>(),
                MissionSelectButton: root.Q<TemplateContainer>(UIElements.MissionSelectBtn).Q<Button>()
            );
        }

        public static CoOpScreenElements CoOpScreen(VisualElement root)
        {
            return new CoOpScreenElements(
                OnlineButton: root.Q<TemplateContainer>(UIElements.OnlineBtn).Q<Button>(),
                SplitScreenButton: root.Q<TemplateContainer>(UIElements.SplitScreenBtn).Q<Button>(),
                LanButton: root.Q<TemplateContainer>(UIElements.LanBtn).Q<Button>()
            );
        }

        public static MultiplayerMenuElements MultiplayerMenu(VisualElement root)
        {
            return new MultiplayerMenuElements(
                OnlineButton: root.Q<TemplateContainer>(UIElements.OnlineBtn).Q<Button>(),
                SplitScreenButton: root.Q<TemplateContainer>(UIElements.SplitScreenBtn).Q<Button>(),
                LanPartyButton: root.Q<TemplateContainer>(UIElements.LanPartyBtn).Q<Button>(),
                OptionsButton: root.Q<TemplateContainer>(UIElements.OptionsBtn).Q<Button>(),
                MessageOfTheDay: root.Q<Label>(UIElements.MotdContent)
            );
        }

        public static ZombiesScreenElements ZombiesScreen(VisualElement root)
        {
            return new ZombiesScreenElements(
                ResumeButton: root.Q<TemplateContainer>(UIElements.ResumeBtn).Q<Button>(),
                NewGameButton: root.Q<TemplateContainer>(UIElements.NewGameBtn).Q<Button>()
            );
        }

        public static CreditsScreenElements CreditsScreen(VisualElement root)
        {
            return new CreditsScreenElements(
                CreditsLabel: root.Q<Label>(UIElements.CreditsLabel)
            );
        }

        public static MissionSelectElements MissionSelect(VisualElement root)
        {
            return new MissionSelectElements(
                Thumbnail: root.Q<Image>(UIElements.MissionThumbnail),
                Header: root.Q<Label>(UIElements.MissionHdr),
                Description: root.Q<Label>(UIElements.MissionDescription),
                SemperFi: root.Q<TemplateContainer>(UIElements.MissionSemperFiBtn).Q<Button>(),
                LittleResistance: root.Q<TemplateContainer>(UIElements.MissionLittleResistanceBtn).Q<Button>(),
                HardLanding: root.Q<TemplateContainer>(UIElements.MissionHardLandingBtn).Q<Button>(),
                Vendetta: root.Q<TemplateContainer>(UIElements.MissionVendettaBtn).Q<Button>(),
                TheirLandTheirBlood: root.Q<TemplateContainer>(UIElements.MissionTheirLandTheirBloodBtn).Q<Button>(),
                BurnEmOut: root.Q<TemplateContainer>(UIElements.MissionBurnEmOutBtn).Q<Button>(),
                Relentless: root.Q<TemplateContainer>(UIElements.MissionRelentlessBtn).Q<Button>(),
                BloodAndIron: root.Q<TemplateContainer>(UIElements.MissionBloodAndIronBtn).Q<Button>(),
                RingOfSteel: root.Q<TemplateContainer>(UIElements.MissionRingOfSteelBtn).Q<Button>(),
                Eviction: root.Q<TemplateContainer>(UIElements.MissionEvictionBtn).Q<Button>(),
                BlackCats: root.Q<TemplateContainer>(UIElements.MissionBlackCatsBtn).Q<Button>(),
                BlowtorchAndCorkscrew: root.Q<TemplateContainer>(UIElements.MissionBlowtorchAndCorkscrewBtn).Q<Button>(),
                BreakingPoint: root.Q<TemplateContainer>(UIElements.MissionBreakingPointBtn).Q<Button>(),
                HeartOfTheReich: root.Q<TemplateContainer>(UIElements.MissionHeartOfTheReichBtn).Q<Button>(),
                Downfall: root.Q<TemplateContainer>(UIElements.MissionDownfallBtn).Q<Button>()
            );
        }

        public static GameOptionsElements GameOptions(VisualElement root)
        {
            return new GameOptionsElements(
                LookInversionButton: root.Q<TemplateContainer>(UIElements.LookInversionBtn).Q<Button>(),
                LookInversionLabel: root.Q<Label>(UIElements.LookInversionLbl),
                StickLayoutButton: root.Q<TemplateContainer>(UIElements.StickLayoutBtn).Q<Button>(),
                StickLayoutLabel: root.Q<Label>(UIElements.StickLayoutLbl),
                ButtonLayoutButton: root.Q<TemplateContainer>(UIElements.ButtonLayoutBtn).Q<Button>(),
                ButtonLayoutLabel: root.Q<Label>(UIElements.ButtonLayoutLbl),
                SensitivityButton: root.Q<TemplateContainer>(UIElements.SensitivityBtn).Q<Button>(),
                SensitivityLabel: root.Q<Label>(UIElements.SensitivityLbl),
                TargetAssistButton: root.Q<TemplateContainer>(UIElements.TargetAssistBtn).Q<Button>(),
                TargetAssistLabel: root.Q<Label>(UIElements.TargetAssistLbl),
                PlayerNameButton: root.Q<TemplateContainer>(UIElements.PlayerNameBtn).Q<Button>(),
                PlayerNameLabel: root.Q<Label>(UIElements.PlayerNameLbl),
                GameVolumeButton: root.Q<TemplateContainer>(UIElements.GameVolumeBtn).Q<Button>()
            );
        }

        public static GameVolumeElements GameVolume(VisualElement root)
        {
            return new GameVolumeElements(
                VoiceButton: root.Q<TemplateContainer>(UIElements.VoiceBtn).Q<Button>(),
                VoiceSlider: root.Q<TemplateContainer>(UIElements.VoiceSlider).Q<Slider>(),
                MusicButton: root.Q<TemplateContainer>(UIElements.MusicBtn).Q<Button>(),
                MusicSlider: root.Q<TemplateContainer>(UIElements.MusicSlider).Q<Slider>(),
                SfxButton: root.Q<TemplateContainer>(UIElements.SfxBtn).Q<Button>(),
                SfxSlider: root.Q<TemplateContainer>(UIElements.SfxSlider).Q<Slider>(),
                CinematicsButton: root.Q<TemplateContainer>(UIElements.CinematicsBtn).Q<Button>(),
                CinematicsSlider: root.Q<TemplateContainer>(UIElements.CinematicsSlider).Q<Slider>(),
                MasterButton: root.Q<TemplateContainer>(UIElements.MasterBtn).Q<Button>(),
                MasterSlider: root.Q<TemplateContainer>(UIElements.MasterSlider).Q<Slider>(),
                VoipButton: root.Q<TemplateContainer>(UIElements.VoipBtn).Q<Button>(),
                VoipSlider: root.Q<TemplateContainer>(UIElements.VoipSlider).Q<Slider>()
            );
        }

        public static NewGameModalElements NewGameModal(VisualElement root)
        {
            return new NewGameModalElements(
                Yes: root.Q<TemplateContainer>(UIElements.YesBtn).Q<Button>(),
                No: root.Q<TemplateContainer>(UIElements.NoBtn).Q<Button>()
            );
        }

        public static ResumeGameModalElements ResumeGameModal(VisualElement root)
        {
            return new ResumeGameModalElements(
                Yes: root.Q<TemplateContainer>(UIElements.YesBtn).Q<Button>(),
                No: root.Q<TemplateContainer>(UIElements.NoBtn).Q<Button>()
            );
        }

        public static DifficultyModalElements DifficultyModal(VisualElement root)
        {
            return new DifficultyModalElements(
                Recruit: root.Q<TemplateContainer>(UIElements.RecruitBtn).Q<Button>(),
                Regular: root.Q<TemplateContainer>(UIElements.RegularBtn).Q<Button>(),
                Hardened: root.Q<TemplateContainer>(UIElements.HardenedBtn).Q<Button>(),
                Veteran: root.Q<TemplateContainer>(UIElements.VeteranBtn).Q<Button>(),
                Icon: root.Q<Image>(UIElements.DifficultyIcon),
                Description: root.Q<Label>(UIElements.DifficultyDescription)
            );
        }
    }
}
