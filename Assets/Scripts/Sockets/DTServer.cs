using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;
using System.Timers;
using System.Collections.Generic;


namespace MixedUpSocket
{

    public class DTServer 
    {

        private Socket _socket; //server socket accepting remove (ghost) client connections
        private List<DTGhostClient> ghostClients;   // Remote client list
        private readonly object DataLock = new object();

        public DTServer(int port)
        {

            string ip = DTClientBase.GetLocalIPAddress();
            ghostClients = new List<DTGhostClient>();
            try
            {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
                _socket.Listen(1);
                _socket.BeginAccept(new System.AsyncCallback(OnClientConnect), _socket);
            }
            catch (System.Exception)
            {
                if (_socket != null)
                {
                    _socket.Shutdown(SocketShutdown.Both);
                    _socket.Close();
                    _socket = null;
                }
            }
        }

        ~DTServer()
        {
            Close();
        }

        public void Close()
        {
            // Locked in case a client connects asynchronously before 
            lock (DataLock)
            {
                // Shutting down and closing the server socket under the DataLock prevents
                // clients reconnecting once we gracefully disconnect them (next).
                if (_socket != null)
                {
                    _socket.Close();
                    _socket = null;
                }

                // The server is no longer bound to or listening on the port.
                // Gracefully shutdown all connected ghost clients.
                foreach (DTGhostClient c in ghostClients)
                {
                    try
                    {
                        if (c != null)
                            c.Close();
                    }
                    catch (Exception)
                    {
                    }
                }

                ghostClients.Clear();
            }
        }

        public bool IsValid { get { return (_socket != null); } }


        private void OnClientConnect(IAsyncResult result)
        {
            if (_socket == null)
            {
                // Server socket that was accepting connections must have been deleted. 
                // HACK: call to socket.EndAccept(...) was not made.
                return;
            }

            try
            {
                // Add the socket to the client list.
                OnClientConnected(_socket.EndAccept(result));
            }
            catch (System.Exception)
            {
            }

            try
            {
                // Restart accept for next client connect.
                _socket.BeginAccept(new System.AsyncCallback(OnClientConnect), _socket);
            }
            catch (System.Exception)
            {
            }
        }

        protected virtual void OnClientConnected(Socket inClient)
        {
            DTGhostClient client = new DTGhostClient(this, inClient);

            // Locked because new DTClientBase client can be added to clientReads asynchronously.
            lock (DataLock)
            {
                ghostClients.Add(client);
            }

        }

        public virtual void Send(DTPacket pkt)
        {
            // Locked because clientReads can be added to asyncronously.
            lock (DataLock)
            {
                foreach (DTGhostClient sc in ghostClients)
                    sc.Send(pkt);
            }
        }


    }
}