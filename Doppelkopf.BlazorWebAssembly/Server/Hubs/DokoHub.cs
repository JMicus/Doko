using C = Doppelkopf.Core.App;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Doppelkopf.Core;
using Newtonsoft.Json;
using Doppelkopf.Core.Connection;

namespace Doppelkopf.BlazorWebAssembly.Server.Hubs
{
    public class DokoHub : AbstractHubBase
    {
        public static string testgame = "Doko";

         

        public DokoHub() : base() {
            //Console.WriteLine(GetHttpContextExtensions.GetHttpContext(this.Context).s)

            if (testgame != null)
            {
                C.Game game = new C.Game()
                {
                    Name = testgame
                };
                game.OnMessagesChanged += Game_OnMessagesChanged;


                Games.Add(game);

                for (int i = 1; i <= 4; i++)
                {
                    game.Player[i].Init("Player " + i);
                    game.Player[i].Token = "0";
                }

                game.Deal();

                //for (int i = 1; i <= 4; i++)
                //{
                //    var player = game.Player[i];

                //    // put card
                //    game.PutCard(player, player.Cards.First());

                //    // messages
                //    //_ = player.AddMessage("my name is " + player.Name);
                //    //_ = player.AddMessage("this is a long text which needs at least two rows to be displayed");
                //}

            }

            
        }

        protected override async Task Debug(C.Game game, C.Player player, string tag)
        {
            await GiveCardsToPlayer(game, game.Player[2], 1, game.Player[2].Cards.Take(3).ToList(), true);
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine("HUB DISCONNECT " + (exception?.Message ?? ""));
            foreach (var game in Games)
            {
                foreach (var player in game.Player)
                {
                    if (player.ConnectionIds.RemoveAll(cid => cid == Context.ConnectionId) > 0)
                    {
                        Console.WriteLine("HUB DISCONNECT " + player.Name);
                        _ = SendPlayerJoined(game, player.No, player.Name);
                    }
                }
            }

            return base.OnDisconnectedAsync(exception);
        }

        private void Game_OnMessagesChanged(C.Game game)
        {
            sendMessages(game).Wait();
        }


        // Client Methods //////////////////////

        protected override async Task Init(string gameName, int playerNo, string playerName)
        {
            Console.WriteLine($"HUB Init {playerNo} {playerName}");
            var game = getGame(gameName);

            // create game if not existing
            if (game == null)
            {
                game = new C.Game()
                {
                    Name = gameName
                };
                Games.Add(game);
                await SendInfo(game, $"Game created: {gameName}");
            }

            // find playerNo if not set
            int pNo = 0;
            var nos = new int[]{ 1, 2, 3, 4 };
            var rand = new Random();
            nos = nos.OrderBy(x => rand.Next()).ToArray();
            while (playerNo == 0 && pNo < 4)
            {
                if (string.IsNullOrEmpty(game.Player[nos[pNo]].Name))
                {
                    playerNo = nos[pNo];
                }
                if (game.Player[nos[pNo]].ConnectionIds.Count == 0)
                {
                    playerNo = nos[pNo];
                }
                pNo++;
            }
            if (playerNo == 0)
            {
                playerNo = game.Player.OrderBy(x => x.InitDateTime).First().No;
            }

            // create player if not existing
            C.Player player = game.Player[playerNo];

            if (player.IsInitialized && player.ConnectionIds.Any())
            {
                await SendInitialized(game, game.Name, playerNo, "");
            }
            else
            {
                player.Init(playerName);
                await SendInitializedToCaller(game.Name, playerNo, player.Token);
            }
        }

