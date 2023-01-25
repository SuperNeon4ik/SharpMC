using System;
using System.IO;
using Newtonsoft.Json;
using SharpMC.Configs;
using SharpMC.Enums;

namespace SharpMC
{
    public static class SharpMC
    {
        public static readonly Logger Logger = new Logger("SharpMC");
        public static PropertiesConfig PropertiesConfig { get; internal set; }

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
            Logger.Log(LogLevel.INFO, "Loading server...");
            SetupPropertiesConfig();
            Logger.Log(LogLevel.INFO, $"Starting server on {PropertiesConfig.IpAddress}:{PropertiesConfig.Port}...");
        }
    }
}