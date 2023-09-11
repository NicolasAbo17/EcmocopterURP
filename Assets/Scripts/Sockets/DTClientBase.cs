
using System.Net.Sockets;
using System.Net;
using System;
using Debug = UnityEngine.Debug;
using UnityEngine.Events;

namespace MixedUpSocket
{
    public interface IRecieve
    {
        // If ip is 192.168.1.X, ID is set to X.
        void Receive(DTPacket pkt, int id);
    }

    public abstract class DTClientBase : IRecieve
    {

        protected string ip = string.Empty;
        protected int id = 0;
        protected Socket _socket = null;

        byte[] _receiveBuffer = new byte[1024];
        byte[] _sendBuffer = new byte[1024];
        byte[] _pktBuffer = new byte[1024];
        static public string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    //hintText.text = ip.ToString();
                    return ip.ToString();
                }
            }
            throw new System.Exception("No network adapters with an IPv4 address in the system!");
        }

        public bool IsConnected
        {
            get
            {
                if (_socket != null)
                    return _socket.Connected;
                return false;
            }
        }

        public bool IsValid
        {
            get
            {
                return (_socket != null);
            }
        }

        // DTClient base constructor: DTClients create sockets and attempt to connect to some server.
        public DTClientBase()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            _sendBuffer[0] = 0x3C;
            _sendBuffer[1] = 0x3C;
        }

        // DTGhostClient base constructor: DTGhostClients are already created and connected to a remote client.
        public DTClientBase(Socket socket)
        {
            _socket = socket;

            // Set ID to remote client
            {
                string ipaddr = _socket.RemoteEndPoint.ToString();
                string[] subs = ipaddr.Split(':');
                ip = subs[0];
                string[] subsip = ip.Split('.');
                id = int.Parse(subsip[subsip.Length - 1]);
            }

            _sendBuffer[0] = 0x3C;
            _sendBuffer[1] = 0x3C;

        }

        public virtual void Close()
        {
            if (_socket != null)
            {
                Debug.Log("DTClientBase::Close()");
                if (_socket.Connected)
                {
                    //Debug.Log("DTClientBase: Sending disconnect on Close");
                    //Send(new DisconnectedPacket(id));

                    Debug.Log("DTClientBase: Shutdown on connected");
                    _socket.Shutdown(SocketShutdown.Both);
                }

                _socket.Close();
                _socket = null;
            }
        }

        protected void BeginReceive()
        {
            if (_socket != null)
            {
                _socket.BeginReceive(_receiveBuffer, 0, _receiveBuffer.Length, SocketFlags.None, new AsyncCallback(OnReceive), this);
            }
            else
            {
                UnityEngine.Debug.Log("Socket invalid on BeginReceive");
            }
        }

        enum PktState { Header1, Header2, Type, Value, Footer1, Footer2 };
        PktState pktState = PktState.Header1;

        int dataCopied = 0;
        int bufferCopied = 0;
        int bufferReceived = 0;
        byte type = 0;
        byte value = 0;
        byte length = 0;
        bool CopyData()
        {
            int bufferAvailable = bufferReceived - bufferCopied;
            int dataBytesNeeded = length - dataCopied;

            if (bufferAvailable < dataBytesNeeded)
            {
                Buffer.BlockCopy(_receiveBuffer, bufferCopied, _pktBuffer, dataCopied, bufferAvailable);
                dataCopied += bufferAvailable;
                bufferCopied += bufferAvailable;
                return false;
            }

            Buffer.BlockCopy(_receiveBuffer, bufferCopied, _pktBuffer, dataCopied, dataBytesNeeded);
            dataCopied += dataBytesNeeded;
            bufferCopied += dataBytesNeeded;
            return true;
        }
        void OnReceive(IAsyncResult result)
        {
            if (_socket == null)
            {
                if (result.IsCompleted)
                    _socket.EndReceive(result);
                UnityEngine.Debug.Log("Socket invalid in OnRecieve ");
                return;
            }
            if (!_socket.Connected)
            {
                if (result.IsCompleted)
                    _socket.EndReceive(result);
                UnityEngine.Debug.Log("Socket no longer connected in OnRecieve ");
                return;
            }

            try
            {
                if (result.IsCompleted)
                {
                    //Check how many bytes are received and call EndReceive to finalize handshake
                    bufferReceived = _socket.EndReceive(result);
                    // Debug.Log("Recieved:" + bufferReceived);

                    bufferCopied = 0;
                    while (bufferCopied < bufferReceived)
                    {
                        switch (pktState)
                        {
                            case PktState.Header1:
                                if (_receiveBuffer[bufferCopied] == 0x3C)
                                    pktState++;
                                bufferCopied++;
                                break;
                            case PktState.Header2:
                                if (_receiveBuffer[bufferCopied] == 0x3C)
                                    pktState++;
                                else
                                {
                                    pktState = PktState.Header1;
                                    Debug.Log("Header2 failed:");
                                }
                                bufferCopied++;
                                break;
                            case PktState.Type:
                                type = _receiveBuffer[bufferCopied];
                                pktState++;
                                bufferCopied++;
                                break;
                            case PktState.Value:
                                value = _receiveBuffer[bufferCopied];
                                pktState++;
                                bufferCopied++;
                                break;
                            case PktState.Footer1:
                                if (_receiveBuffer[bufferCopied] == 0x3E)
                                    pktState++;
                                else
                                {
                                    pktState = PktState.Header1;
                                    Debug.Log("Footer1 failed:");
                                }
                                bufferCopied++;
                                break;
                            case PktState.Footer2:
                                if (_receiveBuffer[bufferCopied] == 0x3E)
                                {
                                    Receive(new DTPacket(type, value), id);
                                }
                                else
                                {
                                    Debug.Log("Footer2 failed:");
                                }
                                pktState = PktState.Header1;
                                bufferCopied++;
                                break;
                        }
                    }

                    // Start receiving again.
                    // Check for connection in the event that a DisconnectPacket was recieved and processed above.
                    if (_socket != null && _socket.Connected)
                        BeginReceive();
                }
                else
                {
                    // Disconnect
                }
            }
            catch (Exception e)
            {
                ReadException(e);
            }
        }

        public abstract void Receive(DTPacket pkt, int id);
        public abstract void ReadException(Exception e);

        public virtual void Send(DTPacket pkt)
        {
            if (_socket != null)
            {
                _sendBuffer[0] = 0x3C;
                _sendBuffer[1] = 0x3C;
                _sendBuffer[2] = pkt.Id();
                _sendBuffer[3] = pkt.Val();
                _sendBuffer[4] = 0x3E;
                _sendBuffer[5] = 0x3E;

                SocketAsyncEventArgs socketAsyncData = new SocketAsyncEventArgs();
                socketAsyncData.SetBuffer(_sendBuffer, 0, 6);
                _socket.SendAsync(socketAsyncData);
            }
        }

    }
}