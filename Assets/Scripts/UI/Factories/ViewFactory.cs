using UI.Views;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Factories
{
    /// <summary>
    /// Generic Factory to Create the UXML Files attached to the View Classes.
    /// Requires UXML files to have the exact same name as the C# class.
    /// </summary>
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
