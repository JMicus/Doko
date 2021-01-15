using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Doppelkopf.App.Enums
{
    public static class CardOrder
    {
        public static string Regular =  "sa.h1.kd.pd.hd.cd.kb.pb.hb.cb.ca.c1.ck.c9.ka.k1.kk.      k9.pa.p1.pk.      p9.ha.   hk.      h9";
        public static string SoloD =    "      kd.pd.hd.cd.                        ka.k1.kk.   kb.k9.pa.p1.pk.   pb.p9.ha.h1.hk.   hb.h9.ca.c1.ck.   cb.c9";
        public static string SoloB =    "                  kb.pb.hb.cb.            ka.k1.kk.kd   .k9.pa.p1.pk.pd.   p9.ha.h1.hk.hd.   h9.ca.c1.ck.cd   .c9";
        public static string SoloK =    "   h1.kd.pd.hd.cd.kb.pb.hb.cb.ka.k1.kk.k9.pa.p1.pk.p9.ha.   hk.h9.ca.c1.ck.c9";
        public static string SoloP =    "   h1.kd.pd.hd.cd.kb.pb.hb.cb.pa.p1.pk.p9.ka.k1.kk.k9.ha.   hk.h9.ca.c1.ck.c9";
        public static string SoloH =    "   h1.kd.pd.hd.cd.kb.pb.hb.cb.ha.   hk.h9.ka.k1.kk.k9.pa.p1.pk.p9.ca.c1.ck.c9";
        public static string SoloC =    "   h1.kd.pd.hd.cd.kb.pb.hb.cb.ca.c1.ck.c9.ka.k1.kk.k9.pa.p1.pk.p9.ha.   hk.h9";
        public static string SoloF =    "ka.k1.kk.kd.kb.k9.pa.p1.pk.pd.pb.p9.ha.h1.hk.hd.hb.h9.ca.c1.ck.cd.cb.c9";

        public static string OrderByName(string orderName)
        {
            foreach (var o in typeof(CardOrder).GetFields())
            {
                if (o.Name.ToLower() == orderName.ToLower())
                {
                    return o.GetValue(null).ToString();
                }
            }
            return "ERROR";
        }
    }
}
