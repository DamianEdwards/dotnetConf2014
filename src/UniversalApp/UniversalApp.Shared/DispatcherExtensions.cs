using System;
using System.Threading.Tasks;

namespace Windows.UI.Core
{
    public static class DispatcherExtensions
    {
        public static bool CheckAccess(this CoreDispatcher dispatcher)
        {
            return dispatcher.HasThreadAccess;
        }

        public static void BeginInvoke(this CoreDispatcher dispatcher, Action action)
        {
            dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action()).AsTask().Forget();
        }
    }
}