        protected override async Task SayHello(C.Game game, C.Player player, string playerToken)
        {
            Console.WriteLine($"HUB SayHello {player?.No}");

            if (game == null)
            {
                await SendInfoToCaller("Es existiert kein solches Spiel.");
                return;
            }

            if (player.Token != playerToken)
            {
                await SendUnauthorizedToCaller(game.Name, player.No, player.Name);
                return;
            }

            //await Groups.AddToGroupAsync(Context.ConnectionId, gameName);

            player.ConnectionIds.Add(Context.ConnectionId);

            Console.WriteLine("HUB SayHello conns: " + string.Join(", ", game.Player.Select(p => p.No + " " + string.Join(" ", p.ConnectionIds))));

            await SendInfo(game, player.Name + " ist dem Spiel beigetreten.");

            await SendPlayerJoined(game, player.No, player.Name);

            foreach (var p in game.Player)
            {
                await SendPlayerJoined(player, p.No, p.Name);
            }
            //await Clients.Group(gameName).SendAsync("PlayerJoined", playerNo, player.Name);

            await SendLayout(player, game.Layout);

            // current state to new client
            
            if (!game.Trick.Empty)
            {
                await SendTrick(player, game.Trick);
            }
            if (!game.LastTrick.Empty)
            {
                await SendLastTrick(player, game.Trick);
            }
            if (player.Cards.Count > 0)
            {
                await SendHand(player, player.Cards);
            }
            if (!string.IsNullOrEmpty(game.ExternalPage))
            {
                await SendExternalPage(player, game.ExternalPage);
            }
            await sendSymbols(game);

            if (game.Player.Where(p => p.Messages.Count() > 0).Count() > 0)
            {
                await sendMessages(game);
            }

            await SendRules(player, game.Rules);

           //TODO await SendStatistics(player, game.Stats);

            //await sendLayout(game);
        }

        //public static async Task SayHelloStatic(string gameName, string playerNo, string playerName) { }

        protected override async Task Deal(C.Game game, C.Player player, bool force = false)
        {
            if (game.Deal(player, force))
            {
                await sendHandToAll(game);
                await SendTrick(game, game.Trick);
                await SendLastTrick(game, game.Trick);
                await sendSymbols(game);
                // TODO await sendStats(game);
            }
            else
            {
                await SendDealQuestion(player);
            }

            
        }

        protected override async Task PutCard(C.Game game, C.Player player, C.Card card)
        {
            try
            {
                // try take trick first
                //bool deleyTrickUpdate = false;
                if (game.Trick.Complete)
                {
                    game.TakeTrick(player);
                    //deleyTrickUpdate = true;
                }

                // put card
                if (game.PutCard(player, card))
                {
                    if (false && testgame == game.Name)
                    {
                        foreach (var p in game.Player.Where(p => p.No != player.No))
                        {
                            game.PutCard(p, p.Cards.FirstOrDefault());
                        }
                    }

                    await SendTrick(game, game.Trick);
                    await SendHand(player, player.Cards);



                    //if (deleyTrickUpdate)
                    //{
                    //    Thread.Sleep(1000); 
                    //}

                    //await sendTrick(game, player, true);
                }
            }
            catch (DokoException e)
            {
                await SendInfo(game, $"Player {player?.No} put card {card} -> ERROR\n{e.Message}");
            }
        }

        protected override async Task TakeCardBack(C.Game game, C.Player player)
        {
            try
            {
                if (game.TakeCardBack(player))
                {
                    await SendHand(player, player.Cards);
                    await SendTrick(game, game.Trick);
                    //sendInfo(gameName, $"{game.Trick.StartPlayer.N} - {game.Trick.ToCode()}");

                }
            }
            catch (DokoException e)
            {
                await SendInfo(game, $"Player {player?.No} takes card back -> ERROR\n{e.Message}");
            }
        }

        protected override async Task TakeTrick(C.Game game, C.Player player)
        {
            try
            {
                if (game.TakeTrick(player))
                {
                    await SendTrick(player, game.Trick);
                    await SendLastTrick(player, game.LastTrick);
                    await sendSymbols(game);
                    await SendInfo(game, $"{player.Name} nimmt den Stich.");
                }
                else
                {
                    await SendInfo(player, "Der Stich ist nicht vollständig");
                }
            }
            catch (Exception e)
            {
                await SendInfo(game, $"{player?.Name} takes the trick -> ERROR\n{e.Message}");
            }

            // game ends
            if (game.Player.Count(x => x.Cards.Count > 0) == 0 && game.Trick.Empty)
            {
                await SendPoints(game, new C.Points(game.Player));
            }
        }

