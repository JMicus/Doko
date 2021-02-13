using Doppelkopf.App;
using Doppelkopf.App.Enums;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Doppelkopf.Hubs
{
    public class DokoHub : Hub
    {
        public static List<Game> Games = new List<Game>();

        private IHubContext<DokoHub> hubContext;

        public DokoHub() : base() {
            //Console.WriteLine(GetHttpContextExtensions.GetHttpContext(this.Context).s)

            if (true)
            {
                Game game = new Game()
                {
                    Name = "Doko"
                };
                game.OnMessagesChanged += Game_OnMessagesChanged;


                Games.Add(game);

                for (int i = 1; i <= 4; i++)
                {
                    game.Player[i].Init("Player " + i);
                    game.Player[i].Token = "0";
                }

                game.Deal();

                for (int i = 1; i <= 4; i++)
                {
                    var player = game.Player[i];

                    // put card
                    game.PutCard(i.ToString(), player.Cards.First().ToCode());

                    // messages
                    //_ = player.AddMessage("short");
                    //_ = player.AddMessage("this is a long text which needs at least two rows to be displayed");
                }

            }

            
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            foreach (var game in Games)
            {
                foreach (var player in game.Player)
                {
                    if (player.ConnectionIds.RemoveAll(cid => cid == Context.ConnectionId) > 0)
                    {
                        _ = sendPlayerJoined(game, player);
                    }
                }
            }

            return base.OnDisconnectedAsync(exception);
        }

        private void Game_OnMessagesChanged(Game game)
        {
            sendMessages(game).Wait();
        }


        // Client Methods //////////////////////
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("Test", user, message);
        }

        public async Task Init(string gameName, string playerNo, string playerName)
        {
            Game game = GetGame(gameName);

            // create game if not existing
            if (game == null)
            {
                game = new Game()
                {
                    Name = gameName
                };
                Games.Add(game);
                await sendInfo(game, $"Game created: {gameName}");
            }

            // find playerNo if not set
            int pNo = 0;
            var nos = new int[]{ 1, 2, 3, 4 };
            var rand = new Random();
            nos = nos.OrderBy(x => rand.Next()).ToArray();
            while (playerNo == "" && pNo < 4)
            {
                if (string.IsNullOrEmpty(game.Player[nos[pNo]].Name))
                {
                    playerNo = nos[pNo].ToString();
                }
                if (game.Player[nos[pNo]].ConnectionIds.Count == 0)
                {
                    playerNo = nos[pNo].ToString();
                }
                pNo++;
            }
            if (playerNo == "")
            {
                playerNo = game.Player.OrderBy(x => x.InitDateTime).First().No.ToString();
            }

            // create player if not existing
            Player player = game.Player[playerNo];

            if (player.IsInitialized && player.ConnectionIds.Any())
            {
                await Clients.Caller.SendAsync("Initialized", game.Name, playerNo, "");
            }
            else
            {
                player.Init(playerName);
                await Clients.Caller.SendAsync("Initialized", game.Name, playerNo, player.Token);
            }
        }

        public async Task SayHello(string gameName, string playerNo, string playerToken)
        {

            Game game = GetGame(gameName);

            if (game == null)
            {
                await Clients.Caller.SendAsync("Info", $"Es existiert kein Spiel mit dem Namen \"{gameName}\"");
                return;
            }

            Player player = game.Player[playerNo];

            if (player.Token != playerToken)
            {
                await Clients.Caller.SendAsync("Unauthorized", gameName, playerNo, player.Name);
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, gameName);

            player.ConnectionIds.Add(Context.ConnectionId);

            await sendInfo(game, player.Name + " ist dem Spiel beigetreten.");

            await sendPlayerJoined(game, player);

            //await Clients.Group(gameName).SendAsync("PlayerJoined", playerNo, player.Name);

            await sendLayout(game);

            // current state to new client
            await sendPlayerJoined(game, null, player);
            
            if (!game.Trick.Empty)
            {
                await sendTrick(game, player);
            }
            if (!game.LastTrick.Empty)
            {
                await sendLastTrick(game, player);
            }
            if (player.Cards.Count > 0)
            {
                await sendHand(player);
            }
            if (!string.IsNullOrEmpty(game.ExternalPage))
            {
                await sendExternalPage(game, player);
            }
            await sendSymbols(game);

            if (game.Player.Where(p => p.Messages.Count() > 0).Count() > 0)
            {
                await sendMessages(game);
            }

            await sendRules(game, player);

            await sendCenter(game);

            await sendStats(game);

            //await sendLayout(game);
        }

        //public static async Task SayHelloStatic(string gameName, string playerNo, string playerName) { }

        public async Task Deal(string gameName, string playerNo, bool force = false)
        {
            Game game = GetGame(gameName);
            
            if (game.Deal(game.Player[playerNo], force))
            {
                await sendHand(game);
                await sendTrick(game);
                await sendLastTrick(game);
                await sendSymbols(game);
                await sendCenter(game, true);
                await sendStats(game);
            }
            else
            {
                await Clients.Caller.SendAsync("DealQuestion");
            }

            
        }

        public async Task PutCard(string gameName, string playerNo, string card)
        {
            var game = GetGame(gameName);

            try
            {
                // try take trick first
                //bool deleyTrickUpdate = false;
                if (game.Trick.Complete)
                {
                    await TakeTrick(gameName, playerNo);
                    //deleyTrickUpdate = true;
                }

                // put card
                if (game.PutCard(playerNo, card))
                {
                    var player = game.Player[playerNo];

                    await sendHand(player);
                    await sendTrick(game);


                    //if (deleyTrickUpdate)
                    //{
                    //    Thread.Sleep(1000); 
                    //}

                    //await sendTrick(game, player, true);
                }
            }
            catch (DokoException e)
            {
                await sendInfo(game, $"Player {playerNo} put card {card} -> ERROR\n{e.Message}");
            }
        }

        public async Task TakeCardBack(string gameName, string playerNo)
        {
            var game = GetGame(gameName);

            try
            {
                var player = game.Player[playerNo];
                
                if (game.TakeCardBack(player))
                {
                    await sendHand(player);
                    await sendTrick(game);
                    //sendInfo(gameName, $"{game.Trick.StartPlayer.N} - {game.Trick.ToCode()}");

                }
            }
            catch (DokoException e)
            {
                await sendInfo(game, $"Player {playerNo} takes card back -> ERROR\n{e.Message}");
            }
        }

        public async Task TakeTrick(string gameName, string player)
        {
            await takeTrick(gameName, player);
        }

        private async Task takeTrick(string gameName, string player, bool noUpdateForPlayer = false)
        {
            var game = GetGame(gameName);

            try
            {
                if (game.TakeTrick(player))
                {
                    if (noUpdateForPlayer)
                    {
                        await sendTrick(game, game.Player[player], true);
                    }
                    else
                    {
                        await sendTrick(game); 
                    }
                    await sendLastTrick(game);
                    await sendInfo(game, $"{game.Player[player].Name} nimmt den Stich.");
                    await sendSymbols(game);
                }
                else
                {
                    await sendInfo(game, "Der Stich ist nicht vollständig", game.Player[player]);
                }
            }
            catch (Exception e)
            {
                await sendInfo(game, $"{game.Player[player].Name} takes the trick -> ERROR\n{e.Message}");
            }

            // game ends
            if (game.Player.Count(x => x.Cards.Count > 0) == 0 && game.Trick.Empty)
            {
                await sendPoints(game);
            }
        }

        public async Task ChangeCardOrder(string gameName, string orderName)
        {
            var game = GetGame(gameName);

            try
            {
                game.Rules.Order = CardOrder.OrderByName(orderName);
                game.SortHandCards();
                await sendHand(game);
            }
            catch (Exception ex)
            {
                await sendInfo(game, $"CardOrder: {orderName} -> ERROR\n{ex.Message}");
            }
        }

        public async Task SetExternalPage(string gameName, string link)
        {
            var game = GetGame(gameName);
            game.ExternalPage = link;

            await sendExternalPage(game);
        }

        public async Task LastTrickBack(string gameName)
        {
            var game = GetGame(gameName);

            if (game.LastTrickBack())
            {
                await sendTrick(game);
                await sendLastTrick(game);
                await sendSymbols(game);
            }
        }

        public async Task GiveCardToPlayer(string gameName, string playerNo, string cardCode, string targetPlayerNo)
        {
            var game = GetGame(gameName);

            var player = game.Player[playerNo];
            var targetPlayer = game.Player[targetPlayerNo];
            var card = player.Cards.Where(x => x.ToCode() == cardCode).FirstOrDefault();
            player.Cards.Remove(card);
            targetPlayer.Cards.Add(card);
            game.SortHandCards();

            await sendHand(player);
            await sendHand(targetPlayer);
        }

        public async Task PlayerMsg(string gameName, string playerNo, string msg)
        {
            var game = GetGame(gameName);
            var player = game.Player[playerNo];


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

                        game.Rules.Layout["cardLayout"] = "HP";
                        game.Rules.Layout["cardImageType"] = "gif";
                        game.Rules.Layout["cardHeight"] = "" + height;
                        game.Rules.Layout["cardWidth"] = "" + (int)(height * 0.605);
                        game.Rules.Layout["cardBorder"] = "true";
                        game.Rules.Layout["background"] = "pergament.jpg";
                        refreshLayout = true;
                        break;

                    case "default":
                        game.Rules.ResetLayout();
                        refreshLayout = true;
                        break;

                    case "layout":
                        game.Rules.Layout[c[2]] = c[3];
                        refreshLayout = true;
                        break;

                    default:
                        break;

                }

                if (refreshLayout)
                {
                    await sendLayout(game);
                    await sendHand(game);
                    await sendTrick(game);
                    await sendLastTrick(game);
                }

                return;
            }


            _ = player.AddMessage(msg);

            await sendMessages(game);
        }

        public async Task SetRules(string gameName, string rulesCode)
        {
            var game = GetGame(gameName);

            game.Rules.FromCode(rulesCode);

            await sendRules(game);
        }

        public async Task AddSymbol(string gameName, string playerNo, string symbol, string hint)
        {
            var game = GetGame(gameName);
            var player = game.Player[playerNo];

            if (symbol == "clear")
            {
                player.Symbols.RemoveAll(s => s.Item1 != "dealSymbol" && !s.Item1.StartsWith("deckCount"));
            }
            else
            {
                player.Symbols.Add((symbol, hint));
            }

            await sendSymbols(game);
        }

        public async Task CardToCenter(string gameName, string playerNo, string card)
        {
            var game = GetGame(gameName);

            game.CardToCenter(playerNo, card);

            await sendHand(game.Player[playerNo]);
            await sendCenter(game);
        }

        public async Task CardFromCenter(string gameName, string playerNo, string card)
        {
            var game = GetGame(gameName);

            game.CardFromCenter(playerNo, card);

            await sendHand(game.Player[playerNo]);
            await sendCenter(game, true);
        }

        // SEND HELPER
        private async Task sendPlayerJoined(Game game, Player player = null, Player receivingPlayer = null)
        {
            var playerToSend = new List<Player>();

            if (player == null)
            {
                playerToSend.AddRange(game.Player.ToList());
            }
            else
            {
                playerToSend.Add(player);
            }

            foreach (var p in playerToSend)
            {
                if (receivingPlayer == null)
                {
                    await sendToAll(game, "PlayerJoined", p.No, p.NameLabel);
                }
                else
                {
                    await sendToPlayer(receivingPlayer, "PlayerJoined", p.No, p.NameLabel);
                }
            }
        }

        private async Task sendTrick(Game game, Player player = null, bool sendToAllExceptPlayer = false)
        {
            if (player == null)
            {
                await sendToAll(game, "Trick", game.Trick.StartPlayer?.No ?? 1, game.Trick.ToCode());
            }
            else if (sendToAllExceptPlayer == false)
            {
                await sendToPlayer(player, "Trick", game.Trick.StartPlayer?.No ?? 1, game.Trick.ToCode());
            }
            else
            {
                foreach (var p in game.Player.AllExcept(player))
                {
                    await sendToPlayer(p, "Trick", game.Trick.StartPlayer?.No ?? 1, game.Trick.ToCode());
                }
            }
            await sendMessages(game);
        }

        private async Task sendLastTrick(Game game, Player player = null)
        {
            if (player == null)
            {
                await sendToAll(game, "LastTrick", game.LastTrick.StartPlayer?.No ?? 1, game.LastTrick.ToCode());
            }
            else
            {
                await sendToPlayer(player, "LastTrick", game.LastTrick.StartPlayer?.No ?? 1, game.LastTrick.ToCode());
            }
        }

        private async Task sendHand(Player player)
        {
            await sendToPlayer(player, "Hand", player.GetHandMsg());
        }

        private async Task sendHand(Game game, Player exceptToPlayer = null)
        {
            foreach (var player in game.Player.AllExcept(exceptToPlayer))
            {
                await sendToPlayer(player, "Hand", player.GetHandMsg());
            }
            /*for (int i = 1; i <= 4; i++)
            {
                var player = game.Player[i];

                if (exceptToPlayer == null || exceptToPlayer != player)
                {
                    
                }
            }*/
        }

        private async Task sendExternalPage(Game game, Player player = null)
        {
            if (player == null)
            {
                await sendToAll(game, "ExternalPage", game.ExternalPage);
            }
            else
            {
                await sendToPlayer(player, "ExternalPage", game.ExternalPage);
            }
        }

        private async Task sendSymbols(Game game)
        {
            await sendToAll(game, "Symbols", string.Join("###", game.Player.Select(p => string.Join("---", p.Symbols.Select(pair => pair.Item1 + "+" + pair.Item2)))));
        }

        private async Task sendMessages(Game game)
        {
            await sendToAll(game, "Messages", string.Join("###", game.Player.Select(p => string.Join("---", p.Messages))));
        }

        private async Task sendRules(Game game, Player player = null)
        {
            if (player == null)
            {
                await sendToAll(game, "Rules", game.Rules.ToCode());
            }
            else
            {
                await sendToPlayer(player, "Rules", game.Rules.ToCode());
            }
        }

        private async Task sendPoints(Game game)
        {
            await sendToAll(game, "Points", string.Join("###", game.Player.Select(p => p.Name + "---" + p.WonPoints)));
        }

        private async Task sendCenter(Game game, bool force = false)
        {
            if (force || game.Center.Count > 0)
            {
                await sendToAll(game, "Center", game.Center.ToCode());
            }
        }

        private async Task sendStats(Game game)
        {
            await sendToAll(game, "Stat", game.Stats());
        }

        private async Task sendLayout(Game game, string layoutName = null)
        {
            if (layoutName != null)
            {
                await sendToAll(game, "Layout", layoutName + ":" + game.Rules.Layout[layoutName]);
            }
            else
            {
                await sendToAll(game, "Layout", string.Join(".", game.Rules.Layout.Select(pair => pair.Key + ":" + pair.Value)));
            }
        }

        // HELPER //////////////////////////////////////////
        private async Task sendInfo(Game game, string info, Player player = null)
        {
            if (player == null)
            {
                //await Clients.Group(game).SendAsync("Info", info);
                await sendToAll(game, "Info", info);
            }
            else
            {
                await sendToPlayer(player, "info", info);
            }
        }

        public static Game GetGame(string gameName)
        {
            return Games.Where(x => x.Name == gameName).FirstOrDefault();
        }

        private async Task sendToPlayer(Player player, string method, int msg1, string msg2)
        {
            await sendToPlayer(player, method, msg1.ToString(), msg2);
        }

        private async Task sendToAll(Game game, string method, object o1, object o2 = null)
        {
            foreach (var player in game.Player)
            {
                await sendToPlayer(player, method, o1, o2);
            }
        }

        private async Task sendToPlayer(Player player, string method, object o1, object o2 = null)
        {
            foreach (var connectionId in player.ConnectionIds)
            {
                if (o2 == null)
                {
                    await Clients.Client(connectionId).SendAsync(method, o1);
                }
                else
                {
                    await Clients.Client(connectionId).SendAsync(method, o1, o2);
                }
            }
        }

    }
}
