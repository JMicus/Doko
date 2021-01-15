using Doppelkopf.App.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Doppelkopf.App
{
    public class Player
    {
        public DateTime InitDateTime;

        public string Name;

        public int No;

        public string Token;

        public List<string> ConnectionIds = new List<string>();

        public List<Card> Cards = new List<Card>();

        private List<Card> wonCards = new List<Card>();

        public List<(string, string)> Symbols = new List<(string, string)>();

        public List<string> Messages = new List<string>();

        public int WonPoints => rules.CountPoints(wonCards);

        public string NameShort => (Name.Count() < 2 ? Name : Name.Substring(0, 2)).ToUpper();

        public bool IsInitialized => !string.IsNullOrEmpty(Token);

        private Rules rules;

        //public Card CenterCard;

        public event Action OnMessagesChanged;

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

        public Card PutCard(string cardCode)
        {
            foreach (var c in Cards)
            {
                if (c.ToCode() == cardCode)
                {
                    Cards.Remove(c);
                    //CenterCard = c;
                    return c;
                }
            }
            return null;
        }

        public void SetCardsFromMsg(string msg)
        {
            ClearHand();

            foreach (var c in msg.Split('.'))
            {
                AddCard(new Card((ECard)Enum.Parse(typeof(ECard), c)));
            }
        }

        public async Task AddMessage(string msg, bool addPlayerName = true)
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
            var toRemove = Symbols.Where(s => s.Item1.Contains("deckCount")).FirstOrDefault();

            if (toRemove != default((string, string)))
            {
                Symbols.Remove(toRemove);

            }

            var tricks = wonCards.Count() / 4;
            if (tricks > 0)
            {
                Symbols.Add(("deckCount" + tricks + "Symbol", tricks + " Striche"));
            }
        }
    }
}
