using System.Collections.Generic;

namespace SharpMC.Protocol.Packets.Clientbound;

public class PacketLoginEncryptionRequest : IServerPacket
{
    public const byte ID = 0x01;
    
    public byte[] PublicKey;
    public byte[] VerifyToken;
    
    public List<byte> ToBytes()
    {
        List<byte> bytes = new List<byte>();
        bytes.Add(ID);
        TypeWriter.WriteString(bytes, "");
        TypeWriter.WriteVarInt(bytes, PublicKey.Length);
        bytes.AddRange(PublicKey);
        TypeWriter.WriteVarInt(bytes, VerifyToken.Length);
        bytes.AddRange(VerifyToken);
        return bytes;
    }
}