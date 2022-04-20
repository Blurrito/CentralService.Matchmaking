using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralService.Api.Matchmaking.Dto.Api.Matchmaking
{
    public struct ServerUpdate
    {
        public int SessionId { get; set; }
        public int Console { get; set; }
        public int State { get; set; }
        public string ActualAddress { get; set; }
        public List<KeyValuePair<string, string>> Properties { get; set; }
    }
}
