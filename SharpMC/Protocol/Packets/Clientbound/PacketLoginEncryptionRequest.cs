using System.Collections.Generic;

namespace SharpMC.Protocol.Packets.Clientbound;

public class PacketLoginEncryptionRequest : IServerPacket
{
    public const byte ID = 0x01;
    
    public string PublicKey;
    public byte[] VerifyToken;
    
    public List<byte> ToBytes()
    {
        List<byte> bytes = new List<byte>();
        bytes.Add(ID);
        // TypeWriter.WriteVarInt(bytes, 20);
        // for (int i = 0; i < 20; i++) bytes.Add(0x00);
        TypeWriter.WriteString(bytes, "");
        // TypeWriter.WriteVarInt(bytes, PublicKey.Length);
        // bytes.AddRange(PublicKey);
        TypeWriter.WriteString(bytes, PublicKey);
        TypeWriter.WriteVarInt(bytes, VerifyToken.Length);
        bytes.AddRange(VerifyToken);
        return bytes;
    }
}