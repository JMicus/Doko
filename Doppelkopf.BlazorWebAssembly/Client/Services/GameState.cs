using Doppelkopf.BlazorWebAssembly.Client.Enums;
using Doppelkopf.BlazorWebAssembly.Client.Pages;
using Doppelkopf.Core.App.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using C = Doppelkopf.Core.App;

namespace Doppelkopf.BlazorWebAssembly.Client.Services
{
    public class GameState
    {


        #region C Objects
        public C.PlayerHolder Players = new C.PlayerHolder(null);
        //public C.Player _testplayer = new C.Player(null, 1);

        public C.Trick Trick = new C.Trick();
        public C.Trick LastTrick = new C.Trick();

        public C.Layout Layout = new C.Layout();
        #endregion

        public Watch<string> ExternalPageUrl = new Watch<string>("");

        private StateService StateService;

        private Game gv => StateService.GameView;
        public C.Player Me => Players[StateService.PlayerNo];



        public GameState(StateService stateService)
        {
            StateService = stateService;
        }

        private void log(string tag, string msg = "")
        {
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss,ffff")} C{StateService.PlayerNo} {tag} {msg}");
        }

        public void InitMessagesFromHub(Core.Connection.Client client)
        {
            client.OnUnauthorized += (gameName, playerNo, playerName) =>
            {
                log("Unauthorized");
                gv.OpenDialogUnauthorized(gameName, playerNo, playerName);
            };

            client.OnPlayerJoined += (no, name) =>
            {
                log("PlayerJoined", no + ", " + name);

                var p = Players[no];
                p.Name = name;
                gv.PlayerViews[p.No - 1].Refresh();

                //StateHasChanged();
            };

            client.OnMessages += (msgs) =>
            {
                log("Messages", msgs.ToString());
                for (int i = 0; i < 4; i++)
                {
                    gv.PlayerViews[i].RefreshMsg(msgs[i]);
                }
                //StateHasChanged();
            };

            client.OnHand += (cards) =>
            {
                log("Hand", cards.ToString());

                // TODO debug
                var first = Me.Cards == null || Me.Cards.Count == 0;


                Me.SetHand(cards);

                gv.HandView.Refresh();
                //_handView.Refresh();
                //StateHasChanged();

                if (false)
                {
                    //StateService.OpenDialog = EDialog.SpecialGame;
                    //gv.Close(C.Enums.EGameType.Poverty);
                }
            };

            client.OnTrick += (trick) =>
            {
                log("Trick", trick.ToString());

                //_trick.FromCode(trick);
                //_trickView.Trick = new Trick(trick);
                gv.TrickView.Refresh(trick);
                //_trickView.Refresh();
                //_trickView.Trick = new Trick(trick);
                //_trick
                //StateHasChanged();
            };

            client.OnLastTrick += (trick) =>
            {
                log("LastTrick", trick.ToString());
                gv.LastTrickView.Refresh(trick);
                //_lastTrickView.Refresh();
                //StateHasChanged();
            };

            client.OnLayout += (layout) =>
            {
                log("Layout");
                //_layout.FromCode(layoutCode);
                Layout = layout;
                //StateHasChanged();
            };

            client.OnDealQuestion += () =>
            {
                gv.OpenDialogDealConfirm();
            };

            client.OnExternalPage += (url) =>
            {
                Console.WriteLine("from server - url: " + url);
                ExternalPageUrl.Value = url;
                //StateService.PointsView.Refresh();
            };

            client.OnPoints += (points) =>
            {
                Console.WriteLine("from server - points: " + string.Join("-", points.List.Select(p => p.Points)));
                gv.OpenDialogPoints(points);
            };

            client.OnCardsFromPlayer += (player, cards, back) => gv.OpenDialogCardsFromPlayer(player, cards, back);

            client.OnSymbols += (symbols) =>
            {
                for (int i = 0; i < 4; i++)
                {
                    var player = Players[i + 1];
                    player.Symbols.Clear();
                    player.Symbols.AddRange(symbols[i]);
                    gv.PlayerViews[i].Refresh();
                }
            };
        }
    }
}
