using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doppelkopf.BlazorWebAssembly.Helper
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

        #region Events
        public event Action<string, string, string> OnInitialized;
        #endregion

        #region ctor
        public Client(NavigationManager NavManager)
        {
            hubConnection = new HubConnectionBuilder()
                .WithUrl(NavManager.ToAbsoluteUri("/dokohub"))
                .Build();

            /*hubConnection.On<string, string, string>("Initialized", (gameName, playerNo, playerToken) =>
            {
                OnInitialized?.Invoke(gameName, playerNo, playerToken);
            });*/


       

            hubConnection.StartAsync();
        }
        #endregion

        #region Methods
        public void On(string method, Action<string> action)
        {
            hubConnection.On<string>(method, action);
        }

        public void On(string method, Action<string, string> action)
        {
            hubConnection.On<string, string>(method, action);
        }

        public void On(string method, Action<string, string, string> action)
        {
            hubConnection.On<string, string, string>(method, action);
        }

        public void Send(string method, string arg1, string arg2 = null, string arg3 = null)
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

        public void Dispose()
        {
            _ = hubConnection.DisposeAsync();
        }
        #endregion
    }
}
