using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpMC.Protocol;

public static class TypeWriter
{
    private const int SEGMENT_BITS = 0x7F;
    private const int CONTINUE_BIT = 0x80;
    
    public static byte[] ToVarInt(int value)
    {
        List<byte> bytes = new List<byte>();
        while (true) {
            if ((value & ~SEGMENT_BITS) == 0) {
                bytes.Add((byte) value);
                return bytes.ToArray();
            }

            bytes.Add((byte)((value & SEGMENT_BITS) | CONTINUE_BIT));

            // Note: >>> means that the sign bit is shifted with the rest of the number rather than being left alone
            value >>>= 7;
        }
    }
    public static void WriteVarInt(List<byte> bytes, int value)
    {
        while (true) {
            if ((value & ~SEGMENT_BITS) == 0) {
                bytes.Add((byte) value);
                return;
            }

            bytes.Add((byte)((value & SEGMENT_BITS) | CONTINUE_BIT));

            // Note: >>> means that the sign bit is shifted with the rest of the number rather than being left alone
            value >>>= 7;
        }
    }
    public static void WriteVarLong(List<byte> bytes, long value) {
        while (true) {
            if ((value & ~((long) SEGMENT_BITS)) == 0) {
                bytes.Add((byte) value);
                return;
            }

            bytes.Add((byte)((value & SEGMENT_BITS) | CONTINUE_BIT));

            // Note: >>> means that the sign bit is shifted with the rest of the number rather than being left alone
            value >>>= 7;
        }
    }

    public static void WriteString(List<byte> bytes, string value)
    {
        byte[] strBytes = Encoding.UTF8.GetBytes(value);
        WriteVarInt(bytes, strBytes.Length);
        bytes.AddRange(strBytes);
    }

    public static void WriteInt64(List<byte> bytes, long value)
    {
        bytes.AddRange(BitConverter.GetBytes(value));
    }
}