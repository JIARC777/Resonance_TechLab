using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactParticles : MonoBehaviour
{
    public float minVelocity = 2f;
    public ParticleSystem emitter;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.relativeVelocity.magnitude);
        if (collision.relativeVelocity.magnitude >= minVelocity)
        {
            emitter.Play();
        }
    }

    //Utility for all throwables included here
    public void SetPlayerLayer()
    {
        gameObject.layer = 13; //layer of player
    }

    public void SetHiderLayer()
    {
        gameObject.layer = 9; //layer of hider
    }
}
