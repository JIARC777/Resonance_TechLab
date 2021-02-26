using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
    public class GroundCollider : MonoBehaviour
    {
        //private BoxCollider groundBox;
        private CapsuleCollider playerBody;
        public VRMovement movement;
        private GameObject playerReference;

        // Start is called before the first frame update
        void Start()
        {
            playerBody = GetComponent<CapsuleCollider>();
            playerReference = movement.gameObject;
        }

        private void FixedUpdate()
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit))
            {
                if (hit.transform.gameObject.CompareTag("SlopingFloor"))
                {
                    playerReference.transform.position = new Vector3(playerReference.transform.position.x, hit.point.y, playerReference.transform.position.z);
                }
            }
        }


        private void OnCollisionEnter(Collision collision)
        {
            movement.isColliding = true;
            Debug.Log("Hit " + collision.gameObject.name);
            movement.HandleCollision(collision);
        }

        private void OnCollisionExit(Collision collision)
        {
            movement.isColliding = false;
        }
    }
}

