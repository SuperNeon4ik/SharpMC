namespace SharpMC.Protocol.Packets.Serverbound
{
    public class PacketClientHandshake
    {
        public const byte ID = 0x00;
        
        public int ProtocolVersion { get; }
        public string ServerAddress { get; }
        public ushort ServerPort { get; }
        public int NextState { get; }

        public PacketClientHandshake(byte[] bytes)
        {
            ProtocolVersion = TypeParser.ReadVarInt(bytes);
            bytes = TypeParser.SkipVarInt(bytes);
            ServerAddress = TypeParser.ReadString(bytes);
            bytes = TypeParser.SkipString(bytes);
            ServerPort = TypeParser.ReadUInt16(bytes);
            bytes = TypeParser.SkipUInt16(bytes);
            NextState = TypeParser.ReadVarInt(bytes);
        }
    }
}