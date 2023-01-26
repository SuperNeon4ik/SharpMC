namespace SharpMC.Protocol.Packets.Serverbound
{
    public class PacketHandshakeClient
    {
        public const byte ID = 0x00;
        
        public int ProtocolVersion { get; }
        public string ServerAddress { get; }
        public ushort ServerPort { get; }
        public int NextState { get; }

        public PacketHandshakeClient(byte[] bytes)
        {
            ProtocolVersion = TypeParser.ReadVarInt(bytes);
            bytes = TypeParser.SkipVarInt(bytes);
            ServerAddress = TypeParser.ReadString(bytes);
            bytes = TypeParser.SkipString(bytes);
            ServerPort = TypeParser.ReadUshort(bytes);
            bytes = TypeParser.SkipUshort(bytes);
            NextState = TypeParser.ReadVarInt(bytes);
        }
    }
}