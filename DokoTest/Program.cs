using Doppelkopf.App.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DokoTest
{
    class Program
    {
        private static double _rel = 2;
        private static int _gap;
        private static int _cw;
        private static int _ch;
        private static int _whGap;

        static void Main(string[] args)
        {

            
            
            
            

            //var l = JsonConvert.DeserializeObject<List<List<string>>>(JsonConvert.SerializeObject(list));

            Console.ReadKey();
        }

        static void trick()
        {
            _cw = 95;
            _ch = 130;

            _gap = (int)(_ch * .1);

            _whGap = (_ch - _cw);

            _rel = 100 / (double)(2 * _ch + 2 * _gap + _cw);

            var Trick = new []{ 0, 0, 0, 0, 0 };


            var _cards = new List<(int card, string Left, string Top, double Rotation)>()
            {
                //(Trick[1],   "0%",  "50%", 270),
                //(Trick[1],   "0%",  "50%", 0),//
                //(Trick[2],  "50%",   "0%",   0),
                //(Trick[3], "100%",  "50%",  90),
                //(Trick[3], "100%",  "50%",  0),//
                //(Trick[4],  "50%", "100%",   0)

                (Trick[1],   r("t"),  r("hg"), 270),
                //(Trick[1],   r(0),  r("hg"), 0),//
                (Trick[2],  r("hhgg"),   r(""),   0),
                (Trick[2],  r("hhggwwggt"),   r("hg"),   0),
                (Trick[2],  r("hhgg"),   r("hhgg"),   0)
            };

            foreach (var c in _cards)
            {
                Console.WriteLine(c.Left + " / " + c.Top);
            }


            Console.WriteLine(r("W"));
            Console.WriteLine(r("H"));

            Console.Read();
        }

        private static string r(double x)
        {
            return Math.Round(_rel * x, 2).ToString().Replace(',', '.') + "%";
        }

        private static string r(string spaceChars)
        {
            if (string.IsNullOrEmpty(spaceChars))
            {
                return "0%";
            }
            return r(spaceChars.ToCharArray()
                                .Select(c => (c == lower(c), lower(c)))
                                .Sum(p => double.Parse(p.Item2.ToString()
                                                                .Replace("g", _gap.ToString())
                                                                .Replace("w", _cw.ToString())
                                                                .Replace("h", _ch.ToString())
                                                                .Replace("t", _whGap.ToString()))
                               * (p.Item1 ? 1 : -1) / 2));
        }

        private static char lower(char c)
        {
            return c.ToString().ToLower()[0];
        }
    }
}
