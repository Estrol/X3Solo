#pragma warning disable IDE0010

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Estrol.X3Solo.Library;
using Estrol.X3Solo.Library.Packet;
using Estrol.X3Solo.Server.Data;

namespace Estrol.X3Solo.Server {
    public class Main {
        public readonly Configuration m_config;
        public bool Ready = false;
        private TCPServer m_server;
        private OJNList m_list;
        private MusicListCaching m_musiclist;
        public string Version;

        private readonly Dictionary<string, Client> clients;

        public delegate void ErrorEvent(object sender, Exception error);
        public event ErrorEvent OnError;

        public delegate void ClientErrorEvent(object sender);
        public event ClientErrorEvent OnCError;

        public Main() {
            m_config = new();
            clients = new();
        }

        public void Intialize(ClientCheck config) {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            try {
                Version = config.Version;

                m_list = OJNListDecoder.Decode(AppDomain.CurrentDomain.BaseDirectory + $"\\Image\\{config.OJNList}");
                m_musiclist = new(AppDomain.CurrentDomain.BaseDirectory + $"\\Image\\{config.OJNList}");
                m_server = new TCPServer(m_config.ServerPort);
                m_server.OnServerMessage += ServerMessage;
                m_server.Intialize();

                Ready = true;
            } catch (Exception e) {
                Log.Write("::Initialize -> {0}", e.Message);
                OnError?.Invoke(this, e);
            }
        }

        public void Stop() {
            m_server.Stop();
            m_server = null;
            m_list = null;

            Log.Write("Server Shutdown!");
            Ready = false;
        }

        public void SendMessage(string message) {
            using PacketBuffer buf = new();
            buf.Write((short)0);
            buf.Write((short)0x7e0);
            buf.Write(message);
            buf.SetLength();

            byte[] data = buf.ToArray();
            foreach (KeyValuePair<string, Client> itr in clients) {
                itr.Value.Send(data);
            }
        }

