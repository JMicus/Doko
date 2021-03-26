using Newtonsoft.Json;
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

        public Card[] Cards { get; set; } = new Card[4];

        public Card this[int i]
        {
            get
            {
                return Cards[i - 1] ?? new Card();
            }
            set
            {
                Cards[i - 1] = value;
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
            return new List<Card>(Cards);
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

        [JsonIgnore]
        public bool Empty
        {
            get
            {
                foreach (var c in Cards)
                {
                    if (c?.Name != null)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        [JsonIgnore]
        public bool Complete
        {
            get
            {
                foreach (var c in Cards)
                {
                    if (c?.Name == null)
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
            Cards = new Card[4];
        }

        public void FromCode(string code)
        {
            Cards = new Card[4];
            int i = 0;
            foreach (var cardCode in code.Split('.'))
            {
                Cards[i++] = new Card(cardCode);
            }

            OnChanged?.Invoke();
        }

        public string ToCode()
        {
            return $"{code(Cards[0])}.{code(Cards[1])}.{code(Cards[2])}.{code(Cards[3])}"; 
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
                trick.SetCard(i, new Card(src[i]));
            }
            trick.StartPlayer = src.StartPlayer;
            return trick;
        }
    }
}
