using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doppelkopf.BlazorWebAssembly.Client.Helper
{
    public static class FormatExtensions
    {
        public static string ToDoubleString(this double d)
        {
            return d.ToString("0.0###", System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
