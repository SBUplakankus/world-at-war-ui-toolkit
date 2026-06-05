using UI.Views;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Services
{
    public static class ViewFactory
    {
        public static TView Create<TView>() where TView : BaseView
        {
            var name = typeof(TView).Name;
            var template = Resources.Load<VisualTreeAsset>($"Views/{name}");

            if (template == null)
            {
                Debug.LogError($"ViewFactory: missing Resources/Views/{name}.uxml");
                return null;
            }

            return (TView)System.Activator.CreateInstance(typeof(TView), new object[] { template });
        }
    }
}
