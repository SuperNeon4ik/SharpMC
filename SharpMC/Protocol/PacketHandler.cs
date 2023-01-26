using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Newtonsoft.Json;
using SharpMC.Enums;
using SharpMC.Modules;
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
                    PacketHandshakingClientHandshake p = new(packetPayload);
                    // Logger.Log(LogLevel.Debug, $"Received Handshake packet: {JsonConvert.SerializeObject(p)}");
                    clientConnection.ProtocolVersion = p.ProtocolVersion;
                    clientConnection.ServerAddress = p.ServerAddress;
                    clientConnection.ServerPort = p.ServerPort;
                    if (p.NextState == 1) clientConnection.State = ClientState.Status;
                    else if (p.NextState == 2) clientConnection.State = ClientState.Login;
                    
                }
                else if (clientConnection.State == ClientState.Status)
                {
                    // Status Request
                    PacketStatusServerStatusResponce statusResponcePacketStatus = new PacketStatusServerStatusResponce();
                    PacketStatusServerStatusResponce.StatusData statusData = new PacketStatusServerStatusResponce.StatusData();
                    statusData.version = new PacketStatusServerStatusResponce.StatusVersion
                    {
                        name = SharpMC.PropertiesConfig.ServerVersion,
                        protocol = PROTOCOL_VERSION
                    };
                    statusData.players = new PacketStatusServerStatusResponce.StatusPlayers
                    {
                        max = SharpMC.PropertiesConfig.MaxPlayers,
                        online = 0,
                        sample = Array.Empty<PacketStatusServerStatusResponce.StatusPlayer>()
                    };
                    statusData.description = new PacketStatusServerStatusResponce.StatusDescription()
                    {
                        text = ChatColor.ReplaceAlternativeColorCodes('&', SharpMC.PropertiesConfig.Motd)
                    };
                    statusResponcePacketStatus.JsonData = statusData;
                    SendServerPacket(clientConnection, statusResponcePacketStatus);
                }
                else if (clientConnection.State == ClientState.Login)
                {
                    // Login Start
                    PacketLoginClientStart p = new PacketLoginClientStart(packetPayload);
                    clientConnection.Name = p.Name;
                    clientConnection.UUID = p.PlayerUUID;
                    clientConnection.ClientLogger.Log(LogLevel.Info, $"Logging in as {clientConnection.Name}[{clientConnection.UUID}].");
                    
                    // Send Encryption Response
                    PacketLoginEncryptionRequest encryptionRequest = new PacketLoginEncryptionRequest();

                    byte[] verifyToken = new byte[4];
                    Random random = new Random();
                    random.NextBytes(verifyToken);

                    encryptionRequest.PublicKey = File.ReadAllText("public.crt");
                    encryptionRequest.VerifyToken = verifyToken;
                    
                    SendServerPacket(clientConnection, encryptionRequest, true);
                }
            }
            else if (packetId == 0x01)
            {
                if (clientConnection.State == ClientState.Status)
                {
                    // Ping
                    PacketStatusClientPing p = new(packetPayload);
                    PacketStatusServerPong pongPacketStatus = new PacketStatusServerPong();
                    pongPacketStatus.Timestamp = p.Timestamp;
                    SendServerPacket(clientConnection, pongPacketStatus);
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
            if (debug)
                Logger.Log(LogLevel.Debug, $"Sending bytes: {BitConverter.ToString(bytes.ToArray())}");
            clientConnection.ClientSocket.Send(bytes.ToArray());
        }
    }
}