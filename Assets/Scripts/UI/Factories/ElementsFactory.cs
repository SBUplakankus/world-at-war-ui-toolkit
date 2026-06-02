using UI.Constants;
using UI.Records;
using UnityEngine.UIElements;

namespace UI.Factories
{
    public static class ElementsFactory
    {
        public static MenuShellElements MenuShell(VisualElement root)
        {
            return new MenuShellElements(
                MenuName: root.Q<Label>(UIElements.MenuName),
                Username: root.Q<Label>(UIElements.UsernameLabel),
                GameVersion: root.Q<Label>(UIElements.VersionLabel),
                BackButton: root.Q<Button>(UIElements.BackBtn)
            );
        }

        public static MainMenuElements MainMenu(VisualElement root)
        {
            return new MainMenuElements(
                SoloButton: root.Q<Button>(UIElements.SoloBtn),
                CoOpButton: root.Q<Button>(UIElements.CoopBtn),
                ZombiesButton: root.Q<Button>(UIElements.ZombiesBtn),
                MultiplayerButton: root.Q<Button>(UIElements.MultiBtn),
                OptionsButton: root.Q<Button>(UIElements.OptionsBtn),
                CreditsButton: root.Q<Button>(UIElements.CreditsBtn),
                MessageLabel: root.Q<Label>(UIElements.MotdContent)
            );
        }
    }
}