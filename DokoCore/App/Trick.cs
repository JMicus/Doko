using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DokoCore.App
{
    public class Trick
    {
        public Player StartPlayer;
        public Player WonPlayer;

        public Card[] Cards { get; private set; } = new Card[4];

        public Card GetCard(Player player)
        {
            return Cards[player.No - 1];
        }

        public Card GetCard(int player)
        {
            return Cards[player - 1];
        }

        public void SetCard(Player player, Card card)
        {
            SetCard(player.No, card);
        }

        public Card TakeCard(Player player)
        {
            var card = Cards[player.No - 1];
            Cards[player.No - 1] = null;
            return card;
        }

        public void SetCard(int player, Card card)
        {
            Cards[player - 1] = card;
        }

        public bool Empty
        {
            get
            {
                foreach (var c in Cards)
                {
                    if (c != null)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public void Clear()
        {
            Cards = new Card[4];
        }

        public bool Complete
        {
            get
            {
                foreach (var c in Cards)
                {
                    if (c == null)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public string ToCode()
        {
            return $"{code(Cards[0])}.{code(Cards[1])}.{code(Cards[2])}.{code(Cards[3])}"; 
        }

        private static string code(Card c)
        {
            return c?.ToCode() ?? "";
        }

        public static Trick CopyFrom(Trick src)
        {
            var trick = new Trick();
            for (int i = 1; i <= 4; i++)
            {
                trick.SetCard(i, src.GetCard(i));
            }
            trick.StartPlayer = src.StartPlayer;
            return trick;
        }
    }
}
