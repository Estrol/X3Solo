using System;
using System.Net;
using System.Net.Sockets;

namespace Estrol.X3Solo.Library {
    public class TCPServer {
        private readonly int m_port;
        private Socket m_server;

        public delegate void ServerEventSender(object sender, Client state);
        public event ServerEventSender OnServerMessage;

        public TCPServer(int tcp_port) {
            m_port = tcp_port;
            m_server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Intialize() {
            m_server.Bind(new IPEndPoint(IPAddress.Any, m_port));
            m_server.Listen(m_port);
            Log.Write("Server now listen at port: {0}", m_port);

            m_server.BeginAccept(OnAsyncConnection, m_server);
        }

        public void Stop() {
            m_server.Close();
            m_server = null;
        }

        public void Read(Client client) {
            client.m_data = null;
            client.m_length = 0;
            client.m_raw = new byte[Client.MAXSIZE];

            client.Socket.BeginReceive(client.m_raw, 0, Client.MAXSIZE, SocketFlags.None, OnAsyncData, client);
        }

        public void Send(Client client, byte[] data, ushort length = 0) {
            if (length == 0)
                length = BitConverter.ToUInt16(data, 0);

            try {
                client.Socket.BeginSend(data, 0, length, SocketFlags.None, (IAsyncResult ar) => { }, client);
            } catch (Exception e) {
                HandleException(e);
            }
        }

        private void OnAsyncConnection(IAsyncResult ar) {
            try {
                Log.Write("A client connected");

                Socket socket = (Socket)ar.AsyncState;
                Client client = new Client() {
                    m_socket = socket.EndAccept(ar),
                    m_server = this,
                    m_raw = new byte[Client.MAXSIZE]
                };

                client.Socket.BeginReceive(client.m_raw, 0, Client.MAXSIZE, SocketFlags.None, OnAsyncData, client);
                m_server.BeginAccept(OnAsyncConnection, m_server);
            } catch (Exception e) {
                HandleException(e);
            }
        }

        private void OnAsyncData(IAsyncResult ar) {
            try {
                Client client = (Client)ar.AsyncState;
                client.m_length = BitConverter.ToUInt16(client.m_raw, 0);
                client.m_data = new byte[client.m_length];

                Buffer.BlockCopy(client.m_raw, 0, client.m_data, 0, client.m_length);
                client.m_raw = null;

                OnServerMessage?.Invoke(this, client);
            } catch (Exception e) {
                HandleException(e);
            }
        }

        private static void HandleException(Exception e) {
            if (e is ObjectDisposedException) {
                //Log.Write("[C# Exception] A thread tried to access disposed object.");
            } else if (e is SocketException err) {
                if (err.ErrorCode == 10054) {
                    Log.Write("[C# Exception] A thread tried to access socket that already disconnected");
                }
            } else {
                Log.Write("[C# Unhandled Exception] {0}\n{0}", e.Message, e.StackTrace);
            }
        }
    }
}
