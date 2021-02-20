using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Doppelkopf.Core.Connection
{
    public class Client : IDisposable
    {
        #region Singleton
        //private static Client _instance;

        //public static Client Instance { get; set; }

        //public static void Initialize(Uri hubUrl)
        //{
        //    Instance = new Client(hubUrl);
        //}
        #endregion

        #region Fields
        private HubConnection hubConnection;
        #endregion

        #region Properties
        [Inject]
        protected NavigationManager NavigationManager { get; set; }
        #endregion

        
        public delegate void InitializedAction(string gameName, string playerNo, string playerToken);
        public delegate void UnauthorizedAction(string gameName, string playerNo, string playerName);
        public delegate void PlayerJoinedAction(string no, string name);
        public delegate void MessagesAction(string msgs);
        public delegate void HandAction(string hand);
        public delegate void LayoutAction(string layout);

        
        public event InitializedAction OnInitialized;
        public event UnauthorizedAction OnUnauthorized;
        public event PlayerJoinedAction OnPlayerJoined;
        public event MessagesAction OnMessages;
        public event HandAction OnHand;
        public event LayoutAction OnLayout;

        #region ctor
        public Client(NavigationManager NavManager)
        {
            hubConnection = new HubConnectionBuilder()
                .WithUrl(NavManager.ToAbsoluteUri("/dokohub"))
                .Build();

            
            On("Initialized", (string gameName, string playerNo, string playerToken) => OnInitialized?.Invoke(gameName, playerNo, playerToken));
            On("Unauthorized", (string gameName, string playerNo, string playerName) => OnUnauthorized?.Invoke(gameName, playerNo, playerName));
            On("PlayerJoined", (string no, string name) => OnPlayerJoined?.Invoke(no, name));
            On("Messages", (string msgs) => OnMessages?.Invoke(msgs));
            On("Hand", (string hand) => OnHand?.Invoke(hand));
            On("Layout", (string layout) => OnLayout?.Invoke(layout));


            hubConnection.StartAsync();
        }
        #endregion

        #region Methods
        private void On(string method, Action<string> action)
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

        
        public void Init(string gameName, string playerNo, string playerName)
        {
            hubConnection.SendAsync("Init", gameName, playerNo, playerName);
        }

        public void SayHello(string gameName, string playerNo, string playerToken)
        {
            hubConnection.SendAsync("SayHello", gameName, playerNo, playerToken);
        }

        public void PlayerMsg(string gameName, string playerNo, string msg)
        {
            hubConnection.SendAsync("PlayerMsg", gameName, playerNo, msg);
        }


        public void Dispose()
        {
            _ = hubConnection.DisposeAsync();
        }
        #endregion
    }
}
