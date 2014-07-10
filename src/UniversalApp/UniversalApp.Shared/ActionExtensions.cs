using System.Threading.Tasks;
#if SILVERLIGHT
using System.Windows.Threading;
#else
using Dispatcher = Windows.UI.Core.CoreDispatcher;
#endif
using Windows.UI.Core;

namespace System
{
    public static class ActionExtensions
    {
        public static void Dispatch(this Action action, Dispatcher dispatcher)
        {
            if (dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                dispatcher.BeginInvoke(() => action());
            }
        }

        public static void Dispatch<T>(this Action<T> action, T arg, Dispatcher dispatcher)
        {
            if (dispatcher.CheckAccess())
            {
                action(arg);
            }
            else
            {
                dispatcher.BeginInvoke(() => action(arg));
            }
        }

        public static void Dispatch<T1, T2>(this Action<T1, T2> action, T1 arg1, T2 arg2, Dispatcher dispatcher)
        {
            if (dispatcher.CheckAccess())
            {
                action(arg1, arg2);
            }
            else
            {
                dispatcher.BeginInvoke(() => action(arg1, arg2));
            }
        }

        public static void Dispatch<T1, T2, T3>(this Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3, Dispatcher dispatcher)
        {
            if (dispatcher.CheckAccess())
            {
                action(arg1, arg2, arg3);
            }
            else
            {
                dispatcher.BeginInvoke(() => action(arg1, arg2, arg3));
            }
        }
    }
}
