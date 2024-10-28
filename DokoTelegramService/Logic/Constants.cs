using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DokoTelegramService.Logic
{
    internal static class Constants
    {
        public const long DEBUG_CHAT_ID = 451839284;
        public const string BOT_TOKEN = "873836395:AAGGC11zxw90X8n3yLC1bjlMTNGiKrOa-20";

        //public const string DOKO_HUB_URI = "http://beta.doko.click/dokohub";
        public const string DOKO_HUB_URI = "https://localhost:44356/dokohub";

        public const string YES = "Ja";
        public const string NO = "Nein";
        public const string RESTART = "Restart";
        public const string INIT = "Init";
        public const string SAY_HELLO = "Hello";
        public const string ACTIONS = "Aktionen";
        public const string GO_BACK = "Zurück";
        public const string DEAL = "Geben";

        public const int MAX_BUTTON_ROW_COUNT = 5;
    }
}