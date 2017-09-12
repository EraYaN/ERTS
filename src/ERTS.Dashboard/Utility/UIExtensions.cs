using System;
using System.Windows;
using System.Windows.Threading;

namespace ERTS.Dashboard.Utility
{
    public static class UIExtensions
    {
        private static Action EmptyDelegate = delegate () { };
        /// <summary>
        /// Refreshes/Redraws a UIElement
        /// </summary>
        /// <param name="uiElement">The element to be refreshed/redrawed</param>
        public static void Refresh(this UIElement uiElement)
        {
            uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }
    }
}
