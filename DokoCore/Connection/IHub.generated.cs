using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Doppelkopf.Core.App;
using Doppelkopf.Core.App.Config;
using Doppelkopf.Core.App.Enums;

namespace Doppelkopf.Core.Connection
{
    public interface IHub
    {
        #region (generated) methods
        
        // creation info: Generate:103, init: Generate:26
        Task AddSymbol(Game game, Player player, int playerOfSymbol, Symbol symbolCT);
        
        // creation info: Generate:103, init: Generate:26
        Task ChangeCardOrder(Game game, Player player, EGameType cardOrderE);
        
        // creation info: Generate:103, init: Generate:26
        Task Deal(Game game, Player player, bool force);
        
        // creation info: Generate:103, init: Generate:26
        Task Debug(Game game, Player player, string tag);
        
        // creation info: Generate:103, init: Generate:26
        Task GiveCardsToPlayer(Game game, Player player, int receivingPlayerNo, List<Card> cardsCT, bool cardsBack);
        
        // creation info: Generate:103, init: Generate:26
        Task Init(string newGameName, int myPlayerNo, string myPlayerName, string myPlayerToken);
        
        // creation info: Generate:103, init: Generate:26
        Task LastTrickBack(Game game, Player player);
        
        // creation info: Generate:103, init: Generate:26
        Task PlayerMsg(Game game, Player player, string msg);
        
        // creation info: Generate:103, init: Generate:26
        Task PutCard(Game game, Player player, Card cardCT);
        
        // creation info: Generate:103, init: Generate:26
        Task SayHello(Game game, Player player);
        
        // creation info: Generate:103, init: Generate:26
        Task SetExternalPage(Game game, Player player, string url);
        
        // creation info: Generate:103, init: Generate:26
        Task SetSettings(Game game, Player player, DokoSettings settingsCT);
        
        // creation info: Generate:103, init: Generate:26
        Task TakeCardBack(Game game, Player player);
        
        // creation info: Generate:103, init: Generate:26
        Task TakeTrick(Game game, Player player);
        
        #endregion (generated) methods
    }
}