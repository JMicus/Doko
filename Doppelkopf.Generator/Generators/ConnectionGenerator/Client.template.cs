﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace NAMESPACE
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

        //DELEGATES

        //EVENTS

        #region ctor
        public Client(NavigationManager NavManager)
        {
            hubConnection = new HubConnectionBuilder()
                .WithUrl(NavManager.ToAbsoluteUri("/dokohub"))
                .Build();

            //CTOR


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

        //METHODS

        public void Dispose()
        {
            _ = hubConnection.DisposeAsync();
        }
        #endregion
    }
}
