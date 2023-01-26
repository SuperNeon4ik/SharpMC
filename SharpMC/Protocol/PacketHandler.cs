using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Newtonsoft.Json;
using SharpMC.Enums;
using SharpMC.Protocol.Packets;
using SharpMC.Protocol.Packets.Clientbound;
using SharpMC.Protocol.Packets.Serverbound;

namespace SharpMC.Protocol
{
    public class PacketHandler
    {
        private static readonly Logger Logger = new("Protocol");
        public const int PROTOCOL_VERSION = 760;

        public static void ProcessPacket(ClientConnection clientConnection, byte[] packet)
        {
            if (packet.Length == 0) return;
            byte packetId = packet[0];
            byte[] packetPayload = packet.Skip(1).ToArray();

            if (packetId == 0x00)
            {
                if (clientConnection.State == ClientState.Handshaking)
                {
                    // Handshake
                    PacketClientHandshake p = new(packetPayload);
                    Logger.Log(LogLevel.Debug, $"Received Handshake packet: {JsonConvert.SerializeObject(p)}");
                    if (p.NextState == 1) clientConnection.State = ClientState.Status;
                    else if (p.NextState == 2) clientConnection.State = ClientState.Login;
                    
                }
                else if (clientConnection.State == ClientState.Status)
                {
                    PacketServerStatus statusPacket = new PacketServerStatus();
                    PacketServerStatus.StatusData statusData = new PacketServerStatus.StatusData();
                    statusData.version = new PacketServerStatus.StatusVersion
                    {
                        name = SharpMC.PropertiesConfig.ServerVersion,
                        protocol = PROTOCOL_VERSION
                    };
                    statusData.players = new PacketServerStatus.StatusPlayers
                    {
                        max = SharpMC.PropertiesConfig.MaxPlayers,
                        online = 0,
                        sample = Array.Empty<PacketServerStatus.StatusPlayer>()
                    };
                    statusData.description = new PacketServerStatus.StatusDescription()
                    {
                        text = ChatColor.ReplaceAlternativeColorCodes('&', SharpMC.PropertiesConfig.Motd)
                    };
                    statusPacket.JsonData = statusData;
                    SendServerPacket(clientConnection, statusPacket);
                }
            }
            else if (packetId == 0x01)
            {
                if (clientConnection.State == ClientState.Status)
                {
                    // Ping
                    PacketClientPing p = new(packetPayload);
                    PacketServerPong pongPacket = new PacketServerPong();
                    pongPacket.Timestamp = p.Timestamp;
                    SendServerPacket(clientConnection, pongPacket);
                }
            }
            else
            {
                Logger.Log(LogLevel.Info,
                    $"[{clientConnection.ClientSocket.RemoteEndPoint}] Received unknown packet: ({BitConverter.ToString(new[] { packetId })}) {BitConverter.ToString(packetPayload)}");
            }
        }

        public static void SendServerPacket(ClientConnection clientConnection, IServerPacket packet, bool debug = false)
        {
            if (debug)
                Logger.Log(LogLevel.Debug, $"[{clientConnection.ClientSocket.RemoteEndPoint}] Sending packet: {JsonConvert.SerializeObject(packet)}");
            List<byte> bytes = packet.ToBytes();
            bytes.InsertRange(0, TypeWriter.ToVarInt(bytes.Count));
            clientConnection.ClientSocket.Send(bytes.ToArray());
        }
    }
}