using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DokoTelegramService.Enums
{
    internal enum EChatStatus
    {
        INITIAL,
        ASK_NAME,
        ASK_GAME_NAME,
        JOIN_GAME,
        DEBUG,
        IN_GAME,
        DIALOG,
        ACTIONS_MENU,
        AWAIT_SERVER_MESSAGE
    }
}