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
                Method.GameAndPlayer("PlayerMsg", "msg"),
                Method.GameAndPlayer("PutCard", "cardCode"),
                Method.GameAndPlayer("TakeTrick"),
                Method.GameAndPlayer("LastTrickBack"),
                Method.GameAndPlayer("TakeCardBack"),
                Method.GameAndPlayer("Deal", "force.bool"),
                Method.GameAndPlayer("GiveCardsToPlayer", "receivingPlayerNo", "cards.List<Card>"),
                Method.GameAndPlayer("ChangeCardOrder", "cardOrder.e-EGameType")
            };

            var sToC = new List<Method>()
            {
                Method.OnMsgFromHub("Initialized", "gameName", "playerNo", "playerToken"),
                Method.OnMsgFromHub("Unauthorized", "gameName", "playerNo", "playerName"),
                Method.OnMsgFromHub("PlayerJoined", "no", "name"),
                Method.OnMsgFromHub("Messages", "msgs"),
                Method.OnMsgFromHub("Hand", "hand"),
                Method.OnMsgFromHub("Layout", "layout"),
                Method.OnMsgFromHub("Trick", "startPlayerNo", "trick"),
                Method.OnMsgFromHub("LastTrick", "startPlayerNo", "trick"),
                Method.OnMsgFromHub("Points", "points"),
                Method.OnMsgFromHub("Symbols", "symbols"),
                new Method("DealQuestion")
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
                cMethodCode += createMethod($"public void {method.Name}({method.Params.ToString(true, true)})",
                                            $"hubConnection.SendAsync(\"{method.Name}\", {method.Params.ToString(false, true, true, true)});");

                hMethodCode += createMethod($"Task {method.Name}({method.Params.ToString(true, true, true, false, true)})");


                //public void On(string method, Action<string> action)
                //{
                //    hubConnection.On<string>(method, action);
                //}
            }
            foreach (var method in sToC)
            {
                var strings = string.Join(", ", method.Params.Select(p => "string"));
                var delName = $"{ method.Name }Action";

                cDelegateCode += $"\r\n        public delegate void {delName}({method.Params.StringFull});";

                cEventsCode += $"\r\n        public event {delName} On{method.Name};";

                //var actionName = $"action_{string.Join("_", method.Params.Select(p => p))}";
                //cMethodCode += createMethod($"private void _{method.Name}({delName} action)",
                //                            $"hubConnection.On<{strings}>(\"{method.Name}\", ({method.paramsStringFull}) => action({method.paramsString}));");

                cCtorCode += $"\r\n            On(\"{method.Name}\", ({method.Params.StringFull}) => On{method.Name}?.Invoke({method.Params.StringNames}));";

                
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
            internal enum TypeType
            {
                Basic, Complex, Enum
            }

            internal class ParamList : List<(string Name, string Type, TypeType TypeType, bool IsHidden)>
            {

                internal string StringFull => string.Join(", ", this.Select(p => p.Type + " " + p.Name));
                internal string StringNames => string.Join(", ", this.Select(p => p.Name));
                internal string StringTypes => string.Join(", ", this.Select(p => p.Type));


                internal string ToString(bool type, bool name, bool showHidden = false, bool serializeComplexName = false, bool complexIsString = false)
                {
                    return string.Join(", ", this.Where(p => showHidden || !p.IsHidden).Select(p =>
                    {
                        var res = "";

                        if (type)
                        {
                            if (complexIsString && p.TypeType != TypeType.Basic)
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
                            if (serializeComplexName && p.TypeType == TypeType.Complex)
                            {
                                res += $"JsonConvert.SerializeObject({p.Name})";
                            }
                            else if (serializeComplexName && p.TypeType == TypeType.Enum)
                            {
                                res += $"Parsenum.E2S({p.Name})";
                            }
                            else
                            {
                                res += p.Name;
                            }
                        }

                        return res.Trim();
                    }));
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
                return new Method(name, "-gameName", "-playerNo");
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
