using DokoTelegramService.Tools;
using Doppelkopf.Core.App;
using Doppelkopf.Core.App.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolKit.Telegram;

namespace DokoTelegramService.Logic
{
    internal class MessageSender
    {
        private Session _session;
        private Bot _bot;

        //private string[][] _buttons = new string[Constants.MAX_BUTTON_ROW_COUNT][]; // max 5 rows

        public MessageSender(Session session, Bot bot)
        {
            _session = session;
            _bot = bot;
        }

        public void Send(string msg, bool clearButtons = false)
        {
            if (clearButtons)
            {
                ClearButtons();
                _bot.SendMessageAndClearReplyButtons(_session.ChatId, msg);
                return;
            }

            if (_session.Buttons == null)
            {
                _bot.SendMessage(_session.ChatId, msg);
                return;
            }

            _bot.SendMessageWithReplyButtons(_session.ChatId, msg, _session.Buttons);
            _session.Buttons = new string[Constants.MAX_BUTTON_ROW_COUNT][];
        }

        public void SendObjectInfo(string message, object obj)
        {
            Send(message + "\r\n" + JsonConvert.SerializeObject(obj, Formatting.Indented));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="session"></param>
        /// <param name="question"></param>
        /// <param name="yesAction"></param>
        /// <param name="noAction"></param>
        /// <param name="otherAnswerAction">If null, yesAction will be executed instead</param>
        public void SendYesNoQuestion(string question, Action? yesAction, Action? noAction, Action<string>? otherAnswerAction = null, Action? finalAction = null)
        {
            _session.NextAction = msg =>
            {
                if (msg.Trim().ToLower() == Constants.NO.ToLower())
                {
                    noAction?.Invoke();
                }
                else if (msg.Trim().ToLower() == Constants.YES.ToLower())
                {
                    yesAction?.Invoke();
                }
                else
                {
                    otherAnswerAction?.Invoke(msg);
                }
            };

            if (finalAction != null)
            {
                _session.LastActions.Push(finalAction);
            }

            _session.Sender.SetAllButtons(Constants.YES, Constants.NO);
            _session.Sender.Send(question);
        }

        // ???
        public void SendQuestion(string question, Action<string> action)
        {
            _session.NextAction = action;
            _bot.SendMessageAndClearReplyButtons(_session.ChatId, question);
            //_bot.SendMessage(session.ChatId, question);
        }

        public void SendHand(List<Card> cards)
        {
            _session.GameButtons.Hand = cards;
            Send("Deine Karten");
        }

        public void SendTrick(Trick trick)
        {
            _session.GameButtons.Trick = trick.Cards;
            Send("Stich geändert");
        }

        #region buttons

        public void ClearButtons()
        {
            _session.Buttons = new string[Constants.MAX_BUTTON_ROW_COUNT][];

            for (int i = 0; i < Constants.MAX_BUTTON_ROW_COUNT; i++)
            {
                _session.Buttons[i] = new string[0];
            }
        }

        public void SetAllButtons(params string[] buttons)
        {
            SetAllButtons(new[] { buttons });
        }

        public void SetAllButtons(params string[][] buttons)
        {
            if (buttons.Length > _session.Buttons.Length)
            {
                throw new InvalidOperationException("Max row count: " + _session.Buttons.Length);
            }

            for (int i = 0; i < _session.Buttons.Length; i++)
            {
                if (i < buttons.Length)
                {
                    _session.Buttons[i] = buttons[i];
                }
                else
                {
                    _session.Buttons[i] = new string[0];
                }
            }
        }

        public void SetTopButtons(params string[] buttons)
        {
            var menuButtons = _session.Buttons.Last();
            SetAllButtons(buttons);
            SetMenuButtons(menuButtons);
        }

        public void SetMenuButtons(params string[] buttons)
        {
            _session.Buttons[_session.Buttons.Length - 1] = buttons;
        }

        public void SendMenu(string msg, string[] buttons, string[] menuButtons)
        {
            SetTopButtons(buttons);
            SetMenuButtons(menuButtons);
            Send(msg);
        }

        #endregion buttons
    }
}