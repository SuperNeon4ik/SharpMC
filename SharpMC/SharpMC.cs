using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
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
        public static byte[] RsaPrivateKey { get; internal set; }
        public static byte[] RsaPublicKey { get; internal set; }

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
            
            // Create RSA keypair
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.KeySize = 1024;
                rsa.PersistKeyInCsp = false;

                // string subject = "CN=superneon4ik.me";
                // var certReq = new CertificateRequest(subject, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                // certReq.CertificateExtensions.Add(new X509BasicConstraintsExtension(true, false, 0, true)); 
                // certReq.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(certReq.PublicKey, false));
                //
                // var expirate = DateTimeOffset.Now.AddYears(5);
                // var caCert  = certReq.CreateSelfSigned(DateTimeOffset.Now, expirate);
                //
                // var clientKey = RSA.Create(2048);
                // var clientReq = new CertificateRequest(subject, clientKey,HashAlgorithmName.SHA256,RSASignaturePadding.Pkcs1);
                // clientReq.CertificateExtensions.Add(new X509BasicConstraintsExtension(false, false, 0, false));
                // clientReq.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.NonRepudiation, false));
                // clientReq.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(clientReq.PublicKey, false));
                // byte[] serialNumber = BitConverter.GetBytes(DateTime.Now.ToBinary());
                // var clientCert = clientReq.Create(caCert, DateTimeOffset.Now, expirate, serialNumber);
                
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("-----BEGIN PUBLIC KEY-----");
                builder.AppendLine(Convert.ToBase64String(rsa.ExportCspBlob(false), Base64FormattingOptions.InsertLineBreaks));
                builder.AppendLine("-----END PUBLIC KEY-----");
                File.WriteAllText("public.crt", builder.ToString());

                Logger.Log(LogLevel.Info, "Generated Keypair.");
                // Logger.Log(LogLevel.Debug, $"Public Key: {Encoding.Default.GetString(RsaPublicKey)}");
            }

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