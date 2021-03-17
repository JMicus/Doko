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
        
        Task Init(string gameName, string playerNo, string playerName);

        Task SayHello(string gameName, string playerNo, string playerToken);

        Task PlayerMsg(string gameName, string playerNo, string msg);

        Task PutCard(string gameName, string playerNo, string cardCode);

        Task TakeTrick(string gameName, string playerNo);

        Task LastTrickBack(string gameName, string playerNo);

        Task TakeCardBack(string gameName, string playerNo);

        Task Deal(string gameName, string playerNo, bool force);

        Task GiveCardsToPlayer(string gameName, string playerNo, string receivingPlayerNo, string cardsCT);

        Task ChangeCardOrder(string gameName, string playerNo, string cardOrderE);


    }
}
