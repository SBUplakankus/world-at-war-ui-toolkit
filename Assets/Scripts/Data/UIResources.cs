using System.Collections.Generic;
using UI.Enums;
using UnityEngine;
using UnityEngine.UIElements;

namespace Data
{
    /// <summary>
    /// Contains the Resources used in the UI. Parses text from JSON and loads the SVG's, Images and Audio
    /// from Resources into read-only dictionaries.
    /// </summary>
    public static class UIResources
    {
        private static ContentData _content;
        private static bool _loaded;

        private static void EnsureLoaded()
        {
            if (_loaded) return;
            _loaded = true;

            var asset = Resources.Load<TextAsset>("Data/content");
            if (!asset)
            {
                Debug.LogError("UIResources: missing Resources/Data/content.json");
                return;
            }
            _content = JsonUtility.FromJson<ContentData>(asset.text);
            Resources.UnloadAsset(asset);

            if (_content == null)
                Debug.LogError("UIResources: Resources/Data/content.json failed to deserialize");
        }

        private static Dictionary<Difficulty, VectorImage> _difficultyIcons;
        public static IReadOnlyDictionary<Difficulty, VectorImage> DifficultyIcons
        {
            get
            {
                if (_difficultyIcons != null) return _difficultyIcons;
                _difficultyIcons = new Dictionary<Difficulty, VectorImage>
                {
                    [Difficulty.Recruit] = Resources.Load<VectorImage>("Icons/Recruit"),
                    [Difficulty.Regular] = Resources.Load<VectorImage>("Icons/Regular"),
                    [Difficulty.Hardened] = Resources.Load<VectorImage>("Icons/Hardened"),
                    [Difficulty.Veteran] = Resources.Load<VectorImage>("Icons/Veteran")
                };
                foreach (var entry in _difficultyIcons)
                    if (!entry.Value)
                        Debug.LogWarning($"UIResources: missing icon for {entry.Key}");
                return _difficultyIcons;
            }
        }

        private static Dictionary<Missions, Texture2D> _missionThumbnails;
        public static IReadOnlyDictionary<Missions, Texture2D> MissionThumbnails
        {
            get
            {
                if (_missionThumbnails != null) return _missionThumbnails;
                _missionThumbnails = new Dictionary<Missions, Texture2D>
                {
                    [Missions.SemperFi] = Resources.Load<Texture2D>("Thumbnails/SemperFi"),
                    [Missions.LittleResistance] = Resources.Load<Texture2D>("Thumbnails/LittleResistance"),
                    [Missions.HardLanding] = Resources.Load<Texture2D>("Thumbnails/HardLanding"),
                    [Missions.Vendetta] = Resources.Load<Texture2D>("Thumbnails/Vendetta"),
                    [Missions.TheirLandTheirBlood] = Resources.Load<Texture2D>("Thumbnails/TLTB"),
                    [Missions.BurnEmOut] = Resources.Load<Texture2D>("Thumbnails/BurnEmOut"),
                    [Missions.Relentless] = Resources.Load<Texture2D>("Thumbnails/Relentless"),
                    [Missions.BloodAndIron] = Resources.Load<Texture2D>("Thumbnails/BloodAndIron"),
                    [Missions.RingOfSteel] = Resources.Load<Texture2D>("Thumbnails/RingOfSteel"),
                    [Missions.Eviction] = Resources.Load<Texture2D>("Thumbnails/Eviction"),
                    [Missions.BlackCats] = Resources.Load<Texture2D>("Thumbnails/BlackCats"),
                    [Missions.BlowtorchAndCorkscrew] = Resources.Load<Texture2D>("Thumbnails/BlowAndCork"),
                    [Missions.BreakingPoint] = Resources.Load<Texture2D>("Thumbnails/BreakingPoint"),
                    [Missions.HeartOfTheReich] = Resources.Load<Texture2D>("Thumbnails/HOTR"),
                    [Missions.Downfall] = Resources.Load<Texture2D>("Thumbnails/Downfall")
                };
                foreach (var entry in _missionThumbnails)
                    if (!entry.Value)
                        Debug.LogWarning($"UIResources: missing thumbnail for {entry.Key}");
                return _missionThumbnails;
            }
        }

        private static Dictionary<Audio, AudioClip> _audioClips;
        public static IReadOnlyDictionary<Audio, AudioClip> AudioClips
        {
            get
            {
                if (_audioClips != null) return _audioClips;
                _audioClips = new Dictionary<Audio, AudioClip>
                {
                    [Audio.Hover] = Resources.Load<AudioClip>("Audio/Hover"),
                    [Audio.Click] = Resources.Load<AudioClip>("Audio/Click")
                };
                foreach (var entry in _audioClips)
                    if (!entry.Value)
                        Debug.LogWarning($"UIResources: missing audio clip for {entry.Key}");
                return _audioClips;
            }
        }

        private static Dictionary<Missions, string> _titles;
        public static IReadOnlyDictionary<Missions, string> MissionTitles
        {
            get
            {
                EnsureLoaded();
                if (_titles != null) return _titles;
                _titles = new Dictionary<Missions, string>();

                if (_content?.missions == null) return _titles;
                foreach (var m in _content.missions)
                    if (System.Enum.TryParse<Missions>(m.name, out var key))
                        _titles[key] = m.title;
                    else
                        Debug.LogWarning($"UIResources: unknown mission name \"{m.name}\" in content.json");

                return _titles;
            }
        }

        private static Dictionary<Missions, string> _descriptions;
        public static IReadOnlyDictionary<Missions, string> MissionDescriptions
        {
            get
            {
                EnsureLoaded();
                if (_descriptions != null) return _descriptions;
                _descriptions = new Dictionary<Missions, string>();

                if (_content?.missions == null) return _descriptions;
                foreach (var m in _content.missions)
                    if (System.Enum.TryParse<Missions>(m.name, out var key))
                        _descriptions[key] = m.description;
                    else
                        Debug.LogWarning($"UIResources: unknown mission name \"{m.name}\" in content.json");

                return _descriptions;
            }
        }

        private static Dictionary<Difficulty, string> _difficultyDescriptions;
        public static IReadOnlyDictionary<Difficulty, string> DifficultyDescriptions
        {
            get
            {
                EnsureLoaded();
                if (_difficultyDescriptions != null) return _difficultyDescriptions;
                _difficultyDescriptions = new Dictionary<Difficulty, string>();

                if (_content?.difficulties == null) return _difficultyDescriptions;
                foreach (var d in _content.difficulties)
                    if (System.Enum.TryParse<Difficulty>(d.name, out var key))
                        _difficultyDescriptions[key] = d.description;
                    else
                        Debug.LogWarning($"UIResources: unknown difficulty name \"{d.name}\" in content.json");

                return _difficultyDescriptions;
            }
        }

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
