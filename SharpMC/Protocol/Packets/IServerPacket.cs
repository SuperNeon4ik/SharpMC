using System.Collections.Generic;

namespace SharpMC.Protocol.Packets;

public interface IServerPacket
{
    List<byte> ToBytes();
}