using DokoTelegramService.Enums;
using DokoTelegramService.Tools;
using Doppelkopf.Core.Connection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using ToolKit.Telegram;

namespace DokoTelegramService.Logic
{
    internal class Session : IDisposable
    {
        #region static

        public static event Action SessionListChanged;

        public static event Action<Session> SessionChanged;

        private static Dictionary<long, Session> _sessionByChatId { get; set; } = new Dictionary<long, Session>();
        //private static Dictionary<string, Session> _sessionByPlayerToken { get; set; } = new Dictionary<string, Session>();

        public static List<Session> Sessions => _sessionByChatId.Select(x => x.Value).ToList();

        public static Session GetOrAdd(long chatId, Bot bot)
        {
            var session = Get(chatId);

            if (session == null)
            {
                session = new Session(bot)
                {
                    ChatId = chatId,
                };

                _sessionByChatId.Add(session.ChatId, session);
                SessionListChanged?.Invoke();
            }

            return session;
        }

        public static Session Get(long chatId)
        {
            if (_sessionByChatId.TryGetValue(chatId, out var session))
            {
                return session;
            }

            return null;
        }

        private static void InvoceChanged(Session session)
        {
            SessionChanged?.Invoke(session);
        }

        #endregion static

        public long ChatId { get; set; }

        [JsonIgnore]
        internal ClientValuesVO ClientValues { get; set; } = new ClientValuesVO();

        public string GameName
        {
            get { return ClientValues.GameName; }
            set { ClientValues.GameName = value; }
        }

        public string PlayerName
        {
            get { return ClientValues.PlayerName; }
            set { ClientValues.PlayerName = value; }
        }

        public int PlayerNo
        {
            get { return ClientValues.PlayerNo; }
            set { ClientValues.PlayerNo = value; }
        }

        public string PlayerToken
        {
            get { return ClientValues.PlayerToken; }
            set { ClientValues.PlayerToken = value; }
        }

        private EChatStatus _status = EChatStatus.INITIAL;

        public EChatStatus Status
        {
            get { return _status; }
            private set
            {
                _status = value;
                Session.InvoceChanged(this);
            }
        }

        internal Client Client { get; private set; }
        internal MessageSender Sender { get; private set; }
        internal Action<string>? NextAction { get; set; }
        internal Stack<Action> LastActions { get; } = new Stack<Action>();

        internal GameButtons GameButtons { get; } = new GameButtons();
        internal string[][]? Buttons { get; set; } = new string[Constants.MAX_BUTTON_ROW_COUNT][];

        internal Message? CurrentMessage { get; set; }

        public Session(Bot bot)
        {
            Sender = new MessageSender(this, bot);
        }

        public void CreateClient(string hubUri)
        {
            if (Client != null)
            {
                throw new InvalidOperationException("The session has already a client. " + this.ToString());
            }

            Client = new Client(hubUri, ClientValues);
        }

        public void HandleAction()
        {
            if (LastActions.Any())
            {
                LastActions.Pop().Invoke();
                return;
            }

            var msg = CurrentMessage;

            switch (Status)
            {
                case EChatStatus.AWAIT_SERVER_MESSAGE:
                    Sender.Send("A message from the server is awaited.");
                    break;

                case EChatStatus.INITIAL:

                    PlayerName = msg.Chat.FirstName ?? "Fremde(r)";

                    Sender.SendYesNoQuestion($"Hallo {PlayerName},\r\nmöchtest du deinen Namen behalten?",
                        null,
                        () => Sender.SendQuestion("Wie möchtest du heißen?",
                            msg =>
                            {
                                PlayerName = msg.Trim().Replace(" ", "_");
                            }),
                        null,
                        () => DoAction(EChatStatus.ASK_GAME_NAME));

                    break;

                case EChatStatus.ASK_GAME_NAME:

                    if (string.IsNullOrEmpty(GameName))
                    {
                        Sender.SendQuestion(
                            "Wie soll dein Spiel heißen?\r\nMit dem Namen können deine Mitspieler dem gleiche Speil beitreten.",
                            msg =>
                            {
                                GameName = msg.Trim();
                                DoAction(EChatStatus.JOIN_GAME);
                            });
                    }
                    else
                    {
                        Sender.SendYesNoQuestion(
                            $"Du bist im Spiel '{GameName}', möchtest du im Spiel bleiben?",
                            () => DoAction(EChatStatus.JOIN_GAME),
                            () =>
                            {
                                GameName = null;
                                DoAction(EChatStatus.ASK_GAME_NAME);
                            });
                    }

                    break;

                case EChatStatus.JOIN_GAME:

                    if (Client == null)
                    {
                        CreateClient(Constants.DOKO_HUB_URI);
                        Client.SetReceiver(new MessageReceiver(this));
                    }

                    ChangeStatus(EChatStatus.AWAIT_SERVER_MESSAGE);

                    Client.Init(GameName, PlayerNo, PlayerName, null);

                    break;

                case EChatStatus.DEBUG:

                    switch (msg.Text)
                    {
                        case Constants.RESTART:
                            DoAction(EChatStatus.JOIN_GAME);
                            break;

                        case Constants.INIT:
                            Client.Init(GameName, PlayerNo, PlayerName, null);
                            break;

                        case Constants.SAY_HELLO:
                            Client.SayHello();
                            break;
                    }
                    break;

                case EChatStatus.IN_GAME:

                    switch (msg.Text)
                    {
                        case Constants.ACTIONS:
                            openMenu(
                                (Constants.DEAL, () => Client.Deal(false))
                            );
                            break;
                    }
                    break;

                case EChatStatus.DIALOG:
                    throw new NotImplementedException("Every dialog message should be handled. Message: " + msg);
            }

            //var name = "Iason";

            //PlayerName = name;

            //_client.Init("testgame", 1, name, message.Chat.Id.ToString());
            //_client.SayHello(message.Chat.Id.ToString());
        }

        private void openMenu(params (string Button, Action Action)[] actions)
        {
            var previousStatus = Status;
            LastActions.Push(() => ChangeStatus(previousStatus));

            Status = EChatStatus.DIALOG;

            NextAction = msg => actions.FirstOrDefault(a => a.Button == msg).Action?.Invoke();

            Sender.SendMenu(
                "Was möchtest du tun?",
                actions.Select(a => a.Button).ToArray(),
                new[] { Constants.GO_BACK });
        }

        public void DoAction(EChatStatus status)
        {
            ChangeStatus(status);
            HandleAction();
        }

        public void ChangeStatus(EChatStatus status)
        {
            if (status == Status)
            {
                return;
            }

            Status = status;

            // send buttons
            switch (status)
            {
                case EChatStatus.DEBUG:
                    Sender.SetAllButtons(Constants.INIT, Constants.SAY_HELLO, Constants.RESTART);
                    break;

                case EChatStatus.IN_GAME:
                    GameButtons.MenuButtons = new List<string> { Constants.ACTIONS };
                    Buttons = GameButtons.GetButtons();
                    Sender.Send($"Du bist im Spiel {GameName}");
                    break;

                case EChatStatus.DIALOG:
                    // do noting
                    break;

                default:
                    Sender.ClearButtons();
                    break;
            }
        }

        public void Dispose()
        {
            Client.Disconnect();
        }

        public override string ToString()
        {
            return ChatId + " " + JsonConvert.SerializeObject(ClientValues);
            //return $"Session {ChatId} with {ClientValues.PlayerName} ({PlayerToken})";
        }
    }
}