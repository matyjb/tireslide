using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlusPointsOnHit : MonoBehaviour, IResetable
{
    public int points = 15;
    private new BoxCollider collider;

    private AudioSource audioSource;

    private ParticleSystem[] confettis;

    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<BoxCollider>();
        TryGetComponent(out audioSource);
        confettis = GetComponentsInChildren<ParticleSystem>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && collider.enabled)
        {
            collider.enabled = false;
            PointsManager.instance.AddScaledPoints(points);
            if(audioSource != null)
            {
                audioSource.Play();
                // confetti anim start
                foreach (var item in confettis)
                {
                    item.Play();
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ResetToInitial()
    {
        collider.enabled = true;
    }
}
