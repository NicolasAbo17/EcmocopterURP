using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightHelper : MonoBehaviour
{
    [SerializeField] List<Light> lights = new List<Light>();
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip clip;

    int lastState;
    // Start is called before the first frame update
    public void SetColor(int state)
    {
        foreach (Light light in lights)
        {
            if (state == 0)
            {
                light.color = Color.blue;
            }
            else
            {
                light.color = Color.white;
            }
        }
    }

    public void SetIntensity(int state)
    {
        foreach (Light light in lights)
        {
            switch (state)
            {
                case 0:
                    light.intensity = 0;
                    break;

                case 1:
                    light.intensity = .5f;
                    break;

                case 2:
                    light.intensity = 1;
                    break;

                case 3:
                    light.intensity = 1.5f;
                    break;
            }
            light.intensity = state;
        }
    }

    public void PlayClick(int state)
    {
        Debug.Log("Last State: " + lastState + " State: " + state);
        if (lastState != state)
            aud.PlayOneShot(clip);
        lastState = state;
    }
}
