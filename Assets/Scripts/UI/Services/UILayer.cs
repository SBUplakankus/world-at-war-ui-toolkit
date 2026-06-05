using System.Collections.Generic;
using UI.Views;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Services
{
    public class UILayer
    {
        private readonly VisualElement _container;
        private readonly Stack<BaseView> _history = new();
        private BaseView _current;
        private readonly bool _isModal; 
        
        public bool IsEmpty => _history.Count == 0;

        public UILayer(VisualElement container, bool isModal = false)
        {
            _container = container;
            _isModal = isModal;
        }

        public void Push(BaseView view)
        {
            if (_current != null)
            {
                _current.Deactivate();
                _history.Push(_current);
            }

            _current = view;
            _container.Clear();
            _container.Add(view.Root);
            view.Activate();
        }

        public void Pop()
        {
            if (_history.Count == 0)
            {
                Debug.LogWarning("UILayer: nothing to pop.");
                return;
            }

            _current.Deactivate();
            _current = _history.Pop();

            _container.Clear();
            _container.Add(_current.Root);
            
            if(_isModal)
                _container.pickingMode = PickingMode.Position;
            
            _current.Activate();
        }

        public void Clear()
        {
            _current?.Dispose();
            _current = null;
            _history.Clear();
            _container.Clear();
            
            if(_isModal)
                _container.pickingMode = PickingMode.Ignore;
        }
    }
}