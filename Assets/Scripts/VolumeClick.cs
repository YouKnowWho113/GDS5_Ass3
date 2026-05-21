using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeClick : MonoBehaviour
{ 
    public AudioSource audioSource;
    public float reducedVolume = 0.3f;

    public void ReduceVolume()
    {
        if (audioSource != null)
        {
            audioSource.volume = reducedVolume;
        }
    }
}