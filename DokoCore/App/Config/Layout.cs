using Doppelkopf.Core.App.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Doppelkopf.Core.App.Config
{
    public class Layout
    {
        public Setting<CardLayout> CardLayout { get; set; } = new Setting<CardLayout>(new CardLayout());
    }
}