        public void ServerMessage(object o, Client client) {
            if (!clients.ContainsKey(client.UUID)) {
                client.PacketHandler = new(client, Version);

                clients.Add(client.UUID, client);
            }

            IPacket[] packets = client.PacketHandler.CreatePacket(client.Buffer);
            foreach (IPacket packet in packets) {
                switch (packet.Id) {
                    case Opcodes.Connect: {
                        client.Send(new byte[] {
                            0x04, 0x00, 0xf2, 0x03
                        });

                        break;
                    }

                    case Opcodes.PlanetConnect: {
                        client.Send(new byte[] {
                            0x04, 0x00, 0xf4, 0x03
                        });

                        break;
                    }

                    case Opcodes.Login: {
                        client.Send(new byte[] {
                            0x0c, 0x00, 0xf0, 0x03, 0x00, 0x00, 0x00, 0x00,
                            0xa4, 0x9d, 0x00, 0x00
                        });

                        break;
                    }

                    case Opcodes.PlanetLogin: {
                        client.Send(new byte[] {
                            0x08, 0x00, 0xe9, 0x03, 0x00, 0x00, 0x00, 0x00
                        });

                        break;
                    }

                    case Opcodes.RoomNameChange: {
                        string name = GetString(packet.Data);

                        using PacketBuffer buf = new();

                        buf.Write((short)0x00);
                        buf.Write((short)0xbb9);
                        buf.Write(name);
                        buf.SetLength();

                        client.Send(buf.ToArray());
                        break;
                    }

                    case Opcodes.Tutorial1: {
                        break;
                    }

                    case Opcodes.Tutorial2: {
                        break;
                    }

                    case Opcodes.Channel: {
                        using PacketBuffer buf = new();

                        buf.Write((short)0x00);
                        buf.Write(new byte[] {
                            0xEB, 0x03, 0x2C, 0x01, 0x00, 0x00, 0x00, 0x00
                        });

                        for (int i = 0; i < 20; i++) {
                            buf.Write((short)i);
                            buf.Write((short)20);
                            buf.Write((short)0);
                            buf.Write((short)0);
                            buf.Write((short)0);
                            buf.Write((short)1);
                            buf.Write((byte)0);
                        }

                        buf.SetLength();

                        client.Send(buf.ToArray());
                        break;
                    }

                    case Opcodes.EnterCH: {
                        byte[] data = new byte[] {
                            0x10, 0x00, 0xed, 0x03, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00
                        };

                        int iLevel = m_config.Rank;
                        byte[] bLevel = BitConverter.GetBytes(iLevel);

                        data[12] = bLevel[0];
                        data[13] = bLevel[1];
                        data[14] = bLevel[2];
                        data[15] = bLevel[3];

                        client.Send(data);
                        break;
                    }

                    case Opcodes.GetChar: {
                        using PacketBuffer buf = new();

                        buf.Write((short)0);
                        buf.Write((short)0x7d1);
                        buf.Write(0);
                        buf.Write(m_config.Name);
                        buf.Write((byte)m_config.Value("Gender"));
                        buf.Write(10);
                        buf.Write(100000); // mCash
                        buf.Write(0);
                        buf.Write(m_config.Value("Level"));
                        buf.Write(0);
                        buf.Write(0);
                        buf.Write(0);
                        buf.Write(0);
                        buf.Write(0);
                        buf.Write((byte)0);

                        // Character
                        buf.Write(m_config.Value("Instrument")); // 0
                        buf.Write(m_config.Value("Hair")); // 1
                        buf.Write(m_config.Value("Accessory")); // 2
                        buf.Write(m_config.Value("Glove")); // 3
                        buf.Write(m_config.Value("Necklace")); // 4
                        buf.Write(m_config.Value("Top")); // 5
                        buf.Write(m_config.Value("Pant")); // 6
                        buf.Write(m_config.Value("Glass")); // 7
                        buf.Write(m_config.Value("Earring")); // 8
                        buf.Write(m_config.Value("ClothAccessory")); // 9
                        buf.Write(m_config.Value("Shoe")); // 10
                        buf.Write(m_config.Value("Face")); // 11
                        buf.Write(m_config.Value("Wing")); // 12
                        buf.Write(m_config.Value("InstrumentAccessory")); // 13
                        buf.Write(m_config.Value("Pet")); // 14
                        buf.Write(m_config.Value("HairAccessory")); // 15

                        int[][] RingItem = { // Guessed Ring Item, Maybe invalid if change ItemData_China.dat
                            new []{ 156, 100000 }, // Mirror
                            new []{ 154, 100000 }, // Random
                            new []{ 152, 100000 }, // Panic
                            new []{ 150, 100000 }, // Hidden
                            new []{ 148, 100000 }, // Sudden
                            new []{ 146, 100000 } // Dark
                        };

                        for (int i = 0; i < 35; i++) {
                            if (i < RingItem.Length) {
                                int[] item = RingItem[i];

                                buf.Write(item[0]);
                            } else {
                                buf.Write(0);
                            }
                        }

                        buf.Write(RingItem.Length);
                        foreach (int[] item in RingItem) {
                            buf.Write(item[0]);
                            buf.Write(item[1]);
                        }

                        buf.SetLength();
                        client.Send(buf.ToArray());
                        break;
                    }

                    case Opcodes.LeaveCH: {
                        client.Send(new byte[] {
                            0x08, 00, 0xe6, 0x07, 0x00, 0x00, 0x00, 0x00
                        });
                        break;
                    }

                    case Opcodes.OJNList: {
                        if (m_musiclist.VerifyCache()) {
                            byte[] data = m_musiclist.LoadCache();

                            client.Send(data);
                        } else {
                            if (!m_musiclist.CreateCache()) {
                                Log.Write(m_musiclist.GetLastMessage());
                            }

                            byte[] data = m_musiclist.LoadCache();

                            client.Send(data);
                        }
                        break;
                    }

                    case Opcodes.ClientList: {
                        client.Send(new byte[] { 0x04, 0x00, 0xe9, 0x07 });
                        break;
                    }

                    case Opcodes.CreateRoom: {
                        byte mode = packet.Data[2];

                        Log.Write("CreateRoom -> Mode = {0}", mode == 0x1 ? "VS" : "Solo");
                        client.Send(new byte[] {
                            0x0d, 0x00, 0xd6, 0x07, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x00, 0x00, 0x00
                        });
                        break;
                    }

                    case Opcodes.GameStart: {
                        Random rnd = new();

                        using PacketBuffer buf = new();
                        buf.Write(new byte[] { 0x0c, 0x00, 0xab, 0x0f, 0x00, 0x00, 0x00, 0x00 });
                        buf.Write(rnd.Next(1, int.MaxValue));

                        client.Send(buf.ToArray());
                        break;
                    }

                    case Opcodes.ApplyRing: {
                        using PacketBuffer buf = new();
                        buf.Write((short)0);
                        buf.Write((short)0xfad);
                        buf.Write((byte)0);
                        if (client.Data != null) {
                            buf.Write(client.Data);
                        }

                        buf.SetLength();

                        client.Send(buf.ToArray());
                        break;
                    }

                    case Opcodes.GameQuit: {
                        using PacketBuffer buf = new();
                        buf.Write(new byte[] { 0x09, 0x00, 0xb6, 0x0f, 0x00 });
                        buf.Write(m_config.Level);

                        client.Send(buf.ToArray());
                        break;
                    }

                    case Opcodes.GamePing: {
                        short flag1 = BitConverter.ToInt16(packet.Data, 2);
                        short flag2 = BitConverter.ToInt16(packet.Data, 4);

                        using PacketBuffer buf = new();
                        buf.Write((short)0x0);
                        buf.Write((short)0xfaf);
                        buf.Write((byte)0x00);
                        buf.Write(flag1);
                        buf.Write(flag2);
                        buf.Write(new byte[] { // other player ig?
                            0xFF, 0xFF, 0xFF, 0xFF,
                            0xFF, 0xFF, 0xFF, 0xFF
                        });
                        buf.SetLength();

                        client.Send(buf.ToArray());
                        break;
                    }

                    case Opcodes.ScoreSubmit: {
                        ushort kool = BitConverter.ToUInt16(packet.Data, 2);
                        ushort great = BitConverter.ToUInt16(packet.Data, 4);
                        ushort bad = BitConverter.ToUInt16(packet.Data, 6);
                        ushort miss = BitConverter.ToUInt16(packet.Data, 8);
                        ushort maxcombo = BitConverter.ToUInt16(packet.Data, 10);
                        ushort jam = BitConverter.ToUInt16(packet.Data, 12);
                        ushort pass = BitConverter.ToUInt16(packet.Data, 14);
                        int score = BitConverter.ToInt32(packet.Data, 16);
                        short gem = 0;

                        // Accuracy
                        gem = (short)CalculateAccuracy(kool, great, bad, miss);

                        // Score
                        score = (short)CalculateScore(kool, great, bad, miss);

                        // Server accept the score
                        client.Send(new byte[] {
                            0x06, 0x00, 0xb1, 0x0f, 0x00, 0x01
                        });

                        // Show the leaderboard
                        using PacketBuffer buf = new();
                        buf.Write((short)0);
                        buf.Write((short)0xfb2);
                        buf.Write((byte)8); // Max users
                        buf.Write(0); // Slot
                        buf.Write(1); // Game Rank
                        buf.Write(kool);
                        buf.Write(great);
                        buf.Write(bad);
                        buf.Write(miss);
                        buf.Write(maxcombo);
                        buf.Write(jam);
                        buf.Write(score);
                        buf.Write(gem); // Accuracy

                        // rest part that 
                        buf.Write(new byte[] {
                            0x17, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0x00, 0x00, 0x01, 0x00, 0x01, 0x00,
                            0x00, 0x00, 0x00, 0x02, 0x00, 0x00,
                            0x00, 0x00, 0x03, 0x00, 0x00, 0x00,
                            0x00, 0x04, 0x00, 0x00, 0x00, 0x00,
                            0x05, 0x00, 0x00, 0x00, 0x00, 0x06,
                            0x00, 0x00, 0x00, 0x00, 0x07, 0x00,
                            0x00, 0x00, 0x00
                        });

                        buf.SetLength();
                        client.Send(buf.ToArray());
                        break;
                    }

                    case Opcodes.RoomPlrColorChange: {
                        byte color = packet.Data[2];

                        client.Send(new byte[] {
                            0x06, 0x00, 0xa5, 0x0f, 0x00, color
                        });
                        client.Send(new byte[] {
                            0x06, 0x00, 0xa9, 0x0f, 0x00, 0x01
                        });
                        break;
                    }

                    case Opcodes.LeaveRoom: {
                        client.Send(new byte[] {
                            0x08, 0x00, 0xbe, 0x0b, 0x00, 0x00, 0x00, 0x00
                        });
                        break;
                    }

                    case Opcodes.RoomBGChange: {
                        packet.FullData[2] = 0xa3;

                        client.Send(packet.FullData);
                        break;
                    }

                    case Opcodes.RoomRingChange: {
                        int totalRing = packet.Data[3];
                        if (totalRing > 3) {
                            Log.Write("RoomRingChange -> Unexpected ring count = {0}/3", totalRing);
                        }

                        byte[] ring_data = new byte[packet.Data.Length - 6];
                        client.Data = ring_data;

                        Buffer.BlockCopy(packet.Data, 6, ring_data, 0, packet.Data.Length - 6);

                        using PacketBuffer buf = new();
                        buf.Write((short)0);
                        buf.Write((short)0xfb8);
                        buf.Write((byte)totalRing);
                        buf.Write(ring_data);
                        buf.SetLength();

                        client.Send(buf.ToArray());
                        break;
                    }

                    case Opcodes.GetRoom: {
                        using (PacketBuffer buf = new()) {
                            buf.Write((short)0);
                            buf.Write((short)0x7db);
                            buf.Write(0);

                            buf.Write(m_config.Name);
                            buf.Write("X3SoloEmu");
                            buf.Write(1);
                            buf.SetLength();

                            client.Send(buf.ToArray());
                        }

                        using (PacketBuffer buf = new()) {
                            buf.Write((short)0);
                            buf.Write((short)0x7d3);
                            buf.Write(20);

                            for (int i = 0; i < 20; i++) {
                                buf.Write((byte)i);
                                buf.Write(new byte[11]);
                                buf.Write((byte)0xff);
                                buf.Write(new byte[9]);
                            }

                            buf.Write((byte)0xff);
                            buf.Write(new byte[11]);
                            buf.SetLength();

                            client.Send(buf.ToArray());
                        }

                        break;
                    }

                    case Opcodes.GetCash: {
                        client.Send(new byte[] {
                            0x08, 0x00, 0xa5, 0x13,
                            0x00, 0x00, 0x00, 0x00 // Set: 0 MCash
                        });
                        break;
                    }

                    case Opcodes.SetSongID: {
                        ushort SongID = BitConverter.ToUInt16(packet.Data, 2);
                        Log.Write("SetSongID -> SongID = {0}", SongID);

                        ushort p_len = (ushort)(packet.Data.Length + 2);
                        byte[] len = BitConverter.GetBytes(p_len);
                        byte[] data = len.Concat(packet.Data).ToArray();
                        data[2] = 0xa1;

                        client.Send(data);
                        break;
                    }

                    case Opcodes.LobbyChat: {
                        string msg = GetString(packet.Data);

                        using PacketBuffer buf = new();
                        buf.Write((short)0);
                        buf.Write((short)0x07dd);

                        char[] usr = m_config.Name.ToCharArray();
                        buf.Write(Encoding.UTF8.GetBytes(usr));
                        buf.Write((byte)0);

                        char[] cMsg = msg.ToCharArray();
                        buf.Write(Encoding.UTF8.GetBytes(cMsg));
                        buf.Write((byte)0);

                        buf.SetLength();

                        client.Send(buf.ToArray());
                        break;
                    }

                    case Opcodes.RoomChat: {
                        string msg = GetString(packet.Data);

                        using PacketBuffer buf = new();
                        buf.Write((short)0);
                        buf.Write((short)0xbc4);

                        char[] usr = m_config.Name.ToCharArray();
                        buf.Write(Encoding.UTF8.GetBytes(usr));
                        buf.Write((byte)0);

                        char[] cMsg = msg.ToCharArray();
                        buf.Write(Encoding.UTF8.GetBytes(cMsg));
                        buf.Write((byte)0);

                        buf.SetLength();

                        client.Send(buf.ToArray());
                        break;
                    }

                    case Opcodes.TCPPing: {
                        client.Send(new byte[] {
                            0x04, 0x00, 0x71, 0x17
                        });
                        break;
                    }

                    case Opcodes.Disconnect: {
                        try {
                            _ = clients.Remove(client.UUID);
                        } catch (Exception) { }

                        client.Socket.Disconnect(true);
                        return;
                    }

                    default: {
                        Log.Write("Default -> Unknown PacketId = {0}", (ushort)packet.Id);

                        using PacketBuffer buf = new();
                        buf.Write((short)0);
                        buf.Write((short)0x7e0);
                        buf.Write($"Unknown PacketId: {(ushort)packet.Id}");
                        buf.SetLength();

                        byte[] data = buf.ToArray();
                        client.Send(data);
                        break;
                    }
                }
            }

            client.Read();
        }

