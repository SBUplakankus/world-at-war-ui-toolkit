using System.IO;
using UnityEngine;

namespace Data
{
    public static class SaveDataManager
    {
        public static PlayerSaveData CurrentSave => Load();
        
        private static string SavePath => Application.persistentDataPath + "/playersavedata.json";
        
        public static void Save(PlayerSaveData data)
        {
            var json = JsonUtility.ToJson(data, prettyPrint: true);
            File.WriteAllText(SavePath, json);
        }

        public static PlayerSaveData Load()
        {
            if (!File.Exists(SavePath))
                return new PlayerSaveData();

            var json = File.ReadAllText(SavePath);
            return JsonUtility.FromJson<PlayerSaveData>(json);
        }
        
        public static bool SaveFileExists => File.Exists(SavePath);

        public static void Delete()
        {
            if (File.Exists(SavePath))
                File.Delete(SavePath);
        }
    }
}