using C = Doppelkopf.Core.App;
using Doppelkopf.BlazorWebAssembly.Client.Helper;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doppelkopf.BlazorWebAssembly.Client.Pages
{
    public partial class Game : ComponentBase
    {
        #region Inject
        [Inject]
        private NavigationManager NavManager { get; set; }

        [Inject]
        private DialogService DialogService { get; set; }

        [Inject]
        private IJSRuntime JSRuntime { get; set; }
        #endregion

        #region Basic fields
        private string gameName;
        private string playerNo;
        private string playerName;
        private string token;
        #endregion

        #region C Objects
        public C.PlayerHolder _players = new C.PlayerHolder(null);

        private C.Layout _layout = new C.Layout();
        #endregion

        #region Client Objects
        private Helper.Client _client;

        private string test = "-";
        #endregion

        #region Properties
        private C.Player me => _players[playerNo];
        #endregion

        protected override void OnInitialized()
        {
            base.OnInitialized();
            NavManager.TryGetQueryString<string>("game", out gameName);
            NavManager.TryGetQueryString<string>("player", out playerNo);
            NavManager.TryGetQueryString<string>("name", out playerName);
            NavManager.TryGetQueryString<string>("token", out token);

            if (!string.IsNullOrEmpty(gameName))
            {
                JSRuntime.InvokeVoidAsync("MainPage.setMenuTitle", gameName);
            }

            DialogService.OnClose += Close;

            
        }

        protected override Task OnInitializedAsync()
        {
            _client = new Helper.Client(NavManager);

            initMessagesFromHub();

            _client.Send("SayHello", gameName, playerNo, token);

            return base.OnInitializedAsync();
        }

        private void initMessagesFromHub()
        {
            _client.On("Unauthorized", (gameName, playerNo, playerName) => {
                Console.WriteLine("Unauthorized");
                DialogService.Confirm("Spieler*in " + playerNo + " (" + playerName + ") ist bereits im Spiel " + gameName + " angemeldet.",
                                    "Platz belegt",
                                    new ConfirmOptions()
                                    {
                                        OkButtonText = "OK",
                                        CancelButtonText = "Abbrechen",
                                                       
                                    });
            });

            _client.On("PlayerJoined", (no, name) =>
            {
                log("PlayerJoined", no + ", " + name);
                _players[no] = new C.Player(null, int.Parse(no))
                {
                    Name = name
                };
                StateHasChanged();
            });

            _client.On("Messages", (msgs) =>
            {
                log("Messages",msgs);
                test = msgs;
                StateHasChanged();
            });

            _client.On("Hand", (cards) =>
            {
                me.SetHandByMsg(cards);
                StateHasChanged();
            });

            _client.On("Layout", (layoutCode) =>
            {
                _layout.FromCode(layoutCode);
                StateHasChanged();
            });
        }

        private void onClickTest()
        {
            _client.Send("PlayerMsg", gameName, playerNo, $"Hi, my name is {playerName}");
        }

        private void log(string tag, string msg = "")
        {
            Console.WriteLine($"C{playerNo} {tag} {msg}");
        }

        private void Close(dynamic result)
        {
            NavManager.NavigateTo($"/login?game={gameName}&player={playerName}");
        }
    }
}
