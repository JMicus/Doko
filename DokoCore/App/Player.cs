using Doppelkopf.Core.App.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using static Doppelkopf.Core.App.Enums.Symbol;

namespace Doppelkopf.Core.App
{
    public class Player
    {
        [JsonIgnore]
        public DateTime InitDateTime { get; set; }

        public string Name;

        public int No;

        [JsonIgnore]
        public string Token { get; set; }

        [JsonIgnore]
        public HashSet<string> ConnectionIds { get; set; } = new HashSet<string>();

        [JsonIgnore]
        public List<Card> Cards { get; set; } = new List<Card>();

        [JsonIgnore]
        private List<Card> wonCards { get; set; } = new List<Card>();

        [JsonIgnore]
        public List<Symbol> Symbols { get; set; } = new List<Symbol>();

        [JsonIgnore]
        public List<string> Messages { get; set; } = new List<string>();

        [JsonIgnore]
        public int WonPoints => rules.CountPoints(wonCards);

        [JsonIgnore]
        public string NameShort => (Name.Count() < 2 ? Name : Name.Substring(0, 2)).ToUpper();

        public string NameLabel
        {
            get
            {
                return ConnectionIds.Count > 0 ? Name : "-"; 
            }
        }

        [JsonIgnore]
        public bool IsInitialized => !string.IsNullOrEmpty(Token);

        private Rules rules;

        //public Card CenterCard;


        public Player(Rules rules, int no)
        {
            this.rules = rules;
            No = no;
        }

        public void Init(string name)
        {
            Name = name;
            Token = new Random().Next(1000, 10000).ToString();
            InitDateTime = DateTime.Now;
        }

        public void AddCard(Card card)
        {
            Cards.Add(card);
        }

        public void ClearHand()
        {
            Cards.Clear();
        }

        public string GetHandMsg()
        {
            return string.Join(".", Cards.Select(x => x.ToCode()));
        }

        public void SetHandByMsg(string handMsg)
        {
            Cards.Clear();
            Cards.AddRange(handMsg.Split('.').Select(code => new Card(code)).ToList());
        }

        public void SetHand(List<Card> cards)
        {
            Cards.Clear();
            Cards.AddRange(cards);
        }

        public Card PutCard(Card card)
        {
            foreach (var c in Cards)
            {
                if (c.ToCode() == card.ToCode())
                {
                    Cards.Remove(c);
                    //CenterCard = c;
                    return c;
                }
            }
            return null;
        }

        public async Task AddMessage(string msg, bool addPlayerName = false)
        {
            var msgPlus = (addPlayerName ? (NameShort + ": ") : "") + msg;
            Messages.Add(msgPlus);
            await Task.Delay(10000);
            Messages.Remove(msgPlus);
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void AddWonCards(List<Card> cards)
        {
            wonCards.AddRange(cards);
            updateTricksSymbol();
        }

        public void RemoveLastWonCards(int no = -1)
        {
            if (no == -1)
            {
                wonCards.Clear();
            }
            else
            {
                wonCards.RemoveRange(wonCards.Count() - no, no);
            }
            updateTricksSymbol();
        }

        public int WonCardsCount()
        {
            return wonCards.Count();
        }

        private void updateTricksSymbol()
        {
            var toRemove = Symbols.Where(s => s.Type == ESymbol.trickCount).FirstOrDefault();

            if (toRemove != null)
            {
                Symbols.Remove(toRemove);

            }

            var tricks = wonCards.Count() / 4;
            if (tricks > 0)
            {
                Symbols.Add(new Symbol(ESymbol.trickCount, tricks));
            }
        }
    }
}
