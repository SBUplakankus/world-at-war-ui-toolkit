using UI.Constants;
using UI.Records;
using UnityEngine.UIElements;

namespace UI.Factories
{
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
    }
}