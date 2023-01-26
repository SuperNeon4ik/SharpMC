namespace SharpMC.Protocol.Packets.Serverbound;

public class PacketStatusClientPing
{
    public const byte ID = 0x01;

    public long Timestamp { get; }

    public PacketStatusClientPing(byte[] bytes)
    {
        Timestamp = TypeParser.ReadInt64(bytes);
    }
}