using NewSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnMeOnTemp : MonoBehaviour
{
    public GameObject[] ControlPanel;
    public GameObject[] manager;
    public GameObject[] controllers;    
    public Camera targetCamera;   
    // Start is called before the first frame update 
 
    public void TurnOn()
    {
        ControlPanel = GameObject.FindGameObjectsWithTag("ControlPanel");     
        manager = GameObject.FindGameObjectsWithTag("CameraRig");
        controllers = GameObject.FindGameObjectsWithTag("Controllers");
        targetCamera = Camera.main;

        targetCamera.clearFlags = CameraClearFlags.Skybox;

        for (int i = 0; i < manager.Length; i++)
        {
            manager[i].GetComponent<OVRManager>().isInsightPassthroughEnabled = false;          
        }

        for (int i = 0; i < controllers.Length; i++)
        {
            controllers[i].SetActive(false);
        }
      
        for (int i = 0; i < ControlPanel.Length; i++)
        {          
            ControlPanel[i].SetActive(false);
        }
    }
}
