using System.Net.Sockets;
using System;
using System.Diagnostics;
using UnityEngine;
using System.Collections;
using Debug = UnityEngine.Debug;
using MixedUp;


namespace MixedUpSocket
{
    public class DTGhostClient : DTClientBase
    {
        DTServer _server = null;

        //// Initialize with awakeRecieved true.
        //// First actual awake recieved starts server responsive verification.
        //long _sendTicks;
        //long _offsetMSecs;
        //long _firstOffsetMSecs = 0;
        public int Id { get { return id; } }

        public DTGhostClient(DTServer svr, Socket skt) : base(skt)
        {
            _server = svr;

            //_awakeRecieved = true; // Just in case a check from server is done first.
            //_offsetMSecs = -1; // Setting negative indicates invalid because calculated offset is always positive. 

//            _server.NotifyConnected(id);
 //           BeginReceive();
        }

        public override void Close()
        {
            bool socketNull = (_socket == null);

            if (_socket == null)
                Debug.Log("DTGhostClient: Close(): _socket already null");
            else if (IsConnected)
                Debug.Log("DTGhostClient: Close(): _socket connected");
            else
                Debug.Log("DTGhostClient: Close(): _socket disconnected");

            // Disconnecting ghost prevents sending graceful disconnect to client.
            if (IsConnected)
                _socket.Disconnect(false);

            base.Close();
        }

        public override void Receive(DTPacket pkt, int serverid)
        {
        }

        public override void ReadException(Exception exception)
        {
            // When a read exception occurs in a ghost client this client needs to be closed.
            Debug.Log("DTGhostClient: ReadException ungraceful disconnect: " + exception);
            Close();
        }
    }
}