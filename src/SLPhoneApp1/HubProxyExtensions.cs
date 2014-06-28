using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Microsoft.AspNet.SignalR.Client;

namespace Microsoft.AspNet.SignalR.Client
{
    public static class HubProxyExtensions
    {
        public static DispatchingHubProxy UsingDispatcher(this IHubProxy proxy, Dispatcher dispatcher)
        {
            return new DispatchingHubProxy(proxy, dispatcher);
        }
    }
}
