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
using Doppelkopf.Core.App.Config;

namespace Doppelkopf.Core.Connection
{
    public abstract class AbstractHubBase : Hub
    {
        private static string debugOutputDir = @"debugOut";

        public static List<Game> Games = new List<Game>();

        #region (generated) methods

        // creation info: Generate:108, init: Generate:26
        protected abstract Task AddSymbol(Game game, Player player, int playerOfSymbol, Symbol symbolCT);

        // creation info: Generate:113, init: Generate:26
        public async Task AddSymbol_H(string gameName, string playerNo, string playerToken, string playerOfSymbol, string symbolCT)
        {
            logTransferObj("AddSymbol", "gameName", gameName);
            logTransferObj("AddSymbol", "playerNo", playerNo);
            logTransferObj("AddSymbol", "playerToken", playerToken);
            logTransferObj("AddSymbol", "playerOfSymbol", playerOfSymbol);
            logTransferObj("AddSymbol", "symbolCT", symbolCT);
            (var game, var player) = getGameAndPlayer(gameName, playerNo, playerToken);
            if (player == null) return;
            await AddSymbol(game, player, int.Parse(playerOfSymbol), JsonConvert.DeserializeObject<Symbol>(symbolCT));
        }

        // creation info: Generate:108, init: Generate:26
        protected abstract Task ChangeCardOrder(Game game, Player player, EGameType cardOrderE);

        // creation info: Generate:113, init: Generate:26
        public async Task ChangeCardOrder_H(string gameName, string playerNo, string playerToken, string cardOrderE)
        {
            logTransferObj("ChangeCardOrder", "gameName", gameName);
            logTransferObj("ChangeCardOrder", "playerNo", playerNo);
            logTransferObj("ChangeCardOrder", "playerToken", playerToken);
            logTransferObj("ChangeCardOrder", "cardOrderE", cardOrderE);
            (var game, var player) = getGameAndPlayer(gameName, playerNo, playerToken);
            if (player == null) return;
            await ChangeCardOrder(game, player, Parsenum.S2E<EGameType>(cardOrderE));
        }

        // creation info: Generate:108, init: Generate:26
        protected abstract Task Deal(Game game, Player player, bool force);

        // creation info: Generate:113, init: Generate:26
        public async Task Deal_H(string gameName, string playerNo, string playerToken, string force)
        {
            logTransferObj("Deal", "gameName", gameName);
            logTransferObj("Deal", "playerNo", playerNo);
            logTransferObj("Deal", "playerToken", playerToken);
            logTransferObj("Deal", "force", force);
            (var game, var player) = getGameAndPlayer(gameName, playerNo, playerToken);
            if (player == null) return;
            await Deal(game, player, bool.Parse(force));
        }

        // creation info: Generate:108, init: Generate:26
        protected abstract Task Debug(Game game, Player player, string tag);

        // creation info: Generate:113, init: Generate:26
        public async Task Debug_H(string gameName, string playerNo, string playerToken, string tag)
        {
            logTransferObj("Debug", "gameName", gameName);
            logTransferObj("Debug", "playerNo", playerNo);
            logTransferObj("Debug", "playerToken", playerToken);
            logTransferObj("Debug", "tag", tag);
            (var game, var player) = getGameAndPlayer(gameName, playerNo, playerToken);
            if (player == null) return;
            await Debug(game, player, tag);
        }

        // creation info: Generate:108, init: Generate:26
        protected abstract Task GiveCardsToPlayer(Game game, Player player, int receivingPlayerNo, List<Card> cardsCT, bool cardsBack);

        // creation info: Generate:113, init: Generate:26
        public async Task GiveCardsToPlayer_H(string gameName, string playerNo, string playerToken, string receivingPlayerNo, string cardsCT, string cardsBack)
        {
            logTransferObj("GiveCardsToPlayer", "gameName", gameName);
            logTransferObj("GiveCardsToPlayer", "playerNo", playerNo);
            logTransferObj("GiveCardsToPlayer", "playerToken", playerToken);
            logTransferObj("GiveCardsToPlayer", "receivingPlayerNo", receivingPlayerNo);
            logTransferObj("GiveCardsToPlayer", "cardsCT", cardsCT);
            logTransferObj("GiveCardsToPlayer", "cardsBack", cardsBack);
            (var game, var player) = getGameAndPlayer(gameName, playerNo, playerToken);
            if (player == null) return;
            await GiveCardsToPlayer(game, player, int.Parse(receivingPlayerNo), JsonConvert.DeserializeObject<List<Card>>(cardsCT), bool.Parse(cardsBack));
        }

