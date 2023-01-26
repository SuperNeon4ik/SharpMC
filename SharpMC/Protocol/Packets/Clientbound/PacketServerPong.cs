using System.Collections.Generic;

namespace SharpMC.Protocol.Packets.Clientbound;

public class PacketServerPong : IServerPacket
{
    public const byte ID = 0x01;
    public long Timestamp;

    public List<byte> ToBytes()
    {
        List<byte> bytes = new List<byte>();
        bytes.Add(ID);
        TypeWriter.WriteVarLong(bytes, Timestamp);
        return bytes;
    }
}