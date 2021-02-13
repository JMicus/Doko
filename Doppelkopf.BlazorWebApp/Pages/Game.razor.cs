using Doppelkopf.BlazorWebApp.ClientCode.Helper;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doppelkopf.BlazorWebApp.Pages
{
    public partial class Game : ComponentBase
    {
        [Inject]
        private NavigationManager NavManager { get; set; }

        private string gameName;
        private string playerNo;
        private string token;

        protected override void OnInitialized()
        {
            NavManager.TryGetQueryString<string>("game", out gameName);
            NavManager.TryGetQueryString<string>("player", out playerNo);
            NavManager.TryGetQueryString<string>("token", out token);

            Client.Initialize(NavManager.ToAbsoluteUri("/dokohub"));

            initMessagesFromHub();
        }

        private void initMessagesFromHub()
        {
        }
    }
}
