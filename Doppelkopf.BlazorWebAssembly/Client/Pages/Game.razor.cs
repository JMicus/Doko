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
    public partial class Game : ComponentBase, IDisposable
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

        [Inject]
        private StateService StateService { get; set; }

        [Inject]
        private Core.Connection.Client Client { get; set; }
        #endregion

        #region Child Components
        public Hand HandView;
        public PlayerView[] PlayerViews = new PlayerView[4];
        public TrickView TrickView;
        public TrickView LastTrickView;
        #endregion

        private GameState gs => StateService.GameState;
        

        #region Client Objects
        //private Core.Connection.Client _client;

        private string test = "-";



        #endregion

        private EDialog _openDialog = EDialog.None;
        public EDialog OpenDialog
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

        #region Properties
        #endregion

        private void debug()
        {
            //Client.Debug("poverty");
            Console.WriteLine(StateService.CurrentPage);
        }

        protected override bool ShouldRender()
        {
            return false;
        }

        protected override void OnAfterRender(bool firstRender)
        {
            Console.WriteLine("Game render " + (firstRender ? "(first)" : ""));
            base.OnAfterRender(firstRender);
        }

        protected override void OnInitialized()
        {
            Console.WriteLine("Game initialized");
            base.OnInitialized();

            MenuService.OpenTab.Value = EMenuAction.PageTable;

            if (StateService.Init(Client, NavManager, JSRuntime))
            {

            }

            DialogService.OnClose += Close;
            MenuService.OnClick += onMenuClick;

            StateService.GameView = this;

            if (!StateService.InGame.Value)
            {
                StateService.InGame.Value = true;
                Client.SayHello(StateService.Token);
            }

        }


        public void OpenDialogUnauthorized(string gameName, int playerNo, string playerName)
        {
            _openDialog = EDialog.Login;
            DialogService.Confirm("Spieler*in " + playerNo + " (" + playerName + ") ist bereits im Spiel " + gameName + " angemeldet.",
                                    "Platz belegt",
                                    new ConfirmOptions()
                                    {
                                        OkButtonText = "OK",
                                        CancelButtonText = "Abbrechen"
                                    });
        }

        public void OpenDialogDealConfirm()
        {
            _openDialog = EDialog.Deal;
            DialogService.Confirm("Soll wirklich neu gegeben werden?", "Neu Geben", new ConfirmOptions() { OkButtonText = "Ja", CancelButtonText = "Nein" });
        }

        public void OpenDialogPoints(C.Points points)
        {
            _openDialog = EDialog.Points;
            DialogService.Open<PointsView>("Ergebnis",
                                           new Dictionary<string, object>() { { "Points", points } });
        }

        public void OpenDialogCardsFromPlayer(C.Player player, List<C.Card> cards, bool cardsBack)
        {
            _openDialog = cardsBack ? EDialog.ReceiveCardsAndReturn : EDialog.ReceiveCards;
            DialogService.Open<SelectCardsView>("Trumpfarmut",
                                                        new SelectCardsView.SelectCardsViewParameters()
                                                        {
                                                            Text = $"{player.Name} gibt dir diese Karten:",
                                                            Players = new List<Player>() { player},
                                                            Cards = cards,
                                                            Layout = gs.Layout,
                                                            SelectionMode = false
                                                        }.ToDict());

        }

        private void onClickTest()
        {
            Client.PlayerMsg($"Hi, my name is {StateService.PlayerName}");
        }

        private bool onMenuClick(EMenuAction click)
        {
            switch (click)
            {
                case EMenuAction.Deal:
                    Client.Deal(false);
                    break;

                case EMenuAction.SpecialGame:
                    _openDialog = EDialog.SpecialGame;
                    DialogService.Open<SpecialGameView>("Sonderspiel");
                    break;

                case EMenuAction.LeaveGame:
                    StateService.InGame.Value = false;
                    //NavManager.NavigateTo("login");
                    break;

                case EMenuAction.Debug:
                    debug();
                    break;

                default:
                    return false;
            }

            return true;
        }

        private void onHandClick(C.Card card)
        {
            Client.PutCard(card);
        }

        private void onTakeTrick(object o)
        {
            Client.TakeTrick();
        }

        private void onTakeTrickBack(object o)
        {
            Client.LastTrickBack();
        }

        private void onLastCardBack(object o)
        {
            Client.TakeCardBack();
        }

        //private void onSendOnEnter(KeyboardEventArgs args)
        //{
        //    if (args.Code == "Enter")
        //    {
        //        if (!string.IsNullOrEmpty(_chatInputTextUpdated))
        //        {
        //            Client.PlayerMsg(_chatInputTextUpdated);
        //        }
        //        _chatInputText = "";
        //        _chatInputTextUpdated = "";
        //    }
        //    log("CHAT", _chatInputText);
        //}
        //private RadzenTextArea _chatTextArea;
        private string _chatInputText = null;
        //private string _chatInputTextUpdated;

        private void onSendTextChanged(ChangeEventArgs args)
        {
            var val = args.Value.ToString();
            log("CHAT", "val|" + val + "|");
            log("CHAT", "bin|" + _chatInputText + "|");
            //_chatInputTextUpdated = val;
            if (val.Contains('\n'))
            {
                Client.PlayerMsg(val.Replace("\n", "").Replace("\r", ""));
                _chatInputText = "";
                StateHasChanged();
            }
        }

        private void log(string tag, string msg = "")
        {
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss,ffff")} C{StateService.PlayerNo} {tag} {msg}");
        }

        private void Close(dynamic result)
        {
            var closedDialog = _openDialog;
            _openDialog = EDialog.None;

            if (result == null)
            {
                log("DIALOG", "Closed with no result");
                return;
            }

            SelectCardsView.SelectCardsViewResult viewResult;

            switch (closedDialog)
            {
                case EDialog.Login:
                    StateService.InGame.Value = false;
                    NavManager.NavigateTo(StateService.CreateUrl("login"));
                    break;

                case EDialog.Deal when result == true:
                    Client.Deal(true);
                    break;

                case EDialog.Points when result == true:
                    Client.Deal(false);
                    break;

                case EDialog.SpecialGame:
                    if (result == C.Enums.EGameType.Poverty)
                    {
                        //log("DIALOG", "armut");
                        _openDialog = EDialog.Poverty;
                        DialogService.Dispose();
                        DialogService.Open<SelectCardsView>("Trumpfarmut",
                                                            new SelectCardsView.SelectCardsViewParameters()
                                                            {
                                                                Cards = gs.Me.Cards,
                                                                Players = gs.Players.Where(p => p != gs.Me).ToList(),
                                                                Layout = gs.Layout,
                                                                SelectionMode = true
                                                            }.ToDict());
                    }
                    else
                    {
                        log("CHANGECARDORDER", result);

                        Client.ChangeCardOrder(result);
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
                    Client.GiveCardsToPlayer(viewResult.Player.No, viewResult.Cards, true);
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

                    log("DIALOG", "ReceiveCardsAndReturn - " + _openDialog);
                    _openDialog = EDialog.PovertyReturn;
                    DialogService.Open<SelectCardsView>("Trumpfarmut",
                                                        new SelectCardsView.SelectCardsViewParameters()
                                                        {
                                                            Text = "Karten zurückgeben:",
                                                            Cards = gs.Me.Cards,
                                                            Players = new List<C.Player>() { viewResult.Player },
                                                            Layout = gs.Layout,
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
                    Client.GiveCardsToPlayer(viewResult.Player.No, viewResult.Cards, false);
                    break;

                default:
                    log("DIALOG", "default - " + _openDialog);
                    break;
            }
            
        }

        public void Dispose()
        {
            DialogService.OnClose -= Close;
            MenuService.OnClick -= onMenuClick;
        }
    }
}
