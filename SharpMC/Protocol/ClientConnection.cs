using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Threading;
using SharpMC.Enums;

namespace SharpMC.Protocol;

public class ClientConnection
{
    private const int SEGMENT_BITS = 0x7F;
    private const int CONTINUE_BIT = 0x80;
    
    public Socket ClientSocket;
    public ClientState State;
    public readonly Thread PacketThread;
    public readonly Logger Logger;

    public ClientConnection(Socket clientSocket)
    {
        ClientSocket = clientSocket;
        State = ClientState.Handshaking;
        PacketThread = new Thread(HandleClientConnection);
        PacketThread.Start();
        Logger = new Logger(ClientSocket.RemoteEndPoint.ToString());
    }

    public void HandleClientConnection()
    {
        SharpMC.Clients.Add(this);

        Logger clientLogger = new Logger(ClientSocket.RemoteEndPoint.ToString());
        clientLogger.Log(LogLevel.Debug, "Client connected.");

        byte[] buffer = new byte[1024];

        int packetLenght = 0;
        int position = 0;
        byte currentByte;
        bool readingPacketLenght = true;

        List<byte> extraBuffer = new List<byte>();
        bool extraBufferRead = false;
        List<byte> packet = new List<byte>();
        List<byte> rawpacket = new List<byte>();

        while (IsConnected())
        {
            int numByte;
            if (extraBuffer.Count == 0 || extraBufferRead)
            {
                numByte = ClientSocket.Receive(buffer);
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
                    rawpacket.Add(currentByte);
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
                        rawpacket.Add(buffer[i]);
                        if (packet.Count == packetLenght)
                        {
                            readingPacketLenght = true;
                            // Logger.Log(LogLevel.Debug,
                            //     $"Packet received (raw): {BitConverter.ToString(rawpacket.ToArray())}");
                            PacketHandler.ProcessPacket(this, packet.ToArray());
                            rawpacket.Clear();
                            packet.Clear();
                            packetLenght = 0;
                            position = 0;
                            buffer = new byte[1024];
                        }
                    }
                }
            }
        }

        SharpMC.Clients.Remove(this);
        clientLogger.Log(LogLevel.Debug, "Client disconnected.");
    }

    public bool IsConnected()
    {
        bool part1 = ClientSocket.Poll(1000, SelectMode.SelectRead);
        bool part2 = (ClientSocket.Available == 0);
        if (part1 && part2)
            return false;
        else
            return true;
    }

    public void Close()
    {
        ClientSocket.Shutdown(SocketShutdown.Both);
        ClientSocket.Close();
    }
}