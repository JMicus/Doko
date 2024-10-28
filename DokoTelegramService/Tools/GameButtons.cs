using Doppelkopf.Core.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DokoTelegramService.Tools
{
    internal class GameButtons
    {
        public int MaxHandCards { get; set; } = 12;

        public List<Card> Hand { get; set; } = new List<Card>();

        public Card[] Trick = new Card[4];

        public List<string> MenuButtons { get; set; } = new List<string>();

        public string[][] GetButtons()
        {
            var emptyRow = Enumerable.Range(0, MaxHandCards).Select(_ => CardVisualizer.EMPTY).ToArray();

            // fill up hand with empty cards
            var handCards = new Card[MaxHandCards];
            Hand.CopyTo(handCards, (MaxHandCards - Hand.Count) / 2);
            var hand = handCards.Select(c => c.CardMessage()).ToArray();

            // create trick
            var trick = emptyRow.ToArray();
            var start = (MaxHandCards / 2) - 2;

            trick[start + 0] = placeCard(Trick[3], 1);
            trick[start + 1] = placeCard(Trick[2], 2);
            trick[start + 2] = placeCard(Trick[0], 0);
            trick[start + 3] = placeCard(Trick[1], 1);

            return new string[][]
            {
                MenuButtons.ToArray(),
                trick,
                hand
            };
        }

        private string placeCard(Card card, int top)
        {
            var c = Enumerable.Range(0, 4).Select(_ => CardVisualizer.EMPTY).ToArray();

            card.CardMessage().Split("\r\n").CopyTo(c, top);

            return string.Join("\r\n", c);
        }
    }
}