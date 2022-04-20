using CentralService.Api.Matchmaking.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralService.Api.Matchmaking.Core.Factories
{
    public static class ServerManagerFactory
    {
        public static IServerManager GetManager() => new ServerManager();
    }
}
