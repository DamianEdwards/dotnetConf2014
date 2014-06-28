using System;
using System.Windows;
using System.Windows.Threading;
using Microsoft.AspNet.SignalR.Client;

namespace SLPhoneApp1
{
    public class DispatchingHubConnection : HubConnection
    {
        private readonly Dispatcher _dispatcher;
        private Action<StateChange> _stateChanged;
        private Action _connectionSlow;
        private Action _reconnecting;
        private Action _reconnected;
        private Action _closed;
        private Action<Exception> _error;

        public DispatchingHubConnection(string url, Dispatcher dispatcher)
            : base(url)
        {
            _dispatcher = dispatcher ?? Deployment.Current.Dispatcher;
            base.StateChanged += HubConnection_StateChanged;
            base.ConnectionSlow += HubConnection_ConnectionSlow;
            base.Reconnecting += HubConnection_Reconnecting;
            base.Reconnected += HubConnection_Reconnected;
            base.Closed += HubConnection_Closed;
            base.Error += HubConnection_Error;
        }

        public new event Action<StateChange> StateChanged
        {
            add { _stateChanged = (Action<StateChange>)Action.Combine(_stateChanged, value); }
            remove { Action.Remove(_stateChanged, value); }
        }

        private void HubConnection_StateChanged(StateChange stateChange)
        {
            if (_stateChanged != null)
            {
                SmartDispatch(() => _stateChanged(stateChange));
            }
        }

        public new event Action ConnectionSlow
        {
            add { _connectionSlow = (Action)Action.Combine(_connectionSlow, value); }
            remove { Action.Remove(_connectionSlow, value); }
        }

        private void HubConnection_ConnectionSlow()
        {
            if (_connectionSlow != null)
            {
                SmartDispatch(() => _connectionSlow());
            }
        }

        public new event Action Reconnecting
        {
            add { _reconnecting = (Action)Action.Combine(_reconnecting, value); }
            remove { Action.Remove(_reconnecting, value); }
        }

        private void HubConnection_Reconnecting()
        {
            if (_reconnecting != null)
            {
                SmartDispatch(_reconnecting);
            }
        }

        public new event Action Reconnected
        {
            add { _reconnected = (Action)Action.Combine(_reconnected, value); }
            remove { Action.Remove(_reconnected, value); }
        }

        private void HubConnection_Reconnected()
        {
            if (_reconnected != null)
            {
                SmartDispatch(_reconnected);
            }
        }

        public new event Action Closed
        {
            add { _closed = (Action)Action.Combine(_closed, value); }
            remove { Action.Remove(_closed, value); }
        }

        private void HubConnection_Closed()
        {
            if (_closed != null)
            {
                SmartDispatch(_closed);
            }
        }

        public new event Action<Exception> Error
        {
            add { _error = (Action<Exception>)Action.Combine(_error, value); }
            remove { Action.Remove(_error, value); }
        }

        void HubConnection_Error(Exception ex)
        {
            if (_error != null)
            {
                SmartDispatch(() => _error(ex));
            }
        }

        public new DispatchingHubProxy CreateHubProxy(string name)
        {
            return new DispatchingHubProxy(base.CreateHubProxy(name), _dispatcher);
        }

        private void SmartDispatch(Action action)
        {
            if (_dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                _dispatcher.BeginInvoke(() => action());
            }
        }
    }
}
