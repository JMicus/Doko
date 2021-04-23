using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Doppelkopf.Core.App.Enums
{
    public class Symbol
    {
        public enum ESymbol
        {
            [Description("Fuchs")]
            fox,

            [Description("Jens")]
            jens,

            [Description("Charlie")]
            charlie,

            //[Description("Charlie")]
            trickCount,

            dealer
        }

        public ESymbol Type;

        public string Data;

        [JsonIgnore]
        public string FileName
        {
            get
            {
                var name = "";
                if (Type == ESymbol.trickCount)
                {
                    name = $"deckCount{Data}Symbol";
                }
                else
                {
                    name = Type.ToString();
                }
                return name + ".png";
            }
        }

        public Symbol()
        {
        }

        public Symbol(ESymbol type, int data) : this(type, data.ToString())
        {
        }

        public Symbol(ESymbol type, string data = null)
        {
            if (type == ESymbol.trickCount && (!int.TryParse(data, out var i) || i <= 0 || i >12))
            {
                throw new ArgumentException($"The data of type {type} has to be an int between 1 an 12 but it's {data}.");
            }

            Type = type;
            Data = data;
        }


    }
}
