using System.Collections.Generic;
using UI.Enums;
using UnityEngine;
using UnityEngine.UIElements;

namespace Data
{
    public static class UIResources
    {
        private static ContentData _content;
        private static bool _loaded;

        private static void EnsureLoaded()
        {
            if (_loaded) return;
            _loaded = true;

            var asset = Resources.Load<TextAsset>("Data/content");
            if (asset != null)
            {
                _content = JsonUtility.FromJson<ContentData>(asset.text);
                Resources.UnloadAsset(asset);
            }
        }

        public static readonly Dictionary<Difficulty, VectorImage> DifficultyIcons = new()
        {
            [Difficulty.Recruit] = Resources.Load<VectorImage>("Icons/Recruit"),
            [Difficulty.Regular] = Resources.Load<VectorImage>("Icons/Regular"),
            [Difficulty.Hardened] = Resources.Load<VectorImage>("Icons/Hardened"),
            [Difficulty.Veteran] = Resources.Load<VectorImage>("Icons/Veteran")
        };

        public static Dictionary<Missions, string> MissionTitles
        {
            get
            {
                EnsureLoaded();
                if (_titles == null)
                {
                    _titles = new Dictionary<Missions, string>();
                    if (_content?.missions != null)
                        foreach (var m in _content.missions)
                            if (System.Enum.TryParse<Missions>(m.name, out var key))
                                _titles[key] = m.title;
                }
                return _titles;
            }
        }
        private static Dictionary<Missions, string> _titles;

        public static Dictionary<Missions, string> MissionDescriptions
        {
            get
            {
                EnsureLoaded();
                if (_descriptions == null)
                {
                    _descriptions = new Dictionary<Missions, string>();
                    if (_content?.missions != null)
                        foreach (var m in _content.missions)
                            if (System.Enum.TryParse<Missions>(m.name, out var key))
                                _descriptions[key] = m.description;
                }
                return _descriptions;
            }
        }
        private static Dictionary<Missions, string> _descriptions;

        public static Dictionary<Difficulty, string> DifficultyDescriptions
        {
            get
            {
                EnsureLoaded();
                if (_difficultyDescriptions == null)
                {
                    _difficultyDescriptions = new Dictionary<Difficulty, string>();
                    if (_content?.difficulties != null)
                        foreach (var d in _content.difficulties)
                            if (System.Enum.TryParse<Difficulty>(d.name, out var key))
                                _difficultyDescriptions[key] = d.description;
                }
                return _difficultyDescriptions;
            }
        }
        private static Dictionary<Difficulty, string> _difficultyDescriptions;

        public static string[] MessagesOfTheDay
        {
            get
            {
                EnsureLoaded();
                return _content?.messagesOfTheDay ?? System.Array.Empty<string>();
            }
        }

        public static string[] OptionInversion
        {
            get
            {
                EnsureLoaded();
                return _content?.options?.inversion ?? System.Array.Empty<string>();
            }
        }

        public static string[] OptionStickLayout
        {
            get
            {
                EnsureLoaded();
                return _content?.options?.stickLayout ?? System.Array.Empty<string>();
            }
        }

        public static string[] OptionButtonLayout
        {
            get
            {
                EnsureLoaded();
                return _content?.options?.buttonLayout ?? System.Array.Empty<string>();
            }
        }

        public static string[] OptionSensitivity
        {
            get
            {
                EnsureLoaded();
                return _content?.options?.sensitivity ?? System.Array.Empty<string>();
            }
        }

        public static string[] OptionTargetAssist
        {
            get
            {
                EnsureLoaded();
                return _content?.options?.targetAssist ?? System.Array.Empty<string>();
            }
        }
    }
}
