using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doppelkopf.Core.App
{
    public class Trick
    {
        public event Action OnChanged;

        public Player StartPlayer;
        public Player WonPlayer;

        public Card[] _cards { get; private set; } = new Card[4];

        public Card this[int i]
        {
            get
            {
                return _cards[i - 1] ?? new Card();
            }
            set
            {
                _cards[i - 1] = value;
            }
        }

        public Card this[Player p]
        {
            get
            {
                return this[p.No];
            }
            set
            {
                this[p.No] = value;
            }
        }

        public List<Card> ToList()
        {
            return new List<Card>(_cards);
        }

        public void SetCard(Player player, Card card)
        {
            SetCard(player.No, card);
        }

        public Card TakeCard(Player player)
        {
            var card = _cards[player.No - 1];
            _cards[player.No - 1] = null;
            return card;
        }

        public void SetCard(int player, Card card)
        {
            _cards[player - 1] = card;
        }

        public bool Empty
        {
            get
            {
                foreach (var c in _cards)
                {
                    if (c?.Name != null)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public Trick()
        {

        }

        public Trick(string code)
        {
            this.FromCode(code);
        }

        public void Clear()
        {
            _cards = new Card[4];
        }

        public bool Complete
        {
            get
            {
                foreach (var c in _cards)
                {
                    if (c?.Name == null)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public void FromCode(string code)
        {
            _cards = new Card[4];
            int i = 0;
            foreach (var cardCode in code.Split('.'))
            {
                _cards[i++] = new Card(cardCode);
            }

            OnChanged?.Invoke();
        }

        public string ToCode()
        {
            return $"{code(_cards[0])}.{code(_cards[1])}.{code(_cards[2])}.{code(_cards[3])}"; 
        }

        public override string ToString()
        {
            return ToCode();
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
                trick.SetCard(i, src[i]);
            }
            trick.StartPlayer = src.StartPlayer;
            return trick;
        }
    }
}
