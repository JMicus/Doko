using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Doppelkopf.App;
using Doppelkopf.Client;
using Doppelkopf.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Doppelkopf.Pages
{
    public class gameModel : PageModel
    {
        public string GameName;
        public string PlayerNo;
        public string PlayerToken;

        //public int CardHeight = 200;

        public void OnGet(string gameName, string playerNo, string playerToken)
        {
            GameName = gameName;
            PlayerNo = playerNo;
            PlayerToken = playerToken;
        }

    }
}