        // creation info: Generate:108, init: Generate:26
        protected abstract Task Init(string newGameName, int myPlayerNo, string myPlayerName, string myPlayerToken);

        // creation info: Generate:113, init: Generate:26
        public async Task Init_H(string newGameName, string myPlayerNo, string myPlayerName, string myPlayerToken)
        {
            logTransferObj("Init", "newGameName", newGameName);
            logTransferObj("Init", "myPlayerNo", myPlayerNo);
            logTransferObj("Init", "myPlayerName", myPlayerName);
            logTransferObj("Init", "myPlayerToken", myPlayerToken);
            await Init(newGameName, int.Parse(myPlayerNo), myPlayerName, myPlayerToken);
        }

        // creation info: Generate:108, init: Generate:26
        protected abstract Task LastTrickBack(Game game, Player player);

        // creation info: Generate:113, init: Generate:26
        public async Task LastTrickBack_H(string gameName, string playerNo, string playerToken)
        {
            logTransferObj("LastTrickBack", "gameName", gameName);
            logTransferObj("LastTrickBack", "playerNo", playerNo);
            logTransferObj("LastTrickBack", "playerToken", playerToken);
            (var game, var player) = getGameAndPlayer(gameName, playerNo, playerToken);
            if (player == null) return;
            await LastTrickBack(game, player);
        }

        // creation info: Generate:108, init: Generate:26
        protected abstract Task PlayerMsg(Game game, Player player, string msg);

        // creation info: Generate:113, init: Generate:26
        public async Task PlayerMsg_H(string gameName, string playerNo, string playerToken, string msg)
        {
            logTransferObj("PlayerMsg", "gameName", gameName);
            logTransferObj("PlayerMsg", "playerNo", playerNo);
            logTransferObj("PlayerMsg", "playerToken", playerToken);
            logTransferObj("PlayerMsg", "msg", msg);
            (var game, var player) = getGameAndPlayer(gameName, playerNo, playerToken);
            if (player == null) return;
            await PlayerMsg(game, player, msg);
        }

        // creation info: Generate:108, init: Generate:26
        protected abstract Task PutCard(Game game, Player player, Card cardCT);

        // creation info: Generate:113, init: Generate:26
        public async Task PutCard_H(string gameName, string playerNo, string playerToken, string cardCT)
        {
            logTransferObj("PutCard", "gameName", gameName);
            logTransferObj("PutCard", "playerNo", playerNo);
            logTransferObj("PutCard", "playerToken", playerToken);
            logTransferObj("PutCard", "cardCT", cardCT);
            (var game, var player) = getGameAndPlayer(gameName, playerNo, playerToken);
            if (player == null) return;
            await PutCard(game, player, JsonConvert.DeserializeObject<Card>(cardCT));
        }

        // creation info: Generate:108, init: Generate:26
        protected abstract Task SayHello(Game game, Player player);

        // creation info: Generate:113, init: Generate:26
        public async Task SayHello_H(string gameName, string playerNo, string playerToken)
        {
            logTransferObj("SayHello", "gameName", gameName);
            logTransferObj("SayHello", "playerNo", playerNo);
            logTransferObj("SayHello", "playerToken", playerToken);
            (var game, var player) = getGameAndPlayer(gameName, playerNo, playerToken);
            if (player == null) return;
            await SayHello(game, player);
        }

        // creation info: Generate:108, init: Generate:26
        protected abstract Task SetExternalPage(Game game, Player player, string url);

        // creation info: Generate:113, init: Generate:26
        public async Task SetExternalPage_H(string gameName, string playerNo, string playerToken, string url)
        {
            logTransferObj("SetExternalPage", "gameName", gameName);
            logTransferObj("SetExternalPage", "playerNo", playerNo);
            logTransferObj("SetExternalPage", "playerToken", playerToken);
            logTransferObj("SetExternalPage", "url", url);
            (var game, var player) = getGameAndPlayer(gameName, playerNo, playerToken);
            if (player == null) return;
            await SetExternalPage(game, player, url);
        }

