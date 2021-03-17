using Doppelkopf.App.Enums;
using Doppelkopf.BlazorWebAssembly.Client.Shared;
using System;
using Doppelkopf.Core.App.Enums;

namespace DokoWebTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(SpecialGameView.SpecialGame.KreuzSolo.GetText());
            Console.WriteLine(SpecialGameView.SpecialGame.PikSolo.GetText());

            Console.ReadKey();
        }
    }
}
