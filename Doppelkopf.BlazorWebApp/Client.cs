using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doppelkopf.BlazorWebApp
{
    public class Client : IDisposable
    {
        #region Singleton
        //private static Client _instance;

        public static Client Instance { get; set; }

        public static void Initialize(Uri hubUrl)
        {
            Instance = new Client(hubUrl);
        }
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
        public Client(Uri hubUrl)
        {
            hubConnection = new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .Build();

            /*hubConnection.On<string, string, string>("Initialized", (gameName, playerNo, playerToken) =>
            {
                OnInitialized?.Invoke(gameName, playerNo, playerToken);
            });*/


       

            hubConnection.StartAsync();
        }
        #endregion

        #region Methods
        public void On(string method, Action<string, string, string> action)
        {
            hubConnection.On<string, string, string>(method, action);
        }

        public void Init(string gameName, string playerNo, string playerName)
        {
            hubConnection.SendAsync("Init", gameName, playerName, playerName);
        }

        public void Dispose()
        {
            _ = hubConnection.DisposeAsync();
        }
        #endregion
    }
}
