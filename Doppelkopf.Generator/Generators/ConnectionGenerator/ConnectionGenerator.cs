using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Doppelkopf.Generator.Generators.ConnectionGenerator
{
    class ConnectionGenerator : AbstractGenerator
    {
        public static void Generate()
        {
            string c = getProjectFile(@"Generators\ConnectionGenerator\Client.template.cs");
            string h = getProjectFile(@"Generators\ConnectionGenerator\IHub.template.cs");

            var cToS = new List<Method>()
            {
                Method.GameAndPlayer("Init", "playerName"),
                Method.GameAndPlayer("SayHello", "playerToken"),
                Method.GameAndPlayer("PlayerMsg", "msg")
            };

            var sToC = new List<Method>()
            {
                Method.OnMsgFromHub("Initialized", "gameName", "playerNo", "playerToken"),
                Method.OnMsgFromHub("Unauthorized", "gameName", "playerNo", "playerName"),
                Method.OnMsgFromHub("PlayerJoined", "no", "name"),
                Method.OnMsgFromHub("Messages", "msgs"),
                Method.OnMsgFromHub("Hand", "hand"),
                Method.OnMsgFromHub("Layout", "layout")
            };

            #region CLIENT
            // NAMESPACE

            c = c.Replace("NAMESPACE", "Doppelkopf.Core.Connection");
            #endregion


            #region HUB
            // NAMESPACE

            h = h.Replace("NAMESPACE", "Doppelkopf.Core.Connection");
            #endregion

            #region BOTH
            var cMethodCode = "";
            var hMethodCode = "";
            var cDelegateCode = "";
            var cEventsCode = "";
            var cCtorCode = "";
            foreach (var method in cToS)
            {
                cMethodCode += createMethod($"public void {method.Name}({method.paramsStringFull})",
                                            $"hubConnection.SendAsync(\"{method.Name}\", {method.paramsString});");

                hMethodCode += createMethod($"Task {method.Name}({method.paramsStringFull})");


                //public void On(string method, Action<string> action)
                //{
                //    hubConnection.On<string>(method, action);
                //}
            }
            foreach (var method in sToC)
            {
                var strings = string.Join(", ", method.Params.Select(p => "string"));
                var delName = $"{ method.Name }Action";

                cDelegateCode += $"\r\n        public delegate void {delName}({method.paramsStringFull});";

                cEventsCode += $"\r\n        public event {delName} On{method.Name};";

                //var actionName = $"action_{string.Join("_", method.Params.Select(p => p))}";
                //cMethodCode += createMethod($"private void _{method.Name}({delName} action)",
                //                            $"hubConnection.On<{strings}>(\"{method.Name}\", ({method.paramsStringFull}) => action({method.paramsString}));");

                cCtorCode += $"\r\n            On(\"{method.Name}\", ({method.paramsStringFull}) => On{method.Name}?.Invoke({method.paramsString}));";

                
            }
            c = c.Replace("//METHODS", cMethodCode);
            c = c.Replace("//DELEGATES", cDelegateCode);
            c = c.Replace("//EVENTS", cEventsCode);
            c = c.Replace("//CTOR", cCtorCode);
            h = h.Replace("//METHODS", hMethodCode);
            #endregion





            //Console.WriteLine(c);
            writeRootFile(@"DokoCore\Connection\Client.generated.cs", c);
            writeRootFile(@"DokoCore\Connection\IHub.generated.cs", h);
        }

        private static string createMethod(string def, string body = null)
        {
            var t = "        ";
            var m = "";

            m += "\r\n" + t + def + "";

            if (body == null)
            {
                m += ";";
            }
            else
            {
                m += $"\r\n{t}{{\r\n";
                m += $"{t}    {body}\r\n";
                m += $"{t}}}";
            }

            m += "\r\n";

            return m;
        }

        class Method
        {
            internal string Name;
            internal List<string> Params = new List<string>();

            internal string paramsStringFull => string.Join(", ", Params.Select(p => "string " + p));
            internal string paramsString => string.Join(", ", Params);

            internal Method(string name)
            {
                Name = name;
            }

            internal Method(string name, string param1) : this(name)
            {
                Params.Add(param1);
            }

            internal Method(string name, string param1, string param2) : this(name, param1)
            {
                Params.Add(param2);
            }

            internal Method(string name, string param1, string param2, string param3) : this(name, param1, param2)
            {
                Params.Add(param3);
            }

            internal static Method GameAndPlayer(string name, string param3)
            {
                return new Method(name, "gameName", "playerNo", param3);
            }

            internal static Method OnMsgFromHub(string name, string param1)
            {
                return new Method(name, param1);
            }

            internal static Method OnMsgFromHub(string name, string param1, string param2)
            {
                var m = OnMsgFromHub(name, param1);
                m.Params.Add(param2);
                return m;
            }

            internal static Method OnMsgFromHub(string name, string param1, string param2, string param3)
            {
                var m = OnMsgFromHub(name, param1, param2);
                m.Params.Add(param3);
                return m;
            }
        }
    }
}
