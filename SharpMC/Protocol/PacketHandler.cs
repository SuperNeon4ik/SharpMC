using System;
using System.Linq;
using System.Net.Sockets;
using Newtonsoft.Json;
using SharpMC.Enums;
using SharpMC.Protocol.Packets.Serverbound;

namespace SharpMC.Protocol
{
    public class PacketHandler
    {
        private static readonly Logger Logger = new("Protocol");

        public static void ProcessPacket(Socket clientSocket, byte[] packet)
        {
            if (packet.Length == 0) return;
            byte packetId = packet[0];
            byte[] packetPayload = packet.Skip(1).ToArray();

            if (packetId == 0x00)
            {
                // Handshake or Status Request
                if (packetPayload.Length > 0)
                {
                    PacketHandshakeClient p = new(packetPayload);
                    Logger.Log(LogLevel.DEBUG, $"Received Handshake packet: {JsonConvert.SerializeObject(p)}");
                }
                else
                {
                    Logger.Log(LogLevel.DEBUG, "Received Status Request packet.");
                }
            }
            else
            {
                Logger.Log(LogLevel.INFO,
                    $"[{clientSocket.RemoteEndPoint}] Received unknown packet: ({BitConverter.ToString(new[] { packetId })}) {BitConverter.ToString(packetPayload)}");
            }
        } 
    }
}