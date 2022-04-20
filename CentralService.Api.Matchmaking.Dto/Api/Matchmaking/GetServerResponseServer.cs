using CentralService.Utility.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralService.Api.Matchmaking.Dto.Api.Matchmaking
{
    public struct GetServerResponseServer
    {
        public List<string> FieldValues { get; set; }
        public byte Flags { get; set; }
        public int PublicAddress { get; set; }
        public ushort PublicPort { get; set; }
        public string LocalAddress { get; set; }
        public ushort LocalPort { get; set; }
        public string ActualAddress { get; set; }
        public int SessionId { get; set; }

        public bool ContainsLocalIp => (Flags & 0x2) != 0;
        public bool AllowsNatnegCommunication => (Flags & 0x4) != 0;
        public bool ContainsIMCPAddress => (Flags & 0x8) != 0;
        public bool ContainsNonStandardPublicPort => (Flags & 0x10) != 0;
        public bool ContainsNonStandardLocalPort => (Flags & 0x20) != 0;
        public bool ContainsKeys => (Flags & 0x40) != 0;

        public int TotalLength => 5
                + (ContainsNonStandardPublicPort ? 2 : 0)
                + (ContainsLocalIp ? 4 : 0)
                + (ContainsNonStandardLocalPort ? 2 : 0)
                + (ContainsIMCPAddress ? 4 : 0)
                + FieldValues.Sum(x => x.Length + 2);

        public GetServerResponseServer(List<string> Fields, List<KeyValuePair<string, string>> ServerProperties, string ActualAddress, int SessionId)
        {
            this.SessionId = SessionId;
            FieldValues = new List<string>();
            foreach (string Field in Fields)
            {
                string Value = ServerProperties.FirstOrDefault(x => x.Key == Field).Value;
                if (Value != null)
                    FieldValues.Add(Value);
            }

            Flags = 0x48;

            string PublicAddress = ServerProperties.FirstOrDefault(x => x.Key == "publicip").Value;
            if (PublicAddress != null)
                this.PublicAddress = int.Parse(PublicAddress);
            else
                this.PublicAddress = 0;

            LocalAddress = ServerProperties.FirstOrDefault(x => x.Key == "localip0").Value;
            if (LocalAddress != null)
                Flags |= 0x2;
            else
                LocalAddress = null;

            string NatnegSupport = ServerProperties.FirstOrDefault(x => x.Key == "natneg").Value;
            if (NatnegSupport == "1")
                Flags |= 0x4;

            string PublicPort = ServerProperties.FirstOrDefault(x => x.Key == "publicport").Value;
            if (PublicPort != null)
            {
                this.PublicPort = ushort.Parse(PublicPort);
                Flags |= 0x10;
            }
            else
                this.PublicPort = 0;

            string LocalPort = ServerProperties.FirstOrDefault(x => x.Key == "localport").Value;
            if (LocalPort != null)
            {
                this.LocalPort = ushort.Parse(LocalPort);
                Flags |= 0x20;
            }
            else
                this.LocalPort = 0;

            this.ActualAddress = ActualAddress;
        }

        public void ToByteArray(BigEndianWriter Writer)
        {
            Writer.Write(Flags);
            Writer.Write(PublicAddress);
            if (ContainsNonStandardPublicPort)
                Writer.Write(PublicPort);
            if (ContainsLocalIp)
            {
                string[] SplitAddress = LocalAddress.Split('.');
                foreach (string AddressByte in SplitAddress)
                    Writer.Write(Convert.ToByte(AddressByte));
            }
            if (ContainsNonStandardLocalPort)
                Writer.Write(LocalPort);
            if (ContainsIMCPAddress)
                Writer.Write(new byte[4] { 0x00, 0x00, 0x00, 0x00 });
            foreach (string Value in FieldValues)
                Writer.Write($"ÿ{ Value }\0");
        }
    }
}
