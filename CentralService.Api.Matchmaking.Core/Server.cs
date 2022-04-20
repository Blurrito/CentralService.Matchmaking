using CentralService.Api.Matchmaking.Dto.Api.Matchmaking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralService.Api.Matchmaking.Core
{
    public class Server
    {
        public int SessionId { get; set; }
        public int Console { get; set; }
        public string GameName { get; set; }
        public string ActualAddress { get; set; }
        public List<KeyValuePair<string, string>> Properties { get; set; }

        public Server(ServerUpdate Update, string GameName)
        {
            SessionId = Update.SessionId;
            Console = Update.SessionId;
            Properties = Update.Properties;
            ActualAddress = Update.ActualAddress;
            this.GameName = GameName;
        }
    }
}
