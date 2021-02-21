using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    // This is the central event for detecting a player
    public delegate void PlayerDetected(Vector3 position);
    // Called On OnTriggerEnter if tag == Player, indicating a player was found
    public event PlayerDetected DetectedPlayer;

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            // this just uses the basic transform we may need to reconfigure for ther character controller
            DetectedPlayer(other.transform.position);
        }
    }
}
