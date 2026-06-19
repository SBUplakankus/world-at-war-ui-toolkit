using System;
using UnityEngine.UIElements;

namespace UI.Views
{
    /// <summary>
    /// Base class for all views that implements IDisposable.
    /// Calls GetElements and Bind on Activation and Unbind on Deactivation.
    /// </summary>
    public abstract class BaseView : IDisposable
    {
        public VisualElement Root { get; private set; }
        private bool _disposed;

        protected BaseView(VisualTreeAsset template)
        {
            Root = template.CloneTree();
            Root.style.flexGrow = 1;
        }

        public void Activate()
        {
            GetElements();
            Bind();
        }

        public void Deactivate() => UnBind();

        protected abstract void GetElements();
        protected virtual void Bind() { }
        protected virtual void UnBind() { }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                UnBind();
                Root.RemoveFromHierarchy();
                Root = null;
            }

            _disposed = true;
        }
    }
}