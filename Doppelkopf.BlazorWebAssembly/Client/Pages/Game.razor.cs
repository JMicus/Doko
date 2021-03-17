using C = Doppelkopf.Core.App;
using Doppelkopf.BlazorWebAssembly.Client.Helper;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Doppelkopf.Core.App;
using Doppelkopf.BlazorWebAssembly.Client.Shared;
using Doppelkopf.BlazorWebAssembly.Client.Enums;
using Newtonsoft.Json;
using Doppelkopf.BlazorWebAssembly.Client.Services;
using static Doppelkopf.BlazorWebAssembly.Client.Services.MenuService;
using Microsoft.AspNetCore.Components.Web;
using Radzen.Blazor;

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

        [Inject]
        private MenuService MenuService { get; set; }

        #endregion

        #region Basic fields
        private string gameName;
        private string playerNo;
        private string playerName;
        private string token;
        #endregion

        #region Child Components
        private Hand _handView;
        private PlayerView[] _playerViews = new PlayerView[4];
        private TrickView _trickView;
        private TrickView _lastTrickView;
        #endregion

        #region C Objects
        public C.PlayerHolder _players = new C.PlayerHolder(null);
        public C.Player _testplayer = new Player(null, 1);

        private C.Trick _trick = new Trick();
        private C.Trick _lastTrick = new Trick();

        private C.Layout _layout = new C.Layout();
        #endregion

        #region Client Objects
        private Core.Connection.Client _client;

        private string test = "-";

        private EDialog? _openDialog;

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
                //JSRuntime.InvokeVoidAsync("MainPage.setMenuTitle", gameName);
            }

            DialogService.OnClose += Close;

            MenuService.OnClick += onMenuClick;
            MenuService.InGame = true;

            log("INIT");
        }

        protected override Task OnInitializedAsync()
        {
            _client = new Core.Connection.Client(NavManager.ToAbsoluteUri("/dokohub"), gameName, playerNo);

            initMessagesFromHub();

            _client.SayHello(token);

            return base.OnInitializedAsync();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                base.OnAfterRender(firstRender); 
            }
            Console.WriteLine("Game render " + (firstRender ? "(first)" : ""));
        }

        private void initMessagesFromHub()
        {
            _client.OnUnauthorized += (gameName, playerNo, playerName) => {
                log("Unauthorized");
                _openDialog = EDialog.Login;
                DialogService.Confirm("Spieler*in " + playerNo + " (" + playerName + ") ist bereits im Spiel " + gameName + " angemeldet.",
                                    "Platz belegt",
                                    new ConfirmOptions()
                                    {
                                        OkButtonText = "OK",
                                        CancelButtonText = "Abbrechen"
                                    });
            };

            _client.OnPlayerJoined += (no, name) =>
            {
                log("PlayerJoined", no + ", " + name);

                var p = _players[no];
                p.Name = name;
                _playerViews[p.No - 1].Refresh();

                //StateHasChanged();
            };

            _client.OnMessages += (msgs) =>
            {
                log("Messages",msgs);
                var m = JsonConvert.DeserializeObject<List<List<string>>>(msgs);
                for (int i = 0; i < 4; i++)
                {
                    _playerViews[i].RefreshMsg(m[i]);
                }
                //StateHasChanged();
            };

            _client.OnHand += (cards) =>
            {
                log("Hand", cards);
                me.SetHandByMsg(cards);
                _handView.Refresh();
                //_handView.Refresh();
                //StateHasChanged();
            };

            _client.OnTrick += (startPlayerNo, trick) =>
            {
                log("Trick", trick);

                //_trick.FromCode(trick);
                //_trickView.Trick = new Trick(trick);
                _trickView.Refresh(trick);
                //_trickView.Refresh();
                //_trickView.Trick = new Trick(trick);
                //_trick
                //StateHasChanged();
            };

            _client.OnLastTrick += (startPlayerNo, trick) =>
            {
                log("LastTrick", trick);
                _lastTrickView.Refresh(trick);
                //_lastTrickView.Refresh();
                //StateHasChanged();
            };

            _client.OnLayout += (layoutCode) =>
            {
                log("Layout");
                _layout.FromCode(layoutCode);
                //StateHasChanged();
            };

            _client.OnDealQuestion += () =>
            {
                _openDialog = EDialog.Deal;
                DialogService.Confirm("Soll wirklich neu gegeben werden?", "Neu Geben", new ConfirmOptions() { OkButtonText = "Ja", CancelButtonText = "Nein" });
            };

            _client.OnPoints += (points) =>
            {
                _openDialog = EDialog.Points;
                DialogService.Open<PointsView>("Ergebnis",
                                               new Dictionary<string, object>() { { "Points", new C.Points(points) } });
            };
        }

        private void onClickTest()
        {
            _client.PlayerMsg($"Hi, my name is {playerName}");
        }

        private void onMenuClick(MenuClick click)
        {
            switch (click)
            {
                case MenuClick.Deal:
                    onDealClick(null);
                    break;
                case MenuClick.SpecialGame:
                    _openDialog = EDialog.SpecialGame;
                    DialogService.Open<SpecialGameView>("Sonderspiel");
                    break;

            }
        }

        private void onHandClick(C.Card card)
        {
            _client.PutCard(card.ToCode());
        }

        private void onTakeTrick(object o)
        {
            _client.TakeTrick();
        }

        private void onTakeTrickBack(object o)
        {
            _client.LastTrickBack();
        }

        private void onDealClick(object o)
        {
            _client.Deal(false);
        }

        private void onLastCardBack(object o)
        {
            _client.TakeCardBack();
        }

        private void onSendOnEnter(KeyboardEventArgs args)
        {
            if (args.Code == "Enter")
            {
                if (!string.IsNullOrEmpty(_chatInputTextUpdated))
                {
                    _client.PlayerMsg(_chatInputTextUpdated);
                }
                _chatInputText = "";
                _chatInputTextUpdated = "";
            }
            log("CHAT", _chatInputText);
        }
        private RadzenTextArea _chatTextArea;
        private string _chatInputText = "ölkjölkj";
        private string _chatInputTextUpdated;

        private void onSendTextChanged(ChangeEventArgs args)
        {
            var val = args.Value.ToString();
            _chatInputTextUpdated = val;
        }

        private void log(string tag, string msg = "")
        {
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss,ffff")} C{playerNo} {tag} {msg}");
        }

        private void Close(dynamic result)
        {
            switch (_openDialog)
            {
                case EDialog.Login:
                    NavManager.NavigateTo($"/login?game={gameName}&player={playerName}");
                    break;
                case EDialog.Deal when result == true:
                    _client.Deal(true);
                    break;
                case EDialog.Points when result = false:
                    _client.Deal(false);
                    break;
                case EDialog.SpecialGame:
                    if (result == C.Enums.EGameType.Poverty)
                    {
                        log("DIALOG", "armut");
                    }
                    else
                    {
                        log("DIALOG", result.ToString());
                        _client.ChangeCardOrder(result);
                    }
                    break;
                case EDialog.Poverty:
                    log("Poverty", result);
                    break;

            }
            _openDialog = null;
        }
    }
}
