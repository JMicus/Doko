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
            string nameSpace = "Doppelkopf.Core.Connection";

            string cFile = @"DokoCore\Connection\Client.generated.cs";
            string iFile = @"DokoCore\Connection\IHub.generated.cs";
            string hFile = @"DokoCore\Connection\AbstractHubBase.generated.cs";

            string c = getRootFile(cFile);
            string i = getRootFile(iFile);
            string h = getRootFile(hFile);

            var cToS = new List<Method>()
            {
                new Method("Init", "newGameName", "myPlayerNo.int", "myPlayerName"),
                Method.GameAndPlayer("Debug", "tag"),
                Method.GameAndPlayer("SayHello", "playerToken"),
                Method.GameAndPlayer("PlayerMsg", "msg"),
                Method.GameAndPlayer("PutCard", "card.Card"),
                Method.GameAndPlayer("TakeTrick"),
                Method.GameAndPlayer("LastTrickBack"),
                Method.GameAndPlayer("TakeCardBack"),
                Method.GameAndPlayer("Deal", "force.bool"),
                Method.GameAndPlayer("GiveCardsToPlayer", "receivingPlayerNo.int", "cards.List<Card>", "cardsBack.bool"),
                Method.GameAndPlayer("AddSymbol", "playerOfSymbol.int", "symbol.Symbol"),
                Method.GameAndPlayer("ChangeCardOrder", "cardOrder.e-EGameType"),
                Method.GameAndPlayer("SetSettings", "settings.DokoSettings"),
                Method.GameAndPlayer("SetExternalPage", "url")
            }.OrderBy(m => m.Name).ToList();

            var sToC = new List<Method>()
            {
                Method.OnMsgFromHub("Initialized", "gameName", "playerNo.int", "playerToken"),
                Method.OnMsgFromHub("Unauthorized", "gameName", "playerNo.int", "playerName"),
                Method.OnMsgFromHub("PlayerJoined", "playerNo.int", "name"),
                Method.OnMsgFromHub("Messages", "messages.List<List<string>>"),
                Method.OnMsgFromHub("Hand", "hand.List<Card>"),
                Method.OnMsgFromHub("CardsFromPlayer", "player.Player", "cards.List<Card>", "cardsBack.bool"),
                Method.OnMsgFromHub("Settings", "settings.DokoSettings"),
                Method.OnMsgFromHub("Statistics", "stats"),
                Method.OnMsgFromHub("Trick", "trick.Trick"),
                Method.OnMsgFromHub("LastTrick", "trick.Trick"),
                Method.OnMsgFromHub("Points", "points.Points"),
                Method.OnMsgFromHub("Symbols", "symbols.List<List<Symbol>>"),
                Method.OnMsgFromHub("Info", "msg"),
                Method.OnMsgFromHub("ExternalPage", "url"),
                new Method("DealQuestion")
            }.OrderBy(m => m.Name).ToList();

            #region CLIENT
            // NAMESPACE

            //c = c.Replace("NAMESPACE", nameSpace);
            #endregion


            #region IHUB
            // NAMESPACE

            //i = i.Replace("NAMESPACE", nameSpace);
            #endregion

            #region HUB
            // NAMESPACE

            //h = h.Replace("NAMESPACE", nameSpace);
            #endregion

            #region BOTH
            var cMethodCode = "";
            var iMethodCode = "";
            var hMethodCode = "";
            var cDelegateCode = "";
            var cEventsCode = "";
            var cCtorCode = "";
            foreach (var method in cToS)
            {
                cMethodCode += createMethod($"public void {method.Name}({method.Params.ToString(true, true)})",
                                            $"hubConnection.SendAsync(\"{method.Name}_H\", {method.Params.ToString(false, true, true, Method.ESerialize.Ser)});");

                iMethodCode += createMethod($"Task {method.Name}({method.Params.ToString(true, true, true, Method.ESerialize.None, true)})");
                
                var temp = createMethod($"protected abstract Task {method.Name}({method.Params.ToString(true, true, true, Method.ESerialize.None, false)})");
                temp = temp.Replace("string gameName, int playerNo", "Game game, Player player");
                hMethodCode += temp;

                temp = createMethod($"public async Task {method.Name}_H({method.Params.ToString(true, true, true, Method.ESerialize.None, true)})",
                                    string.Join("", method.Params.Select(p => $"logTransferObj(\"{method.Name}\", \"{p.Name}\", {p.Name});")) + 
                                    (method.Params[0].Name == "gameName"
                                    ? $"var game = getGame(gameName);var player = game?.Player[playerNo];"
                                    : "") +
                                    $"await {method.Name}({method.Params.ToString(false, true, true, Method.ESerialize.Des, false)});");
                temp = temp.Replace("gameName, int.Parse(playerNo)", "game, player");
                hMethodCode += temp;


                //public void On(string method, Action<string> action)
                //{
                //    hubConnection.On<string>(method, action);
                //}
            }
            foreach (var method in sToC)
            {
                var strings = string.Join(", ", method.Params.Select(p => "string"));
                var delName = $"{ method.Name }Action";

                cDelegateCode += $"\r\npublic delegate void {delName}({method.Params.StringFull});";

                cEventsCode += $"\r\npublic event {delName} On{method.Name};";

                //var actionName = $"action_{string.Join("_", method.Params.Select(p => p))}";
                //cMethodCode += createMethod($"private void _{method.Name}({delName} action)",
                //                            $"hubConnection.On<{strings}>(\"{method.Name}\", ({method.paramsStringFull}) => action({method.paramsString}));");

                cCtorCode += $"\r\nOn(\"{method.Name}\", ({method.Params.ToString(true, true, complexIsString: true)}) => On{method.Name}?.Invoke({method.Params.ToString(false, true, serializeComplexName: Method.ESerialize.Des)}));";

                //var logData = string.Join("", method.Params.Select(p => $"logTransferObj(\"{method.Name}\", \"{p.Name}\", {p.Name});"));
                //Func<string, string> logSer = (string p) => (p.Contains("Json")
                //                                ? $"\"{p.Split(new char[] { '(', ')' })[1]}\", {p.Replace(")", ", Formatting.Indented)")}, true"
                //                                : $"\"{p.Split('.')[0]}\", {p}");

                var logData = string.Join("", method.Params.ToList(false, showHidden: false, serializeComplexName: Method.ESerialize.SerOut)
                                                           .Select(p => $"logTransferObj(\"{method.Name}\", \"{p.Name}\", {p.Output});")
                                         );//.Replace(", \"\"", "");

                if (logData == "")
                {
                    logData = $"logTransferObj(\"{method.Name}\", \"NONE\", \"\");";
                }


                var temp = createMethod($"protected async Task Send{method.Name}(Game game, {method.Params.ToString(true, true, showHidden: false)})",
                                        logData + 
                                        $"await sendToAll(game, \"{method.Name}\", {method.Params.ToString(false, showHidden: false, serializeComplexName: Method.ESerialize.Ser)});");
                temp = temp.Replace(", )", ")");
                hMethodCode += temp;

                temp = createMethod($"protected async Task Send{method.Name}(Player player, {method.Params.ToString(true, true, showHidden: false)})",
                                        logData + 
                                            $"await sendToPlayer(player, \"{method.Name}\", {method.Params.ToString(false, showHidden: false, serializeComplexName: Method.ESerialize.Ser)});");
                temp = temp.Replace(", )", ")");
                hMethodCode += temp;

                temp = createMethod($"protected async Task Send{method.Name}ToCaller({method.Params.ToString(true, true, showHidden: false)})",
                                        logData + 
                                            $"await Clients.Caller.SendAsync(\"{method.Name}\", {method.Params.ToString(false, showHidden: false, serializeComplexName: Method.ESerialize.Ser)});");
                temp = temp.Replace(", )", ")");
                hMethodCode += temp;


            }
            c = replaceRegion(c, "delegates", cDelegateCode);
            c = replaceRegion(c, "events", cEventsCode);
            c = replaceRegion(c, "methods", cMethodCode);
            c = replaceRegion(c, "ctor", cCtorCode);
            i = replaceRegion(i, "methods", iMethodCode);
            h = replaceRegion(h, "methods", hMethodCode);
            #endregion


            writeRootFile(cFile, c);
            writeRootFile(iFile, i);
            writeRootFile(hFile, h);
        }

        private static string replaceRegion(string text, string regionName, string regionContent)
        {
            var head = "#region (generated) " + regionName;
            var foot = "#endregion";

            var xStart = text.IndexOf(head);

            if (xStart < 0)
            {
                Console.WriteLine($"Region {regionName} not found.");
                Console.ReadKey();
                return text;
            }

            var t = "";
            for (int x = xStart - 1; text[x] == ' '; x--) t += " ";


            return text.Substring(0, xStart) +
                   head + "\r\n" +
                   t + regionContent.Replace("\n", "\n" + t) + "\r\n" +
                   t + text.Substring(text.IndexOf(foot, xStart));
        }

        private static string createMethod(string def, string body = null)
        {
            var t = ""; // "        ";
            var m = "";

            m += "\r\n" + t + def + "";

            if (body == null)
            {
                m += ";";
            }
            else
            {
                m += $"\r\n{t}{{\r\n";

                foreach (var bodyLine in body.Split(';', StringSplitOptions.RemoveEmptyEntries))
                {
                    m += $"{t}    {bodyLine};\r\n";
                }
                m += $"{t}}}";
            }

            m += "\r\n";

            return m;
        }

        class Method
        {
            internal enum TypeType
            {
                Basic, Complex, Enum
            }

            internal enum ESerialize
            {
                None, Ser, Des, SerOut
            }

            internal class ParamList : List<(string Name, string Type, TypeType TypeType, bool IsHidden)>
            {

                internal string StringFull => string.Join(", ", this.Select(p => p.Type + " " + p.Name));
                internal string StringNames => string.Join(", ", this.Select(p => p.Name));
                internal string StringTypes => string.Join(", ", this.Select(p => p.Type));

                internal string ToString(bool type, bool name = true, bool showHidden = false, ESerialize serializeComplexName = ESerialize.None, bool complexIsString = false)
                {
                    return string.Join(", ", ToList(type, name, showHidden, serializeComplexName, complexIsString).Select(p => p.Output));
                }

                internal List<(string Name, string Type, TypeType TypeType, bool IsHidden, string Output)> ToList(bool type, bool name = true, bool showHidden = false, ESerialize serializeComplexName = ESerialize.None, bool complexIsString = false)
                {
                    return this.Where(p => showHidden || !p.IsHidden).Select(p =>
                    {
                        var res = "";

                        if (type)
                        {
                            if (complexIsString)
                            {
                                res += "string";
                            }
                            else
                            {
                                res += p.Type;
                            }
                        }

                        res += " ";

                        if (name)
                        {
                            bool ser = serializeComplexName == ESerialize.Ser || serializeComplexName == ESerialize.SerOut;
                            bool des = serializeComplexName == ESerialize.Des;
                            if (ser && p.TypeType == TypeType.Complex)
                            {
                                res += serializeComplexName == ESerialize.SerOut
                                       ? $"JsonConvert.SerializeObject({p.Name}, Formatting.Indented)"
                                       : $"JsonConvert.SerializeObject({p.Name})";
                            }
                            else if (ser && p.TypeType == TypeType.Enum)
                            {
                                res += $"Parsenum.E2S({p.Name})";
                            }
                            else if (ser && p.Type != "string")
                            {
                                res += $"{p.Name}.ToString()";
                            }
                            else if (des && p.TypeType == TypeType.Complex)
                            {
                                res += $"JsonConvert.DeserializeObject<{p.Type}>({p.Name})";
                            }
                            else if (des && p.TypeType == TypeType.Enum)
                            {
                                res += $"Parsenum.S2E<{p.Type}>({p.Name})";
                            }
                            else if (des && p.Type != "string")
                            {
                                res += $"{p.Type}.Parse({p.Name})";
                            }
                            else
                            {
                                res += p.Name;
                            }
                        }

                        return (p.Name, p.Type, p.TypeType, p.IsHidden, res.Trim());
                    }).ToList();
                }

                internal void Add(string param)
                {
                    var type = "string";
                    if (param.Contains('.'))
                    {
                        type = param.Split('.')[1];
                    }

                    var name = param.Split('.')[0];
                    
                    var typeType = TypeType.Basic;
                    
                    if (type.StartsWith("e-"))
                    {
                        type = type.Substring(2);
                        typeType = TypeType.Enum;
                    }
                    else if (!(new[] { "string", "bool", "int" }).Contains(type))
                    {
                        typeType = TypeType.Complex;
                    }



                    var isHidden = name.StartsWith("-");

                    name = name.Replace("-", "");

                    if (typeType == TypeType.Complex)
                    {
                        name += "CT";
                    }
                    if (typeType == TypeType.Enum)
                    {
                        name += "E";
                    }

                    base.Add((name, type, typeType, isHidden));
                }
            }

            internal string Name;
            internal ParamList Params = new ParamList();

            

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

            internal static Method GameAndPlayer(string name)
            {
                return new Method(name, "-gameName", "-playerNo.int");
            }
            internal static Method GameAndPlayer(string name, string param3)
            {
                var m = GameAndPlayer(name);
                m.Params.Add(param3);
                return m;
            }

            internal static Method GameAndPlayer(string name, string param3, string param4)
            {
                var m = GameAndPlayer(name, param3);
                m.Params.Add(param4);
                return m;
            }

            internal static Method GameAndPlayer(string name, string param3, string param4, string param5)
            {
                var m = GameAndPlayer(name, param3, param4);
                m.Params.Add(param5);
                return m;
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
