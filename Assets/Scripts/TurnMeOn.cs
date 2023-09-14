using NewSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnMeOn : MonoBehaviour
{
    public GameObject Helicopter;
    public GameObject Ancor;
    public GameObject Menu;
    public GameObject[] ControlPanel;
    public GameObject[] Environment;
    public GameObject[] manager;
    public GameObject[] controllers;
    public GameObject[] ancor;
    public Camera targetCamera;
    public GameObject[] client;
    public GameObject[] Mover;
    // Start is called before the first frame update 

    public void TurnOn()
    {
        ControlPanel = GameObject.FindGameObjectsWithTag("ControlPanel");
        Environment = GameObject.FindGameObjectsWithTag("Environment");
        manager = GameObject.FindGameObjectsWithTag("CameraRig");
        controllers = GameObject.FindGameObjectsWithTag("Controllers");
        Mover = GameObject.FindGameObjectsWithTag("Mover");
        targetCamera = Camera.main;

        targetCamera.clearFlags = CameraClearFlags.Skybox;

        GameObject heli = Instantiate(Helicopter, new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z), new Quaternion(0, this.transform.rotation.y, 0, this.transform.rotation.w));

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
            SharedAnchorControlPanel shared = ControlPanel[i].GetComponent<SharedAnchorControlPanel>();
            if (shared._isCreateMode)
            {
                shared.OnCreateModeButtonPressed();
            }
            ControlPanel[i].SetActive(false);
        }


        for (int i = 0; i < Environment.Length; i++)
        {
            for (int y = 0; y < Environment[i].transform.childCount; y++)
            {
                Environment[i].transform.GetChild(y).gameObject.SetActive(true);
            }
        }

        client = GameObject.FindGameObjectsWithTag("Client");
        for (int i = 0; i < client.Length; i++)
        {
            client[i].GetComponent<NewClient>().LightHelper = heli.GetComponentInChildren<LightHelper>();
            client[i].GetComponent<NewClient>().SetEnvironment();
        }

        ancor = GameObject.FindGameObjectsWithTag("SpatialAncor");

        for (int i = 0; i < ancor.Length; i++)
        {
            Destroy(ancor[i]);
        }

        Menu.SetActive(false);
        Ancor.SetActive(false);

        for (int i = 0; i < Mover.Length; i++)
        {
            //Mover[i].GetComponent<SceneMover>().enabled = true; 
        }
    }
}
