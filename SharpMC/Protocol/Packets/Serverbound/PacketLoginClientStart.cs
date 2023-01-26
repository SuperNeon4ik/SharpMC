using System;
using java.util;

namespace SharpMC.Protocol.Packets.Serverbound;

public class PacketLoginClientStart
{
    public const byte ID = 0x00;
    
    public string Name { get; }
    public bool HasPlayerUUID { get; }
    public UUID PlayerUUID { get; }

    public PacketLoginClientStart(byte[] bytes)
    {
        Name = TypeParser.ReadString(bytes);
        bytes = TypeParser.SkipString(bytes);
        HasPlayerUUID = TypeParser.ReadBool(bytes);
        if (HasPlayerUUID)
        {
            bytes = TypeParser.SkipBool(bytes);
            PlayerUUID = TypeParser.ReadUUID(bytes);
        }
        else
        {
            PlayerUUID = UUID.randomUUID();
        }
    }
}