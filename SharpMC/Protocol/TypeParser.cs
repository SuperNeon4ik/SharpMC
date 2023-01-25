using System;
using System.Net.Sockets;

namespace SharpMC.Protocol
{
    public class TypeParser
    {
        private const int SEGMENT_BITS = 0x7F;
        private const int CONTINUE_BIT = 0x80;
        
        // public int ReadVarInt(Socket socket) {
        //     int value = 0;
        //     int position = 0;
        //     byte currentByte;
        //
        //     while (true) {
        //         currentByte = read;
        //         value |= (currentByte & SEGMENT_BITS) << position;
        //
        //         if ((currentByte & CONTINUE_BIT) == 0) break;
        //
        //         position += 7;
        //
        //         if (position >= 32) throw new Exception("VarInt is too big");
        //     }
        //
        //     return value;
        // }
        //
        // public long ReadVarLong() {
        //     long value = 0;
        //     int position = 0;
        //     byte currentByte;
        //
        //     while (true) {
        //         currentByte = readByte();
        //         value |= (long) (currentByte & SEGMENT_BITS) << position;
        //
        //         if ((currentByte & CONTINUE_BIT) == 0) break;
        //
        //         position += 7;
        //
        //         if (position >= 64) throw new Exception("VarLong is too big");
        //     }
        //
        //     return value;
        // }
    }
}