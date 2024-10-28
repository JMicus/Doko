using Doppelkopf.Generator.Generators.ConnectionGenerator;
using System;
using ToolKit.Core.ParallelTools;

namespace Doppelkopf.Generator
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                throw new Exception("One argument expected");
            }

            var autoClose = false;

            foreach (var arg in args)
            {
                switch (arg.ToLower())
                {
                    case "connection":
                        ConnectionGenerator.Generate();
                        break;

                    case "-autoclose":
                        autoClose = true;
                        break;
                }
            }

            Console.WriteLine("DONE");

            if (autoClose)
            {
                PTools.RunDelayed(() => Environment.Exit(0), 3000);
            }
            //Console.ReadKey();
        }
    }
}