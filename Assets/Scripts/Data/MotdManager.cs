using UnityEngine;

namespace Data
{
    public static class MotdManager
    {
        private static MotdEntry[] _messages;

        private static void EnsureLoaded()
        {
            if (_messages != null) return;
            var asset = Resources.Load<TextAsset>("motd");
            var wrapper = JsonUtility.FromJson<MotdWrapper>(
                "{\"items\":" + asset.text + "}");
            _messages = wrapper.items;
        }

        public static MotdEntry RandomMessage
        {
            get
            {
                EnsureLoaded();
                return _messages[Random.Range(0, _messages.Length)];
            }
        }

        [System.Serializable]
        private class MotdWrapper
        {
            public MotdEntry[] items;
        }
    }
}
