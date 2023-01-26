namespace SharpMC.Protocol.Packets.Serverbound;

public class PacketClientPing
{
    public const byte ID = 0x01;

    public long Timestamp { get; }

    public PacketClientPing(byte[] bytes)
    {
        Timestamp = TypeParser.ReadVarLong(bytes);
    }
}