using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NewSetting
{
    public class NewClient : MonoBehaviour
    {
        public LightHelper LightHelper;

        public static bool day = false;
        public static bool night = false;

        public static bool city = false;
        public static bool ocean = false;


        public static bool rainy = false;
        public static bool sunny = false;

        public Material[] skyboxes;
        public GameObject sun;
        public GameObject rain;
        public GameObject heliLight;
        public Color color1;
        public Color color2;
        public Color color3;
        public Color color4;

        PhotonView view;

        private void Start()
        {
            view = PhotonView.Get(this);
        }

        [PunRPC]
        public void SetDay()
        {
            day = true;
            night = false;
            SetEnvironment();
            Debug.Log("Day, true");
        }
        [PunRPC]
        public void SetNight()
        {
            night = true;
            day = false;
            SetEnvironment();
            Debug.Log("Night, true");
        }
        [PunRPC]
        public void SetCity()
        {
            city = true;
            ocean = false;
            SetEnvironment();
            Debug.Log("City, true");
        }
        [PunRPC]
        public void SetOcean()
        {
            ocean = true;
            city = false;
            SetEnvironment();
            Debug.Log("Ocean, true");
        }
        [PunRPC]
        public void SetRainy()
        {
            rainy = true;
            sunny = false;
            SetEnvironment();
            Debug.Log("Rainy, true");
        }
        [PunRPC]
        public void SetSunny()
        {
            sunny = true;
            rainy = false;
            SetEnvironment();
            Debug.Log("Sunny, true");
        }

        public void SetEnvironment()
        {
            if (sunny)
            {
                // Rain off
                rain.gameObject.SetActive(false);
                if (day) // Daytime 
                {
                    RenderSettings.skybox = skyboxes[0];
                    RenderSettings.ambientIntensity = 1.0f;
                    RenderSettings.fogColor = color1;
                    RenderSettings.fogMode = FogMode.Linear;
                    RenderSettings.fogStartDistance = 1500.0f;
                    RenderSettings.fogEndDistance = 7000.0f;
                    sun.gameObject.SetActive(true);
                    // Light off
                    heliLight.gameObject.SetActive(false);
                }
                else // Nighttime
                {
                    RenderSettings.skybox = skyboxes[1];
                    RenderSettings.ambientIntensity = 2.8f;
                    RenderSettings.fogColor = color2;
                    RenderSettings.fogMode = FogMode.Linear;
                    RenderSettings.fogStartDistance = 1500.0f;
                    RenderSettings.fogEndDistance = 7000.0f;
                    sun.gameObject.SetActive(false);
                    // Light on
                    heliLight.gameObject.SetActive(true);
                }
            }
            else
            {
                // Rain on
                rain.gameObject.SetActive(true);
                if (day) // Daytime
                {
                    RenderSettings.skybox = skyboxes[2];
                    RenderSettings.ambientIntensity = 1.0f;
                    RenderSettings.fogColor = color3;
                    RenderSettings.fogMode = FogMode.Exponential;
                    RenderSettings.fogDensity = 0.0004f;
                    sun.gameObject.SetActive(false);
                    // Light on
                    heliLight.gameObject.SetActive(true);
                }
                else // Nighgttime
                {
                    RenderSettings.skybox = skyboxes[3];
                    RenderSettings.ambientIntensity = 2.2f;
                    RenderSettings.fogColor = color4;
                    RenderSettings.fogMode = FogMode.Exponential;
                    RenderSettings.fogDensity = 0.0004f;
                    sun.gameObject.SetActive(false);
                    // Light on
                    heliLight.gameObject.SetActive(true);
                }
            }
            DynamicGI.UpdateEnvironment();
        }

        [PunRPC]
        public void SetLightColor(int value)
        {
            LightHelper.SetColor(value);
            LightHelper.PlayClick(value);
        }

        public void LightColor(int value)
        {
            view.RPC("SetLightColor", RpcTarget.Others, value);
        }

        [PunRPC]
        public void SetLightIntensity(int value)
        {
            LightHelper.SetIntensity(value);
            LightHelper.PlayClick(value);
        }

        public void LightIntensity(int value)
        {
            view.RPC("SetLightIntensity", RpcTarget.Others, value);
        }
    }
}
