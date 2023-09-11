using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

using MixedUpSocket;

namespace MixedUp
{
    public enum Settings
    {
        Day,
        Night,
        City,
        Ocean,
        Rainy,
        Sunny,
        Begin
    }

    public class Server : MonoBehaviour
    {
        DTServer _server = null;
        //bool testingMode = true;
        protected void OnEnable()
        {
            _server = new DTServer(9002);
        }
        protected void OnDisable()
        {

            _server.Close();
            _server = null;
        }

        public void Send(byte id, byte val)
        {
            DTPacket pkt = new DTPacket(id, val);
            _server.Send(pkt);
        }

        public void Day()
        {
            Send((byte)Settings.Day, 1);
            Client.day = true;
            Client.night = false;
        }

        public void Night()
        {
            Send((byte)Settings.Night, 1);
            Client.night = true;
            Client.day = false;
        }

        public void City()
        {
            Send((byte)Settings.City, 1);
            Client.city = true;
            Client.ocean = false;
        }

        public void Ocean()
        {
            Send((byte)Settings.Ocean, 1);
            Client.ocean = true;
            Client.city = false;
        }

        public void Rain()
        {
            Send((byte)Settings.Rainy, 1);
            Client.rain = true;
            Client.sunny = false;
        }

        public void Sunny()
        {
            Send((byte)Settings.Sunny, 1);
            Client.sunny = true;
            Client.rain = false;
        }

        public void Begin()
        {
            if (Client.city == false && Client.ocean == false)
            {
                Debug.Log("Select the settings");
            }
            else
            {
                Send((byte)Settings.Begin, 1);
            }

        }

        void Update()
        {
        }


    }
}