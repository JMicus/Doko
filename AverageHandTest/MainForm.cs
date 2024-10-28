using AverageHandTest.UI;
using Doppelkopf.Core.App;
using Doppelkopf.Core.App.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AverageHandTest
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            bool playable = true;

            var game = new Game();

            for (int i = 0; i < 8; i++)
            {
                game.Deal(force: true);
            };

            foreach (var entry in game.History.Cards)
            {
                foreach (var hand in entry)
                {
                    showHand(hand);
                }
            }

            // fix buid!
            //foreach (var hand in AverageHandService.AverageCards(game.History.Cards, game.Rules))
            //{
            //    showHand(hand);
            //}

            /*// schwein durch h1 ersetzen
            game.History.Cards.ForEach(h => h.ToList().ForEach(hand => hand.ForEach(c =>
            {
                if (c.Name == ECard.SA)
                {
                    c.Name = ECard.H1;
                }
            })));

            var handsToShow = new List<List<Card>>();

            var rnd = new Random();




            var avgCardsLst = new List<List<Card>>();

            for (int i = 0; i < 4; i++)
            {
                avgCardsLst.Add(new List<Card>());
            }


            if (playable)
            {

                foreach (var card in new[] { ECard.H1, ECard.KD, ECard.CA })
                {
                    new int[] { 0, 1, 2, 3 }.OrderBy(
                                                        i => game.History.Cards.Select(hands => hands[i])
                                                                               .SelectMany(hand => hand)
                                                                               .Where(c => c.Name == card)
                                                                               .Count())
                                                   .Skip(2).ToList()
                                                   .ForEach(
                                                        i => avgCardsLst[i].Add(new Card(card))); 
                }
            }





            for (int i = 0; i < 4; i++)
            {
                //var cards = game.History.Cards.Select(x => x[i])
                //                              .SelectMany(x => x)
                //                              .GroupBy(x => x.Name)
                //                              .OrderByDescending(x => x.Count())
                //                              .Take(10)
                //                              .Select(x => x.First())
                //                              .ToList();

                var hands = game.History.Cards.Select(x => x[i]);
                var allCards = hands.SelectMany(x => x);

                float count = hands.Count();

                var avgCards = avgCardsLst[i];


                var trumpfCount = allCards.Where(x => isTrumpf(x)).Count();

                if (!playable)
                {        // herz 10?
                    if (allCards.Where(x => x.Name == ECard.H1).Count() * 2 >= count + rnd.Next(1))
                    {
                        avgCards.Add(new Card(ECard.H1));
                    }

                    // kreuz Dame?
                    if (allCards.Where(x => x.Name == ECard.KD).Count() * 2 >= count + rnd.Next(1))
                    {
                        avgCards.Add(new Card(ECard.KD));
                    }
                }


                var isDameNoKD = new Func<Card, bool>(x => isDame(x) && x.Name != ECard.KD);

                var dameCount = allCards.Where(x => isDame(x)).Count();

                avgCards.AddRange(getMostAppearedWhere(allCards, x => isDameNoKD(x), dameCount / count - avgCards.Where(x => x.Name == ECard.KD).Count()));


                // add Buben
                var maxTrumpfLeft = trumpfCount / count - avgCards.Where(x => isTrumpf(x)).Count();
                var bubeCount = allCards.Where(x => isBube(x)).Count();
                avgCards.AddRange(getMostAppearedWhere(allCards, x => isBube(x), Math.Min(maxTrumpfLeft, bubeCount / count)));

                // add Karo
                var trumpfLeft = trumpfCount / count - avgCards.Where(x => isTrumpf(x)).Count();
                avgCards.AddRange(getMostAppearedWhere(allCards, x => isKaro(x), trumpfLeft));

                // Fehlfarben
                var avgFehlfarbenCount = hands.Select(hand => hand.Where(c => !isTrumpf(c))
                                                                    .GroupBy(x => x.NameCode[0])
                                                                    .Count())
                                                .Sum(x => x) / count;

                var fehlList = allCards.Where(c => !isTrumpf(c))
                                       .GroupBy(x => x.NameCode[0])
                                       .OrderByDescending(x => x.Count())
                                       .Select(x => x.First().NameCode[0])
                                       .ToList();

                avgCards.AddRange(getMostAppearedWhere(allCards.Where(x => !isTrumpf(x))
                                                                .ToList(),
                                                                c => fehlList.GetRange(0, (int)(avgFehlfarbenCount + .5f)).Contains(c.NameCode[0]),
                                                                game.Rules.Deck.Count / 4 - avgCards.Count));


                //for (int i = 0; i < avgFehlfarbenCount)

                game.Rules.SortCards(avgCards);

                handsToShow.Add(avgCards);
            }

            handsToShow.ForEach(x => showHand(x));


            if (playable)
            {
                // make playable
                var cardsOut = handsToShow.SelectMany(x => x).Select(x => x.Name).ToList();
                var cardsIn = game.Rules.Deck.Select(x => x.Name).ToList();


                for (int o = 0; o < cardsOut.Count; o++)
                {
                    for (int i = 0; i < cardsOut.Count; i++)
                    {
                        if (cardsOut[o] == cardsIn[i])
                        {
                            cardsIn.RemoveAt(i--);
                            cardsOut.RemoveAt(o--);
                            break;
                        }
                    };
                };

                var inSorted = cardsIn.OrderBy(x => game.Rules.Order.IndexOf(Card.NameCodeOf(x))).ToList();
                var outSorted = cardsOut.OrderBy(x => game.Rules.Order.IndexOf(Card.NameCodeOf(x))).ToList();
                for (int i = 0; i < inSorted.Count(); i++)
                {

                    handsToShow.SelectMany(x => x).Where(c => c.Name == outSorted[i]).OrderBy(x => rnd.NextDouble()).First().Name = cardsIn[i];
                }
                
                handsToShow.ForEach(x =>
                {
                    game.Rules.SortCards(x);
                    showHand(x);

                });
            }
            */
        }

        private static int dist(string order, ECard a, ECard b)
        {
            return dist(order, new Card(a), new Card(b));
        }

        private static int dist(string order, Card a, Card b)
        {
            var d = 0;

            if (isTrumpf(a) != isTrumpf(b))
            {
                d += 100;
            }
            if (!isTrumpf(a) && a.NameCode[0] != b.NameCode[0])
            {
                d += 5;
            }

            d += Math.Abs(order.IndexOf(a.NameCode) - order.IndexOf(b.NameCode));

            return d;
        }

        private static bool isTrumpf(Card c)
        {
            return isDame(c) || isBube(c) || isKaro(c) || c.Name == ECard.H1;
        }

        private static bool isDame(Card c)
        {
            return c.NameCode[1] == 'd';
        }

        private static bool isBube(Card c)
        {
            return c.NameCode[1] == 'b';
        }

        private static bool isKaro(Card c)
        {
            return c.NameCode[0] == 'c' && !isDame(c) && !isBube(c);
        }

        private static bool isFehl(Card c, char color)
        {
            return c.NameCode[0] == color && !isTrumpf(c);
        }

        private static List<Card> getMostAppearedWhere(IEnumerable<Card> cards, Func<Card, bool> where, float count)
        {
            return cards.Where(x => where(x))
                        .GroupBy(x => x.Name)
                        .OrderByDescending(x => x.Count())
                        .Take((int)(count + .5f))
                        .Select(x => x.First())
                        .ToList();
        }

        private void showHand(List<Card> cards)
        {
            var handControl = new HandControl();
            handControl.Cards = cards;

            flowLayoutPanel.Controls.Add(handControl);
        }
    }
}
