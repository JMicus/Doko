using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Doppelkopf.Core.App
{
    public class CardList : List<Card>
    {
        public Card Remove(string cardCode)
        {
            foreach (var c in this)
            {
                if (c.ToCode() == cardCode)
                {
                    this.Remove(c);
                    //CenterCard = c;
                    return c;
                }
            }
            return null;
        }

        public string ToCode()
        {
            return string.Join(".", this.Select(c => c?.ToCode() ?? ""));
        }
    }
}
