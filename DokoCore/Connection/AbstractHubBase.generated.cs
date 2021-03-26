using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Doppelkopf.Core.App;
using Doppelkopf.Core.App.Enums;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace Doppelkopf.Core.Connection
{
    public abstract class AbstractHubBase : Hub
    {
        private static string debugOutputDir = @"C:\Users\acer\Documents\ProgrammeCode\Doppelkopf\debugOut";

        public static List<Game> Games = new List<Game>();

        
        protected abstract Task AddSymbol(Game game, Player player, int playerOfSymbol, Symbol symbolCT);

        public async Task AddSymbol_H(string gameName, string playerNo, string playerOfSymbol, string symbolCT)
        {
            logTransferObj("AddSymbol", "gameName", gameName);
            logTransferObj("AddSymbol", "playerNo", playerNo);
            logTransferObj("AddSymbol", "playerOfSymbol", playerOfSymbol);
            logTransferObj("AddSymbol", "symbolCT", symbolCT);
            var game = getGame(gameName);
            var player = game?.Player[playerNo];
            await AddSymbol(game, player, int.Parse(playerOfSymbol), JsonConvert.DeserializeObject<Symbol>(symbolCT));
        }

        protected abstract Task ChangeCardOrder(Game game, Player player, EGameType cardOrderE);

        public async Task ChangeCardOrder_H(string gameName, string playerNo, string cardOrderE)
        {
            logTransferObj("ChangeCardOrder", "gameName", gameName);
            logTransferObj("ChangeCardOrder", "playerNo", playerNo);
            logTransferObj("ChangeCardOrder", "cardOrderE", cardOrderE);
            var game = getGame(gameName);
            var player = game?.Player[playerNo];
            await ChangeCardOrder(game, player, Parsenum.S2E<EGameType>(cardOrderE));
        }

        protected abstract Task Deal(Game game, Player player, bool force);

        public async Task Deal_H(string gameName, string playerNo, string force)
        {
            logTransferObj("Deal", "gameName", gameName);
            logTransferObj("Deal", "playerNo", playerNo);
            logTransferObj("Deal", "force", force);
            var game = getGame(gameName);
            var player = game?.Player[playerNo];
            await Deal(game, player, bool.Parse(force));
        }

        protected abstract Task Debug(Game game, Player player, string tag);

        public async Task Debug_H(string gameName, string playerNo, string tag)
        {
            logTransferObj("Debug", "gameName", gameName);
            logTransferObj("Debug", "playerNo", playerNo);
            logTransferObj("Debug", "tag", tag);
            var game = getGame(gameName);
            var player = game?.Player[playerNo];
            await Debug(game, player, tag);
        }

        protected abstract Task GiveCardsToPlayer(Game game, Player player, int receivingPlayerNo, List<Card> cardsCT, bool cardsBack);

        public async Task GiveCardsToPlayer_H(string gameName, string playerNo, string receivingPlayerNo, string cardsCT, string cardsBack)
        {
            logTransferObj("GiveCardsToPlayer", "gameName", gameName);
            logTransferObj("GiveCardsToPlayer", "playerNo", playerNo);
            logTransferObj("GiveCardsToPlayer", "receivingPlayerNo", receivingPlayerNo);
            logTransferObj("GiveCardsToPlayer", "cardsCT", cardsCT);
            logTransferObj("GiveCardsToPlayer", "cardsBack", cardsBack);
            var game = getGame(gameName);
            var player = game?.Player[playerNo];
            await GiveCardsToPlayer(game, player, int.Parse(receivingPlayerNo), JsonConvert.DeserializeObject<List<Card>>(cardsCT), bool.Parse(cardsBack));
        }

        protected abstract Task Init(string newGameName, int myPlayerNo, string myPlayerName);

        public async Task Init_H(string newGameName, string myPlayerNo, string myPlayerName)
        {
            logTransferObj("Init", "newGameName", newGameName);
            logTransferObj("Init", "myPlayerNo", myPlayerNo);
            logTransferObj("Init", "myPlayerName", myPlayerName);
            await Init(newGameName, int.Parse(myPlayerNo), myPlayerName);
        }

        protected abstract Task LastTrickBack(Game game, Player player);

        public async Task LastTrickBack_H(string gameName, string playerNo)
        {
            logTransferObj("LastTrickBack", "gameName", gameName);
            logTransferObj("LastTrickBack", "playerNo", playerNo);
            var game = getGame(gameName);
            var player = game?.Player[playerNo];
            await LastTrickBack(game, player);
        }

        protected abstract Task PlayerMsg(Game game, Player player, string msg);

        public async Task PlayerMsg_H(string gameName, string playerNo, string msg)
        {
            logTransferObj("PlayerMsg", "gameName", gameName);
            logTransferObj("PlayerMsg", "playerNo", playerNo);
            logTransferObj("PlayerMsg", "msg", msg);
            var game = getGame(gameName);
            var player = game?.Player[playerNo];
            await PlayerMsg(game, player, msg);
        }

        protected abstract Task PutCard(Game game, Player player, Card cardCT);

        public async Task PutCard_H(string gameName, string playerNo, string cardCT)
        {
            logTransferObj("PutCard", "gameName", gameName);
            logTransferObj("PutCard", "playerNo", playerNo);
            logTransferObj("PutCard", "cardCT", cardCT);
            var game = getGame(gameName);
            var player = game?.Player[playerNo];
            await PutCard(game, player, JsonConvert.DeserializeObject<Card>(cardCT));
        }

        protected abstract Task SayHello(Game game, Player player, string playerToken);

        public async Task SayHello_H(string gameName, string playerNo, string playerToken)
        {
            logTransferObj("SayHello", "gameName", gameName);
            logTransferObj("SayHello", "playerNo", playerNo);
            logTransferObj("SayHello", "playerToken", playerToken);
            var game = getGame(gameName);
            var player = game?.Player[playerNo];
            await SayHello(game, player, playerToken);
        }

        protected abstract Task SetExternalPage(Game game, Player player, string url);

        public async Task SetExternalPage_H(string gameName, string playerNo, string url)
        {
            logTransferObj("SetExternalPage", "gameName", gameName);
            logTransferObj("SetExternalPage", "playerNo", playerNo);
            logTransferObj("SetExternalPage", "url", url);
            var game = getGame(gameName);
            var player = game?.Player[playerNo];
            await SetExternalPage(game, player, url);
        }

        protected abstract Task SetRules(Game game, Player player, Rules rulesCT);

        public async Task SetRules_H(string gameName, string playerNo, string rulesCT)
        {
            logTransferObj("SetRules", "gameName", gameName);
            logTransferObj("SetRules", "playerNo", playerNo);
            logTransferObj("SetRules", "rulesCT", rulesCT);
            var game = getGame(gameName);
            var player = game?.Player[playerNo];
            await SetRules(game, player, JsonConvert.DeserializeObject<Rules>(rulesCT));
        }

        protected abstract Task TakeCardBack(Game game, Player player);

        public async Task TakeCardBack_H(string gameName, string playerNo)
        {
            logTransferObj("TakeCardBack", "gameName", gameName);
            logTransferObj("TakeCardBack", "playerNo", playerNo);
            var game = getGame(gameName);
            var player = game?.Player[playerNo];
            await TakeCardBack(game, player);
        }

        protected abstract Task TakeTrick(Game game, Player player);

        public async Task TakeTrick_H(string gameName, string playerNo)
        {
            logTransferObj("TakeTrick", "gameName", gameName);
            logTransferObj("TakeTrick", "playerNo", playerNo);
            var game = getGame(gameName);
            var player = game?.Player[playerNo];
            await TakeTrick(game, player);
        }

        protected async Task SendCardsFromPlayer(Game game, Player playerCT, List<Card> cardsCT, bool cardsBack)
        {
            logTransferObj("CardsFromPlayer", "playerCT", JsonConvert.SerializeObject(playerCT, Formatting.Indented));
            logTransferObj("CardsFromPlayer", "cardsCT", JsonConvert.SerializeObject(cardsCT, Formatting.Indented));
            logTransferObj("CardsFromPlayer", "cardsBack", cardsBack.ToString());
            await sendToAll(game, "CardsFromPlayer", JsonConvert.SerializeObject(playerCT), JsonConvert.SerializeObject(cardsCT), cardsBack.ToString());
        }

        protected async Task SendCardsFromPlayer(Player player, Player playerCT, List<Card> cardsCT, bool cardsBack)
        {
            logTransferObj("CardsFromPlayer", "playerCT", JsonConvert.SerializeObject(playerCT, Formatting.Indented));
            logTransferObj("CardsFromPlayer", "cardsCT", JsonConvert.SerializeObject(cardsCT, Formatting.Indented));
            logTransferObj("CardsFromPlayer", "cardsBack", cardsBack.ToString());
            await sendToPlayer(player, "CardsFromPlayer", JsonConvert.SerializeObject(playerCT), JsonConvert.SerializeObject(cardsCT), cardsBack.ToString());
        }

        protected async Task SendCardsFromPlayerToCaller(Player playerCT, List<Card> cardsCT, bool cardsBack)
        {
            logTransferObj("CardsFromPlayer", "playerCT", JsonConvert.SerializeObject(playerCT, Formatting.Indented));
            logTransferObj("CardsFromPlayer", "cardsCT", JsonConvert.SerializeObject(cardsCT, Formatting.Indented));
            logTransferObj("CardsFromPlayer", "cardsBack", cardsBack.ToString());
            await Clients.Caller.SendAsync("CardsFromPlayer", JsonConvert.SerializeObject(playerCT), JsonConvert.SerializeObject(cardsCT), cardsBack.ToString());
        }

        protected async Task SendDealQuestion(Game game)
        {
            await sendToAll(game, "DealQuestion");
        }

        protected async Task SendDealQuestion(Player player)
        {
            await sendToPlayer(player, "DealQuestion");
        }

        protected async Task SendDealQuestionToCaller()
        {
            await Clients.Caller.SendAsync("DealQuestion");
        }

        protected async Task SendExternalPage(Game game, string url)
        {
            logTransferObj("ExternalPage", "url", url);
            await sendToAll(game, "ExternalPage", url);
        }

        protected async Task SendExternalPage(Player player, string url)
        {
            logTransferObj("ExternalPage", "url", url);
            await sendToPlayer(player, "ExternalPage", url);
        }

        protected async Task SendExternalPageToCaller(string url)
        {
            logTransferObj("ExternalPage", "url", url);
            await Clients.Caller.SendAsync("ExternalPage", url);
        }

        protected async Task SendHand(Game game, List<Card> handCT)
        {
            logTransferObj("Hand", "handCT", JsonConvert.SerializeObject(handCT, Formatting.Indented));
            await sendToAll(game, "Hand", JsonConvert.SerializeObject(handCT));
        }

        protected async Task SendHand(Player player, List<Card> handCT)
        {
            logTransferObj("Hand", "handCT", JsonConvert.SerializeObject(handCT, Formatting.Indented));
            await sendToPlayer(player, "Hand", JsonConvert.SerializeObject(handCT));
        }

        protected async Task SendHandToCaller(List<Card> handCT)
        {
            logTransferObj("Hand", "handCT", JsonConvert.SerializeObject(handCT, Formatting.Indented));
            await Clients.Caller.SendAsync("Hand", JsonConvert.SerializeObject(handCT));
        }

        protected async Task SendInfo(Game game, string msg)
        {
            logTransferObj("Info", "msg", msg);
            await sendToAll(game, "Info", msg);
        }

        protected async Task SendInfo(Player player, string msg)
        {
            logTransferObj("Info", "msg", msg);
            await sendToPlayer(player, "Info", msg);
        }

        protected async Task SendInfoToCaller(string msg)
        {
            logTransferObj("Info", "msg", msg);
            await Clients.Caller.SendAsync("Info", msg);
        }

        protected async Task SendInitialized(Game game, string gameName, int playerNo, string playerToken)
        {
            logTransferObj("Initialized", "gameName", gameName);
            logTransferObj("Initialized", "playerNo", playerNo.ToString());
            logTransferObj("Initialized", "playerToken", playerToken);
            await sendToAll(game, "Initialized", gameName, playerNo.ToString(), playerToken);
        }

        protected async Task SendInitialized(Player player, string gameName, int playerNo, string playerToken)
        {
            logTransferObj("Initialized", "gameName", gameName);
            logTransferObj("Initialized", "playerNo", playerNo.ToString());
            logTransferObj("Initialized", "playerToken", playerToken);
            await sendToPlayer(player, "Initialized", gameName, playerNo.ToString(), playerToken);
        }

        protected async Task SendInitializedToCaller(string gameName, int playerNo, string playerToken)
        {
            logTransferObj("Initialized", "gameName", gameName);
            logTransferObj("Initialized", "playerNo", playerNo.ToString());
            logTransferObj("Initialized", "playerToken", playerToken);
            await Clients.Caller.SendAsync("Initialized", gameName, playerNo.ToString(), playerToken);
        }

        protected async Task SendLastTrick(Game game, Trick trickCT)
        {
            logTransferObj("LastTrick", "trickCT", JsonConvert.SerializeObject(trickCT, Formatting.Indented));
            await sendToAll(game, "LastTrick", JsonConvert.SerializeObject(trickCT));
        }

        protected async Task SendLastTrick(Player player, Trick trickCT)
        {
            logTransferObj("LastTrick", "trickCT", JsonConvert.SerializeObject(trickCT, Formatting.Indented));
            await sendToPlayer(player, "LastTrick", JsonConvert.SerializeObject(trickCT));
        }

        protected async Task SendLastTrickToCaller(Trick trickCT)
        {
            logTransferObj("LastTrick", "trickCT", JsonConvert.SerializeObject(trickCT, Formatting.Indented));
            await Clients.Caller.SendAsync("LastTrick", JsonConvert.SerializeObject(trickCT));
        }

        protected async Task SendLayout(Game game, Layout layoutCT)
        {
            logTransferObj("Layout", "layoutCT", JsonConvert.SerializeObject(layoutCT, Formatting.Indented));
            await sendToAll(game, "Layout", JsonConvert.SerializeObject(layoutCT));
        }

        protected async Task SendLayout(Player player, Layout layoutCT)
        {
            logTransferObj("Layout", "layoutCT", JsonConvert.SerializeObject(layoutCT, Formatting.Indented));
            await sendToPlayer(player, "Layout", JsonConvert.SerializeObject(layoutCT));
        }

        protected async Task SendLayoutToCaller(Layout layoutCT)
        {
            logTransferObj("Layout", "layoutCT", JsonConvert.SerializeObject(layoutCT, Formatting.Indented));
            await Clients.Caller.SendAsync("Layout", JsonConvert.SerializeObject(layoutCT));
        }

        protected async Task SendMessages(Game game, List<List<string>> messagesCT)
        {
            logTransferObj("Messages", "messagesCT", JsonConvert.SerializeObject(messagesCT, Formatting.Indented));
            await sendToAll(game, "Messages", JsonConvert.SerializeObject(messagesCT));
        }

        protected async Task SendMessages(Player player, List<List<string>> messagesCT)
        {
            logTransferObj("Messages", "messagesCT", JsonConvert.SerializeObject(messagesCT, Formatting.Indented));
            await sendToPlayer(player, "Messages", JsonConvert.SerializeObject(messagesCT));
        }

        protected async Task SendMessagesToCaller(List<List<string>> messagesCT)
        {
            logTransferObj("Messages", "messagesCT", JsonConvert.SerializeObject(messagesCT, Formatting.Indented));
            await Clients.Caller.SendAsync("Messages", JsonConvert.SerializeObject(messagesCT));
        }

        protected async Task SendPlayerJoined(Game game, int playerNo, string name)
        {
            logTransferObj("PlayerJoined", "playerNo", playerNo.ToString());
            logTransferObj("PlayerJoined", "name", name);
            await sendToAll(game, "PlayerJoined", playerNo.ToString(), name);
        }

        protected async Task SendPlayerJoined(Player player, int playerNo, string name)
        {
            logTransferObj("PlayerJoined", "playerNo", playerNo.ToString());
            logTransferObj("PlayerJoined", "name", name);
            await sendToPlayer(player, "PlayerJoined", playerNo.ToString(), name);
        }

        protected async Task SendPlayerJoinedToCaller(int playerNo, string name)
        {
            logTransferObj("PlayerJoined", "playerNo", playerNo.ToString());
            logTransferObj("PlayerJoined", "name", name);
            await Clients.Caller.SendAsync("PlayerJoined", playerNo.ToString(), name);
        }

        protected async Task SendPoints(Game game, Points pointsCT)
        {
            logTransferObj("Points", "pointsCT", JsonConvert.SerializeObject(pointsCT, Formatting.Indented));
            await sendToAll(game, "Points", JsonConvert.SerializeObject(pointsCT));
        }

        protected async Task SendPoints(Player player, Points pointsCT)
        {
            logTransferObj("Points", "pointsCT", JsonConvert.SerializeObject(pointsCT, Formatting.Indented));
            await sendToPlayer(player, "Points", JsonConvert.SerializeObject(pointsCT));
        }

        protected async Task SendPointsToCaller(Points pointsCT)
        {
            logTransferObj("Points", "pointsCT", JsonConvert.SerializeObject(pointsCT, Formatting.Indented));
            await Clients.Caller.SendAsync("Points", JsonConvert.SerializeObject(pointsCT));
        }

        protected async Task SendRules(Game game, Rules rulesCT)
        {
            logTransferObj("Rules", "rulesCT", JsonConvert.SerializeObject(rulesCT, Formatting.Indented));
            await sendToAll(game, "Rules", JsonConvert.SerializeObject(rulesCT));
        }

        protected async Task SendRules(Player player, Rules rulesCT)
        {
            logTransferObj("Rules", "rulesCT", JsonConvert.SerializeObject(rulesCT, Formatting.Indented));
            await sendToPlayer(player, "Rules", JsonConvert.SerializeObject(rulesCT));
        }

        protected async Task SendRulesToCaller(Rules rulesCT)
        {
            logTransferObj("Rules", "rulesCT", JsonConvert.SerializeObject(rulesCT, Formatting.Indented));
            await Clients.Caller.SendAsync("Rules", JsonConvert.SerializeObject(rulesCT));
        }

        protected async Task SendStatistics(Game game, string stats)
        {
            logTransferObj("Statistics", "stats", stats);
            await sendToAll(game, "Statistics", stats);
        }

        protected async Task SendStatistics(Player player, string stats)
        {
            logTransferObj("Statistics", "stats", stats);
            await sendToPlayer(player, "Statistics", stats);
        }

        protected async Task SendStatisticsToCaller(string stats)
        {
            logTransferObj("Statistics", "stats", stats);
            await Clients.Caller.SendAsync("Statistics", stats);
        }

        protected async Task SendSymbols(Game game, List<List<Symbol>> symbolsCT)
        {
            logTransferObj("Symbols", "symbolsCT", JsonConvert.SerializeObject(symbolsCT, Formatting.Indented));
            await sendToAll(game, "Symbols", JsonConvert.SerializeObject(symbolsCT));
        }

        protected async Task SendSymbols(Player player, List<List<Symbol>> symbolsCT)
        {
            logTransferObj("Symbols", "symbolsCT", JsonConvert.SerializeObject(symbolsCT, Formatting.Indented));
            await sendToPlayer(player, "Symbols", JsonConvert.SerializeObject(symbolsCT));
        }

        protected async Task SendSymbolsToCaller(List<List<Symbol>> symbolsCT)
        {
            logTransferObj("Symbols", "symbolsCT", JsonConvert.SerializeObject(symbolsCT, Formatting.Indented));
            await Clients.Caller.SendAsync("Symbols", JsonConvert.SerializeObject(symbolsCT));
        }

        protected async Task SendTrick(Game game, Trick trickCT)
        {
            logTransferObj("Trick", "trickCT", JsonConvert.SerializeObject(trickCT, Formatting.Indented));
            await sendToAll(game, "Trick", JsonConvert.SerializeObject(trickCT));
        }

        protected async Task SendTrick(Player player, Trick trickCT)
        {
            logTransferObj("Trick", "trickCT", JsonConvert.SerializeObject(trickCT, Formatting.Indented));
            await sendToPlayer(player, "Trick", JsonConvert.SerializeObject(trickCT));
        }

        protected async Task SendTrickToCaller(Trick trickCT)
        {
            logTransferObj("Trick", "trickCT", JsonConvert.SerializeObject(trickCT, Formatting.Indented));
            await Clients.Caller.SendAsync("Trick", JsonConvert.SerializeObject(trickCT));
        }

        protected async Task SendUnauthorized(Game game, string gameName, int playerNo, string playerName)
        {
            logTransferObj("Unauthorized", "gameName", gameName);
            logTransferObj("Unauthorized", "playerNo", playerNo.ToString());
            logTransferObj("Unauthorized", "playerName", playerName);
            await sendToAll(game, "Unauthorized", gameName, playerNo.ToString(), playerName);
        }

        protected async Task SendUnauthorized(Player player, string gameName, int playerNo, string playerName)
        {
            logTransferObj("Unauthorized", "gameName", gameName);
            logTransferObj("Unauthorized", "playerNo", playerNo.ToString());
            logTransferObj("Unauthorized", "playerName", playerName);
            await sendToPlayer(player, "Unauthorized", gameName, playerNo.ToString(), playerName);
        }

        protected async Task SendUnauthorizedToCaller(string gameName, int playerNo, string playerName)
        {
            logTransferObj("Unauthorized", "gameName", gameName);
            logTransferObj("Unauthorized", "playerNo", playerNo.ToString());
            logTransferObj("Unauthorized", "playerName", playerName);
            await Clients.Caller.SendAsync("Unauthorized", gameName, playerNo.ToString(), playerName);
        }


        protected static Game getGame(string gameName)
        {
            return Games.Where(x => x.Name == gameName).FirstOrDefault();
        }

        private async Task sendToAll(Game game, string method, object o1 = null, object o2 = null, object o3 = null)
        {
            foreach (var player in game.Player)
            {
                await sendToPlayer(player, method, o1, o2, o3);
            }
        }

        private async Task sendToPlayer(Player player, string method, object o1 = null, object o2 = null, object o3 = null)
        {
            foreach (var connectionId in player.ConnectionIds)
            {
                if (o1 == null)
                {
                    await Clients.Client(connectionId).SendAsync(method);
                }
                else if (o2 == null)
                {
                    await Clients.Client(connectionId).SendAsync(method, o1);
                }
                else if (o3 == null)
                {
                    await Clients.Client(connectionId).SendAsync(method, o1, o2);
                }
                else
                {
                    await Clients.Client(connectionId).SendAsync(method, o1, o2, o3);
                }
            }
        }

        private void logTransferObj(string method, string paramName, string param)
        {
            if (!string.IsNullOrEmpty(debugOutputDir))
            {
                var extension = param.StartsWith("{") || param.StartsWith("[") ? "json" : "txt";
                var fileName = $"{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ff")}_{method}_{paramName}.{extension}";
                var path = Path.Combine(debugOutputDir, fileName);
                File.WriteAllText(path, param.ToString());
            }
        }
    }
}
