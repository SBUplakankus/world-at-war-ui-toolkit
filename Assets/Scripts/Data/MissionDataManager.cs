using System.Linq;
using UnityEngine;

namespace Data
{
    public static class MissionDataManager
    {
        private static MissionEntry[] _missions;

        private static void EnsureLoaded()
        {
            if (_missions != null) return;
            var asset = Resources.Load<TextAsset>("missions");
            _missions = JsonUtility.FromJson<Wrapper>(
                "{\"items\":" + asset.text + "}").items;
        }

        public static MissionEntry[] AllMissions
        {
            get
            {
                EnsureLoaded();
                return _missions;
            }
        }

        public static MissionEntry GetById(string id)
        {
            EnsureLoaded();
            return _missions.FirstOrDefault(m => m.id == id);
        }

        [System.Serializable]
        private class Wrapper
        {
            public MissionEntry[] items;
        }
    }
}
