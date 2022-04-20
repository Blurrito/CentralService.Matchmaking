using CentralService.Api.Matchmaking.Core.Factories;
using CentralService.Api.Matchmaking.Dto.Api.Common;
using CentralService.Api.Matchmaking.Dto.Api.Matchmaking;
using CentralService.Api.Matchmaking.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CentralService.Api.Matchmaking.Controllers
{
    [ApiController]
    [Route("api/ds/server")]
    public class ServerController : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<ApiResponse?>> GetServers(GetServerRequest Request)
        {
            try
            {
                using (IServerManager Manager = ServerManagerFactory.GetManager())
                    return Ok(Manager.GetServers(Request));
            }
            catch
            {
                return BadRequest(new ApiResponse(false, null));
            }
        }

        [HttpPut("manageserver")]
        public async Task<ActionResult> UpdateServer(ServerUpdate Update)
        {
            try
            {
                using (IServerManager Manager = ServerManagerFactory.GetManager())
                    Manager.UpdateServer(Update);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
