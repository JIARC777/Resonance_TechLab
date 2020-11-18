using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Valve.VR;
using Valve.VR.InteractionSystem;

public class TwoHandedAimWeapon : MonoBehaviour
{
    //TODO: Track if otherHand is currently attached, and give it somewhere to attach to
    public void OnHelpUpdate(Hand hand)//X forward y up z right 
    {
        Vector3 _averageRightOfHands = Vector3.Lerp(-hand.transform.right, -hand.otherHand.transform.right, 0.2f);
        transform.rotation = Quaternion.LookRotation( hand.otherHand.transform.position - hand.transform.position, _averageRightOfHands);
    }
}
