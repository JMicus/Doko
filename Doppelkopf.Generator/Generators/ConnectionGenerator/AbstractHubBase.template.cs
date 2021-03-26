using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Doppelkopf.Core.App;
using Doppelkopf.Core.App.Enums;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace NAMESPACE
{
    public abstract class AbstractHubBase : Hub
    {
        private static string debugOutputDir = @"C:\Users\acer\Documents\ProgrammeCode\Doppelkopf\debugOut";

        public static List<Game> Games = new List<Game>();

        //METHODS

        protected static Game getGame(string gameName)
        {
            return Games.Where(x => x.Name == gameName).FirstOrDefault();
        }

        private async Task sendToAll(Game game, string method, object o1 = null, object o2 = null, object o3 = null)
        {
            foreach (var player in game.Player)
            {
                await sendToPlayer(player, method, o1, o2, o3);
            }
        }

        private async Task sendToPlayer(Player player, string method, object o1 = null, object o2 = null, object o3 = null)
        {
            foreach (var connectionId in player.ConnectionIds)
            {
                if (o1 == null)
                {
                    await Clients.Client(connectionId).SendAsync(method);
                }
                else if (o2 == null)
                {
                    await Clients.Client(connectionId).SendAsync(method, o1);
                }
                else if (o3 == null)
                {
                    await Clients.Client(connectionId).SendAsync(method, o1, o2);
                }
                else
                {
                    await Clients.Client(connectionId).SendAsync(method, o1, o2, o3);
                }
            }
        }

        private void logTransferObj(string method, string paramName, string param)
        {
            if (!string.IsNullOrEmpty(debugOutputDir))
            {
                var extension = param.StartsWith("{") || param.StartsWith("[") ? "json" : "txt";
                var fileName = $"{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ff")}_{method}_{paramName}.{extension}";
                var path = Path.Combine(debugOutputDir, fileName);
                File.WriteAllText(path, param.ToString());
            }
        }
    }
}
