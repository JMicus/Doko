using DokoCore.Core.App.Enums;
using Doppelkopf.Core.App.Config;
using Doppelkopf.Core.App.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Doppelkopf.Core.App.Enums.Symbol;

namespace Doppelkopf.Core.App
{
    public class Game
    {
        public string Name;

        public CardsHandler CardsHandler = new CardsHandler();
        public Config.DokoSettings Settings = new Config.DokoSettings();
        public History History = new History();

        public Trick Trick = new Trick();
        public Trick LastTrick = new Trick();

        public CardList Center = new CardList();

        private Random rnd = new Random();

        public string ExternalPage = "";

        public PlayerHolder Player;

        public event Action<Game> OnMessagesChanged;

        public Game()
        {
            Player = new PlayerHolder();
        }


        public bool Deal(Player player = null, bool force = false, int? testGameCardsPerPlayer = null)
        {
            var state = GameState();

            if (!force && state != EGameState.Cleared && state != EGameState.Finished)
            {
                return false;
            }

            Trick.Clear();
            LastTrick.Clear();
            Center.Clear();

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

            var deck = CardsHandler.CreateDeck(Settings.Rules.Value);

            deck = deck.OrderBy(x => rnd.NextDouble()).ToList();

            if (testGameCardsPerPlayer.HasValue)
            {
                deck = deck.GetRange(0, testGameCardsPerPlayer.Value * 4);
            }

            for (int i = 0; i<deck.Count; i++)
            {
                Player[i % 4 + 1].AddCard(deck[i]);
            }

            CardsHandler.Order = CardOrder.Regular;
            SortHandCards();

            History.Add(this);

            // symbols
            clearSymbols();

            if (player != null)
            {
                History.DealCount[player.No]++;
                player.Symbols.Add(new Symbol(ESymbol.dealSymbol));
            }

            return true;
        }

        public void SortHandCards(bool detectPigs = true)
        {
            for (int i = 1; i <= 4; i++)
            {
                CardsHandler.SortCards(Player[i].Cards, Settings.Rules.Value, detectPigs);
            }
        }

        public bool PutCard(Player player, Card card)
        {
            if (Trick[player].Name.HasValue)
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
            if (!Trick[player].Name.HasValue)
            {
                return false;
            }

            player.AddCard(Trick.TakeCard(player));

            SortHandCards(false);

            return true;
        }

        public bool TakeTrick(Player player)
        {
            if (!Trick.Complete)
            {
                return false;
            }

            player.AddWonCards(Trick.ToList());

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

        //public void CardToCenter(string playerNo, string card)
        //{
        //    var player = Player[playerNo];
        //    Center.Add(player.PutCard(card));
        //}

        public void CardFromCenter(string playerNo, string card)
        {
            var player = Player[playerNo];
            player.AddCard(Center.Remove(card));
            SortHandCards();
        }

        private void clearSymbols()
        {
            foreach (var p in Player)
            {
                p.Symbols.Clear();
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
            var s = History.Stats(Settings.Rules.Value);

            for (int i = 1; i <= 4; i++)
            {
                s = s.Replace("##P" + i, Player[i].Name);
            }

            return s;
        }
    }
}
