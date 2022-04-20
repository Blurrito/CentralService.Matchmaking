using CentralService.Utility.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralService.Api.Matchmaking.Dto.Api.Matchmaking
{
    public struct GetServerResponse
    {
        public List<string> FieldNames { get; set; }
        public List<GetServerResponseServer> FoundServers { get; set; }

        public int TotalLength => 8
                + FieldNames.Sum(x => x.Length + 2)
                + FoundServers.Sum(x => x.TotalLength)
                + 5;

        public GetServerResponse(List<string> FieldNames, List<GetServerResponseServer> FoundServers)
        {
            this.FieldNames = FieldNames;
            this.FoundServers = FoundServers;
        }

        public byte[] ToByteArray(int ClientAddress, int ClientPort)
        {
            byte[] ReturnArray = new byte[TotalLength];
            using (BigEndianWriter Writer = new BigEndianWriter(new MemoryStream(ReturnArray)))
            {
                Writer.Write(ClientAddress);
                Writer.Write(ClientPort);
                Writer.Write(FieldNames.Count, false);
                foreach (string Name in FieldNames)
                    Writer.Write($"{ Name }\0\0");
                foreach (GetServerResponseServer FoundServer in FoundServers)
                    FoundServer.ToByteArray(Writer);
                Writer.Write(new byte[] { 0x00, 0xFF, 0xFF, 0xFF, 0xFF });
            }
            return ReturnArray;
        }
    }
}