        // creation info: Generate:108, init: Generate:26
        protected abstract Task SetSettings(Game game, Player player, DokoSettings settingsCT);

        // creation info: Generate:113, init: Generate:26
        public async Task SetSettings_H(string gameName, string playerNo, string playerToken, string settingsCT)
        {
            logTransferObj("SetSettings", "gameName", gameName);
            logTransferObj("SetSettings", "playerNo", playerNo);
            logTransferObj("SetSettings", "playerToken", playerToken);
            logTransferObj("SetSettings", "settingsCT", settingsCT);
            (var game, var player) = getGameAndPlayer(gameName, playerNo, playerToken);
            if (player == null) return;
            await SetSettings(game, player, JsonConvert.DeserializeObject<DokoSettings>(settingsCT));
        }

        // creation info: Generate:108, init: Generate:26
        protected abstract Task TakeCardBack(Game game, Player player);

        // creation info: Generate:113, init: Generate:26
        public async Task TakeCardBack_H(string gameName, string playerNo, string playerToken)
        {
            logTransferObj("TakeCardBack", "gameName", gameName);
            logTransferObj("TakeCardBack", "playerNo", playerNo);
            logTransferObj("TakeCardBack", "playerToken", playerToken);
            (var game, var player) = getGameAndPlayer(gameName, playerNo, playerToken);
            if (player == null) return;
            await TakeCardBack(game, player);
        }

        // creation info: Generate:108, init: Generate:26
        protected abstract Task TakeTrick(Game game, Player player);

        // creation info: Generate:113, init: Generate:26
        public async Task TakeTrick_H(string gameName, string playerNo, string playerToken)
        {
            logTransferObj("TakeTrick", "gameName", gameName);
            logTransferObj("TakeTrick", "playerNo", playerNo);
            logTransferObj("TakeTrick", "playerToken", playerToken);
            (var game, var player) = getGameAndPlayer(gameName, playerNo, playerToken);
            if (player == null) return;
            await TakeTrick(game, player);
        }

        // creation info: Generate:162, init: OnMsgFromHub:465 <- OnMsgFromHub:470 <- OnMsgFromHub:477 <- Generate:44
        protected async Task SendCardsFromPlayer(Game game, Player playerCT, List<Card> cardsCT, bool cardsBack)
        {
            logTransferObj("CardsFromPlayer", "playerCT", JsonConvert.SerializeObject(playerCT, Formatting.Indented));
            logTransferObj("CardsFromPlayer", "cardsCT", JsonConvert.SerializeObject(cardsCT, Formatting.Indented));
            logTransferObj("CardsFromPlayer", "cardsBack", cardsBack.ToString());
            await sendToAll(game, "CardsFromPlayer", JsonConvert.SerializeObject(playerCT), JsonConvert.SerializeObject(cardsCT), cardsBack.ToString());
        }

        // creation info: Generate:169, init: OnMsgFromHub:465 <- OnMsgFromHub:470 <- OnMsgFromHub:477 <- Generate:44
        protected async Task SendCardsFromPlayer(Player player, Player playerCT, List<Card> cardsCT, bool cardsBack)
        {
            logTransferObj("CardsFromPlayer", "playerCT", JsonConvert.SerializeObject(playerCT, Formatting.Indented));
            logTransferObj("CardsFromPlayer", "cardsCT", JsonConvert.SerializeObject(cardsCT, Formatting.Indented));
            logTransferObj("CardsFromPlayer", "cardsBack", cardsBack.ToString());
            await sendToPlayer(player, "CardsFromPlayer", JsonConvert.SerializeObject(playerCT), JsonConvert.SerializeObject(cardsCT), cardsBack.ToString());
        }

        // creation info: Generate:176, init: OnMsgFromHub:465 <- OnMsgFromHub:470 <- OnMsgFromHub:477 <- Generate:44
        protected async Task SendCardsFromPlayerToCaller(Player playerCT, List<Card> cardsCT, bool cardsBack)
        {
            logTransferObj("CardsFromPlayer", "playerCT", JsonConvert.SerializeObject(playerCT, Formatting.Indented));
            logTransferObj("CardsFromPlayer", "cardsCT", JsonConvert.SerializeObject(cardsCT, Formatting.Indented));
            logTransferObj("CardsFromPlayer", "cardsBack", cardsBack.ToString());
            await Clients.Caller.SendAsync("CardsFromPlayer", JsonConvert.SerializeObject(playerCT), JsonConvert.SerializeObject(cardsCT), cardsBack.ToString());
        }

