using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Doppelkopf.Core.App
{
    public class Layout : Dictionary<string, string>
    {
        public Layout()
        {
            ResetLayout();
        }

        public void ResetLayout()
        {
            this.Clear();
            Add("cardLayout", "Basic");
            Add("cardImageType", "png");
            Add("cardHeight", "128");
            Add("cardWidth",  "95");
            Add("cardBorder", "false");
            Add("background", "green.png");
        }

        public void FromCode(string code)
        {
            this.Clear();
            foreach (var pair in code.Split('.'))
            {
                var s = pair.Split(':');
                this.Add(s[0], s[1]);
            }
        }

        public string ToCode()
        {
            return string.Join(".", this.Select(pair => pair.Key + ":" + pair.Value));
        }
    }
}
