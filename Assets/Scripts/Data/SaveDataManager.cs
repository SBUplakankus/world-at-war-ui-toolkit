using System.IO;
using UnityEngine;

namespace Data
{
    /// <summary>
    /// Manages the Mock JSON Save Data. Partly vibe coded but does it's job for this prototype.
    /// </summary>
    public static class SaveDataManager
    {
        private static PlayerSaveData _cachedSave;
        private static bool _dirty;
        private static float _lastWriteTime;
        private static bool _quitSubscribed;
        private const float ThrottleMs = 500f;

        private static string SavePath => Application.persistentDataPath + "/playersavedata.json";

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

        public static void Save(PlayerSaveData data)
        {
            _cachedSave = data;
            _dirty = true;

            if (!_quitSubscribed)
            {
                _quitSubscribed = true;
                Application.quitting += Flush;
            }

            if (Time.realtimeSinceStartup * 1000f - _lastWriteTime < ThrottleMs)
                return;

            _lastWriteTime = Time.realtimeSinceStartup * 1000f;
            WriteToDisk(data);
        }

        public static void Flush()
        {
            if (!_dirty || _cachedSave == null) return;
            _dirty = false;
            WriteToDisk(_cachedSave);
        }

        private static void WriteToDisk(PlayerSaveData data)
        {
            try
            {
                var json = JsonUtility.ToJson(data, prettyPrint: true);
                File.WriteAllText(SavePath, json);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"SaveDataManager: failed to write save file — {ex.Message}");
            }
        }

        public static PlayerSaveData Load()
        {
            _cachedSave = LoadFromDisk();
            return _cachedSave;
        }

        private static PlayerSaveData LoadFromDisk()
        {
            if (File.Exists(SavePath))
            {
                var json = File.ReadAllText(SavePath);
                return JsonUtility.FromJson<PlayerSaveData>(json);
            }

            return CreateDefaultSave();
        }

        private static PlayerSaveData CreateDefaultSave()
        {
            var asset = Resources.Load<TextAsset>("Data/defaults");
            if (asset != null)
            {
                var settings = JsonUtility.FromJson<PlayerSettingsData>(asset.text);
                Resources.UnloadAsset(asset);
                return new PlayerSaveData
                {
                    username = "Player",
                    settings = settings
                };
            }

            return new PlayerSaveData();
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
