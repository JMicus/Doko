using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Text;
using Doppelkopf.Core.App;
using Doppelkopf.Core.App.Enums;
using Newtonsoft.Json;

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

        private string gameName;
        private string playerNo;
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
        public delegate void TrickAction(string startPlayerNo, string trick);
        public delegate void LastTrickAction(string startPlayerNo, string trick);
        public delegate void PointsAction(string points);
        public delegate void SymbolsAction(string symbols);
        public delegate void DealQuestionAction();

        
        public event InitializedAction OnInitialized;
        public event UnauthorizedAction OnUnauthorized;
        public event PlayerJoinedAction OnPlayerJoined;
        public event MessagesAction OnMessages;
        public event HandAction OnHand;
        public event LayoutAction OnLayout;
        public event TrickAction OnTrick;
        public event LastTrickAction OnLastTrick;
        public event PointsAction OnPoints;
        public event SymbolsAction OnSymbols;
        public event DealQuestionAction OnDealQuestion;

        #region ctor
        public Client(Uri hubUri, string myGameName, string myPlayerNo)
        {
            hubConnection = new HubConnectionBuilder()
                .WithUrl(hubUri)
                .Build();

            this.gameName = myGameName;
            this.playerNo = myPlayerNo;

            
            On("Initialized", (string gameName, string playerNo, string playerToken) => OnInitialized?.Invoke(gameName, playerNo, playerToken));
            On("Unauthorized", (string gameName, string playerNo, string playerName) => OnUnauthorized?.Invoke(gameName, playerNo, playerName));
            On("PlayerJoined", (string no, string name) => OnPlayerJoined?.Invoke(no, name));
            On("Messages", (string msgs) => OnMessages?.Invoke(msgs));
            On("Hand", (string hand) => OnHand?.Invoke(hand));
            On("Layout", (string layout) => OnLayout?.Invoke(layout));
            On("Trick", (string startPlayerNo, string trick) => OnTrick?.Invoke(startPlayerNo, trick));
            On("LastTrick", (string startPlayerNo, string trick) => OnLastTrick?.Invoke(startPlayerNo, trick));
            On("Points", (string points) => OnPoints?.Invoke(points));
            On("Symbols", (string symbols) => OnSymbols?.Invoke(symbols));
            On("DealQuestion", () => OnDealQuestion?.Invoke());


            hubConnection.StartAsync();
        }
        #endregion

        #region Methods
        private void On(string method, Action action)
        {
            hubConnection.On(method, action);
        }

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

        
        public void Init(string playerName)
        {
            hubConnection.SendAsync("Init", gameName, playerNo, playerName);
        }

        public void SayHello(string playerToken)
        {
            hubConnection.SendAsync("SayHello", gameName, playerNo, playerToken);
        }

        public void PlayerMsg(string msg)
        {
            hubConnection.SendAsync("PlayerMsg", gameName, playerNo, msg);
        }

        public void PutCard(string cardCode)
        {
            hubConnection.SendAsync("PutCard", gameName, playerNo, cardCode);
        }

        public void TakeTrick()
        {
            hubConnection.SendAsync("TakeTrick", gameName, playerNo);
        }

        public void LastTrickBack()
        {
            hubConnection.SendAsync("LastTrickBack", gameName, playerNo);
        }

        public void TakeCardBack()
        {
            hubConnection.SendAsync("TakeCardBack", gameName, playerNo);
        }

        public void Deal(bool force)
        {
            hubConnection.SendAsync("Deal", gameName, playerNo, force);
        }

        public void GiveCardsToPlayer(string receivingPlayerNo, List<Card> cardsCT)
        {
            hubConnection.SendAsync("GiveCardsToPlayer", gameName, playerNo, receivingPlayerNo, JsonConvert.SerializeObject(cardsCT));
        }

        public void ChangeCardOrder(EGameType cardOrderE)
        {
            hubConnection.SendAsync("ChangeCardOrder", gameName, playerNo, Parsenum.E2S(cardOrderE));
        }


        public void Dispose()
        {
            _ = hubConnection.DisposeAsync();
        }
        #endregion
    }
}
