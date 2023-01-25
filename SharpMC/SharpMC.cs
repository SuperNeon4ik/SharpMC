using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using SharpMC.Configs;
using SharpMC.Enums;

namespace SharpMC
{
    public static class SharpMC
    {
        public static readonly Logger Logger = new Logger("SharpMC");
        public static PropertiesConfig PropertiesConfig { get; internal set; }
        public static Socket Socket { get; internal set; }

        private static Thread packetListenerThread;
        private static bool isServerStopping = false;
        
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

                    byte[] buffer = new Byte[1024];
                    string data = null;
                    
                    int value = 0;
                    int position = 0;
                    byte currentByte;
                    bool isVarIntOver = false;

                    while (true)
                    {
                        int numByte = clientSocket.Receive(buffer);
                        // data += BitConverter.ToString(buffer, 0, numByte);
                        
                        for (int i = 0; i < numByte; i++)
                        {
                            currentByte = buffer[numByte];
                            value |= (currentByte & SEGMENT_BITS) << position;
                            if ((currentByte & CONTINUE_BIT) == 0)
                            {
                                isVarIntOver = true;
                                break;
                            }
                            position += 7;
                            if (position >= 32) throw new Exception("VarInt is too big");
                        }

                        if (isVarIntOver)
                        {
                            data += value + ";";
                        }
                    }

                    Console.WriteLine("Text received -> {0}", data);

                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                }
            }
            catch (SocketException)
            {
                Logger.Log(LogLevel.ERROR, "PacketListener was interrupted.");
            }
        } 
    }
}