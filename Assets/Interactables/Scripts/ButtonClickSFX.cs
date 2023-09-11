using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]

public class ButtonClickSFX : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip clip;

    public void PlayClick(bool val)
    {
        aud.PlayOneShot(clip);
    }
}
