using Doppelkopf.Core.App.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doppelkopf.Core.App
{
    public class Card
    {
        public static string NameCodeOf(ECard? eCard)
        {
            if (eCard == null)
            {
                return "";
            }
            return Enum.GetName(typeof(ECard), eCard).ToLower();
        }

        private ECard? _name;
        public ECard? Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;

                //Image = (Image)Resources.ResourceManager.GetObject(Parsenum.E2S(Name).ToLower());
            }
        }

        public int No { get; set; }

        /// <summary>
        /// Two chars name related to <see cref="ECard"/>
        /// </summary>
        public string NameCode => NameCodeOf(Name.Value);

        public string FileName => Name == null ? "c0" : NameCode + No;

        public bool IsTrumpf => IsDame || IsBube || IsKaro || Name == ECard.H1;
        public bool IsDame => ToCode()[1] == 'd';
        public bool IsBube => ToCode()[1] == 'b';
        public bool IsKaro => ToCode()[0] == 'c' && !IsDame && !IsBube;

        public bool IsTrumpfKaro => IsTrumpf && IsKaro;

        public EColor Color;

        public string ColorChar
        {
            get
            {
                switch (Color)
                {
                    case EColor.Trumpf:
                        return "t";
                    case EColor.Kreuz:
                        return "k";
                    case EColor.Pik:
                        return "p";
                    case EColor.Herz:
                        return "h";
                    case EColor.Karo:
                        return "c";
                    default:
                        return "_";
                }
            }
            set
            {
                switch (value)
                {
                    case "t":
                        Color = EColor.Trumpf;
                        break;
                    case "k":
                        Color = EColor.Kreuz;
                        break;
                    case "p":
                        Color = EColor.Pik;
                        break;
                    case "h":
                        Color = EColor.Herz;
                        break;
                    case "c":
                        Color = EColor.Karo;
                        break;
                }
            }
        }

        //public string ImageName { get; internal set; }

        public Card(ECard name, int no = 0)
        {
            Name = name;
            No = no;
        }

        public Card(string code)
        {
            if (code != "")
            {
                Name = (ECard)Enum.Parse(typeof(ECard), code.Substring(0, 2).ToUpper());
                No = int.Parse(code[2].ToString());
                ColorChar = code[3].ToString();
            }
        }

        public Card() { }

        /// <summary>
        /// Returns NCT: CardColor, Card-Number/Head, Trumpf/Fehl-Color
        /// </summary>
        /// <returns></returns>
        public string ToCode()
        {
            if (Name == null)
            {
                return "";
            }
            return NameCode + No + ColorChar;
        }
    }
}
