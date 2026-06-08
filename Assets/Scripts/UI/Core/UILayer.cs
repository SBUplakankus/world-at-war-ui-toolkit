using System.Collections.Generic;
using UI.Views;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Core
{
    public class UILayer
    {
        private readonly VisualElement _container;
        private readonly Stack<BaseView> _history = new();
        private BaseView _current;
        
        public bool IsEmpty => _history.Count == 0;

        public UILayer(VisualElement container)
        {
            _container = container;
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
            
            _current.Activate();
        }

        public void Clear()
        {
            _current?.Dispose();
            _current = null;
            _history.Clear();
            _container.Clear();
        }
    }
}