        // creation info: Generate:162, init: Generate:44
        protected async Task SendDealQuestion(Game game)
        {
            logTransferObj("DealQuestion", "NONE", "");
            await sendToAll(game, "DealQuestion");
        }

        // creation info: Generate:169, init: Generate:44
        protected async Task SendDealQuestion(Player player)
        {
            logTransferObj("DealQuestion", "NONE", "");
            await sendToPlayer(player, "DealQuestion");
        }

        // creation info: Generate:176, init: Generate:44
        protected async Task SendDealQuestionToCaller()
        {
            logTransferObj("DealQuestion", "NONE", "");
            await Clients.Caller.SendAsync("DealQuestion");
        }

        // creation info: Generate:162, init: OnMsgFromHub:465 <- Generate:44
        protected async Task SendExternalPage(Game game, string url)
        {
            logTransferObj("ExternalPage", "url", url);
            await sendToAll(game, "ExternalPage", url);
        }

        // creation info: Generate:169, init: OnMsgFromHub:465 <- Generate:44
        protected async Task SendExternalPage(Player player, string url)
        {
            logTransferObj("ExternalPage", "url", url);
            await sendToPlayer(player, "ExternalPage", url);
        }

        // creation info: Generate:176, init: OnMsgFromHub:465 <- Generate:44
        protected async Task SendExternalPageToCaller(string url)
        {
            logTransferObj("ExternalPage", "url", url);
            await Clients.Caller.SendAsync("ExternalPage", url);
        }

        // creation info: Generate:162, init: OnMsgFromHub:465 <- Generate:44
        protected async Task SendHand(Game game, List<Card> handCT)
        {
            logTransferObj("Hand", "handCT", JsonConvert.SerializeObject(handCT, Formatting.Indented));
            await sendToAll(game, "Hand", JsonConvert.SerializeObject(handCT));
        }

        // creation info: Generate:169, init: OnMsgFromHub:465 <- Generate:44
        protected async Task SendHand(Player player, List<Card> handCT)
        {
            logTransferObj("Hand", "handCT", JsonConvert.SerializeObject(handCT, Formatting.Indented));
            await sendToPlayer(player, "Hand", JsonConvert.SerializeObject(handCT));
        }

        // creation info: Generate:176, init: OnMsgFromHub:465 <- Generate:44
        protected async Task SendHandToCaller(List<Card> handCT)
        {
            logTransferObj("Hand", "handCT", JsonConvert.SerializeObject(handCT, Formatting.Indented));
            await Clients.Caller.SendAsync("Hand", JsonConvert.SerializeObject(handCT));
        }

        // creation info: Generate:162, init: OnMsgFromHub:465 <- Generate:44
        protected async Task SendInfo(Game game, string msg)
        {
            logTransferObj("Info", "msg", msg);
            await sendToAll(game, "Info", msg);
        }

        // creation info: Generate:169, init: OnMsgFromHub:465 <- Generate:44
        protected async Task SendInfo(Player player, string msg)
        {
            logTransferObj("Info", "msg", msg);
            await sendToPlayer(player, "Info", msg);
        }

        // creation info: Generate:176, init: OnMsgFromHub:465 <- Generate:44
        protected async Task SendInfoToCaller(string msg)
        {
            logTransferObj("Info", "msg", msg);
            await Clients.Caller.SendAsync("Info", msg);
        }

        // creation info: Generate:162, init: OnMsgFromHub:465 <- OnMsgFromHub:470 <- OnMsgFromHub:477 <- Generate:44
        protected async Task SendInitialized(Game game, string gameName, int playerNo, string playerToken)
        {
            logTransferObj("Initialized", "gameName", gameName);
            logTransferObj("Initialized", "playerNo", playerNo.ToString());
            logTransferObj("Initialized", "playerToken", playerToken);
            await sendToAll(game, "Initialized", gameName, playerNo.ToString(), playerToken);
        }

