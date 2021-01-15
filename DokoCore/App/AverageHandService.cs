using Doppelkopf.App;
using Doppelkopf.App.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DokoCore.App
{
    public static class AverageHandService
    {
        public static List<List<Card>> AverageCards(List<List<Card>[]> cards, Rules rules)
        {
            bool playable = false;

            // schwein durch h1 ersetzen
            cards.ForEach(h => h.ToList().ForEach(hand => hand.ForEach(c =>
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
                                                        i => cards.Select(hands => hands[i])
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

                var hands = cards.Select(x => x[i]);
                var allCards = hands.SelectMany(x => x);

                float count = hands.Count();

                var avgCards = avgCardsLst[i];


                var trumpfCount = allCards.Where(x => x.IsTrumpf).Count();

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


                var isDameNoKD = new Func<Card, bool>(x => x.IsDame && x.Name != ECard.KD);

                var dameCount = allCards.Where(x => x.IsDame).Count();

                avgCards.AddRange(getMostAppearedWhere(allCards, x => isDameNoKD(x), dameCount / count - avgCards.Where(x => x.Name == ECard.KD).Count()));


                // add Buben
                var maxTrumpfLeft = trumpfCount / count - avgCards.Where(x => x.IsTrumpf).Count();
                var bubeCount = allCards.Where(x => x.IsBube).Count();
                avgCards.AddRange(getMostAppearedWhere(allCards, x => x.IsBube, Math.Min(maxTrumpfLeft, bubeCount / count)));

                // add Karo
                var trumpfLeft = trumpfCount / count - avgCards.Where(x => x.IsTrumpf).Count();
                avgCards.AddRange(getMostAppearedWhere(allCards, x => x.IsKaro, trumpfLeft));

                // Fehlfarben
                var avgFehlfarbenCount = hands.Select(hand => hand.Where(c => !c.IsTrumpf)
                                                                    .GroupBy(x => x.NameCode[0])
                                                                    .Count())
                                                .Sum(x => x) / count;

                var fehlList = allCards.Where(c => !c.IsTrumpf)
                                       .GroupBy(x => x.NameCode[0])
                                       .OrderByDescending(x => x.Count())
                                       .Select(x => x.First().NameCode[0])
                                       .ToList();

                avgCards.AddRange(getMostAppearedWhere(allCards.Where(x => !x.IsTrumpf)
                                                                .ToList(),
                                                                c => fehlList.GetRange(0, (int)(avgFehlfarbenCount + .5f)).Contains(c.NameCode[0]),
                                                                rules.Deck.Count / 4 - avgCards.Count));


                //for (int i = 0; i < avgFehlfarbenCount)

                rules.SortCards(avgCards);

                handsToShow.Add(avgCards);
            }

            //handsToShow.ForEach(x => showHand(x));


            if (playable)
            {
                // make playable
                var cardsOut = handsToShow.SelectMany(x => x).Select(x => x.Name).ToList();
                var cardsIn = rules.Deck.Select(x => x.Name).ToList();


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

                var inSorted = cardsIn.OrderBy(x => rules.Order.IndexOf(Card.NameCodeOf(x))).ToList();
                var outSorted = cardsOut.OrderBy(x => rules.Order.IndexOf(Card.NameCodeOf(x))).ToList();
                for (int i = 0; i < inSorted.Count(); i++)
                {

                    handsToShow.SelectMany(x => x).Where(c => c.Name == outSorted[i]).OrderBy(x => rnd.NextDouble()).First().Name = cardsIn[i];
                }

                handsToShow.ForEach(x =>
                {
                    rules.SortCards(x);

                });
            }
                
            return handsToShow;
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
    }
}
