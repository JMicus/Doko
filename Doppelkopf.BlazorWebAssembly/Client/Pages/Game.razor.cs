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

        private EDialog _openDialog = EDialog.None;
        private EDialog openDialog
        {
            get
            {
                return _openDialog;
            }
            set
            {
                _openDialog = value;
                log("OPENDIALOG", _openDialog.ToString());
            }
        }

        #endregion

        #region Properties
        private C.Player me => _players[playerNo];
        #endregion

        private void debug()
        {
            _client.Debug("poverty");
        }

        protected override bool ShouldRender()
        {
            return false;
        }

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
                openDialog = EDialog.Login;
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
                log("Messages", msgs.ToString());
                for (int i = 0; i < 4; i++)
                {
                    _playerViews[i].RefreshMsg(msgs[i]);
                }
                //StateHasChanged();
            };

            _client.OnHand += (cards) =>
            {
                // TODO debug
                var first = me.Cards == null || me.Cards.Count == 0;


                log("Hand", cards.ToString());
                me.SetHand(cards);

                _handView.Refresh();
                //_handView.Refresh();
                //StateHasChanged();

                if (false)
                {
                    openDialog = EDialog.SpecialGame;
                    Close(C.Enums.EGameType.Poverty);
                }
            };

            _client.OnTrick += (trick) =>
            {
                log("Trick", trick.ToString());

                //_trick.FromCode(trick);
                //_trickView.Trick = new Trick(trick);
                _trickView.Refresh(trick);
                //_trickView.Refresh();
                //_trickView.Trick = new Trick(trick);
                //_trick
                //StateHasChanged();
            };

            _client.OnLastTrick += (trick) =>
            {
                log("LastTrick", trick.ToString());
                _lastTrickView.Refresh(trick);
                //_lastTrickView.Refresh();
                //StateHasChanged();
            };

            _client.OnLayout += (layout) =>
            {
                log("Layout");
                //_layout.FromCode(layoutCode);
                _layout = layout;
                //StateHasChanged();
            };

            _client.OnDealQuestion += () =>
            {
                openDialog = EDialog.Deal;
                DialogService.Confirm("Soll wirklich neu gegeben werden?", "Neu Geben", new ConfirmOptions() { OkButtonText = "Ja", CancelButtonText = "Nein" });
            };

            _client.OnPoints += (points) =>
            {
                openDialog = EDialog.Points;
                DialogService.Open<PointsView>("Ergebnis",
                                               new Dictionary<string, object>() { { "Points", points } });
            };

            _client.OnCardsFromPlayer += (player, cards, cardsBack) =>
            {
                openDialog = cardsBack ? EDialog.ReceiveCardsAndReturn : EDialog.ReceiveCards;
                DialogService.Open<SelectCardsView>("Trumpfarmut",
                                                    new SelectCardsView.SelectCardsViewParameters()
                                                    {
                                                        Text = $"{player.Name} gibt dir diese Karten:",
                                                        Players = new List<Player>() { player },
                                                        Cards = cards,
                                                        Layout = _layout,
                                                        SelectionMode = false
                                                    }.ToDict());
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
                    openDialog = EDialog.SpecialGame;
                    DialogService.Open<SpecialGameView>("Sonderspiel");
                    break;

                case MenuClick.Debug:
                    debug();
                    break;
            }
        }

        private void onHandClick(C.Card card)
        {
            _client.PutCard(card);
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
            var closedDialog = openDialog;
            openDialog = EDialog.None;

            if (result == null)
            {
                log("DIALOG", "Closed with no result");
                return;
            }

            SelectCardsView.SelectCardsViewResult viewResult;

            switch (closedDialog)
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
                        //log("DIALOG", "armut");
                        openDialog = EDialog.Poverty;
                        DialogService.Open<SelectCardsView>("Trumpfarmut",
                                                            new SelectCardsView.SelectCardsViewParameters()
                                                            {
                                                                Cards = me.Cards,
                                                                Players = _players.Where(p => p != me).ToList(),
                                                                Layout = _layout,
                                                                SelectionMode = true
                                                            }.ToDict());
                    }
                    else
                    {
                        log("CHANGECARDORDER", result);

                        _client.ChangeCardOrder(result);
                    }
                    break;

                case EDialog.Poverty:

                    viewResult = result as SelectCardsView.SelectCardsViewResult;

                    if (viewResult == null)
                    {
                        log("Poverty", "");
                        //_openDialog = EDialog.ReceiveCardsAndReturn;
                        // TODO re open dialog
                        break;
                    }

                    log("Poverty", $"{string.Join(".", viewResult.Cards.Select(c => c.ToCode()))} to {viewResult.Player.NameLabel}");
                    _client.GiveCardsToPlayer(viewResult.Player.No, viewResult.Cards, true);
                    break;

                case EDialog.ReceiveCards:
                    // do nothing
                    break;

                case EDialog.ReceiveCardsAndReturn:

                    viewResult = result as SelectCardsView.SelectCardsViewResult;

                    if (viewResult == null)
                    {
                        log("DIALOG", "ReceiveCardsAndReturn");
                        break;
                    }

                    log("DIALOG", "ReceiveCardsAndReturn - " + openDialog);
                    openDialog = EDialog.PovertyReturn;
                    DialogService.Open<SelectCardsView>("Trumpfarmut",
                                                        new SelectCardsView.SelectCardsViewParameters()
                                                        {
                                                            Text = "Karten zurückgeben:",
                                                            Cards = me.Cards,
                                                            Players = new List<C.Player>() { viewResult.Player },
                                                            Layout = _layout,
                                                            SelectionMode = true
                                                        }.ToDict());
                    break;

                case EDialog.PovertyReturn:

                    viewResult = result as SelectCardsView.SelectCardsViewResult;

                    if (viewResult == null)
                    {
                        log("PovertyReturn", "");
                        break;
                    }

                    log("PovertyReturn", $"{string.Join(".", viewResult.Cards.Select(c => c.ToCode()))} to {viewResult.Player.NameLabel}");
                    _client.GiveCardsToPlayer(viewResult.Player.No, viewResult.Cards, false);
                    break;

                default:
                    log("DIALOG", "default - " + openDialog);
                    break;
            }
            
        }
    }
}
