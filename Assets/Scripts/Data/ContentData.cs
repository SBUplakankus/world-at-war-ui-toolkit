namespace Data
{
    [System.Serializable]
    public class ContentData
    {
        public string[] messagesOfTheDay;
        public MissionContent[] missions;
        public DifficultyContent[] difficulties;
        public OptionData options;
    }

    [System.Serializable]
    public class MissionContent
    {
        public string name;
        public string title;
        public string description;
    }

    [System.Serializable]
    public class DifficultyContent
    {
        public string name;
        public string description;
    }

    [System.Serializable]
    public class OptionData
    {
        public string[] inversion;
        public string[] stickLayout;
        public string[] buttonLayout;
        public string[] sensitivity;
        public string[] targetAssist;
    }
}
