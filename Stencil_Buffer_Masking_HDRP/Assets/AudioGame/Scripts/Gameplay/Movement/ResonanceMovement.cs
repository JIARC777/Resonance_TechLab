using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Purpose: Control the player's movement
 * 
 */

namespace Valve.VR.InteractionSystem
{
    public class ResonanceMovement : MonoBehaviour
    {
        public CharacterController controller;
        public SteamVR_Action_Vector2 moveInput;
        public Transform playerViewCamera;

        //player movement stats
        public float speed = 12f;
        public float gravity = -9.81f;
        public float jumpHeight = 3f;

        //running into objects
        public float kickForce = 2f;
        //How long before the player can make another mistake?
        public float kickResetTime = 2f;
        private float lastKickTime = -1f;
        //How much should the object move upward?
        public float upForceAmount = 50f;

        //a sphere positioned at the player's feet to determine distance from ground
        public Transform groundCheck;
        public float groundDistance = 0.2f; //radius of sphere

        //what the player can consider to be ground
        public LayerMask groundMask;

        //the player's current vertical velocity
        Vector3 verticalVelocity;
        //is the player on the ground?
        bool isGrounded;

        private void FixedUpdate()
        {
            //borrowed from SteamVR, set player's height collider correctly
            float distanceFromFloor = Vector3.Dot(playerViewCamera.localPosition, Vector3.up);
            controller.height = Mathf.Max(controller.radius, distanceFromFloor);
            controller.center = playerViewCamera.localPosition - 0.5f * distanceFromFloor * Vector3.up;
        }

        void Update()
        {
            //Use groundCheck to determine if the player is on the ground, adjust vertical velocity accordingly
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
            if (isGrounded && verticalVelocity.y < 0)
            {
                verticalVelocity.y = -2f;
            }

            //Vector rotation math from the controller joystick and rotation to determine direction
            Vector2 coordinates = moveInput.axis;
            float x = playerViewCamera.transform.forward.x * coordinates.y;
            float z = 0;
            //Calculate horizontal vector for player to move in
            Vector3 direction = new Vector3(playerViewCamera.transform.forward.x * coordinates.y + playerViewCamera.transform.right.x * coordinates.x,
                0, playerViewCamera.transform.forward.z * coordinates.y + playerViewCamera.transform.right.z * coordinates.x);
            //Move in that direction every frame
            controller.Move(direction * speed * Time.deltaTime);

            //physics of free fall, multiply by time again
            verticalVelocity.y += gravity * Time.deltaTime;
            controller.Move(verticalVelocity * Time.deltaTime);

        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            //Do not allow kick to happen unless enough time has passed
            if (Time.time > lastKickTime + kickResetTime)
            {
                //Not preferrable, do better with how our model heirarchies work!
                Rigidbody hitObjectBody = GetAppropriateRigidBody(hit);

                if (hitObjectBody)
                {
                    Debug.Log("Kicking " + hitObjectBody.name);
                    Vector3 kickDirection = playerViewCamera.rotation * Vector3.forward;
                    //Add an amount of upward because its interesting that way
                    kickDirection += Vector3.up * upForceAmount;
                    kickDirection = kickDirection.normalized;
                    hitObjectBody.AddForce(kickDirection * kickForce);
                    lastKickTime = Time.time;
                }
            }
        }

        //this is terrible
        private Rigidbody GetAppropriateRigidBody(ControllerColliderHit hit)
        {
            
            Rigidbody body = null;
            if (hit.transform.parent)
            {
                //First try to get the body from the object's parent
                body = hit.transform.parent.GetComponent<Rigidbody>();
            }
            if (!body)
                //if the parent doesn't have one or if no parent, get it from the object itself
                body = hit.transform.GetComponent<Rigidbody>();
            return body;
        }
    }
}
