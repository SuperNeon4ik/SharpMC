using System.Collections.Generic;
using java.util;

namespace SharpMC.Protocol.Packets.Clientbound;

public class PacketLoginSuccess : IServerPacket
{
    public const byte ID = 0x02;

    public UUID Uuid;
    public string Username;

    public List<byte> ToBytes()
    {
        List<byte> bytes = new List<byte>();
        
        TypeWriter.WriteUuid(bytes, Uuid);
        TypeWriter.WriteString(bytes, Username);
        TypeWriter.WriteVarInt(bytes, 0);
        // TypeWriter.WriteString(bytes, NameProperty);
        // TypeWriter.WriteString(bytes, ValueProperty);
        // TypeWriter.WriteBool(bytes, false);
        
        return bytes;
    }
}