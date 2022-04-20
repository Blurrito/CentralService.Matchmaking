using CentralService.Api.Matchmaking.Dto.Api.Common;
using CentralService.Api.Matchmaking.Dto.Api.Matchmaking;
using CentralService.Api.Matchmaking.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralService.Api.Matchmaking.Core
{
    public class ServerManager : IServerManager
    {
        private static List<Server> _Servers = new List<Server>();
        private static readonly object _ServersLock = new object();

        public ApiResponse? GetServers(GetServerRequest Request)
        {
            List<GetServerResponseServer> ResponseServers = new List<GetServerResponseServer>();
            List<string> FieldNames = Request.Fields.Split('\\').ToList();
            FieldNames.RemoveAll(x => x == string.Empty);

            List<Server> GameServers = GetServers(Request.GameName);
            if (GameServers.Count > 0)
            {
                List<Server> FilteredList = FilterServerList(Request.Filters, GameServers);
                if (FilteredList.Count > 0)
                {
                    foreach (Server Server in FilteredList)
                    {
                        GetServerResponseServer ResponseServer = new GetServerResponseServer(FieldNames, Server.Properties, Server.ActualAddress, Server.SessionId);
                        if (ResponseServer.FieldValues.Count == FieldNames.Count)
                            ResponseServers.Add(ResponseServer);
                    }
                }
            }
            return new ApiResponse(true, new GetServerResponse(FieldNames, ResponseServers));
        }

        private List<Server> GetServers(string GameName)
        {
            List<Server> Servers = new List<Server>();
            lock (_ServersLock)
                Servers = _Servers.Where(x => x.GameName == GameName).ToList();
            return Servers;
        }

        public void UpdateServer(ServerUpdate Update)
        {
            if (Update.State == 2)
                RemoveServer(Update.SessionId);
            string GameName = Update.Properties.FirstOrDefault(x => x.Key == "gamename").Value;
            if (GameName != null)
                lock (_ServersLock)
                {
                    Server Server = _Servers.FirstOrDefault(x => x.SessionId == Update.SessionId && x.GameName == GameName);
                    if (Server != null)
                    {
                        Server.ActualAddress = Update.ActualAddress;
                        Server.Console = Update.Console;
                        Server.Properties = Update.Properties;
                    }
                    else
                        _Servers.Add(new Server(Update, GameName));
                }
        }

        public static void RemoveServer(int SessionId)
        {
            lock (_ServersLock)
                _Servers.RemoveAll(x => x.SessionId == SessionId);
        }

        public void Dispose() { }

        private List<Server> FilterServerList(string Filters, List<Server> GameServers)
        {
            FilterCollection Collection = new FilterCollection(Filters);
            List<Server> SortedList = new List<Server>(10);
            int CurrentServer = 0;
            do
            {
                if (Collection.Run(GameServers[CurrentServer]))
                    SortedList.Add(GameServers[CurrentServer]);
                CurrentServer++;
            }
            while (CurrentServer < GameServers.Count && SortedList.Count < SortedList.Capacity);
            return SortedList;
        }
    }
}
