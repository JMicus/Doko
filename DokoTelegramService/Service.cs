using DokoTelegramService.Enums;
using DokoTelegramService.Logic;
using DokoTelegramService.Tools;
using Doppelkopf.Core.App;
using Doppelkopf.Core.App.Enums;
using Doppelkopf.Core.Connection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolKit.Core.ParallelTools;
using ToolKit.Telegram;
using TMessage = Telegram.Bot.Types.Message;

namespace DokoTelegramService
{
    internal class Service
    {
        internal Bot _bot;

        /// <summary>
        /// Autofill name and game
        /// </summary>
        private const bool DEBUG_MODE
#if DEBUG
                = true;

#else
                = false;
#endif

        internal Dictionary<long, string> _token = new Dictionary<long, string>();

        public void Start()
        {
            _bot = new Bot(Constants.BOT_TOKEN);

            //_bot.SendMessage(CHAT_ID, CardVisualizer.TestMessage());

            //sendHand(null);

            _bot.MessageReceived += messageReceived;

            if (DEBUG_MODE)
            {
                var session = Session.GetOrAdd(Constants.DEBUG_CHAT_ID, _bot);
                session.PlayerName = "t";
                session.GameName = "Testspiel";

                //var rnd = new Random();

                ////Func<string[], string[]> toButtons

                //var cards = Enum.GetValues<ECard>()
                //    .Select(c => new Card(c))
                //    .Where(c => c.Name != null)
                //    .OrderBy(c => rnd.Next())
                //    .Take(10)
                //    .ToList();

                //var empty = CardVisualizer.CardMessage(null).Split("\r\n")[0];

                //var tc = cards.Take(4)
                //    .Select(c => c.CardMessage())
                //    .ToArray();

                ////var gps = new[]
                ////{
                ////    (2, 0),
                ////    (3, 1),
                ////    (1, 2),
                ////    (0, 1)
                ////};

                ////var grid = new string[4][];
                ////for (int i = 0; i < grid.Length; i++)
                ////{
                ////    grid[i] = new string[4];
                ////}

                ////for (int i = 0; i < gps.Length; i++)
                ////{
                ////    var pos = gps[i];
                ////    var parts = tc[i].Split("\r\n");
                ////    grid[pos.Item2][pos.Item1] = parts[0];
                ////    grid[pos.Item2 + 1][pos.Item1] = parts[1];
                ////}

                ////var trick = string.Join("\r\n", grid.Select(row => string.Join("", row.Select(c => c ?? empty))));

                //session.GameButtons.Trick = cards.Take(4).ToArray();
                //session.GameButtons.Hand = cards;
                //session.GameButtons.MenuButtons = new List<string> { Constants.ACTIONS };
                //session.Buttons = session.GameButtons.GetButtons();
                //session.Sender.Send($"Du bist im Spiel {session.GameName}");

                //_bot.SendMessageWithReplyButtons(Constants.DEBUG_CHAT_ID, "test", new[]
                //{
                //    new []{trick},
                //    cards.Select(c => c.CardMessage()).ToArray(),
                //    //new[] {
                //    //    string.Format("{0}{1}{2}\r\n{3}{4}{5}\r\n{6}{7}{8}\r\n{9}{10}{11}",
                //    //        empty, tc[0][0], empty,
                //    //        tc[3][0], tc[0][0], tc[1][0],
                //    //        tc[3][1], tc[2][0], tc[1][1],
                //    //        empty, tc[2][1], empty)},
                //    new []{Constants.ACTIONS, Constants.ACTIONS}
                //});

                //Environment.Exit(0);

                session.DoAction(EChatStatus.JOIN_GAME);

                //session.ChangeStatus(EChatStatus.IN_GAME);
            }
        }

        private void messageReceived(TMessage message)
        {
            // ignore old messages
            if (message.Date < DateTime.Now.ToUniversalTime().AddSeconds(-20))
            {
                return;
            }

            //var res = obj.Text[0].

            // debug
            //_bot.SendMessage(CHAT_ID, "-> " + string.Join(" ", message.Text.ToCharArray()));

            //var card = new Card(message.Text);

            //if (card.Name != null)
            //{
            //    _bot.SendMessage(CHAT_ID, card.CardMessage());
            //}

            ////////
            var session = Session.GetOrAdd(message.Chat.Id, _bot);

            session.CurrentMessage = message;

            if (session.NextAction == null)
            {
                session.HandleAction();
                return;
            }

            var nextAction = session.NextAction;
            session.NextAction = null;
            nextAction(session.CurrentMessage.Text!);

            while (session.NextAction == null
                && session.LastActions.TryPop(out var action))
            {
                action();
            }
        }
    }
}