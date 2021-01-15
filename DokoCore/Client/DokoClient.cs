using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doppelkopf.Client
{
    public class DokoClient
    {
        public HubConnection connection;

        private string game;
        private int playerNo;

        public event Action<string> GetHand;
        public event Action<int, string> GetTrick;

        public DokoClient(string game, int playerNo)
        {
            this.game = game;
            this.playerNo = playerNo;

            var url = "https://janisdoppelkopf.azurewebsites.net/dokoHub";

#if DEBUG
            url = "http://localhost:5000/dokoHub";
#endif

            Console.WriteLine(url);

            connection = new HubConnectionBuilder()
                .WithUrl(url)
                .WithAutomaticReconnect()
                .Build();

            

            /*connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };*/

            connection.On<string>("Info", (msg) =>
            {
                Console.WriteLine("Info from server: " + msg);
            });

            connection.On<string>("Hand", (msg) =>
            {
                Console.WriteLine("Hand: " + msg);
                GetHand?.Invoke(msg);
            });

            connection.On<int, string>("Trick", (startPlayerNo, cards) =>
            {
                GetTrick(startPlayerNo, cards);
            });

        }

        public async Task Start()
        {
            Console.WriteLine("connect...");
            await connection.StartAsync();
        }

        public async Task SayHello(string playerName)
        {
            try
            {
                await connection.InvokeAsync("SayHello", game, playerNo.ToString(), playerName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task Deal(bool force = false)
        {
            try
            {
                await connection.InvokeAsync("Deal", game, force);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task PutCard(string card)
        {
            try
            {
                await connection.InvokeAsync("PutCard", game, playerNo.ToString(), card);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task TakeTrick()
        {
            try
            {
                await connection.InvokeAsync("TakeTrick", game, playerNo.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task ChangeCardOrder(string orderName)
        {
            try
            {
                await connection.InvokeAsync("ChangeCardOrder", game, orderName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }
}
