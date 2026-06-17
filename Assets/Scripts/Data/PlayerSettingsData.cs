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

        public PlayerSettingsData()
        {
            invertLook = "Off";
            stickLayout = "Default";
            buttonLayout = "Default";
            sensitivity = "3";
            targetAssist = "On";
            playerName = "Player";

            masterVolume = 0.75f;
            musicVolume = 0.75f;
            sfxVolume = 0.75f;
            voiceVolume = 0.75f;
            cinematicsVolume = 0.75f;
            voipVolume = 0.75f;
        }
    }
}
