using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignWithPlayerBody : MonoBehaviour
{
    public CharacterController playerBody;
    void Update()
    {
        transform.localPosition = new Vector3(playerBody.center.x, 0, playerBody.center.z);
    }
}
