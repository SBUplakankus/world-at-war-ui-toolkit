using System.IO;
using UnityEngine;

namespace Data
{
    public static class SaveDataManager
    {
        private static PlayerSaveData _cachedSave;

        public static PlayerSaveData CurrentSave
        {
            get
            {
                if (_cachedSave != null)
                    return _cachedSave;

                _cachedSave = LoadFromDisk();
                return _cachedSave;
            }
        }

        private static string SavePath => Application.persistentDataPath + "/playersavedata.json";

        public static void Save(PlayerSaveData data)
        {
            _cachedSave = data;
            var json = JsonUtility.ToJson(data, prettyPrint: true);
            File.WriteAllText(SavePath, json);
        }

        public static PlayerSaveData Load()
        {
            _cachedSave = LoadFromDisk();
            return _cachedSave;
        }

        private static PlayerSaveData LoadFromDisk()
        {
            if (!File.Exists(SavePath))
                return new PlayerSaveData();

            var json = File.ReadAllText(SavePath);
            return JsonUtility.FromJson<PlayerSaveData>(json);
        }

        public static bool SaveFileExists => File.Exists(SavePath);

        public static void Delete()
        {
            _cachedSave = null;
            if (File.Exists(SavePath))
                File.Delete(SavePath);
        }
    }
}