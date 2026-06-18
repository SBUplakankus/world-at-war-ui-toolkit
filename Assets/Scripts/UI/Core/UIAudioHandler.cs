using Data;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Core
{
    [RequireComponent(typeof(UIDocument), typeof(AudioSource))]
    public class UIAudioHandler : MonoBehaviour
    {
        private AudioSource _source;
        private VisualElement _root;

        private void Awake()
        {
            _source = GetComponent<AudioSource>();
            _source.playOnAwake = false;
            _root = GetComponent<UIDocument>().rootVisualElement;
        }

        private void OnEnable()
        {
            _root.RegisterCallback<PointerEnterEvent>(OnPointerEnter, TrickleDown.TrickleDown);
            _root.RegisterCallback<ClickEvent>(OnClick);
        }

        private void OnDisable()
        {
            _root.UnregisterCallback<PointerEnterEvent>(OnPointerEnter, TrickleDown.TrickleDown);
            _root.UnregisterCallback<ClickEvent>(OnClick);
        }

        private void OnPointerEnter(PointerEnterEvent evt)
        {
            if (evt.target is not VisualElement el) return;
            if (!el.ClassListContains("audio--hover")) return;
            if (UIResources.AudioClips.TryGetValue(UI.Enums.Audio.Hover, out var clip))
                _source.PlayOneShot(clip);
        }

        private void OnClick(ClickEvent evt)
        {
            if (evt.target is not VisualElement el) return;
            if (!el.ClassListContains("audio--click")) return;
            if (UIResources.AudioClips.TryGetValue(UI.Enums.Audio.Click, out var clip))
                _source.PlayOneShot(clip);
        }
    }
}
