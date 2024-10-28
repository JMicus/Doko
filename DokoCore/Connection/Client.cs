using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Doppelkopf.Core.Connection
{
    public partial class Client
    {
        #region Fields

        private HubConnection _hubConnection;
        private Uri _hubUri;

        private string gameName => Values.GameName;
        private int playerNo => Values.PlayerNo;
        private string playerToken => Values.PlayerToken;

        #endregion Fields

        public ClientValuesVO Values { get; set; } = new ClientValuesVO();

        private HubConnection hubConnection
        {
            get
            {
                if (_hubConnection == null)
                {
                    initializeConnection();
                }
                return _hubConnection;
            }
            set
            {
                _hubConnection = value;
            }
        }

        [Obsolete("Use contructor with uri")]
        public Client()
        {
        }

        public Client(string hubUri, ClientValuesVO values = null)
        {
            _hubUri = new Uri(hubUri);

            Values = values ?? new ClientValuesVO();
        }

        public void SetReceiver(AbstractServerMessageReceiver receiver)
        {
            receiver.Initialize(this);
        }

        #region Methods

        [Obsolete("Use contructor with uri")]
        public void Init(Uri hubUri)
        {
            _hubUri = hubUri;
        }

        public void Disconnect()
        {
            hubConnection.DisposeAsync();
            hubConnection = null;
        }

        private void initializeConnection()
        {
            hubConnection = new HubConnectionBuilder()
                .WithUrl(_hubUri)
                .Build();

            if (hubConnection.State == HubConnectionState.Connected)
            {
                //_ = hubConnection.StopAsync();
            }

            initializeGenerated();

            hubConnection.StartAsync();
        }

        protected void On(string method, Action action)
        {
            hubConnection.On(method, action);
        }

        protected void On(string method, Action<string> action)
        {
            hubConnection.On<string>(method, action);
        }

        private void On(string method, Action<string, string> action)
        {
            hubConnection.On<string, string>(method, action);
        }

        private void On(string method, Action<string, string, string> action)
        {
            hubConnection.On<string, string, string>(method, action);
        }

        private void Send(string method, string arg1, string arg2 = null, string arg3 = null)
        {
            if (arg3 != null)
            {
                hubConnection.SendAsync(method, arg1, arg2, arg3);
            }
            else if (arg2 != null)
            {
                hubConnection.SendAsync(method, arg1, arg2);
            }
            else
            {
                hubConnection.SendAsync(method, arg1);
            }
        }

        #endregion Methods
    }
}