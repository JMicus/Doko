using System;
using System.Collections.Generic;
using System.Text;

namespace Doppelkopf.Core.App.Config
{
    public class DokoSettings
    {
        public Setting<Layout> Layout { get; set; } = new Setting<Layout>(new Layout());

        public Setting<Rules> Rules { get; set; } = new Setting<Rules>(new Rules());
    }
}
