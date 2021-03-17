using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doppelkopf.Core.App.Enums
{
    public enum EGameType
    {
        [Description("Damensolo")]
        SoloQueen,

        [Description("Bubensolo")]
        SoloJack,

        [Description("Kreuz-Solo")]
        SoloClubs,

        [Description("Pik-Solo")]
        SoloSpades,

        [Description("Herz-Solo")]
        SoloHearts,

        [Description("Karo-Solo")]
        SoloDiamonds,

        [Description("Fleischlos-Solo")]
        SoloNoTrumps,

        [Description("Trumpfarmut")]
        Poverty,

        [Description("Normalspiel")]
        Regular
    }
}
