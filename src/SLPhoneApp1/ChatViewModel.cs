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
        private HubConnection _hubConnection;
        private DispatchingHubProxy _hubProxy;
        private string _message;
        private bool _canSend;
        private string _status;
        private object _locker = new object();
        
        public ChatViewModel(Dispatcher dispatcher)
        {
            Dispatcher = dispatcher;
            SendMessageCommand = new DelegateCommand(async () => await SendMessage(), () => CanSend);
            Messages = new ObservableCollection<ReceivedMessage>();
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

        public ObservableCollection<ReceivedMessage> Messages { get; private set; }

        public ICommand SendMessageCommand { get; private set; }

        protected void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private async Task Connect()
        {
            lock (_locker)
            {
                if (_hubConnection != null && _hubConnection.State != ConnectionState.Disconnected)
                {
                    return;
                }

                _hubConnection = new HubConnection(_hubUrl);
                _hubConnection.Reconnecting += () =>
                {
                    CanSend = false;
                    Status = "Connection reconnecting...";
                };
                _hubConnection.StateChanged += e =>
                {
                    if (e.OldState == ConnectionState.Reconnecting && e.NewState == ConnectionState.Connected)
                    {
                        Status = string.Format("Connected to {0} via {1}", _hubUrl, _hubConnection.Transport.Name);
                        CanSend = true;
                    }
                };
                _hubConnection.Closed += async () =>
                {
                    CanSend = false;
                    Status = "Connection lost, reconnecting in a bit...";
                    await Task.Delay(3000);
                    var ignore = Connect();
                };

                _hubProxy = _hubConnection.CreateHubProxy("chat").UsingDispatcher(Dispatcher);
                _hubProxy.On<string>("userJoined", userName =>
                    Messages.Add(new ReceivedMessage(string.Format("{0} joined", userName))));
                _hubProxy.On<string>("newMessage", message =>
                    Messages.Add(new ReceivedMessage(message)));
            }

            Status = "Connecting...";
            while (true)
            {

                try
                {
                    await _hubConnection.Start(new LongPollingTransport());
                    Status = string.Format("Connected to {0} via {1}", _hubUrl, _hubConnection.Transport.Name);
                    CanSend = true;
                    break;
                }
                catch (Exception)
                {
                    Status = string.Format("Connecting to {0} failed, trying again in a bit", _hubUrl);
                }
                await Task.Delay(3000);
            }
        }

        private async Task SendMessage()
        {
            var msg = Message;
            Message = string.Empty;
            //Status = "Sending message: " + msg;

            if (!string.IsNullOrWhiteSpace(msg))
            {
                try
                {
                    await _hubProxy.Invoke("Send", msg);
                    //Status = "Message sent";
                }
                catch (Exception ex)
                {
                    //Status = "Error sending message";
                   
                    Dispatcher.BeginInvoke(() =>
                        Messages.Add(new ReceivedMessage(string.Format("Error sending message: {0}", ex.Message)))
                    );
                }
            }
        }

        private class DelegateCommand : ICommand
        {
            private readonly Action _execute;
            private readonly Func<bool> _canExecute;

            public DelegateCommand(Action execute, Func<bool> canExecute)
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