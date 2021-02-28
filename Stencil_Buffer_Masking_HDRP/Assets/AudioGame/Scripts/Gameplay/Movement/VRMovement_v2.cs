using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
    public class VRMovement_v2 : MonoBehaviour
    {
        public CharacterController controller;
        public SteamVR_Action_Vector2 moveInput;
        public Transform playerViewCamera;

        public float speed = 12f;
        public float gravity = -9.81f;
        public float jumpHeight = 3f;

        public Transform groundCheck;
        public float groundDistance = 0.2f; //radius of sphere

        public LayerMask groundMask;

        Vector3 velocity;
        bool isGrounded;

        private void FixedUpdate()
        {
            float distanceFromFloor = Vector3.Dot(playerViewCamera.localPosition, Vector3.up);
            controller.height = Mathf.Max(controller.radius, distanceFromFloor);
            controller.center = playerViewCamera.localPosition - 0.5f * distanceFromFloor * Vector3.up;
        }

        // Update is called once per frame
        void Update()
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            Vector2 coordinates = moveInput.axis;
            float x = playerViewCamera.transform.forward.x * coordinates.y;
            float z = 0;
            Vector3 direction = new Vector3(playerViewCamera.transform.forward.x * coordinates.y + playerViewCamera.transform.right.x * coordinates.x,
                0, playerViewCamera.transform.forward.z * coordinates.y + playerViewCamera.transform.right.z * coordinates.x);
            //parentRigidBody.AddForce(new Vector3(playerViewCamera.transform.forward.x * coordinates.y, 0, playerViewCamera.transform.forward.z * coordinates.y) * moveSpeed, ForceMode.VelocityChange);
            //parentRigidBody.AddForce(new Vector3(playerViewCamera.transform.right.x * coordinates.x, 0, playerViewCamera.transform.right.z * coordinates.x) * moveSpeed, ForceMode.VelocityChange);

            //genius
            Vector3 move = transform.right * x + transform.forward * z;

            controller.Move(direction * speed * Time.deltaTime);

            velocity.y += gravity * Time.deltaTime;

            //physics of free fall, multiply by time again
            controller.Move(velocity * Time.deltaTime);

            
        }
    }
}
