using Doppelkopf.Core.App;
using Doppelkopf.Core.App.Config;
using Doppelkopf.Core.App.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Doppelkopf.Core.Connection
{
    /// <summary>
    /// Handles all messages from server.
    /// Is used from client
    /// </summary>
    public abstract class AbstractServerMessageReceiver
    {
        internal void Initialize(Client client)
        {
            #region (generated) init
            
            client.OnCardsFromPlayer += OnCardsFromPlayer;
            client.OnDealQuestion += OnDealQuestion;
            client.OnExternalPage += OnExternalPage;
            client.OnHand += OnHand;
            client.OnInfo += OnInfo;
            client.OnInitialized += OnInitialized;
            client.OnLastTrick += OnLastTrick;
            client.OnMessages += OnMessages;
            client.OnPlayerJoined += OnPlayerJoined;
            client.OnPoints += OnPoints;
            client.OnSettings += OnSettings;
            client.OnStatistics += OnStatistics;
            client.OnSymbols += OnSymbols;
            client.OnTrick += OnTrick;
            client.OnUnauthorized += OnUnauthorized;
            #endregion (generated) init
        }

        #region (generated) methods
        
        protected abstract void OnCardsFromPlayer(Player playerCT, List<Card> cardsCT, bool cardsBack);
        protected abstract void OnDealQuestion();
        protected abstract void OnExternalPage(string url);
        protected abstract void OnHand(List<Card> handCT);
        protected abstract void OnInfo(string msg);
        protected abstract void OnInitialized(string gameName, int playerNo, string playerToken);
        protected abstract void OnLastTrick(Trick trickCT);
        protected abstract void OnMessages(List<List<string>> messagesCT);
        protected abstract void OnPlayerJoined(int playerNo, string name);
        protected abstract void OnPoints(Points pointsCT);
        protected abstract void OnSettings(DokoSettings settingsCT);
        protected abstract void OnStatistics(string stats);
        protected abstract void OnSymbols(List<List<Symbol>> symbolsCT);
        protected abstract void OnTrick(Trick trickCT);
        protected abstract void OnUnauthorized(string message);
        #endregion (generated) methods
    }
}