        public static uint CalculateScore(int kool, int great, int bad, int miss) {
            int max_notes = kool + great + bad + miss;
            if (max_notes == 0) {
                return 0;
            }

            double baseAcc = ((kool * 100.0) + (great * 99.0)) / (max_notes * 100.0);
            double baseScore = baseAcc * 1000000;
            double finalScore = baseScore > 900000 ? (baseScore * 9) - 8000000 : baseScore / 9;
            if (!IsClear(kool, great, bad, miss, max_notes)) {
                finalScore *= 0.8;
            }

            return (uint)finalScore;
        }

        public static bool IsClear(int kool, int good, int bad, int miss, int maxNotes) {
            return kool + good + bad + miss >= maxNotes - 1;
        }

        public static int CalculateAccuracy(int kool, int great, int bad, int miss) {
            int max_notes = kool + great + bad + miss;
            return max_notes == 0 ? 0 : (int)(((kool * 100.0) + (great * 99.0)) / (max_notes * 100.0));
        }

        public static string GetString(byte[] data) {
            int length = 0;
            for (int i = 0; i < data.Length; i++) {
                if (data[i + 2] == 0x00) {
                    length = i;
                    break;
                }
            }

            byte[] buf = new byte[length];
            Buffer.BlockCopy(data, 2, buf, 0, length);

            return Encoding.UTF8.GetString(buf);
        }

        public static string ToHexString(byte[] bData) {
            return BitConverter.ToString(bData).Replace("-", " ");
        }
    }
}