using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Sockets;
using UnityEngine;
using System.Reflection;

namespace RS
{
    public class NetworkHandler : MonoBehaviour
    {
        public delegate void DisconnectCallback();

        private TcpClient client;
        public List<Packet> OutPackets = new List<Packet>();
        //public JagexBuffer OutBuffer = new DefaultJagexBuffer(new byte[5000]);
        public JagexBuffer InBuffer = new DefaultJagexBuffer(new byte[5000]);
        private JagexBuffer wrapperBuffer = new DefaultJagexBuffer(new byte[5000]);
        public bool Connected = false;
        public DisconnectCallback OnDisconnect = null;

        private int lastOpcode = -1;
        private int lastSize = -1;

        private ISAACCipher decryptCipher;
        public Dictionary<int, PacketHandler> PacketHandlers = new Dictionary<int, PacketHandler>();

        public int lastKeepAlive = 0;

        public void Write(Packet packet)
        {
            OutPackets.Add(packet);
        }

        private string ArrToStr(byte[] b)
        {
            var sb = new StringBuilder();
            foreach (var b2 in b)
            {
                sb.Append((sbyte)b2).Append(" ");
            }
            return sb.ToString();
        }

        public void Flush(Packet packet)
        {
            var headerBuf = new Packet(1);
            headerBuf.WriteOpcode(packet.Opcode);
            
            var stream = client.GetStream();
            stream.Write(headerBuf.Array(), 0, 1);
            stream.Write(packet.Array(), 0, packet.Position());
        }

        public void FlushAll()
        {
            foreach (var packet in OutPackets)
            {
                Flush(packet);
            }
            client.GetStream().Flush();
            OutPackets.Clear();
        }

        public NetworkHandler()
        {
            PacketHandlers.Add(36, new ChangeSmallSettingPacketHandler());
            PacketHandlers.Add(53, new SetWidgetItemsPacketHandler());
            PacketHandlers.Add(65, new ActorUpdatePacketHandler());
            PacketHandlers.Add(71, new SetTabPacketHandler());
            PacketHandlers.Add(73, new LoadRegionPacketHandler());
            PacketHandlers.Add(81, new PlayerUpdatePacketHandler());
            PacketHandlers.Add(85, new SetRegionTargetPacketHandler());
            PacketHandlers.Add(87, new ChangeBigSettingPacketHandler());
            PacketHandlers.Add(104, new SetPlayerOptionPacketHandler());
            PacketHandlers.Add(109, new DisconnectPacketHandler());
            PacketHandlers.Add(126, new SetWidgetDisabledMessagePacketHandler());
            PacketHandlers.Add(134, new SetStatPacketHandler());
            PacketHandlers.Add(164, new OpenChatOverlayWidgetPacketHandler());
            PacketHandlers.Add(218, new OpenChatUnderlayWidgetPacketHandler());
            PacketHandlers.Add(221, new UpdateSocialStatusPacketHandler());
            PacketHandlers.Add(248, new OpenWidgetsPacketHandler());
            PacketHandlers.Add(249, new InitPacketHandler());
            PacketHandlers.Add(253, new AddChatMessagePacketHandler());

            PacketHandlers.Add(105, new TargetPacketHandler());
            PacketHandlers.Add(84, new TargetPacketHandler());
            PacketHandlers.Add(147, new TargetPacketHandler());
            PacketHandlers.Add(215, new TargetPacketHandler());
            PacketHandlers.Add(4, new TargetPacketHandler());
            PacketHandlers.Add(117, new TargetPacketHandler());
            PacketHandlers.Add(156, new TargetPacketHandler());
            PacketHandlers.Add(44, new TargetPacketHandler());
            PacketHandlers.Add(160, new TargetPacketHandler());
            PacketHandlers.Add(101, new TargetPacketHandler());
            PacketHandlers.Add(151, new TargetPacketHandler());
        }

        public void ResetState()
        {
            lastSize = -1;
            InBuffer.Position(0);
            wrapperBuffer.Position(0);
            Connected = false;
            if (client != null)
            {
                client.Close();
                client = null;
                OnDisconnect();
            }
            decryptCipher = null;
        }

        public bool Connect(string ip, int port)
        {
            client = new TcpClient(ip, port);
            client.NoDelay = true;
            client.ReceiveBufferSize = 5000;
            return client.Connected;
        }