        // creation info: Generate:169, init: OnMsgFromHub:465 <- OnMsgFromHub:470 <- OnMsgFromHub:477 <- Generate:44
        protected async Task SendInitialized(Player player, string gameName, int playerNo, string playerToken)
        {
            logTransferObj("Initialized", "gameName", gameName);
            logTransferObj("Initialized", "playerNo", playerNo.ToString());
            logTransferObj("Initialized", "playerToken", playerToken);
            await sendToPlayer(player, "Initialized", gameName, playerNo.ToString(), playerToken);
        }

        // creation info: Generate:176, init: OnMsgFromHub:465 <- OnMsgFromHub:470 <- OnMsgFromHub:477 <- Generate:44
        protected async Task SendInitializedToCaller(string gameName, int playerNo, string playerToken)
        {
            logTransferObj("Initialized", "gameName", gameName);
            logTransferObj("Initialized", "playerNo", playerNo.ToString());
            logTransferObj("Initialized", "playerToken", playerToken);
            await Clients.Caller.SendAsync("Initialized", gameName, playerNo.ToString(), playerToken);
        }

        // creation info: Generate:162, init: OnMsgFromHub:465 <- Generate:44
        protected async Task SendLastTrick(Game game, Trick trickCT)
        {
            logTransferObj("LastTrick", "trickCT", JsonConvert.SerializeObject(trickCT, Formatting.Indented));
            await sendToAll(game, "LastTrick", JsonConvert.SerializeObject(trickCT));
        }

        // creation info: Generate:169, init: OnMsgFromHub:465 <- Generate:44
        protected async Task SendLastTrick(Player player, Trick trickCT)
        {
            logTransferObj("LastTrick", "trickCT", JsonConvert.SerializeObject(trickCT, Formatting.Indented));
            await sendToPlayer(player, "LastTrick", JsonConvert.SerializeObject(trickCT));
        }

        // creation info: Generate:176, init: OnMsgFromHub:465 <- Generate:44
        protected async Task SendLastTrickToCaller(Trick trickCT)
        {
            logTransferObj("LastTrick", "trickCT", JsonConvert.SerializeObject(trickCT, Formatting.Indented));
            await Clients.Caller.SendAsync("LastTrick", JsonConvert.SerializeObject(trickCT));
        }

        // creation info: Generate:162, init: OnMsgFromHub:465 <- Generate:44
        protected async Task SendMessages(Game game, List<List<string>> messagesCT)
        {
            logTransferObj("Messages", "messagesCT", JsonConvert.SerializeObject(messagesCT, Formatting.Indented));
            await sendToAll(game, "Messages", JsonConvert.SerializeObject(messagesCT));
        }

        // creation info: Generate:169, init: OnMsgFromHub:465 <- Generate:44
        protected async Task SendMessages(Player player, List<List<string>> messagesCT)
        {
            logTransferObj("Messages", "messagesCT", JsonConvert.SerializeObject(messagesCT, Formatting.Indented));
            await sendToPlayer(player, "Messages", JsonConvert.SerializeObject(messagesCT));
        }

        // creation info: Generate:176, init: OnMsgFromHub:465 <- Generate:44
        protected async Task SendMessagesToCaller(List<List<string>> messagesCT)
        {
            logTransferObj("Messages", "messagesCT", JsonConvert.SerializeObject(messagesCT, Formatting.Indented));
            await Clients.Caller.SendAsync("Messages", JsonConvert.SerializeObject(messagesCT));
        }

        // creation info: Generate:162, init: OnMsgFromHub:465 <- OnMsgFromHub:470 <- Generate:44
        protected async Task SendPlayerJoined(Game game, int playerNo, string name)
        {
            logTransferObj("PlayerJoined", "playerNo", playerNo.ToString());
            logTransferObj("PlayerJoined", "name", name);
            await sendToAll(game, "PlayerJoined", playerNo.ToString(), name);
        }

        // creation info: Generate:169, init: OnMsgFromHub:465 <- OnMsgFromHub:470 <- Generate:44
        protected async Task SendPlayerJoined(Player player, int playerNo, string name)
        {
            logTransferObj("PlayerJoined", "playerNo", playerNo.ToString());
            logTransferObj("PlayerJoined", "name", name);
            await sendToPlayer(player, "PlayerJoined", playerNo.ToString(), name);
        }

