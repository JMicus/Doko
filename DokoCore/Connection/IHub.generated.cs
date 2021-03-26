using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Doppelkopf.Core.App;
using Doppelkopf.Core.App.Enums;

namespace Doppelkopf.Core.Connection
{
    public interface IHub
    {
        
        Task AddSymbol(string gameName, string playerNo, string playerOfSymbol, string symbolCT);

        Task ChangeCardOrder(string gameName, string playerNo, string cardOrderE);

        Task Deal(string gameName, string playerNo, string force);

        Task Debug(string gameName, string playerNo, string tag);

        Task GiveCardsToPlayer(string gameName, string playerNo, string receivingPlayerNo, string cardsCT, string cardsBack);

        Task Init(string newGameName, string myPlayerNo, string myPlayerName);

        Task LastTrickBack(string gameName, string playerNo);

        Task PlayerMsg(string gameName, string playerNo, string msg);

        Task PutCard(string gameName, string playerNo, string cardCT);

        Task SayHello(string gameName, string playerNo, string playerToken);

        Task SetExternalPage(string gameName, string playerNo, string url);

        Task SetRules(string gameName, string playerNo, string rulesCT);

        Task TakeCardBack(string gameName, string playerNo);

        Task TakeTrick(string gameName, string playerNo);


    }
}
