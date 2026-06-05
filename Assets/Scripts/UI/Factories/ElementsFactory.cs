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
                ModalViewContainer: root.Q<VisualElement>(UIElements.ModalViewContainer)
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
    }
}