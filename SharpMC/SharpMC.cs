using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
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
            Logger.Log(LogLevel.INFO, $"Done! ({(new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds() - timestamp) / 1000f}s) Press enter to stop the server.");
        }

        public static void Stop()
        {
            Logger.Log(LogLevel.INFO,"Stopping server...");
            Socket.Close();
            Logger.Log(LogLevel.INFO,"Server stopped!");
        }
    }
}