using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Shaker : MonoBehaviour
{
    public float frequency = 28f;   // In Hz

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private bool playTone = false;

    private void Start()
    {
        GenerateTone();
    }

    private void Update()
    {
        if (playTone && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
        else if (!playTone && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    private void GenerateTone()
    {
        // Calculate the maximum possible sample count based on the audio device's sample rate
        int sampleRate = AudioSettings.outputSampleRate;
        int sampleCount = 10000 / 2; // Dividing by 2 to avoid integer overflow
        float[] samples = new float[sampleCount];

        float increment = frequency * 2f * Mathf.PI / sampleRate;
        float phase = 0f;

        for (int i = 0; i < sampleCount; i++)
        {
            samples[i] = Mathf.Sin(phase);
            phase += increment;
        }

        // Set the generated samples to the audio source
        audioSource.clip = AudioClip.Create("Tone", sampleCount, 1, sampleRate, false);
        audioSource.clip.SetData(samples, 0);

        // Play the tone if the playTone boolean is true
        if (playTone)
        {
            audioSource.Play();
        }
    }

    public void ToggleBool()
    {
        playTone = !playTone;
    }

    public void ExitApplication()
    {
        Application.Quit();
    }
}
