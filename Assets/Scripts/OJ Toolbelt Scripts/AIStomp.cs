//sounds from: https://freesound.org/people/JanKoehl/sounds/85600/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(AudioSource))]
//[RequireComponent(typeof(CharacterController))]
public class AIStomp : MonoBehaviour
{
    private AudioSource audioSource;
    //private CharacterController controller;
    //private Rigidbody rb;
    private RichAI richAI;
    [SerializeField] bool randomiseVolume;
    [SerializeField] float minVolume = 0.25f, maxVolume = 0.35f;

    [SerializeField] bool randomisePitch;
    [SerializeField] float minPitch = 0.7f, maxPitch = 0.8f;


    [SerializeField] private List<AudioClip> footstepSounds;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        //controller = GetComponent<CharacterController>();
        //rb = GetComponent<Rigidbody>();
        richAI = GetComponent<RichAI>();
    }

    void Update()
    {
        if (richAI.velocity.magnitude > 2f && !audioSource.isPlaying)
        {
            if (randomiseVolume)
            {
                audioSource.volume = Random.Range(minVolume, maxVolume);
            }

            if (randomisePitch)
            {
                audioSource.pitch = Random.Range(minPitch, maxPitch);
            }

            audioSource.PlayOneShot(footstepSounds[Random.Range(0, footstepSounds.Count)]);
        }
    }
}
