using Doppelkopf.Core.App;
using Doppelkopf.Core.App.Config;
using Doppelkopf.Core.App.Enums;
using Doppelkopf.Core.Connection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolKit.Telegram;

namespace DokoTelegramService.Logic
{
    internal class MessageReceiver : AbstractServerMessageReceiver
    {
        private Session _session;

        private MessageSender Sender => _session.Sender;

        public MessageReceiver(Session session)
        {
            _session = session;
        }

        private void send(string msg)
        {
            _session.Sender.Send(msg);
        }

        #region implementations

        protected override void OnCardsFromPlayer(Player playerCT, List<Card> cardsCT, bool cardsBack)
        {
            //TODO OnCardsFromPlayer
            _session.Sender.SendObjectInfo($"Cards from player {playerCT.Name}:", cardsCT);
        }

        protected override void OnDealQuestion()
        {
            _session.Sender.SendYesNoQuestion(
                "Wirklich neu geben?",
                () => _session.Client.Deal(true),
                null,
                null,
                () => _session.ChangeStatus(Enums.EChatStatus.IN_GAME));
        }

        protected override void OnExternalPage(string url)
        {
            Sender.Send("Points: " + url);
        }

        protected override void OnHand(List<Card> handCT)
        {
            Sender.SendHand(handCT);
        }

        protected override void OnInfo(string msg)
        {
#if DEBUG
            send("INFO: " + msg);
#endif
        }

        protected override void OnInitialized(string gameName, int playerNo, string playerToken)
        {
            _session.GameName = gameName;
            _session.PlayerNo = playerNo;
            _session.PlayerToken = playerToken;

            send($"Initialized:\r\n{JsonConvert.SerializeObject(_session, Formatting.Indented)}");

            _session.ChangeStatus(Enums.EChatStatus.IN_GAME);
            _session.Client.SayHello();
            // just for fun
            //_session.Sender.SendHand(null);
        }

        protected override void OnLastTrick(Trick trickCT)
        {
            //TODO OnLastTrick
            _session.Sender.SendObjectInfo("OnLastTrick:", trickCT);
        }

        protected override void OnMessages(List<List<string>> messagesCT)
        {
            //TODO OnMessages
            Sender.Send("Receiving messages: " + messagesCT.Sum(x => x.Count));
        }

        protected override void OnPlayerJoined(int playerNo, string name)
        {
            send($"{name} ist dem Spiel beigetreten (#{playerNo})");
        }

        protected override void OnPoints(Points pointsCT)
        {
            //TODO OnPoints
            _session.Sender.SendObjectInfo("Points:", pointsCT);
        }

        protected override void OnSettings(DokoSettings settingsCT)
        {
            //TODO onSettings
            _session.Sender.SendObjectInfo("Settings:", settingsCT);
        }

        protected override void OnStatistics(string stats)
        {
            //TODO OnStatistics
            _session.Sender.SendObjectInfo("Result:", stats);
        }

        protected override void OnSymbols(List<List<Symbol>> symbolsCT)
        {
            //TODO OnSymbols
            _session.Sender.SendObjectInfo("Symbols:", symbolsCT);
        }

        protected override void OnTrick(Trick trickCT)
        {
            _session.Sender.SendTrick(trickCT);
        }

        protected override void OnUnauthorized(string message)
        {
            //TODO OnUnauthorized
            _session.Sender.Send("Unauthorized:" + message);
        }

        #endregion implementations
    }
}