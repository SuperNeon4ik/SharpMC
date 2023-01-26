using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using java.util;
using SharpMC.Enums;

namespace SharpMC.Protocol
{
    public static class TypeParser
    {
        private const int SEGMENT_BITS = 0x7F;
        private const int CONTINUE_BIT = 0x80;

        public static int ReadVarInt(byte[] bytes)
        {
            int value = 0;
            int position = 0;

            foreach (var currentByte in bytes)
            {
                value |= (currentByte & SEGMENT_BITS) << position;

                if ((currentByte & CONTINUE_BIT) == 0) break;

                position += 7;

                if (position >= 32) throw new Exception("VarInt is too big");
            }

            return value;
        }

        public static byte[] SkipVarInt(byte[] bytes)
        {
            int value = 0;
            int position = 0;

            bool writeToArray = false;
            List<byte> stripped = new List<byte>();
            foreach (var currentByte in bytes)
            {
                if (writeToArray)
                {
                    stripped.Add(currentByte);
                }
                else
                {
                    value |= (currentByte & SEGMENT_BITS) << position;
                    if ((currentByte & CONTINUE_BIT) == 0)
                    {
                        writeToArray = true;
                        continue;
                    }

                    position += 7;
                    if (position >= 32) throw new Exception("VarInt is too big");
                }
            }

            return stripped.ToArray();
        }
        
        public static long ReadVarLong(byte[] bytes) {
            long value = 0;
            int position = 0;

            foreach (byte currentByte in bytes) {
                value |= (long) (currentByte & SEGMENT_BITS) << position;

                if ((currentByte & CONTINUE_BIT) == 0) break;

                position += 7;

                if (position >= 64) throw new Exception("VarLong is too big");
            }

            return value;
        }
        public static byte[] SkipVarLong(byte[] bytes) {
            long value = 0;
            int position = 0;

            bool writeToArray = false;
            List<byte> stripped = new List<byte>();
            foreach (var currentByte in bytes) {
                if (writeToArray)
                {
                    stripped.Add(currentByte);
                }
                else
                {
                    value |= (long)(currentByte & SEGMENT_BITS) << position;

                    if ((currentByte & CONTINUE_BIT) == 0)
                    {
                        writeToArray = true;
                        continue;
                    }

                    position += 7;

                    if (position >= 64) throw new Exception("VarLong is too big");
                }
            }

            return stripped.ToArray();
        }

        public static string ReadString(byte[] bytes)
        {
            int size = ReadVarInt(bytes);
            bytes = SkipVarInt(bytes);

            byte[] strBytes = bytes.Take(size).ToArray();
            return Encoding.UTF8.GetString(strBytes);
        }

        public static byte[] SkipString(byte[] bytes)
        {
            int size = ReadVarInt(bytes);
            return SkipVarInt(bytes).Skip(size).ToArray();
        }

        public static ushort ReadUInt16(byte[] bytes)
        {
            if (bytes.Length == 0) return 0;
            bytes = bytes.Take(2).ToArray();
            Array.Reverse(bytes);
            return BitConverter.ToUInt16(bytes, 0);
        }

        public static byte[] SkipUInt16(byte[] bytes)
        {
            return bytes.Skip(2).ToArray();
        }
        
        public static int ReadInt32(byte[] bytes)
        {
            if (bytes.Length == 0) return 0;
            bytes = bytes.Take(4).ToArray();
            Array.Reverse(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        public static byte[] SkipInt32(byte[] bytes)
        {
            return bytes.Skip(4).ToArray();
        }
        public static long ReadInt64(byte[] bytes)
        {
            if (bytes.Length == 0) return 0;
            bytes = bytes.Take(8).ToArray();
            Array.Reverse(bytes);
            return BitConverter.ToInt64(bytes, 0);
        }

        public static byte[] SkipInt64(byte[] bytes)
        {
            return bytes.Skip(8).ToArray();
        }

        public static bool ReadBool(byte[] bytes)
        {
            return BitConverter.ToBoolean(bytes.Take(1).ToArray(), 0);
        }
        public static byte[] SkipBool(byte[] bytes)
        {
            return bytes.Skip(1).ToArray();
        }

        public static UUID ReadUUID(byte[] bytes)
        {
            return UUID.nameUUIDFromBytes(bytes.Take(16).ToArray());
        }

        public static byte[] SkipUUID(byte[] bytes)
        {
            return bytes.Skip(16).ToArray();
        }
    }
}