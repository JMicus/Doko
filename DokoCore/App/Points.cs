using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Doppelkopf.Core.App.Helper;

namespace Doppelkopf.Core.App
{
    public class Points
    {
        private List<(string PLayerName, int Points)> _points;

        public Points(PlayerHolder player)
        {
            _points = player.Select(p => (p.Name, p.WonPoints)).ToList();
        }

        public Points(string code)
        {
            _points = code.DokoCodeToList<string, int>().OrderByDescending(p => p.Item2).ToList();
        }

        public List<(string PlayerName, int Points)> ToList()
        {
            return _points;
        }

        public string ToCode()
        {
            return _points.ToDokoCode();
        }
    }
}
