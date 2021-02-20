using DokoCore.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DokoCore
{
    public class DokoException : Exception
    {
        public Player player;

        public DokoException(Player player, string message) : base(message)
        {
            
            this.player = player;
        }
    }
}