        public LoginResponse WriteAuthBlock(string username, string password)
        {
            var stream = client.GetStream();
            stream.ReadTimeout = 5000;

            var nameHash = StringUtils.StringToLong(username);
            var i = (int)(nameHash >> 16 & 31L);

            var @out = new DefaultJagexBuffer(5000);
            @out.Position(0);
            @out.WriteByte(14);
            @out.WriteByte(i);
            stream.Write(@out.Array(), 0, @out.Position());
            @out.Position(0);
            stream.Flush();
            for (var j = 0; j < 8; j++) stream.ReadByte();

            int response = stream.ReadByte();
            if (response == 0)
            {
                InBuffer.Position(0);
                stream.Read(InBuffer.Array(), 0, 8);
                var serverSeed = InBuffer.ReadLong();

                var seed = new int[4];
                var rand = new System.Random();

                seed[0] = (int)(rand.NextDouble() * 9.9999999E7D);
                seed[1] = (int)(rand.NextDouble() * 9.9999999E7D);
                seed[2] = (int)(serverSeed >> 32);
                seed[3] = (int)(serverSeed);

                @out.Position(0);
                @out.WriteByte(10);
                @out.WriteInt(seed[0]);
                @out.WriteInt(seed[1]);
                @out.WriteInt(seed[2]);
                @out.WriteInt(seed[3]);
                @out.WriteInt(350 >> 2240);
                @out.WriteString(username, 10);
                @out.WriteString(password, 10);
                @out.WriteShort(222);
                @out.WriteByte(0);
                @out.RSA(GameConstants.LoginRsaExp, GameConstants.LoginRsaMod);

                wrapperBuffer.Position(0);
                wrapperBuffer.WriteByte(16);
                wrapperBuffer.WriteByte(@out.Position() + 36 + 1 + 2);
                wrapperBuffer.WriteByte(255);
                wrapperBuffer.WriteShort(13);
                wrapperBuffer.WriteByte(1);
                for (var j = 0; j < 9; j++) wrapperBuffer.WriteInt(0);

                GameContext.OutCipher = new ISAACCipher(seed);
                for (var j = 0; j < seed.Length; j++)
                {
                    seed[j] += 50;
                }
                GameContext.InCipher = new ISAACCipher(seed);

                wrapperBuffer.WriteBytes(@out.Array(), 0, @out.Position());
                stream.Write(wrapperBuffer.Array(), 0, wrapperBuffer.Position());
                stream.Flush();

                response = stream.ReadByte();
                @out.Position(0);

                if (response == 2)
                {
                    GameContext.LocalRights = stream.ReadByte();
                    stream.ReadByte();
                    Connected = true;
                    return LoginResponse.SuccessfulLogin;
                }
            }
            return LoginResponse.Unknown;
        }
        
        public void Update()
        {
            if (Connected)
            {
                if (client == null || !client.Connected)
                {
                    Connected = false;
                    if (OnDisconnect != null)
                    {
                        OnDisconnect();
                    }
                    return;
                }

                try
                {
                    while (client.Available > 0)
                    {
                        var stream = client.GetStream();
                        stream.ReadTimeout = 5000;

                        if (lastOpcode == -1)
                        {
                            if (client.Available < 1)
                            {
                                break;
                            }

                            var buffer = new byte[1];
                            stream.Read(buffer, 0, 1);

                            InBuffer.Position(0);
                            InBuffer.WriteBytes(buffer, 0, 1);
                            InBuffer.Position(0);
                            lastOpcode = InBuffer.ReadByte() - GameContext.InCipher.NextInt() & 0xFF;
                            lastSize = GameConstants.PacketSizes[lastOpcode];
                            Debug.Log("Received opcode: " + lastOpcode + "," + lastSize);
                        }

                        if (lastSize == -1 || lastSize == -2)
                        {
                            if (client.Available < 2)
                            {
                                break;
                            }

                            var buffer = new byte[2];
                            stream.Read(buffer, 0, lastSize == -1 ? 1 : 2);

                            InBuffer.Position(0);
                            InBuffer.WriteBytes(buffer, 0, lastSize == -1 ? 1 : 2);
                            InBuffer.Position(0);
                            lastSize = lastSize == -1 ? InBuffer.ReadUByte() : InBuffer.ReadUShort();

                            Debug.Log("Received size: " + lastOpcode + "," + lastSize);
                        }
                        
                        if (client.Available < lastSize)
                        {
                            break;
                        }

                        Debug.Log("Received pkt: " + lastOpcode + "," + lastSize);
                        var pbuffer = new byte[lastSize];
                        stream.Read(pbuffer, 0, pbuffer.Length);
                        var packet = new Packet(lastOpcode, pbuffer);
                        packet.Position(0);

                        PacketHandler handler;
                        if (PacketHandlers.TryGetValue(packet.Opcode, out handler))
                        {
                            handler.Handle(packet.Opcode, packet);
                        }

                        lastOpcode = -1;
                        lastSize = -1;
                    }

                    lastKeepAlive += 1;
                    if (lastKeepAlive >= 50)
                    {
                        lastKeepAlive = 0;
                        Write(new Packet(0));
                    }

                    FlushAll();
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    Connected = false;
                    if (OnDisconnect != null)
                    {
                        OnDisconnect();
                    }
                }
            }
        }
    }
}
