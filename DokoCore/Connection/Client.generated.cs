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

        
        public delegate void CardsFromPlayerAction(Player playerCT, List<Card> cardsCT, bool cardsBack);
        public delegate void DealQuestionAction();
        public delegate void ExternalPageAction(string url);
        public delegate void HandAction(List<Card> handCT);
        public delegate void InfoAction(string msg);
        public delegate void InitializedAction(string gameName, int playerNo, string playerToken);
        public delegate void LastTrickAction(Trick trickCT);
        public delegate void LayoutAction(Layout layoutCT);
        public delegate void MessagesAction(List<List<string>> messagesCT);
        public delegate void PlayerJoinedAction(int playerNo, string name);
        public delegate void PointsAction(Points pointsCT);
        public delegate void RulesAction(Rules rulesCT);
        public delegate void StatisticsAction(string stats);
        public delegate void SymbolsAction(List<List<Symbol>> symbolsCT);
        public delegate void TrickAction(Trick trickCT);
        public delegate void UnauthorizedAction(string gameName, int playerNo, string playerName);

        
        public event CardsFromPlayerAction OnCardsFromPlayer;
        public event DealQuestionAction OnDealQuestion;
        public event ExternalPageAction OnExternalPage;
        public event HandAction OnHand;
        public event InfoAction OnInfo;
        public event InitializedAction OnInitialized;
        public event LastTrickAction OnLastTrick;
        public event LayoutAction OnLayout;
        public event MessagesAction OnMessages;
        public event PlayerJoinedAction OnPlayerJoined;
        public event PointsAction OnPoints;
        public event RulesAction OnRules;
        public event StatisticsAction OnStatistics;
        public event SymbolsAction OnSymbols;
        public event TrickAction OnTrick;
        public event UnauthorizedAction OnUnauthorized;

        #region ctor
        public Client(Uri hubUri, string myGameName, string myPlayerNo)
        {
            hubConnection = new HubConnectionBuilder()
                .WithUrl(hubUri)
                .Build();

            this.gameName = myGameName;
            this.playerNo = myPlayerNo;

            
            On("CardsFromPlayer", (string playerCT, string cardsCT, string cardsBack) => OnCardsFromPlayer?.Invoke(JsonConvert.DeserializeObject<Player>(playerCT), JsonConvert.DeserializeObject<List<Card>>(cardsCT), bool.Parse(cardsBack)));
            On("DealQuestion", () => OnDealQuestion?.Invoke());
            On("ExternalPage", (string url) => OnExternalPage?.Invoke(url));
            On("Hand", (string handCT) => OnHand?.Invoke(JsonConvert.DeserializeObject<List<Card>>(handCT)));
            On("Info", (string msg) => OnInfo?.Invoke(msg));
            On("Initialized", (string gameName, string playerNo, string playerToken) => OnInitialized?.Invoke(gameName, int.Parse(playerNo), playerToken));
            On("LastTrick", (string trickCT) => OnLastTrick?.Invoke(JsonConvert.DeserializeObject<Trick>(trickCT)));
            On("Layout", (string layoutCT) => OnLayout?.Invoke(JsonConvert.DeserializeObject<Layout>(layoutCT)));
            On("Messages", (string messagesCT) => OnMessages?.Invoke(JsonConvert.DeserializeObject<List<List<string>>>(messagesCT)));
            On("PlayerJoined", (string playerNo, string name) => OnPlayerJoined?.Invoke(int.Parse(playerNo), name));
            On("Points", (string pointsCT) => OnPoints?.Invoke(JsonConvert.DeserializeObject<Points>(pointsCT)));
            On("Rules", (string rulesCT) => OnRules?.Invoke(JsonConvert.DeserializeObject<Rules>(rulesCT)));
            On("Statistics", (string stats) => OnStatistics?.Invoke(stats));
            On("Symbols", (string symbolsCT) => OnSymbols?.Invoke(JsonConvert.DeserializeObject<List<List<Symbol>>>(symbolsCT)));
            On("Trick", (string trickCT) => OnTrick?.Invoke(JsonConvert.DeserializeObject<Trick>(trickCT)));
            On("Unauthorized", (string gameName, string playerNo, string playerName) => OnUnauthorized?.Invoke(gameName, int.Parse(playerNo), playerName));


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

        
        public void AddSymbol(int playerOfSymbol, Symbol symbolCT)
        {
            hubConnection.SendAsync("AddSymbol_H", gameName, playerNo.ToString(), playerOfSymbol.ToString(), JsonConvert.SerializeObject(symbolCT));
        }

        public void ChangeCardOrder(EGameType cardOrderE)
        {
            hubConnection.SendAsync("ChangeCardOrder_H", gameName, playerNo.ToString(), Parsenum.E2S(cardOrderE));
        }

        public void Deal(bool force)
        {
            hubConnection.SendAsync("Deal_H", gameName, playerNo.ToString(), force.ToString());
        }

        public void Debug(string tag)
        {
            hubConnection.SendAsync("Debug_H", gameName, playerNo.ToString(), tag);
        }

        public void GiveCardsToPlayer(int receivingPlayerNo, List<Card> cardsCT, bool cardsBack)
        {
            hubConnection.SendAsync("GiveCardsToPlayer_H", gameName, playerNo.ToString(), receivingPlayerNo.ToString(), JsonConvert.SerializeObject(cardsCT), cardsBack.ToString());
        }

        public void Init(string newGameName, int myPlayerNo, string myPlayerName)
        {
            hubConnection.SendAsync("Init_H", newGameName, myPlayerNo.ToString(), myPlayerName);
        }

        public void LastTrickBack()
        {
            hubConnection.SendAsync("LastTrickBack_H", gameName, playerNo.ToString());
        }

        public void PlayerMsg(string msg)
        {
            hubConnection.SendAsync("PlayerMsg_H", gameName, playerNo.ToString(), msg);
        }

        public void PutCard(Card cardCT)
        {
            hubConnection.SendAsync("PutCard_H", gameName, playerNo.ToString(), JsonConvert.SerializeObject(cardCT));
        }

        public void SayHello(string playerToken)
        {
            hubConnection.SendAsync("SayHello_H", gameName, playerNo.ToString(), playerToken);
        }

        public void SetExternalPage(string url)
        {
            hubConnection.SendAsync("SetExternalPage_H", gameName, playerNo.ToString(), url);
        }

        public void SetRules(Rules rulesCT)
        {
            hubConnection.SendAsync("SetRules_H", gameName, playerNo.ToString(), JsonConvert.SerializeObject(rulesCT));
        }

        public void TakeCardBack()
        {
            hubConnection.SendAsync("TakeCardBack_H", gameName, playerNo.ToString());
        }

        public void TakeTrick()
        {
            hubConnection.SendAsync("TakeTrick_H", gameName, playerNo.ToString());
        }


        public void Dispose()
        {
            _ = hubConnection.DisposeAsync();
        }
        #endregion
    }
}
