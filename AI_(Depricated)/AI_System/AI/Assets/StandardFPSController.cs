using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardFPSController : MonoBehaviour
{
    public float moveSpeed = 1;
    public float lookSpeed = 1;
    Transform trans;
    public Transform LookCamera;
    public float clampAngle = 90;

    float xAngle;
    float yAngle;
    // Start is called before the first frame update
    void Start()
    {
        trans = GetComponent<Transform>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }
	void LateUpdate()
	{
        Look();
	}
	void Move()
	{
        float xMove = Input.GetAxis("Horizontal");
        float zMove = Input.GetAxis("Vertical");
        trans.position += ((zMove * transform.forward + xMove * transform.right) * moveSpeed);
    }

    void Look()
	{
        xAngle += Input.GetAxis("Mouse X") * lookSpeed;
        yAngle += Input.GetAxis("Mouse Y") * lookSpeed;
        yAngle = Mathf.Clamp(yAngle, -clampAngle, clampAngle);
        LookCamera.localRotation = Quaternion.Euler(-yAngle, 0, 0);
        trans.rotation = Quaternion.Euler(0, xAngle, 0);
    }
	
}
