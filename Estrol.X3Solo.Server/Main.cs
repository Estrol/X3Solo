using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Estrol.X3Solo.Library;
using Estrol.X3Solo.Server.Data;

namespace Estrol.X3Solo.Server {
    public class Main {
        public readonly Configuration m_config;
        public bool Ready = false;
        private TCPServer m_server;
        private OJNList m_list;
        public string Version;

        private readonly Dictionary<string, Client> clients;
        public delegate void ErrorEvent(object sender, Exception error);
        public event ErrorEvent OnError;

        public Main() {
            m_config = new();
            clients = new();
        }

        public void Intialize(ClientCheck config) {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            try {
                Version = config.Version;

                m_list = OJNListDecoder.Decode(AppDomain.CurrentDomain.BaseDirectory + $"\\Image\\{config.OJNList}");
                m_server = new TCPServer(m_config.ServerPort);
                m_server.OnServerMessage += ServerMessage;
                m_server.Intialize();

                Ready = true;
            } catch (Exception e) {
                Log.Write("ERR: EBFC99 <{0}>", e.Message);
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
            EBuffer buf = new();
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
                clients.Add(client.UUID, client);
            }

            Message message = new(Version, client.Buffer);

            switch (message.Opcode) {
                case Packets.Connect: {
                    client.Send(new byte[] {
                        0x04, 0x00, 0xf2, 0x03
                    });

                    break;
                }

                case Packets.PlanetConnect: {
                    client.Send(new byte[] {
                        0x04, 0x00, 0xf4, 0x03
                    });

                    break;
                }

                case Packets.Login: {
                    client.Send(new byte[] {
                        0x0c, 0x00, 0xf0, 0x03, 0x00, 0x00, 0x00, 0x00,
                        0xa4, 0x9d, 0x00, 0x00
                    });

                    break;
                }

                case Packets.PlanetLogin: {
                    client.Send(new byte[] {
                         0x08, 0x00, 0xe9, 0x03, 0x00, 0x00, 0x00, 0x00
                    });

                    break;
                }

                case Packets.RoomNameChange: {
                    byte[] _data = new byte[message.data.Length - 2];
                    Buffer.BlockCopy(message.data, 2, _data, 0, message.data.Length - 2);
                    string name = GetString(_data);

                    using var buf = new EBuffer();

                    buf.Write((short)0x00);
                    buf.Write((short)0xbb9);
                    buf.Write(name);
                    buf.SetLength();

                    client.Send(buf.ToArray());
                    break;
                }

                case Packets.Tutorial1: {
                    break;
                }

                case Packets.Tutorial2: {
                    break;
                }

                case Packets.Channel: {
                    using var buf = new EBuffer();

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

                case Packets.EnterCH: {
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

                case Packets.GetChar: {
                    using var buf = new EBuffer();
                    buf.Write((short)0);
                    buf.Write((short)0x7d1);
                    buf.Write(0);
                    buf.Write(m_config.Name);
                    buf.Write((byte)m_config.Value("Gender"));

                    buf.Write(new byte[] { 0xC6, 0xF5, 0x02, 0x00, 0x96, 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
                    buf.Write(m_config.Level);
                    buf.Write(new byte[] {
                        0x54, 0x03, 0x00, 0x00, 0xAF, 0x01, 0x00, 0x00, 0x71, 0x02, 0x00, 0x00, 0xBD, 0x30, 0x05, 0x00,
                        0x00, 0x00, 0x00, 0x00, 0x00
                    });

                    int[] chars = m_config.Character;
                    for (int i = 0; i < chars.Length; i++) {
                        buf.Write(chars[i]);
                    }

                    buf.Write(new byte[] {
                        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x99, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                        0x00, 0x00, 0x00, 0x00, 0x9B, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                        0x95, 0x00, 0x00, 0x00, 0x9D, 0x00, 0x00, 0x00, 0x93, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                        0x97, 0x00, 0x00, 0x00, 0x9F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x12, 0x00, 0x00, 0x00, 0xEF, 0x00, 0x00, 0x01,
                        0x09, 0x01, 0x1E, 0x01, 0x40, 0x01, 0x4E, 0x01, 0x50, 0x01, 0x51, 0x01, 0x57, 0x01, 0x59, 0x01,
                        0x84, 0x01, 0x8A, 0x01, 0x93, 0x01, 0xBB, 0x01, 0xDD, 0x01, 0xE1, 0x01, 0xE2, 0x01, 0xE3, 0x01,
                        0xE4, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                        0x07, 0x00, 0x00, 0x00, 0x9D, 0x00, 0x00, 0x00, 0xE7, 0x03, 0x00, 0x00, 0x9B, 0x00, 0x00, 0x00,
                        0xE7, 0x03, 0x00, 0x00, 0x99, 0x00, 0x00, 0x00, 0xE7, 0x03, 0x00, 0x00, 0x97, 0x00, 0x00, 0x00,
                        0xE7, 0x03, 0x00, 0x00, 0x93, 0x00, 0x00, 0x00, 0xE7, 0x03, 0x00, 0x00, 0x95, 0x00, 0x00, 0x00,
                        0xE7, 0x03, 0x00, 0x00, 0x9F, 0x00, 0x00, 0x00, 0xE7, 0x03, 0x00, 0x00
                    });

                    buf.SetLength();
                    client.Send(buf.ToArray());
                    break;
                }

                case Packets.LeaveCH: {
                    client.Send(new byte[] {
                        0x08, 00, 0xe6, 0x07, 0x00, 0x00, 0x00, 0x00
                    });
                    break;
                }

                case Packets.OJNList: {
                    OJN[] songs = m_list.GetHeaders();

                    using var buf = new EBuffer();
                    short length = (short)(6 + (songs.Length * 12) + 12);
                    buf.Write(length);
                    buf.Write((short)0x0fbf);
                    buf.Write((short)m_list.Count);

                    foreach (OJN ojn in songs) {
                        buf.Write((short)ojn.Id);
                        buf.Write((short)ojn.NoteCountEx);
                        buf.Write((short)ojn.NoteCountNx);
                        buf.Write((short)ojn.NoteCountHx);
                        buf.Write(0);
                    }

                    buf.Write((long)0);
                    buf.Write(0);

                    client.Send(buf.ToArray());
                    break;
                }

                case Packets.ClientList: {
                    client.Send(new byte[] { 0x04, 0x00, 0xe9, 0x07 });
                    break;
                }

                case Packets.CreateRoom: {
                    byte mode = message.data[2];

                    Log.Write("Create a {0} room", mode == 0x1 ? "VS" : "Solo");
                    client.Send(new byte[] {
                        0x0d, 0x00, 0xd6, 0x07, 0x00, 0x00, 0x00, 0x00,
                        0x00, 0x00, 0x00, 0x00, 0x00
                    });
                    break;
                }

                case Packets.GameStart: {
                    client.Send(new byte[] {
                        0x0c, 0x00, 0xab, 0x0f, 0x00, 0x00, 0x00, 0x00,
                        0x093, 0x21, 0x74, 0x025
                    });
                    break;
                }

                case Packets.ApplyRing: {
                    using var buf = new EBuffer();
                    buf.Write((short)0);
                    buf.Write((short)0xfad);
                    buf.Write((byte)0);
                    buf.Write(client.Data);
                    buf.SetLength();

                    client.Send(buf.ToArray());
                    break;
                }

                case Packets.GameQuit: {
                    using var buf = new EBuffer();
                    buf.Write(new byte[] { 0x09, 0x00, 0xb6, 0x0f, 0x00 });
                    buf.Write(m_config.Level);

                    client.Send(buf.ToArray());
                    break;
                }

                case Packets.GamePing: {
                    Log.Write(ToHexString(message.fullD));

                    short flag1 = BitConverter.ToInt16(message.data, 2);
                    short flag2 = BitConverter.ToInt16(message.data, 4);

                    using var buf = new EBuffer();
                    buf.Write((short)0x0);
                    buf.Write((short)0xfaf);
                    buf.Write((byte)0x00);
                    buf.Write(flag1);
                    buf.Write(flag2);
                    buf.Write(new byte[] {
                        0xFF, 0xFF, 0xFF, 0xFF,
                        0xFF, 0xFF, 0xFF, 0xFF
                    });
                    buf.SetLength();

                    client.Send(buf.ToArray());
                    break;
                }

                case Packets.ScoreSubmit: {
                    ushort kool = BitConverter.ToUInt16(message.data, 2);
                    ushort great = BitConverter.ToUInt16(message.data, 4);
                    ushort bad = BitConverter.ToUInt16(message.data, 6);
                    ushort miss = BitConverter.ToUInt16(message.data, 8);
                    ushort maxcombo = BitConverter.ToUInt16(message.data, 10);
                    ushort jam = BitConverter.ToUInt16(message.data, 12);
                    ushort pass = BitConverter.ToUInt16(message.data, 14);
                    int score = BitConverter.ToInt32(message.data, 16);
                    short gem = 0;


                    gem = (short)CalculateAccuracy(kool, great, bad, miss);
                    Log.Write($"Accuracy: {gem}");

                    // Server accept the score
                    client.Send(new byte[] {
                        0x06, 0x00, 0xb1, 0x0f, 0x00, 0x01
                    });

                    // Show the leaderboard
                    using var buf = new EBuffer();
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

                    // Lazy part
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

                case Packets.RoomPlrColorChange: {
                    byte color = message.data[2];

                    client.Send(new byte[] {
                        0x06, 0x00, 0xa5, 0x0f, 0x00, color
                    });
                    client.Send(new byte[] {
                        0x06, 0x00, 0xa9, 0x0f, 0x00, 0x01
                    });
                    break;
                }

                case Packets.LeaveRoom: {
                    client.Send(new byte[] {
                        0x08, 0x00, 0xbe, 0x0b, 0x00, 0x00, 0x00, 0x00
                    });
                    break;
                }

                case Packets.RoomBGChange: {
                    message.fullD[2] = 0xa3;

                    client.Send(message.fullD);
                    break;
                }

                case Packets.RoomRingChange: {
                    int totalRing = message.data[3];
                    if (totalRing > 3) {
                        Log.Write("Total ring expected to get max 3 but got more than 3 which {0}", totalRing);
                    }

                    byte[] ring_data = new byte[message.data.Length - 6];
                    client.Data = ring_data;

                    Buffer.BlockCopy(message.data, 6, ring_data, 0, message.data.Length - 6);

                    using var buf = new EBuffer();
                    buf.Write((short)0);
                    buf.Write((short)0xfb8);
                    buf.Write((byte)totalRing);
                    buf.Write(ring_data);
                    buf.SetLength();

                    client.Send(buf.ToArray());
                    break;
                }

                case Packets.GetRoom: {
                    using (var buf = new EBuffer()) {
                        buf.Write((short)0);
                        buf.Write((short)0x7db);
                        buf.Write(0);

                        buf.Write(m_config.Name);
                        buf.Write("x3emuuser");
                        buf.Write(1);
                        buf.SetLength();

                        client.Send(buf.ToArray());
                    }

                    using (var buf = new EBuffer()) {
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

                case Packets.Timest: {
                    client.Send(new byte[] {
                        0x08, 0x00, 0xa5, 0x13, 0xc6,
                        0xf5, 0x02, 0x00
                    });
                    break;
                }

                case Packets.SetSongID: {
                    ushort SongID = BitConverter.ToUInt16(message.data, 2);
                    Log.Write("Set RoomOJNID: {0}", SongID);

                    ushort p_len = (ushort)(message.data.Length + 2);
                    byte[] len = BitConverter.GetBytes(p_len);
                    byte[] data = len.Concat(message.data).ToArray();
                    data[2] = 0xa1;

                    client.Send(data);
                    break;
                }

                case Packets.LobbyChat: {
                    var msg = GetString(message.data);

                    using var buf = new EBuffer();
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

                case Packets.RoomChat: {
                    var msg = GetString(message.data);

                    using var buf = new EBuffer();
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

                case Packets.TCPPing: {
                    client.Send(new byte[] {
                        0x04, 0x00, 0x71, 0x17
                    });
                    break;
                }

                case Packets.Disconnect: {
                    try {
                        clients.Remove(client.UUID);
                    } catch (Exception) {}
                    client.Socket.Disconnect(true);
                    break;
                }

                default: {
                    byte[] opcode = new byte[2];
                    Buffer.BlockCopy(message.data, 0, opcode, 0, 2);

                    string msg = null;
                    if (message.data.Length > 2) {
                        byte[] mData = new byte[message.data.Length - 2];
                        Buffer.BlockCopy(message.data, 2, mData, 0, message.data.Length - 2);

                        msg = ToHexString(mData);
                    }

                    Log.Write("Please report - Unhandled opcode");
                    Log.Write("Code: {0}", message._opcode.ToString("X4"));
                    Log.Write("Data: {0}", msg ?? "Empty");

#if RELEASE
                    InvalidOpcodeException err = new(message._opcode, message.fullD);
                    OnError?.Invoke(this, err);
                    break;
#else
                    break;
#endif
                }
            }

            client.Read();
        }

        public static int CalculateAccuracy(int kool, int great, int bad, int miss) {
            int max_notes = kool + great + bad + miss;
            if (max_notes == 0)
                return 0;

            return (int)(100 * ((kool + (great * 0.33) + (bad * 0.166)) / max_notes)); 
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
