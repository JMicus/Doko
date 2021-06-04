using Doppelkopf.Core.App.Config;
using Doppelkopf.Core.App.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;

namespace Doppelkopf.Core.App
{
    public class History
    {
        public List<List<Card>[]> Cards { get; } = new List<List<Card>[]>();

        public Dictionary<int, int> DealCount = new Dictionary<int, int>();



        /*public int Count => cards.Count;

        public List<Card>[] this[int x]
        {
            get
            {
                return cards[x];
            }
        }*/

        public History()
        {
            for (int i = 1; i <= 4; i++)
            {
                DealCount.Add(i, 0);
            }
        }

        public void Add(Game game)
        {
            var entry = new List<Card>[4];

            for (int i = 0; i < 4; i++)
            {
                entry[i] = new List<Card>(game.Player[i + 1].Cards).Where(x => true).ToList();
            }

            Cards.Add(entry);
        }

        public string Stats(Rules rules, bool includeLast = false)
        {
            var rnd = new Random();

            var list = new List<(string Title, string Hint, string Value)>();

            var cards = Cards.Take(Cards.Count - (includeLast ? 0 : 1)).ToList();

            if (cards.Count == 0)
            {
                return "";
            }

            var playerCards = new List<(int Player, List<Card> Cards)>();

            for (int i = 0; i < 4; i++)
            {
                playerCards.Add((i + 1, new List<Card>()));

                foreach (var game in cards)
                {
                    playerCards[i].Cards.AddRange(game[i]);
                }
            }

            list.Add(("TITEL", "", ""));

            // "Die Alten"
            var playerNo = playerByCardCount(playerCards, ECard.KD, 1, 2);

            list.Add(("- Die Alten:", "Spieler*in mit den meisten Kreuz-Damen", string.Join(" und ", playerNo)));


            // "Der Sonnenkönig"
            list.Add(("- Der Sonnenkönig:", "Spieler*in mit den meisten Herz-Zehnen", playerByCardCountSingle(playerCards, ECard.H1)));


            // "Schweinchen"
            var player = playerByCardCountSingle(playerCards, ECard.SA);
            if (player != null)
            {
                list.Add(("- Das Glücksschweinchen:", "Spieler*in mit den meisten Schweinchen", player));
            }


            // "Königin der Könige"
            list.Add(("- Königin der Könige:", "Spieler*in mit den meisten Königen", playerByCardCountSingle(playerCards, c => c.ToCode()[1] == 'k')));


            // "Dealer"
            list.Add(("- The Dealer:", "Spieler*in, die/der am häufigsten verteilt", "##P" + DealCount.OrderBy(c => rnd.Next())
                                                                                               .OrderByDescending(c => c.Value)
                                                                                               .First()
                                                                                               .Key));

            // "Arme"
            list.Add(("- Der arme Mensch", "Spieler*in mit den wenigsten Trümpfen", playerByCardCountSingle(playerCards, c => !c.IsTrumpf)));


            // average hands

            if (cards.Count > 1)
            {
                int playerCounter = 1;
                list.Add((" ", "", ""));
                list.Add(("DURCHSCHNITTSKARTEN", "", ""));
                foreach (var hand in AverageHandService.AverageCards(cards, rules))
                {
                    var p = "- ##P" + playerCounter++;
                    list.Add((p, "Typische Karten in diesem Spiel für " + p, string.Join("", hand.Select(h => "##C" + h.NameCode + h.No))));
                }
            }

            return string.Join("##;", list.Select(row => row.Title + "##," + row.Value));
        }

        private string playerByCardCountSingle(List<(int Player, List<Card> Cards)> playerCards, ECard card, int minOccurence = 1)
        {
            return playerByCardCount(playerCards, card, minOccurence, 1).FirstOrDefault();
        }

        private string playerByCardCountSingle(List<(int Player, List<Card> Cards)> playerCards, Func<Card, bool> condition, int minOccurence = 1)
        {
            return playerByCardCount(playerCards, condition, minOccurence, 1).FirstOrDefault();
        }

        private IEnumerable<string> playerByCardCount(List<(int Player, List<Card> Cards)> playerCards, ECard card, int minOccurence = 1, int playerCount = 1)
        {
            return playerByCardCount(playerCards, c => c.Name == card, minOccurence, playerCount);
        }

        private IEnumerable<string> playerByCardCount(List<(int Player, List<Card> Cards)> playerCards, Func<Card, bool> condition, int minOccurence = 1, int playerCount = 1)
        {
            var rnd = new Random();
            return playerCards.Select(pc => (pc.Player, pc.Cards.Count(c => condition(c))))
                              .Where(count => count.Item2 >= minOccurence)
                              .OrderBy(_ => rnd.Next())
                              .OrderByDescending(count => count.Item2)
                              .Take(playerCount)
                              .Select(pNo => "##P" + pNo.Player);
        }
    }
}
