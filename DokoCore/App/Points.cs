using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Doppelkopf.Core.App.Helper;

namespace Doppelkopf.Core.App
{
    public class Points
    {
        public List<(string PLayerName, int Points)> List;

        public Points()
        {

        }

        public Points(PlayerHolder player)
        {
            List = player.Select(p => (p.Name, p.WonPoints)).ToList();
        }
    }
}
