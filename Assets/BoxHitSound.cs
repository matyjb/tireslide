using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BoxHitSound : MonoBehaviour
{
    private AudioSource audioSource;

    public AudioClip[] clips;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
            audioSource.PlayOneShot(clips[Random.Range(0, clips.Length)]);
    }
}
