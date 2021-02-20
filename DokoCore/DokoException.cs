using C = Doppelkopf.Core.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doppelkopf.Core
{
    public class DokoException : Exception
    {
        public C.Player player;

        public DokoException(C.Player player, string message) : base(message)
        {
            
            this.player = player;
        }
    }
}
