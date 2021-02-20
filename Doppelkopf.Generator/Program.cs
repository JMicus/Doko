using Doppelkopf.Generator.Generators.ConnectionGenerator;
using System;

namespace Doppelkopf.Generator
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                throw new Exception("One argument expected");
            }
            
            foreach (var arg in args)
            {
                switch (arg)
                {
                    case "connection":
                        ConnectionGenerator.Generate();
                        break;
                }
            }



            Console.WriteLine("DONE");
            //Console.ReadKey();
        }
    }
}
