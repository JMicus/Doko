using Doppelkopf.BlazorWebAssembly.Client.Enums;
using Doppelkopf.BlazorWebAssembly.Client.Helper;
using Doppelkopf.BlazorWebAssembly.Client.Pages;
using Doppelkopf.Core.App.Helper;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doppelkopf.BlazorWebAssembly.Client.Services
{
    public class StateService
    {
        #region private
        private bool _shouldInitialize = true;
        #endregion

        #region user
        public Watch<string> GameName { get; set; } = new Watch<string>("");
        public Watch<int> PlayerNo { get; set; } = new Watch<int>(0);
        public Watch<string> PlayerName { get; set; } = new Watch<string>("");
        public Watch<string> Token { get; set; } = new Watch<string>("");
        #endregion

        public GameState GameState { get; set; }

        public Watch<bool> InGame { get; set; } = new Watch<bool>(false);

        public Watch<bool> Connected { get; set; } = new Watch<bool>(false);

        #region view
        public Game GameView { get; set; }
        public PointsPage PointsView { get; set; }
        #endregion


        private NavigationManager _navManager;

        public string CurrentPage => _navManager?.Uri?.Substring(_navManager.BaseUri.Length).Split('?')[0];

        public EDialog OpenDialog { get; set; }


        public StateService()
        {
            GameState = new GameState(this);
        }

        public string CreateUrl(string page)
        {
            return $"{page}?game={GameName.Value}&player={PlayerNo.Value}&name={PlayerName.Value}&token={Token.Value}";
        }

        public bool Init(Core.Connection.Client client, NavigationManager navManager, IJSRuntime jsRuntime)
        {
            _navManager = navManager;

            var firstInit = _shouldInitialize;

            if (_shouldInitialize)
            {
                _shouldInitialize = false;

                GameName.OnChange += () =>
                {
                    jsRuntime.InvokeVoidAsync("MainPage.setPageTitle", "doko - " + GameName);
                    jsRuntime.InvokeVoidAsync("MainPage.setMenuTitle", GameName.Value);

                    client.GameName = GameName;
                };

                PlayerNo.OnChange += () =>
                {
                    client.PlayerNo = PlayerNo;
                };

                GameState.InitMessagesFromHub(client);

                client.Init(navManager.ToAbsoluteUri("/dokohub"));

            }

            if (navManager.TryGetQueryString<string>("game", out var gameName))
            {
                GameName.Value = gameName;
            }
            if (navManager.TryGetQueryString<int>("player", out var playerNo))
            {
                PlayerNo.Value = playerNo;
            }
            if (navManager.TryGetQueryString<string>("name", out var playerName))
            {
                PlayerName.Value = playerName;
            }
            if (navManager.TryGetQueryString<string>("token", out var token))
            {
                Token.Value = token;
            }

            Console.WriteLine($"StateService INIT {GameName}");
            return firstInit;
        }

               
        
        }
}
