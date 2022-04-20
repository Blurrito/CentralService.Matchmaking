using CentralService.Api.Matchmaking.Dto.Api.Common;
using CentralService.Api.Matchmaking.Dto.Api.Matchmaking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralService.Api.Matchmaking.Interfaces
{
    public interface IServerManager : IDisposable
    {
        ApiResponse? GetServers(GetServerRequest Request);
        void UpdateServer(ServerUpdate Update);
    }
}
