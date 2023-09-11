#define USING_CLIENT_WRAPPER

using UnityEngine;
using MixedUpSocket;
using UnityEngine.SceneManagement;
//using System.Diagnostics;

#if USING_CLIENT_WRAPPER
namespace MixedUp
{
    public class Client : MonoBehaviour
    {
        public DTClient _client;

        public static bool day = false;
        public static bool night = false;

        public static bool city = false;
        public static bool ocean = false;


        public static bool rain = false;
        public static bool sunny = false;
        protected void OnEnable()
        {
            UnityEngine.Debug.Log("Client enabled");
            
            // Creates a new client with the servers IP
            _client = new DTClient("192.168.1.6", 9002);
        }
        protected void OnDisable()
        {
            UnityEngine.Debug.Log("Client disabled");
           
            // Closes client
            _client.Close();
            _client = null;
        }

        private void Update()
        {
            DTPacket pkt = _client.GetPacket();
            // Waits for the server to send a packet
            while (pkt != null)
            {
                // Goes through the packets to see which one was sent
                byte type = pkt.Id();
                byte val = pkt.Val();

                switch ((Settings)type)
                {
                    case Settings.Day:
                        day = true;
                        night = false;
                        Debug.Log("TYPE: " + Settings.Day + " VAL: " + val.ToString());
                        break;
                    case Settings.Night:
                        night = true;
                        day = false;
                        Debug.Log("TYPE: " + Settings.Night + " VAL: " + val.ToString());
                        break;
                    case Settings.City:
                        city = true;
                        ocean = false;
                        Debug.Log("TYPE: " + Settings.City + " VAL: " + val.ToString());
                        break;
                    case Settings.Ocean:
                        ocean = true;
                        city = false;
                        Debug.Log("TYPE: " + Settings.Ocean + " VAL: " + val.ToString());
                        break;
                    case Settings.Rainy:
                        Debug.Log("TYPE: " + Settings.Rainy + " VAL: " + val.ToString());
                        break;
                    case Settings.Sunny:
                        Debug.Log("TYPE: " + Settings.Sunny + " VAL: " + val.ToString());
                        break;
                    case Settings.Begin:
                        Debug.Log("TYPE: " + Settings.Begin + " VAL: " + val.ToString());
                        SceneManager.LoadScene("TestScene");
                        break;
                    default:
                        break;
                }
 
                pkt = _client.GetPacket();
            }

        }

    }

}
#endif