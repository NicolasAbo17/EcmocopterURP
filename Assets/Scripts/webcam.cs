using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class webcam : MonoBehaviour
{
    WebCamTexture webCamTexture;
    Renderer rendRenderer;

    [SerializeField] private float begin;
    [SerializeField] private float end;

    private void Start()
    {
        int find = -1;

        for (int i = 0; i < WebCamTexture.devices.Length; i++)
        {
            Debug.Log(WebCamTexture.devices[i].name);
            if (WebCamTexture.devices[i].name == "ZED-M")
            {
                find = i;
                break;
            }
        }
        WebCamDevice device;
        if (find == -1) {
            Debug.LogError("No hay zed");
        }
        else
        {
            device = WebCamTexture.devices[find];

            webCamTexture = new WebCamTexture(device.name);

            rendRenderer = GetComponent<Renderer>();
            rendRenderer.material.mainTexture = webCamTexture;
            webCamTexture.Play();

            // Adjust UV coordinates to hide the left half
            rendRenderer.material.mainTextureScale = new Vector2(end, end);

        }

    }
}
