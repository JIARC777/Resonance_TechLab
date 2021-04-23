using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;

public class Plug : MonoBehaviour
{
    public float videoDelay = 0f;
    public VideoClip plugInVideo;
    public VideoPlayer discordPlayer;
    public GameObject plugGeoToEnable;
    public Animator effect;
    public UnityEvent pluggedIn;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Plug")
        {
            pluggedIn.Invoke();
            //other.transform.parent.parent.GetComponent<Hand>().DetachObject(other.gameObject);
            Destroy(other.transform.gameObject);
            plugGeoToEnable.SetActive(true);
            //other.transform.parent.SetParent(transform);
            discordPlayer.clip = plugInVideo;
            discordPlayer.Play();
            GetComponent<AudioSource>().Play();
            effect.SetBool("Damage", true);
        }
    }
}
