using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixXR : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnApplicationQuit()
    {
        UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.StopSubsystems();
        UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.DeinitializeLoader();
    }
}
