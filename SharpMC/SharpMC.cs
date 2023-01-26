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

        public static List<ClientConnection> Clients = new List<ClientConnection>();

        public static void SetupPropertiesConfig()
        {
            if (File.Exists(PropertiesConfig.FILE_NAME))
            {
                PropertiesConfig = JsonConvert.DeserializeObject<PropertiesConfig>(
                        File.ReadAllText(PropertiesConfig.FILE_NAME));
            }
            else
            {
                Logger.Log(LogLevel.Warn, PropertiesConfig.FILE_NAME + " doesn't exist. Using default.");
                PropertiesConfig = new PropertiesConfig();
            }
            File.WriteAllText(PropertiesConfig.FILE_NAME, JsonConvert.SerializeObject(PropertiesConfig, Formatting.Indented));
            Logger.Log(LogLevel.Info, "Loaded " + PropertiesConfig.FILE_NAME);
        }

        public static void Start()
        {
            long timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            
            Logger.Log(LogLevel.Info, "Loading server...");
            SetupPropertiesConfig();
            Logger.Log(LogLevel.Info, "Starting server...");
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(PropertiesConfig.IpAddress), PropertiesConfig.Port);
            Socket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Socket.Bind(ipEndPoint);
            Socket.Listen(PropertiesConfig.Port);
            Logger.Log(LogLevel.Info, $"Server listening on {PropertiesConfig.IpAddress}:{PropertiesConfig.Port}...");
            packetListenerThread = new Thread(BeginReadingPackets);
            packetListenerThread.Start();
            Logger.Log(LogLevel.Info, $"Done! ({(new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds() - timestamp) / 1000f}s) Press enter to stop the server.");
        }

        public static void Stop()
        {
            Logger.Log(LogLevel.Info,"Stopping server...");
            foreach (var c in Clients)
            {
                c.Close();
            }
            isServerStopping = true;
            Socket.Close();
            Logger.Log(LogLevel.Info,"Server stopped!");
        }

        public static async void BeginReadingPackets()
        {
            try
            {
                while (!isServerStopping)
                {
                    Socket clientSocket = await Socket.AcceptAsync();
                    new ClientConnection(clientSocket);
                }
            }
            catch (SocketException)
            {
                Logger.Log(LogLevel.Error, "PacketListener was interrupted.");
            }
        }
    }
}