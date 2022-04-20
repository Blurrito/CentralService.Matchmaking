using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralService.Api.Matchmaking.Dto.Api.Matchmaking
{
    public struct GetServerRequest
    {
        public int ClientPort { get; set; }
        public int ListVersion { get; set; }
        public int EncodingVersion { get; set; }
        public int GameVersion { get; set; }
        public string GameName { get; set; }
        public string ServiceName { get; set; }
        public string ValidationString { get; set; }
        public string Filters { get; set; }
        public string Fields { get; set; }
        public int Flags { get; set; }
        public byte[] ExtraData { get; set; }
    }
}
