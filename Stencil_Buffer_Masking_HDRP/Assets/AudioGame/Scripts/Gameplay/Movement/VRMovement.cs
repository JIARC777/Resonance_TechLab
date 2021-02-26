using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
    public class VRMovement : MonoBehaviour
    {
        public SteamVR_Action_Vector2 moveInput;
        public Hand hand;
        public CapsuleCollider playerBody;
        public Camera playerViewCamera;
        public float moveSpeed = 2f;
        public VRMovement instance;
        public bool isColliding = false;
        public float maxSpeed;
        public float offset;

        private Vector3 correctionDirection;

        private Rigidbody parentRigidBody;

        private void Awake()
        {
            if (instance != null && instance != this)
                Destroy(gameObject);
            else
                instance = this;
        }

        private void Start()
        {
            parentRigidBody = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            if (hand == null)
                hand = GetComponent<Hand>();

            if (moveInput == null)
            {
                Debug.Log("You must assign an action to move!");
                return;
            }
        }

        private void FixedUpdate()
        {
            if (hand != null)
            {
                Move();
            }
            //Debug.Log(playerViewCamera.transform.rotation.y);
        }

        public void Move()
        {
            if (!isColliding)
            {
                Quaternion lookingDirection = playerViewCamera.transform.localRotation;
                //Debug.Log(lookingDirection.eulerAngles);
                Vector2 coordinates = moveInput.axis;

                //parentRigidBody.velocity = new Vector3(playerViewCamera.transform.forward.x * coordinates.y, 0, playerViewCamera.transform.forward.z * coordinates.y) * moveSpeed;
                //parentRigidBody.velocity += new Vector3(playerViewCamera.transform.right.x * coordinates.x, 0, playerViewCamera.transform.right.z * coordinates.x) * moveSpeed;
                parentRigidBody.AddForce(new Vector3(playerViewCamera.transform.forward.x * coordinates.y, 0, playerViewCamera.transform.forward.z * coordinates.y) * moveSpeed, ForceMode.VelocityChange);
                parentRigidBody.AddForce(new Vector3(playerViewCamera.transform.right.x * coordinates.x, 0, playerViewCamera.transform.right.z * coordinates.x) * moveSpeed, ForceMode.VelocityChange);
                //Clamping
                parentRigidBody.velocity.Normalize();
                parentRigidBody.velocity *= maxSpeed;
            }
            else
            {
                transform.position += correctionDirection.normalized * offset;
            }

        }

        public void HandleCollision(Collision coll)
        {
            correctionDirection = playerBody.transform.position - coll.GetContact(0).point;
            correctionDirection.y = 0;

        }

    }
}

