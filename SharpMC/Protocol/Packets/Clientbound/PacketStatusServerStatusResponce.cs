using System.Collections.Generic;
using Newtonsoft.Json;

namespace SharpMC.Protocol.Packets.Clientbound;

public class PacketStatusServerStatusResponce : IServerPacket
{
    public const byte ID = 0x00;
    public StatusData JsonData;

    public List<byte> ToBytes()
    {
        List<byte> bytes = new List<byte>();
        bytes.Add(ID);
        TypeWriter.WriteString(bytes, JsonConvert.SerializeObject(JsonData));
        return bytes;
    }

    public class StatusData
    {
        public StatusVersion version;
        public StatusPlayers players;
        public StatusDescription description;
        // public string favicon;
        public bool previewsChat = false;
        public bool enforcesSecureChat = false;
    }
    
    public struct StatusVersion
    {
        public string name;
        public int protocol;
    }
    
    public struct StatusPlayers
    {
        public int max;
        public int online;
        public StatusPlayer[] sample;
    }
    
    public struct StatusPlayer
    {
        public string name;
        public string uuid;
    }
    
    public struct StatusDescription
    {
        public string text;
    }
}