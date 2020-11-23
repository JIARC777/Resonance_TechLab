using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apply_Basic_Force : MonoBehaviour
{

    private Rigidbody rig;

    private Vector3 objectPos = new Vector3();

    private float objectx;
    private float objectY;
    private float objectZ;

    public float timeApplied;

    public Vector3 forceApplied = new Vector3();

    public int lerpMin;
    public int lerpMax;

    public char xyz;

    public bool lerping = false;
    public bool pausing = false;

    private float interpolator;

    // Start is called before the first frame update
    void Start()
    {

        rig = GetComponent<Rigidbody>();
        objectPos = transform.position;

        objectx = objectPos.x;
        objectY = objectPos.y;
        objectZ = objectPos.z;
        Debug.Log(objectx);


    }

    // Update is called once per frame
    void Update()
    {
        if(!pausing)
            interpolator += timeApplied * Time.deltaTime;

        if (!lerping)
            rig.AddForce(forceApplied);
        else
            Lerping();

    }

    private void Lerping()
    {
        if (!pausing)
        {
            rig.isKinematic = false;

            switch (xyz)
            {
                case 'x':

                    transform.position = new Vector3(Mathf.Lerp(lerpMin, lerpMax, interpolator), objectY, objectZ);

                    if (interpolator > 1)
                        {

                            int temp = lerpMax;
                            lerpMax = lerpMin;
                            lerpMin = temp;
                            interpolator = 0f;

                            transform.Rotate(new Vector3(0, 180, 0));                            
                        }
                    break;

                case 'y':

                    transform.position = new Vector3(objectx, Mathf.Lerp(lerpMin, lerpMax, interpolator), objectZ);

                    if (interpolator > 1)
                        {

                            int temp = lerpMax;
                            lerpMax = lerpMin;
                            lerpMin = temp;
                            interpolator = 0f;

                            transform.Rotate(new Vector3(0, 180, 0));                            
                        }
                    break;

                case 'z':

                    transform.position = new Vector3(objectx, objectY, Mathf.Lerp(lerpMin, lerpMax, interpolator));

                    if (interpolator > 1)
                        {

                            int temp = lerpMax;
                            lerpMax = lerpMin;
                            lerpMin = temp;
                            interpolator = 0f;

                            transform.Rotate(new Vector3(0, 180, 0));                            
                        }
                    break;

                default:

                    transform.position = new Vector3(Mathf.Lerp(lerpMin, lerpMax, interpolator), objectY, objectZ);

                    if (interpolator > 1)
                        {

                            int temp = lerpMax;
                            lerpMax = lerpMin;
                            lerpMin = temp;
                            interpolator = 0f;

                            transform.Rotate(new Vector3(0, 180, 0));                            
                        }
                    break;
            }            
        }
        else
        {
            rig.isKinematic = true;
        }
    }
}
