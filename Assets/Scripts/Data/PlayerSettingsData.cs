namespace Data
{
    [System.Serializable]
    public class PlayerSettingsData
    {
        public string invertLook;
        public string stickLayout;
        public string buttonLayout;
        public string sensitivity;
        public string targetAssist;
        public string playerName;

        public float masterVolume;
        public float musicVolume;
        public float sfxVolume;
        public float voiceVolume;
        public float cinematicsVolume;
        public float voipVolume;

        public PlayerSettingsData() { }
    }
}