        protected override async Task ChangeCardOrder(C.Game game, C.Player player, C.Enums.EGameType eGameType)
        {
            try
            {
                game.Rules.Order = C.Enums.CardOrder.OrderByGameType(eGameType);
                game.SortHandCards();
                await SendHand(game, player.Cards);
            }
            catch (Exception ex)
            {
                await SendInfo(game, $"CardOrder: {eGameType} -> ERROR\n{ex.Message}");
            }
        }

        protected override async Task SetExternalPage(C.Game game, C.Player player, string url)
        {
            game.ExternalPage = url;
            await SendExternalPage(game, url);
        }

        protected override async Task LastTrickBack(C.Game game, C.Player player)
        {
            if (game.LastTrickBack())
            {
                await SendTrick(game, game.Trick);
                await SendLastTrick(game, game.LastTrick);
                await sendSymbols(game);
            }
        }


        protected override async Task GiveCardsToPlayer(C.Game game, C.Player player, int receivingPlayerNo, List<C.Card> cards, bool cardsBack)
        {
            var toPlayer = game.Player[receivingPlayerNo];

            foreach (var cardx in cards)
            {
                var card = player.Cards.Where(x => x.ToCode() == cardx.ToCode()).FirstOrDefault();
                player.Cards.Remove(card);
                toPlayer.Cards.Add(card);
            }

            game.SortHandCards();

            await SendHand(player, player.Cards);
            await SendHand(toPlayer, toPlayer.Cards);
            await SendCardsFromPlayer(toPlayer, player, cards, cardsBack);
        }

        protected override async Task PlayerMsg(C.Game game, C.Player player, string msg)
        {
            // command
            if (msg.StartsWith("cmd."))
            {
                var c = msg.Split(".");
                var refreshLayout = false;

                switch (c[1])
                {
                    case "hp":
                        //await PlayerMsg(gameName, playerNo, "cmd.cards.HP.200.gif");
                        var height = 240;

                        game.Layout["cardLayout"] = "HP";
                        game.Layout["cardImageType"] = "gif";
                        game.Layout["cardHeight"] = "" + height;
                        game.Layout["cardWidth"] = "" + (int)(height * 0.605);
                        game.Layout["cardBorder"] = "true";
                        game.Layout["background"] = "pergament.jpg";
                        refreshLayout = true;
                        break;

                    case "default":
                        game.Layout.ResetLayout();
                        refreshLayout = true;
                        break;

                    case "layout":
                        game.Layout[c[2]] = c[3];
                        refreshLayout = true;
                        break;

                    default:
                        break;

                }

                if (refreshLayout)
                {
                    await SendLayout(game, game.Layout);
                    await SendHand(game, player.Cards);
                    await SendTrick(player, game.Trick);
                    await SendLastTrick(player, game.LastTrick);
                }

                return;
            }


            _ = player.AddMessage(msg);

            await sendMessages(game);
        }

        protected override async Task SetRules(C.Game game, C.Player player, C.Rules rules)
        {
            game.Rules = rules;

            await SendRules(game, game.Rules);
        }

        protected override async Task AddSymbol(C.Game game, C.Player player, int playerOfSymbolNo, C.Enums.Symbol symbol)
        {
            var symbolPlayer = game.Player[playerOfSymbolNo];

            if (false) // TODO
            {
                //player.Symbols.RemoveAll(s => s.Item1 != "dealSymbol" && !s.Item1.StartsWith("deckCount"));
            }
            else
            {
                symbolPlayer.Symbols.Add(symbol);
            }

            await sendSymbols(game);
        }

        // SEND HELPER

        private async Task sendHandToAll(C.Game game)
        {
            foreach (var player in game.Player)
            {
                await SendHand(player, player.Cards);
            }
        }

        private async Task sendSymbols(C.Game game)
        {
            await SendSymbols(game, game.Player.Select(p => p.Symbols).ToList());
        }

        private async Task sendMessages(C.Game game)
        {
            await SendMessages(game, game.Player.Select(p => p.Messages).ToList()); //(  string.Join("###", game.Player.Select(p => string.Join("---", p.Messages))));
        }
    }
}
