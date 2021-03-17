using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doppelkopf.Core.App.Enums
{
    public static class Parsenum
    {
        public static string E2S(Enum e)
        {
            return Enum.GetName(e.GetType(), e);
        }

        public static T S2E<T>(string s)
        {
            return (T)Enum.Parse(typeof(T), s);
        }


    }
}
