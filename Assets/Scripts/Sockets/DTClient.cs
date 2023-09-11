using UnityEngine;
using System;
using System.Net.Sockets;
using System.Timers;
using System.Threading;
using System.Collections.Generic;

namespace MixedUpSocket
{
    public class DTClient : DTClientBase
    {
        string _serverIP = string.Empty;
        int _port = 9002;
        Queue<DTPacket> incoming = new Queue<DTPacket>();
        private readonly object DataLock = new object();

        bool testingMode = false;

        // Creates a new client with teh servers IP
        public DTClient(string ip, int p)
        {
            // Server Ip
            _serverIP = ip;
            // Server port
            _port = p;

            // Parses the server ip
            string[] subsip = _serverIP.Split('.');
            id = int.Parse(subsip[subsip.Length - 1]);

            ConnectAsync();
        }

        public override void Close()
        {
            //aTimer.Close();
            base.Close();
        }

        private void ConnectAsync()
        {
            try
            {
                Debug.Log("New Client");
                _socket.BeginConnect(_serverIP, _port, new AsyncCallback(OnConnected), null);
            }
            catch (SocketException )
            {
                // Server must have disconnected or WiFi was lost.
                if (IsConnected)
                    Disconnect();
            }
        }

        private void OnConnected(IAsyncResult AR)
        {
            try
            {
                // Complete the connection.
                _socket.EndConnect(AR);
                if (_socket.Connected)
                {
                    if (testingMode)
                    {
                        Debug.Log("DTClient connected to:" + _socket.RemoteEndPoint.ToString());
                    }

                    // Start async packet read.
                    BeginReceive();
                }
                else
                {
                    if (testingMode)
                    {
                        Debug.Log("DTClient not connected to:" + _socket.RemoteEndPoint.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                if (testingMode)
                {
                    Debug.Log("OnConnected returned: Did not connect. " + e.ToString());
                }

                if (_socket != null)
                {
                    // Try again in a second.
                    Thread.Sleep(1000);
                    ConnectAsync();
                }
            }
        }

        void Disconnect()
        {

            // Disconnect and reuse socket.
            try
            {

                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            }
            catch (Exception)
            {

            }

            ConnectAsync();
        }

        public override void Receive(DTPacket pkt, int serverid)
        {
            lock (DataLock)
            {
                incoming.Enqueue(pkt);
            }

        }

        public DTPacket GetPacket()
        {
            DTPacket retval = null;
            lock (DataLock)
            {
                if (incoming.Count > 0)
                {
                    retval = incoming.Dequeue();
                }
            }
            return retval;
        }

        public override void ReadException(Exception exception)
        {
            bool disconnectedOnRead = false;
            unchecked
            {
                // Note: unchecked scope seems to be the only way to get the hex error code from HResult.
                // See checked and unchecked.
                // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/checked-and-unchecked
                if (exception.HResult == (int)0x80004005)
                    disconnectedOnRead = true;
            }

            if (disconnectedOnRead)
            {
                if (testingMode)
                {
                    Debug.Log("DTClient disconnected during read");
                }
            }
            else
            {
                if (testingMode)
                {
                    Debug.Log("DTClient Unk ReadException: " + exception);
                }
                //Disconnect();
            }
        }
    }
}