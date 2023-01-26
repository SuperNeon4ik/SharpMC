using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using SharpMC.Configs;
using SharpMC.Enums;
using SharpMC.Protocol;

namespace SharpMC
{
    public static class SharpMC
    {
        public static readonly Logger Logger = new Logger("SharpMC");
        public static PropertiesConfig PropertiesConfig { get; internal set; }
        public static Socket Socket { get; internal set; }

        private static Thread packetListenerThread;
        private static bool isServerStopping = false;

        private static List<Socket> clients = new List<Socket>();

        private const int SEGMENT_BITS = 0x7F;
        private const int CONTINUE_BIT = 0x80;

        public static void SetupPropertiesConfig()
        {
            if (File.Exists(PropertiesConfig.FILE_NAME))
            {
                PropertiesConfig = JsonConvert.DeserializeObject<PropertiesConfig>(
                        File.ReadAllText(PropertiesConfig.FILE_NAME));
            }
            else
            {
                Logger.Log(LogLevel.WARN, PropertiesConfig.FILE_NAME + " doesn't exist. Using default.");
                PropertiesConfig = new PropertiesConfig();
                File.WriteAllText(PropertiesConfig.FILE_NAME, JsonConvert.SerializeObject(PropertiesConfig, Formatting.Indented));
                
            }
            Logger.Log(LogLevel.INFO, "Loaded " + PropertiesConfig.FILE_NAME);
        }

        public static void Start()
        {
            long timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            
            Logger.Log(LogLevel.INFO, "Loading server...");
            SetupPropertiesConfig();
            Logger.Log(LogLevel.INFO, "Starting server...");
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(PropertiesConfig.IpAddress), PropertiesConfig.Port);
            Socket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Socket.Bind(ipEndPoint);
            Socket.Listen(PropertiesConfig.Port);
            Logger.Log(LogLevel.INFO, $"Server listening on {PropertiesConfig.IpAddress}:{PropertiesConfig.Port}...");
            packetListenerThread = new Thread(BeginReadingPackets);
            packetListenerThread.Start();
            Logger.Log(LogLevel.INFO, $"Done! ({(new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds() - timestamp) / 1000f}s) Press enter to stop the server.");
        }

        public static void Stop()
        {
            Logger.Log(LogLevel.INFO,"Stopping server...");
            foreach (var c in clients)
            {
                c.Shutdown(SocketShutdown.Both);
                c.Close();
            }
            isServerStopping = true;
            Socket.Close();
            Logger.Log(LogLevel.INFO,"Server stopped!");
        }

        public static void BeginReadingPackets()
        {
            try
            {
                while (!isServerStopping)
                {
                    Socket clientSocket = Socket.Accept();
                    Thread clientThread = new Thread(HandleClientConnection);
                    clientThread.Start(clientSocket);
                }
            }
            catch (SocketException)
            {
                Logger.Log(LogLevel.ERROR, "PacketListener was interrupted.");
            }
        }

        public static void HandleClientConnection(object clientSocketObj)
        {
            if (!(clientSocketObj is Socket))
                throw new Exception("Argument is not Socket.");
            
            Socket clientSocket = (Socket)clientSocketObj;
            clients.Add(clientSocket);

            Logger clientLogger = new Logger(clientSocket.RemoteEndPoint.ToString());
            clientLogger.Log(LogLevel.DEBUG, "Client connected.");
            
            byte[] buffer = new byte[1024];

            int packetLenght = 0;
            int position = 0;
            byte currentByte;
            bool readingPacketLenght = true;

            List<byte> extraBuffer = new List<byte>();
            bool extraBufferRead = false;
            List<byte> packet = new List<byte>();

            while (SocketConnected(clientSocket))
            {
                int numByte;
                if (extraBuffer.Count == 0 || extraBufferRead)
                {
                    numByte = clientSocket.Receive(buffer);
                    extraBufferRead = false;
                }
                else
                {
                    numByte = extraBuffer.Count;
                    buffer = extraBuffer.ToArray();
                    extraBufferRead = true;
                    extraBuffer.Clear();
                }
                
                if (readingPacketLenght)
                {
                    int j = 0;
                    for (; j < numByte; j++)
                    {
                        currentByte = buffer[j];
                        packetLenght |= (currentByte & SEGMENT_BITS) << position;
                        if ((currentByte & CONTINUE_BIT) == 0)
                        {
                            readingPacketLenght = false;
                            break;
                        }

                        position += 7;
                        if (position >= 32) throw new Exception("VarInt is too big");
                    }

                    if (!readingPacketLenght)
                    {
                        j++;
                        for (; j < numByte; j++)
                        {
                            extraBuffer.Add(buffer[j]);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < numByte; i++)
                    {
                        if (readingPacketLenght)
                        {
                            extraBuffer.Add(buffer[i]);
                        }
                        else
                        {
                            packet.Add(buffer[i]);
                            if (packet.Count == packetLenght)
                            {
                                readingPacketLenght = true;
                                PacketHandler.ProcessPacket(clientSocket, packet.ToArray());
                                packet.Clear();
                                packetLenght = 0;
                                position = 0;
                                buffer = new byte[1024];
                            }
                        }
                    }
                }
            }

            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
            clients.Remove(clientSocket);
            clientLogger.Log(LogLevel.DEBUG, "Client disconnected.");
        }

        static bool SocketConnected(Socket s)
        {
            bool part1 = s.Poll(1000, SelectMode.SelectRead);
            bool part2 = (s.Available == 0);
            if (part1 && part2)
                return false;
            else
                return true;
        }
    }
}