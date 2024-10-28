using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Text;
using Doppelkopf.Core.App;
using Doppelkopf.Core.App.Enums;
using Newtonsoft.Json;
using Doppelkopf.Core.App.Config;

namespace Doppelkopf.Core.Connection
{
    public partial class Client
    {
        #region (generated) delegates
        
        public delegate void CardsFromPlayerAction(Player playerCT, List<Card> cardsCT, bool cardsBack);
        public delegate void DealQuestionAction();
        public delegate void ExternalPageAction(string url);
        public delegate void HandAction(List<Card> handCT);
        public delegate void InfoAction(string msg);
        public delegate void InitializedAction(string gameName, int playerNo, string playerToken);
        public delegate void LastTrickAction(Trick trickCT);
        public delegate void MessagesAction(List<List<string>> messagesCT);
        public delegate void PlayerJoinedAction(int playerNo, string name);
        public delegate void PointsAction(Points pointsCT);
        public delegate void SettingsAction(DokoSettings settingsCT);
        public delegate void StatisticsAction(string stats);
        public delegate void SymbolsAction(List<List<Symbol>> symbolsCT);
        public delegate void TrickAction(Trick trickCT);
        public delegate void UnauthorizedAction(string message);
        #endregion (generated) delegates

        #region (generated) events
        
        public event CardsFromPlayerAction OnCardsFromPlayer;
        public event DealQuestionAction OnDealQuestion;
        public event ExternalPageAction OnExternalPage;
        public event HandAction OnHand;
        public event InfoAction OnInfo;
        public event InitializedAction OnInitialized;
        public event LastTrickAction OnLastTrick;
        public event MessagesAction OnMessages;
        public event PlayerJoinedAction OnPlayerJoined;
        public event PointsAction OnPoints;
        public event SettingsAction OnSettings;
        public event StatisticsAction OnStatistics;
        public event SymbolsAction OnSymbols;
        public event TrickAction OnTrick;
        public event UnauthorizedAction OnUnauthorized;
        #endregion (generated) events

        private void initializeGenerated()
        {
            #region (generated) init
            
            On("CardsFromPlayer", (string playerCT, string cardsCT, string cardsBack) => OnCardsFromPlayer?.Invoke(JsonConvert.DeserializeObject<Player>(playerCT), JsonConvert.DeserializeObject<List<Card>>(cardsCT), bool.Parse(cardsBack)));
            On("DealQuestion", () => OnDealQuestion?.Invoke());
            On("ExternalPage", (string url) => OnExternalPage?.Invoke(url));
            On("Hand", (string handCT) => OnHand?.Invoke(JsonConvert.DeserializeObject<List<Card>>(handCT)));
            On("Info", (string msg) => OnInfo?.Invoke(msg));
            On("Initialized", (string gameName, string playerNo, string playerToken) => OnInitialized?.Invoke(gameName, int.Parse(playerNo), playerToken));
            On("LastTrick", (string trickCT) => OnLastTrick?.Invoke(JsonConvert.DeserializeObject<Trick>(trickCT)));
            On("Messages", (string messagesCT) => OnMessages?.Invoke(JsonConvert.DeserializeObject<List<List<string>>>(messagesCT)));
            On("PlayerJoined", (string playerNo, string name) => OnPlayerJoined?.Invoke(int.Parse(playerNo), name));
            On("Points", (string pointsCT) => OnPoints?.Invoke(JsonConvert.DeserializeObject<Points>(pointsCT)));
            On("Settings", (string settingsCT) => OnSettings?.Invoke(JsonConvert.DeserializeObject<DokoSettings>(settingsCT)));
            On("Statistics", (string stats) => OnStatistics?.Invoke(stats));
            On("Symbols", (string symbolsCT) => OnSymbols?.Invoke(JsonConvert.DeserializeObject<List<List<Symbol>>>(symbolsCT)));
            On("Trick", (string trickCT) => OnTrick?.Invoke(JsonConvert.DeserializeObject<Trick>(trickCT)));
            On("Unauthorized", (string message) => OnUnauthorized?.Invoke(message));
            #endregion (generated) init
        }

