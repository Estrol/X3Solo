using Estrol.X3Solo.Library.Packet;
using Estrol.X3Solo.Library.Server;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Estrol.X3Solo.Library {
    public class Client {
        public static int MAXSIZE = 12000;
        public TCPServer m_server;
        public Socket m_socket;
        public XORStateful.Stateful m_stateful;
        public PacketHandler PacketHandler;

        public byte[] m_raw;
        public byte[] m_data;
        public ushort m_length;
        public string UUID;
        public byte[] Data { set; get; }
        public byte[] Buffer => m_data;
        public ushort Length => m_length;
        public Socket Socket => m_socket;
        public string IPAddr {
            get {
                IPEndPoint ip = m_socket.RemoteEndPoint as IPEndPoint;

                return ip.Address.ToString();
            }
        }

        public void Read() {
            m_server.Read(this);
        }

        public void Send(ushort length) {
            m_server.Send(this, Buffer, length);
        }

        public void Send(byte[] data) {
            m_server.Send(this, data);
        }

        public void Send(byte[] data, ushort length) {
            m_server.Send(this, data, length);
        }

        public Client() {
            UUID = Guid.NewGuid().ToString();
        }
    }
}
