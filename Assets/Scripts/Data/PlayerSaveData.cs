namespace Data
{
    [System.Serializable]
    public class PlayerSaveData
    {
        public string username;
        public bool campaignStarted;
        public int missionsCompleted;
        public PlayerSettingsData settings = new PlayerSettingsData();
    }
}