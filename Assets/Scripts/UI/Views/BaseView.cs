using System;
using UnityEngine.UIElements;

namespace UI.Views
{
    public abstract class BaseView : IDisposable
    {
        public VisualElement Root { get; private set; }
        private bool _disposed;

        protected BaseView(VisualTreeAsset template)
        {
            Root = template.CloneTree();
        }

        public void Activate()
        {
            GetElements();
            Bind();
        }

        public void Deactivate() => UnBind();

        protected abstract void GetElements();
        protected abstract void Bind();
        protected abstract void UnBind();

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