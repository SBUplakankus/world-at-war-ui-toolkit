using System.Collections.Generic;
using UI.Views;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Core
{
    /// <summary>
    /// Contains the UI Layer Elements with a Container for injecting views, a history, and the currently
    /// displayed view.
    /// </summary>
    public class UILayer
    {
        private readonly VisualElement _container;
        private readonly Stack<BaseView> _history = new();

        public bool IsEmpty => _history.Count == 0;

        public BaseView Current { get; private set; }

        public UILayer(VisualElement container)
        {
            _container = container;
        }

        public void Push(BaseView view)
        {
            if (Current != null)
            {
                Current.Deactivate();
                _history.Push(Current);
            }

            Current = view;
            _container.Clear();
            _container.Add(view.Root);
            view.Activate();
        }

        public void Pop()
        {
            if (_history.Count == 0) return;

            Current.Deactivate();
            Current = _history.Pop();

            _container.Clear();
            _container.Add(Current.Root);
            
            Current.Activate();
        }

        public void Clear()
        {
            Current?.Dispose();
            Current = null;
            _history.Clear();
            _container.Clear();
        }
    }
}