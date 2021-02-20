using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Doppelkopf.Core.Connection
{
    public interface IHub
    {
        
        Task Init(string gameName, string playerNo, string playerName);

        Task SayHello(string gameName, string playerNo, string playerToken);

        Task PlayerMsg(string gameName, string playerNo, string msg);


    }
}
