using NewSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetLights : MonoBehaviour
{
    GameObject[] client;
    void Start()
    {
        client = GameObject.FindGameObjectsWithTag("Client");
        for (int i = 0; i < client.Length; i++)
        {
            client[i].GetComponent<NewClient>().LightHelper = this.GetComponentInChildren<LightHelper>();
            client[i].GetComponent<NewClient>().SetEnvironment();
        }
    }
}