        // creation info: Generate:176, init: OnMsgFromHub:465 <- OnMsgFromHub:470 <- Generate:44
        protected async Task SendPlayerJoinedToCaller(int playerNo, string name)
        {
            logTransferObj("PlayerJoined", "playerNo", playerNo.ToString());
            logTransferObj("PlayerJoined", "name", name);
            await Clients.Caller.SendAsync("PlayerJoined", playerNo.ToString(), name);
        }

        // creation info: Generate:162, init: OnMsgFromHub:465 <- Generate:44
        protected async Task SendPoints(Game game, Points pointsCT)
        {
            logTransferObj("Points", "pointsCT", JsonConvert.SerializeObject(pointsCT, Formatting.Indented));
            await sendToAll(game, "Points", JsonConvert.SerializeObject(pointsCT));
        }

        // creation info: Generate:169, init: OnMsgFromHub:465 <- Generate:44
        protected async Task SendPoints(Player player, Points pointsCT)
        {
            logTransferObj("Points", "pointsCT", JsonConvert.SerializeObject(pointsCT, Formatting.Indented));
            await sendToPlayer(player, "Points", JsonConvert.SerializeObject(pointsCT));
        }

        // creation info: Generate:176, init: OnMsgFromHub:465 <- Generate:44
        protected async Task SendPointsToCaller(Points pointsCT)
        {
            logTransferObj("Points", "pointsCT", JsonConvert.SerializeObject(pointsCT, Formatting.Indented));
            await Clients.Caller.SendAsync("Points", JsonConvert.SerializeObject(pointsCT));
        }

        // creation info: Generate:162, init: OnMsgFromHub:465 <- Generate:44
        protected async Task SendSettings(Game game, DokoSettings settingsCT)
        {
            logTransferObj("Settings", "settingsCT", JsonConvert.SerializeObject(settingsCT, Formatting.Indented));
            await sendToAll(game, "Settings", JsonConvert.SerializeObject(settingsCT));
        }

        // creation info: Generate:169, init: OnMsgFromHub:465 <- Generate:44
        protected async Task SendSettings(Player player, DokoSettings settingsCT)
        {
            logTransferObj("Settings", "settingsCT", JsonConvert.SerializeObject(settingsCT, Formatting.Indented));
            await sendToPlayer(player, "Settings", JsonConvert.SerializeObject(settingsCT));
        }

        // creation info: Generate:176, init: OnMsgFromHub:465 <- Generate:44
        protected async Task SendSettingsToCaller(DokoSettings settingsCT)
        {
            logTransferObj("Settings", "settingsCT", JsonConvert.SerializeObject(settingsCT, Formatting.Indented));
            await Clients.Caller.SendAsync("Settings", JsonConvert.SerializeObject(settingsCT));
        }

        // creation info: Generate:162, init: OnMsgFromHub:465 <- Generate:44
        protected async Task SendStatistics(Game game, string stats)
        {
            logTransferObj("Statistics", "stats", stats);
            await sendToAll(game, "Statistics", stats);
        }

        // creation info: Generate:169, init: OnMsgFromHub:465 <- Generate:44
        protected async Task SendStatistics(Player player, string stats)
        {
            logTransferObj("Statistics", "stats", stats);
            await sendToPlayer(player, "Statistics", stats);
        }

        // creation info: Generate:176, init: OnMsgFromHub:465 <- Generate:44
        protected async Task SendStatisticsToCaller(string stats)
        {
            logTransferObj("Statistics", "stats", stats);
            await Clients.Caller.SendAsync("Statistics", stats);
        }

        // creation info: Generate:162, init: OnMsgFromHub:465 <- Generate:44
        protected async Task SendSymbols(Game game, List<List<Symbol>> symbolsCT)
        {
            logTransferObj("Symbols", "symbolsCT", JsonConvert.SerializeObject(symbolsCT, Formatting.Indented));
            await sendToAll(game, "Symbols", JsonConvert.SerializeObject(symbolsCT));
        }

        // creation info: Generate:169, init: OnMsgFromHub:465 <- Generate:44
        protected async Task SendSymbols(Player player, List<List<Symbol>> symbolsCT)
        {
            logTransferObj("Symbols", "symbolsCT", JsonConvert.SerializeObject(symbolsCT, Formatting.Indented));
            await sendToPlayer(player, "Symbols", JsonConvert.SerializeObject(symbolsCT));
        }

