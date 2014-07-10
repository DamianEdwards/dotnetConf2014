using System.Threading.Tasks;
using Windows.UI.Core;

namespace System
{
    public static class ActionExtensions
    {
        public static async Task Dispatch(this Action action, CoreDispatcher dispatcher)
        {
            if (dispatcher.HasThreadAccess)
            {
                action();
            }
            else
            {
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action());
            }
        }

        public static async Task Dispatch<T>(this Action<T> action, T arg, CoreDispatcher dispatcher)
        {
            if (dispatcher.HasThreadAccess)
            {
                action(arg);
            }
            else
            {
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action(arg));
            }
        }

        public static async Task Dispatch<T1, T2>(this Action<T1, T2> action, T1 arg1, T2 arg2, CoreDispatcher dispatcher)
        {
            if (dispatcher.HasThreadAccess)
            {
                action(arg1, arg2);
            }
            else
            {
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action(arg1, arg2));
            }
        }

        public static async Task Dispatch<T1, T2, T3>(this Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3, CoreDispatcher dispatcher)
        {
            if (dispatcher.HasThreadAccess)
            {
                action(arg1, arg2, arg3);
            }
            else
            {
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action(arg1, arg2, arg3));
            }
        }
    }
}
