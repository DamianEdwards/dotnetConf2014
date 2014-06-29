using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Transports;

namespace SLPhoneApp1
{
    internal class ChatViewModel : INotifyPropertyChanged
    {
        private readonly string _hubUrl = "http://localhost:61369/";
        private readonly TimeSpan _reconnectDelay = TimeSpan.FromSeconds(3);
        private DispatchingHubConnection _hubConnection;
        private DispatchingHubProxy _hubProxy;
        private string _message;
        private bool _canSend;
        private string _status;
        private object _connectionStateLocker = new object();
        
        public ChatViewModel(Dispatcher dispatcher)
        {
            Dispatcher = dispatcher;
            SendMessageCommand = new DelegateCommand(async () => await SendMessage(), () => CanSend);
            ClearLogCommand = new DelegateCommand(() => LogMessages.Clear());
            Messages = new ObservableCollection<Message>();
            LogMessages = new ObservableCollection<string>();
            Status = "Waiting to connect...";

            var ignore = Connect();
        }

        public Dispatcher Dispatcher { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool CanSend
        {
            get { return _canSend; }
            set
            {
                if (value != _canSend)
                {
                    _canSend = value;
                    OnPropertyChanged();
                    ((DelegateCommand)SendMessageCommand).OnCanExecuteChanged();
                }
            }
        }

        public string Status
        {
            get { return _status; }
            set
            {
                if (value != _status)
                {
                    _status = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Message
        {
            get { return _message; }
            set
            {
                if (value != _message)
                {
                    _message = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<Message> Messages { get; private set; }

        public ObservableCollection<string> LogMessages { get; private set; }

        public ICommand SendMessageCommand { get; private set; }

        public ICommand ClearLogCommand { get; private set; }

        protected void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private async Task Connect()
        {
            lock (_connectionStateLocker)
            {
                if (_hubConnection != null)
                {
                    if (_hubConnection.State != ConnectionState.Disconnected)
                    {
                        return;
                    }

                    _hubConnection.Dispose();
                }

                _hubConnection = new DispatchingHubConnection(_hubUrl, Dispatcher);
                
                // Enable tracing
                _hubConnection.TraceLevel = TraceLevels.All;
                _hubConnection.TraceWriter = new CollectionTraceWriter(LogMessages, Dispatcher);

                // Handle the connection lifetime events
                _hubConnection.Reconnecting += () =>
                {
                    lock (_connectionStateLocker)
                    {
                        if (_hubConnection.State == ConnectionState.Reconnecting)
                        {
                            CanSend = false;
                            Status = "Connection reconnecting...";
                        }
                    }
                };
                _hubConnection.Reconnected += () =>
                {
                    lock (_connectionStateLocker)
                    {
                        if (_hubConnection.State == ConnectionState.Connected)
                        {
                            Status = string.Format("Connected to {0} via {1}", _hubUrl, _hubConnection.Transport.Name);
                            CanSend = true;
                        }
                    }
                };
                _hubConnection.Closed += () =>
                {
                    lock (_connectionStateLocker)
                    {
                        if (_hubConnection.State == ConnectionState.Disconnected)
                        {
                            CanSend = false;
                            Status = "Connection lost, reconnecting in a bit...";
                            var ignore = Task.Run(() => Task.Delay(_reconnectDelay).ContinueWith(_ => Connect()));
                        }
                    }
                };
                _hubConnection.Error += ex =>
                {
                    Status = "Connection error, see log for details";
                };

                _hubProxy = _hubConnection.CreateHubProxy("chat");

                _hubProxy.On<string>("userJoined", userName =>
                {
                    Messages.Add(new Message(string.Format("{0} joined", userName)));
                });
                _hubProxy.On<string, string>("newMessage", (userName, message) =>
                {
                    Messages.Add(new Message(message, userName));
                });
            }

            Status = "Connecting...";

            while (true)
            {
                try
                {
                    // HTTP streaming transports don't work well on the Windows Phone stack so we force long polling.
                    // We're adding support for WebSockets to Windows Store/Phone 8.1 apps in SignalR 2.2.0
                    await _hubConnection.Start(new LongPollingTransport());
                    lock (_connectionStateLocker)
                    {
                        if (_hubConnection.State == ConnectionState.Connected)
                        {
                            Status = string.Format("Connected to {0} via {1}", _hubUrl, _hubConnection.Transport.Name);
                            CanSend = true;
                            break;
                        }
                    }
                }
                catch (Exception)
                {
                    Status = string.Format("Connecting to {0} failed, trying again in a bit", _hubUrl);
                }
                await Task.Delay(_reconnectDelay);
            }
        }

        private async Task SendMessage()
        {
            CanSend = false;

            // Capture the values locally before use
            var msg = Message;
            var proxy = _hubProxy;

            if (proxy != null && !string.IsNullOrWhiteSpace(msg))
            {
                try
                {
                    await proxy.Invoke("Send", msg);
                    Message = string.Empty;
                }
                catch (Exception)
                {
                    Messages.Add(new Message("Error sending message, see log for details"));
                }
            }

            CanSend = true;
        }

        private class DelegateCommand : ICommand
        {
            private readonly Action _execute;
            private readonly Func<bool> _canExecute;

            public DelegateCommand(Action execute, Func<bool> canExecute = null)
            {
                _execute = execute;
                _canExecute = canExecute;
            }

            public event EventHandler CanExecuteChanged;

            public void OnCanExecuteChanged()
            {
                if (CanExecuteChanged != null)
                {
                    CanExecuteChanged(this, EventArgs.Empty);
                }
            }

            public bool CanExecute(object parameter)
            {
                return _canExecute == null ? true : _canExecute();
            }

            public void Execute(object parameter)
            {
                if (CanExecute(parameter))
                {
                    _execute();
                }
            }
        }
    }
}