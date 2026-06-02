using System;
using System.Collections.Generic;
using UI.Enums;
using UnityEngine;
using UnityEngine.UIElements;
using Screen = UI.Enums.Screen;

namespace UI.Services
{
    public class UILibrary : ScriptableObject
    {
        [Serializable]
        private struct ScreenEntry
        {
            public Screen screen;
            public VisualTreeAsset template;
        }

        [Serializable]
        private struct ModalEntry
        {
            public Modal modal;
            public VisualTreeAsset template;
        }
        
        [Header("Libraries")]
        [SerializeField] private List<ScreenEntry> screens;
        [SerializeField] private List<ModalEntry>  modals;
        
        public Dictionary<Screen, VisualTreeAsset> ScreenMap()
        { 
            var screenMap = new Dictionary<Screen, VisualTreeAsset>();
            foreach (var entry in screens)
            {
                if (!screenMap.ContainsKey(entry.screen))
                    screenMap.Add(entry.screen, entry.template);
            }
            return screenMap;
        }

        public Dictionary<Modal, VisualTreeAsset> ModalMap()
        {
            var modalMap = new Dictionary<Modal, VisualTreeAsset>();
            foreach (var entry in modals)
            {
                if (!modalMap.ContainsKey(entry.modal))
                    modalMap.Add(entry.modal, entry.template);
            }
            return modalMap;
        }
    }
}