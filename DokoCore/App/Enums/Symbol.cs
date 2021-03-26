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

        public object Data;

        public Symbol(ESymbol type, object data = null)
        {
            Type = type;
            Data = data;
        }
    }
}
