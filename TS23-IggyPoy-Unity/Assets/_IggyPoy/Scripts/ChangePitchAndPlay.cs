using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangePitchAndPlay : MonoBehaviour
{
    [SerializeField] private AudioSource[] audioSources;
    
    void Start()
    {
        foreach (AudioSource audioSource in audioSources)
        {
            audioSource.pitch = Random.Range(-0.8f, 1.2f);
            audioSource.Play();
        }
    }
}
