using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client.Hubs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Windows.UI.Core;

namespace Microsoft.AspNet.SignalR.Client
{
    public class DispatchingHubProxy : IHubProxy
    {
        private readonly IHubProxy _hubProxy;
        private readonly CoreDispatcher _dispatcher;

        public DispatchingHubProxy(IHubProxy hubProxy, CoreDispatcher dispatcher)
        {
            _hubProxy = hubProxy;
            _dispatcher = dispatcher;
            //_dispatcher = dispatcher ?? Deployment.Current.Dispatcher;
        }

        public JToken this[string name]
        {
            get { return _hubProxy[name]; }
            set { _hubProxy[name] = value; }
        }

        public JsonSerializer JsonSerializer
        {
            get { return _hubProxy.JsonSerializer; }
        }

        public Task Invoke(string method, params object[] args)
        {
            return _hubProxy.Invoke(method, args);
        }

        public Task<T> Invoke<T>(string method, params object[] args)
        {
            return _hubProxy.Invoke<T>(method, args);
        }

        public Task Invoke<T>(string method, Action<T> onProgress, params object[] args)
        {
            return _hubProxy.Invoke<T>(method, onProgress, args);
        }

        public Task<TResult> Invoke<TResult, TProgress>(string method, Action<TProgress> onProgress, params object[] args)
        {
            return _hubProxy.Invoke<TResult, TProgress>(method, onProgress, args);
        }

        public Subscription Subscribe(string eventName)
        {
            return _hubProxy.Subscribe(eventName);
        }

        public void On<T1>(string eventName, Action<T1> action)
        {
            _hubProxy.On<T1>(eventName, a => action.Dispatch(a, _dispatcher).Forget());
        }

        public void On<T1, T2>(string eventName, Action<T1, T2> action)
        {
            _hubProxy.On<T1, T2>(eventName, (a1, a2) => action.Dispatch(a1, a2, _dispatcher).Forget());
        }

        public void On<T1, T2, T3>(string eventName, Action<T1, T2, T3> action)
        {
            _hubProxy.On<T1, T2, T3>(eventName, (a1, a2, a3) => action.Dispatch(a1, a2, a3, _dispatcher).Forget());
        }
    }
}
