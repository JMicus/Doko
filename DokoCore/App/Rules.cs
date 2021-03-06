﻿
using Doppelkopf.App.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doppelkopf.App
{
    public class Rules
    {
        // Settings /////////////////

        private string _order = CardOrder.Regular.Replace(" ", "");

        public string Order
        {
            get
            {
                return _order;
            }
            set
            {
                _order = value.Replace(" ", "");
            }
        }

        public int Pigs = 1;

        public bool Nines = false;

        //public string CardLayout = "Basic";
        //public string CardImageType = "png";
        //public string CardHeight = "128";
        //public int CardBorder = 10;
        public Dictionary<string, string> Layout;


        // //////////////////////////


        public List<Card> Deck
        {
            get
            {
                var deck = new List<Card>();
                for (int i = 0; i < 2; i++)
                {
                    foreach (ECard c in Enum.GetValues(typeof(ECard)))
                    {
                        var cString = Parsenum.E2S(c);

                        if (cString[0] != 'S'
                            && cString[0] != 'X'
                            && (Nines || cString[1] != '9'))
                        {
                            deck.Add(new Card(c, i));
                        }

                    }
                }
                return deck;
            }
        }


        public Rules()
        {
            ResetLayout();
        }

        public void ResetLayout()
        {
            Layout = new Dictionary<string, string>();
            Layout.Add("cardLayout", "Basic");
            Layout.Add("cardImageType", "png");
            Layout.Add("cardHeight", "128");
            Layout["cardWidth"] = "95";
            Layout.Add("cardBorder", "false");
            Layout.Add("background", "green.png");
        }

        //private string cardOrderRegular = "sa.h1.kd.pd.hd.cd.kb.pb.hb.cb.ca.c1.ck.ka.k1.kk.pa.p1.pk.ha.hk";

        public void SortCards(List<Card> cards, bool detectPigs = true)
        {
            var cardOrder = Order.Split('.');

            var dict = new Dictionary<string, int>();
            for (int i = 0; i < cardOrder.Length; i++)
            {
                dict.Add(cardOrder[i], i);
            }

            // schweine
            if (detectPigs)
            {
                if (Pigs > 0)
                {
                    int pigCount = 0;
                    foreach (var c in cards)
                    {
                        if (c.Name == ECard.SA)
                        {
                            c.Name = ECard.CA;
                            c.No = pigCount++;
                        }
                    }

                    if (Order == CardOrder.Regular.Replace(" ", ""))
                    {
                        var ca = cards.Where(x => x.Name == ECard.CA).ToList();
                        if (ca.Count() == 2)
                        {
                            ca[0].Name = ECard.SA;
                            if (Pigs == 2)
                            {
                                ca[1].Name = ECard.SA;
                            }
                        }
                    }
                }
            }

            // sort
            cards.Sort((a, b) => dict[a.NameCode].CompareTo(dict[b.NameCode]));
        }

        public int CountPoints(List<Card> cards)
        {
            return cards.Sum(card =>
            {
                switch (card.NameCode[1])
                {
                    case '1':
                        return 10;
                    case 'a':
                        return 11;
                    case 'k':
                        return 4;
                    case 'd':
                        return 3;
                    case 'b':
                        return 2;
                    default:
                        return 0;
                }
            });
        }

        public string ToCode()
        {
            return "Pigs-" + Pigs + ".Nines-" + Nines;
        }

        public void FromCode(string code)
        {
            var rules = code.Split('.');

            foreach (var rule in rules)
            {
                var pair = rule.Split('-');
                switch (pair[0])
                {
                    case "Pigs":
                        Pigs = Convert.ToInt32(pair[1]);
                        break;
                    case "Nines":
                        Nines = Convert.ToBoolean(pair[1]);
                        break;
                }
            }
        }
    }
}
