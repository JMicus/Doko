using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doppelkopf.Client
{
    public class Card
    {

        public static string ImageFromName(string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    return "/Images/empty.png";
                }
                return "/Images/" + name.Substring(0, 2).ToLower() + ".png";
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }
        }

        

    }
}