        // creation info: Generate:176, init: OnMsgFromHub:465 <- Generate:44
        protected async Task SendSymbolsToCaller(List<List<Symbol>> symbolsCT)
        {
            logTransferObj("Symbols", "symbolsCT", JsonConvert.SerializeObject(symbolsCT, Formatting.Indented));
            await Clients.Caller.SendAsync("Symbols", JsonConvert.SerializeObject(symbolsCT));
        }

        // creation info: Generate:162, init: OnMsgFromHub:465 <- Generate:44
        protected async Task SendTrick(Game game, Trick trickCT)
        {
            logTransferObj("Trick", "trickCT", JsonConvert.SerializeObject(trickCT, Formatting.Indented));
            await sendToAll(game, "Trick", JsonConvert.SerializeObject(trickCT));
        }

        // creation info: Generate:169, init: OnMsgFromHub:465 <- Generate:44
        protected async Task SendTrick(Player player, Trick trickCT)
        {
            logTransferObj("Trick", "trickCT", JsonConvert.SerializeObject(trickCT, Formatting.Indented));
            await sendToPlayer(player, "Trick", JsonConvert.SerializeObject(trickCT));
        }

        // creation info: Generate:176, init: OnMsgFromHub:465 <- Generate:44
        protected async Task SendTrickToCaller(Trick trickCT)
        {
            logTransferObj("Trick", "trickCT", JsonConvert.SerializeObject(trickCT, Formatting.Indented));
            await Clients.Caller.SendAsync("Trick", JsonConvert.SerializeObject(trickCT));
        }

        // creation info: Generate:162, init: OnMsgFromHub:465 <- Generate:44
        protected async Task SendUnauthorized(Game game, string message)
        {
            logTransferObj("Unauthorized", "message", message);
            await sendToAll(game, "Unauthorized", message);
        }

        // creation info: Generate:169, init: OnMsgFromHub:465 <- Generate:44
        protected async Task SendUnauthorized(Player player, string message)
        {
            logTransferObj("Unauthorized", "message", message);
            await sendToPlayer(player, "Unauthorized", message);
        }

        // creation info: Generate:176, init: OnMsgFromHub:465 <- Generate:44
        protected async Task SendUnauthorizedToCaller(string message)
        {
            logTransferObj("Unauthorized", "message", message);
            await Clients.Caller.SendAsync("Unauthorized", message);
        }

        #endregion (generated) methods

        protected static Game getGame(string gameName)
        {
            return Games.Where(x => x.Name == gameName).FirstOrDefault();
        }

        protected (Game Game, Player Player) getGameAndPlayer(string gameName, string playerNo, string playerToken)
        {
            string error = null;

            var game = getGame(gameName);
            var player = game?.Player[playerToken];
            var playerByNo = game?.Player[playerNo];

            if (game == null)
            {
                error = $"Zutritt verboten: Das Spiel '{gameName}' gibt es nicht.";
            }
            else if (player == null)
            {
                error = $"Zutritt verboten: Dein Token {playerToken} ist ungültig.";
            }
            else if (playerByNo == null)
            {
                error = $"Etwas ist schief gelaufen, Spieler*in {playerNo} gibt es nicht.";
            }
            else if (player != playerByNo)
            {
                error = $"Spieler*in {playerByNo.Name} ist bereits im Spiel {gameName} as Nummer {playerNo} angemeldet.";
            }
            else
            {
                return (game, player);
            }

            SendUnauthorizedToCaller(error).Start();

            return default;
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
                Console.WriteLine($"Send to player {player.No}. ConnID: {connectionId}. {method}");
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
                try
                {
                    if (!Directory.Exists(debugOutputDir))
                    {
                        Directory.CreateDirectory(debugOutputDir);
                    }

                    var extension = param != null && (param.StartsWith("{") || param.StartsWith("[")) ? "json" : "txt";
                    var fileName = $"{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-ff")}_{method}_{paramName}.{extension}";
                    var path = Path.Combine(debugOutputDir, fileName);
                    File.WriteAllText(path, param ?? "NULL");
                }
                catch (Exception)
                {
                    debugOutputDir = null;
                }
            }
        }
    }
}