        #region (generated) methods
        
        // creation info: Generate:99, init: Generate:26
        public void AddSymbol(int playerOfSymbol, Symbol symbolCT)
        {
            hubConnection.SendAsync("AddSymbol_H", gameName, playerNo.ToString(), playerToken, playerOfSymbol.ToString(), JsonConvert.SerializeObject(symbolCT));
        }
        
        // creation info: Generate:99, init: Generate:26
        public void ChangeCardOrder(EGameType cardOrderE)
        {
            hubConnection.SendAsync("ChangeCardOrder_H", gameName, playerNo.ToString(), playerToken, Parsenum.E2S(cardOrderE));
        }
        
        // creation info: Generate:99, init: Generate:26
        public void Deal(bool force)
        {
            hubConnection.SendAsync("Deal_H", gameName, playerNo.ToString(), playerToken, force.ToString());
        }
        
        // creation info: Generate:99, init: Generate:26
        public void Debug(string tag)
        {
            hubConnection.SendAsync("Debug_H", gameName, playerNo.ToString(), playerToken, tag);
        }
        
        // creation info: Generate:99, init: Generate:26
        public void GiveCardsToPlayer(int receivingPlayerNo, List<Card> cardsCT, bool cardsBack)
        {
            hubConnection.SendAsync("GiveCardsToPlayer_H", gameName, playerNo.ToString(), playerToken, receivingPlayerNo.ToString(), JsonConvert.SerializeObject(cardsCT), cardsBack.ToString());
        }
        
        // creation info: Generate:99, init: Generate:26
        public void Init(string newGameName, int myPlayerNo, string myPlayerName, string myPlayerToken)
        {
            hubConnection.SendAsync("Init_H", newGameName, myPlayerNo.ToString(), myPlayerName, myPlayerToken);
        }
        
        // creation info: Generate:99, init: Generate:26
        public void LastTrickBack()
        {
            hubConnection.SendAsync("LastTrickBack_H", gameName, playerNo.ToString(), playerToken);
        }
        
        // creation info: Generate:99, init: Generate:26
        public void PlayerMsg(string msg)
        {
            hubConnection.SendAsync("PlayerMsg_H", gameName, playerNo.ToString(), playerToken, msg);
        }
        
        // creation info: Generate:99, init: Generate:26
        public void PutCard(Card cardCT)
        {
            hubConnection.SendAsync("PutCard_H", gameName, playerNo.ToString(), playerToken, JsonConvert.SerializeObject(cardCT));
        }
        
        // creation info: Generate:99, init: Generate:26
        public void SayHello()
        {
            hubConnection.SendAsync("SayHello_H", gameName, playerNo.ToString(), playerToken);
        }
        
        // creation info: Generate:99, init: Generate:26
        public void SetExternalPage(string url)
        {
            hubConnection.SendAsync("SetExternalPage_H", gameName, playerNo.ToString(), playerToken, url);
        }
        
        // creation info: Generate:99, init: Generate:26
        public void SetSettings(DokoSettings settingsCT)
        {
            hubConnection.SendAsync("SetSettings_H", gameName, playerNo.ToString(), playerToken, JsonConvert.SerializeObject(settingsCT));
        }
        
        // creation info: Generate:99, init: Generate:26
        public void TakeCardBack()
        {
            hubConnection.SendAsync("TakeCardBack_H", gameName, playerNo.ToString(), playerToken);
        }
        
        // creation info: Generate:99, init: Generate:26
        public void TakeTrick()
        {
            hubConnection.SendAsync("TakeTrick_H", gameName, playerNo.ToString(), playerToken);
        }
        
        #endregion (generated) methods
    }
}