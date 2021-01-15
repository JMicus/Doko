using DokoCore.App;
using DokoCore.App.Enums;
using Doppelkopf.App.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doppelkopf.App
{
    public class Game
    {
        public string Name;

        public Rules Rules = new Rules();
        public History History = new History();

        public Trick Trick = new Trick();
        public Trick LastTrick = new Trick();

        public CardList Center = new CardList();

        private Random rnd = new Random();

        public string ExternalPage = "";

        public class PlayerHolder : IEnumerable<Player>
        {
            private App.Player[] _player = new App.Player[4];

            public PlayerHolder(Rules rules)
            {
                for (int i = 1; i <= 4; i++)
                {
                    this[i] = new Player(rules, i);
                }
            }

            private int s2i(string playerNo)
            {
                return Convert.ToInt32(playerNo) - 1;
            }

            public IEnumerator<Player> GetEnumerator()
            {
                for (int i = 0; i < 4; i++)
                {
                    yield return _player[i];
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public Player this[string playerNo]
            {
                get
                {
                    return _player[s2i(playerNo)];
                }
                set
                {
                    _player[s2i(playerNo)] = value;
                }
            }

            public Player this[int playerNo]
            {
                get
                {
                    return _player[playerNo - 1];
                }
                set
                {
                    _player[playerNo - 1] = value;
                }
            }

            public List<Player> AllExcept(Player player)
            {
                return _player.Where(p => player == null || p != player).ToList();
            }
        }

        public PlayerHolder Player;

        public event Action<Game> OnMessagesChanged;

        public Game()
        {
            Player = new PlayerHolder(Rules);
            foreach (var p in Player)
            {
                p.OnMessagesChanged += () => OnMessagesChanged(this);
            }
        }


        public bool Deal(Player player = null, bool force = false)
        {
            var state = GameState();

            if (!force && state != EGameState.Cleared && state != EGameState.Finished)
            {
                return false;
            }

            Trick.Clear();
            LastTrick.Clear();
            Center.Clear();

            var deck = Rules.Deck;

            for (int i = 1; i <= 4; i++)
            {
                // show last hand cards
                if (Player[i].Cards.Count() > 0)
                {
                    _ = Player[i].AddMessage("<!--nobackground--> <nobr>" + string.Join("", Player[i].Cards.Select(c => " <img style=\"height: 70px; margin-right: -31px\" src =\"Images/CardsBasic/" + c.NameCode + ".png\"/>")) + "</nobr>", false);
                }

                Player[i].ClearHand();
                Player[i].RemoveLastWonCards();
            }

            deck = deck.OrderBy(x => rnd.NextDouble()).ToList();

            for (int i = 0; i<deck.Count; i++)
            {
                Player[i % 4 + 1].AddCard(deck[i]);
            }

            Rules.Order = CardOrder.Regular;
            SortHandCards();

            History.Add(this);

            // symbols
            clearSymbols();

            if (player != null)
            {
                History.DealCount[player.No]++;
                player.Symbols.Add(("dealSymbol", player.Name + " hat als letztes gegeben"));
            }

            return true;
        }

        public void SortHandCards(bool detectPigs = true)
        {
            for (int i = 1; i <= 4; i++)
            {
                Rules.SortCards(Player[i].Cards, detectPigs);
            }
        }

        public bool PutCard(string playerNo, string card)
        {
            var player = Player[playerNo];
            
            if (Trick.GetCard(player) != null)
            {
                return false;
            }

            if (Trick.Complete) // TODO not necessary ?
            {
                return false;
            }

            if (Trick.Empty)
            {
                Trick.StartPlayer = player;
            }
            
            Trick.SetCard(player, player.PutCard(card));

            return true;
        }

        public bool TakeCardBack(Player player)
        {
            if (Trick.GetCard(player) == null)
            {
                return false;
            }

            player.AddCard(Trick.TakeCard(player));

            SortHandCards(false);

            return true;
        }

        public bool TakeTrick(string playerNo)
        {
            var player = Player[playerNo];

            if (!Trick.Complete)
            {
                return false;
            }

            player.AddWonCards(Trick.Cards.ToList());

            LastTrick = Trick.CopyFrom(Trick);
            LastTrick.WonPlayer = player;

            Trick.Clear();

            return true;
        }

        public bool LastTrickBack()
        {
            if (LastTrick.Empty || !Trick.Empty)
            {
                return false;
            }

            LastTrick.WonPlayer.RemoveLastWonCards(4);

            Trick = App.Trick.CopyFrom(LastTrick);
            LastTrick.Clear();

            return true;
        }

        public void CardToCenter(string playerNo, string card)
        {
            var player = Player[playerNo];
            Center.Add(player.PutCard(card));
        }

        public void CardFromCenter(string playerNo, string card)
        {
            var player = Player[playerNo];
            player.AddCard(Center.Remove(card));
            SortHandCards();
        }

        private void clearSymbols(string symbol = "")
        {
            foreach (var p in Player)
            {
                if (symbol != "")
                {
                    p.Symbols.RemoveAll(s => s.Item1 == symbol);
                }
                else
                {
                    p.Symbols.Clear();
                }
            }
        }

        public EGameState GameState()
        {
            bool wonCards = Player.Sum(p => p.WonCardsCount()) > 0;
            bool handCards = Player.Sum(p => p.Cards.Count()) > 0;


            if (Trick.Empty && !handCards && !wonCards)
            {
                return EGameState.Cleared;
            }

            if (Trick.Empty && !handCards && wonCards)
            {
                return EGameState.Finished;
            }

            if (Trick.Empty && !wonCards && handCards)
            {
                return EGameState.Dealed;
            }

            return EGameState.Playing;
        }

        public string Stats()
        {
            var s = History.Stats(Rules);

            for (int i = 1; i <= 4; i++)
            {
                s = s.Replace("##P" + i, Player[i].Name);
            }

            return s;
        }
    }
}
