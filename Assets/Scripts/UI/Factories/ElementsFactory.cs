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
        private static Button QButton(VisualElement root, string name) => root.Q<TemplateContainer>(name)?.Q<Button>();
        private static Slider QSlider(VisualElement root, string name) => root.Q<TemplateContainer>(name)?.Q<Slider>();

        public static ScreenLayoutElements ScreenLayout(VisualElement root)
        {
            return new ScreenLayoutElements(
                Header: root.Q<Label>(Elements.MenuName),
                Username: root.Q<Label>(Elements.UsernameLabel),
                GameVersion: root.Q<Label>(Elements.VersionLabel),
                BackButton: root.Q<Button>(Elements.BackBtn),
                ScreenViewContainer: root.Q<VisualElement>(Elements.ScreenViewContainer)
            );
        }

        public static ModalLayoutElements ModalLayout(VisualElement root)
        {
            return new ModalLayoutElements(
                Header: root.Q<Label>(Elements.ModalHeader),
                ModalViewContainer: root.Q<VisualElement>(Elements.ModalViewContainer),
                ModalContainer: root.Q<VisualElement>(Elements.ModalContainer)
            );
        }

        public static MainMenuElements MainMenu(VisualElement root)
        {
            return new MainMenuElements(
                SoloButton: QButton(root, Elements.SoloBtn),
                CoOpButton: QButton(root, Elements.CoopBtn),
                ZombiesButton: QButton(root, Elements.ZombiesBtn),
                MultiplayerButton: QButton(root, Elements.MultiBtn),
                OptionsButton: QButton(root, Elements.OptionsBtn),
                CreditsButton: QButton(root, Elements.CreditsBtn),
                MessageLabel: root.Q<Label>(Elements.MotdContent)
            );
        }

        public static NoticeModalElements NoticeModal(VisualElement root)
        {
            return new NoticeModalElements(
                Notice: root.Q<Label>(Elements.Notice),
                Ok: QButton(root, Elements.OkBtn)
            );
        }

        public static SoloScreenElements SoloScreen(VisualElement root)
        {
            return new SoloScreenElements(
                ResumeButton: QButton(root, Elements.ResumeBtn),
                NewGameButton: QButton(root, Elements.NewGameBtn),
                MissionSelectButton: QButton(root, Elements.MissionSelectBtn)
            );
        }

        public static CoOpScreenElements CoOpScreen(VisualElement root)
        {
            return new CoOpScreenElements(
                OnlineButton: QButton(root, Elements.OnlineBtn),
                SplitScreenButton: QButton(root, Elements.SplitScreenBtn),
                LanButton: QButton(root, Elements.LanBtn)
            );
        }

        public static MultiplayerMenuElements MultiplayerMenu(VisualElement root)
        {
            return new MultiplayerMenuElements(
                OnlineButton: QButton(root, Elements.OnlineBtn),
                SplitScreenButton: QButton(root, Elements.SplitScreenBtn),
                LanPartyButton: QButton(root, Elements.LanPartyBtn),
                OptionsButton: QButton(root, Elements.OptionsBtn),
                MessageOfTheDay: root.Q<Label>(Elements.MotdContent)
            );
        }

        public static ZombiesScreenElements ZombiesScreen(VisualElement root)
        {
            return new ZombiesScreenElements(
                ResumeButton: QButton(root, Elements.ResumeBtn),
                NewGameButton: QButton(root, Elements.NewGameBtn)
            );
        }

        public static CreditsScreenElements CreditsScreen(VisualElement root)
        {
            return new CreditsScreenElements(
                CreditsLabel: root.Q<Label>(Elements.CreditsLabel)
            );
        }

        public static MissionSelectElements MissionSelect(VisualElement root)
        {
            return new MissionSelectElements(
                Thumbnail: root.Q<Image>(Elements.MissionThumbnail),
                Header: root.Q<Label>(Elements.MissionHdr),
                Description: root.Q<Label>(Elements.MissionDescription),
                SemperFi: QButton(root, Elements.MissionSemperFiBtn),
                LittleResistance: QButton(root, Elements.MissionLittleResistanceBtn),
                HardLanding: QButton(root, Elements.MissionHardLandingBtn),
                Vendetta: QButton(root, Elements.MissionVendettaBtn),
                TheirLandTheirBlood: QButton(root, Elements.MissionTheirLandTheirBloodBtn),
                BurnEmOut: QButton(root, Elements.MissionBurnEmOutBtn),
                Relentless: QButton(root, Elements.MissionRelentlessBtn),
                BloodAndIron: QButton(root, Elements.MissionBloodAndIronBtn),
                RingOfSteel: QButton(root, Elements.MissionRingOfSteelBtn),
                Eviction: QButton(root, Elements.MissionEvictionBtn),
                BlackCats: QButton(root, Elements.MissionBlackCatsBtn),
                BlowtorchAndCorkscrew: QButton(root, Elements.MissionBlowtorchAndCorkscrewBtn),
                BreakingPoint: QButton(root, Elements.MissionBreakingPointBtn),
                HeartOfTheReich: QButton(root, Elements.MissionHeartOfTheReichBtn),
                Downfall: QButton(root, Elements.MissionDownfallBtn)
            );
        }

        public static GameOptionsElements GameOptions(VisualElement root)
        {
            return new GameOptionsElements(
                LookInversionButton: QButton(root, Elements.LookInversionBtn),
                LookInversionLabel: root.Q<Label>(Elements.LookInversionLbl),
                StickLayoutButton: QButton(root, Elements.StickLayoutBtn),
                StickLayoutLabel: root.Q<Label>(Elements.StickLayoutLbl),
                ButtonLayoutButton: QButton(root, Elements.ButtonLayoutBtn),
                ButtonLayoutLabel: root.Q<Label>(Elements.ButtonLayoutLbl),
                SensitivityButton: QButton(root, Elements.SensitivityBtn),
                SensitivityLabel: root.Q<Label>(Elements.SensitivityLbl),
                TargetAssistButton: QButton(root, Elements.TargetAssistBtn),
                TargetAssistLabel: root.Q<Label>(Elements.TargetAssistLbl),
                PlayerNameButton: QButton(root, Elements.PlayerNameBtn),
                PlayerNameLabel: root.Q<Label>(Elements.PlayerNameLbl),
                GameVolumeButton: QButton(root, Elements.GameVolumeBtn)
            );
        }

        public static GameVolumeElements GameVolume(VisualElement root)
        {
            return new GameVolumeElements(
                VoiceButton: QButton(root, Elements.VoiceBtn),
                VoiceSlider: QSlider(root, Elements.VoiceSlider),
                MusicButton: QButton(root, Elements.MusicBtn),
                MusicSlider: QSlider(root, Elements.MusicSlider),
                SfxButton: QButton(root, Elements.SfxBtn),
                SfxSlider: QSlider(root, Elements.SfxSlider),
                CinematicsButton: QButton(root, Elements.CinematicsBtn),
                CinematicsSlider: QSlider(root, Elements.CinematicsSlider),
                MasterButton: QButton(root, Elements.MasterBtn),
                MasterSlider: QSlider(root, Elements.MasterSlider),
                VoipButton: QButton(root, Elements.VoipBtn),
                VoipSlider: QSlider(root, Elements.VoipSlider)
            );
        }

        public static NewGameModalElements NewGameModal(VisualElement root)
        {
            return new NewGameModalElements(
                Yes: QButton(root, Elements.YesBtn),
                No: QButton(root, Elements.NoBtn)
            );
        }

        public static ResumeGameModalElements ResumeGameModal(VisualElement root)
        {
            return new ResumeGameModalElements(
                Yes: QButton(root, Elements.YesBtn),
                No: QButton(root, Elements.NoBtn)
            );
        }

        public static DifficultyModalElements DifficultyModal(VisualElement root)
        {
            return new DifficultyModalElements(
                Recruit: QButton(root, Elements.RecruitBtn),
                Regular: QButton(root, Elements.RegularBtn),
                Hardened: QButton(root, Elements.HardenedBtn),
                Veteran: QButton(root, Elements.VeteranBtn),
                Icon: root.Q<Image>(Elements.DifficultyIcon),
                Description: root.Q<Label>(Elements.DifficultyDescription)
            );
        }

        public static NoConnectionModalElements NoConnectionModal(VisualElement root)
        {
            return new NoConnectionModalElements(
                Notice: root.Q<Label>(Elements.Notice),
                Icon: root.Q<Image>(Elements.NoConnectionIcon),
                Ok: QButton(root, Elements.OkBtn)
            );
        }
    }
}
