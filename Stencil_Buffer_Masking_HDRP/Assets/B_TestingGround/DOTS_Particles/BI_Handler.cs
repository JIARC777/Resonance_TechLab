using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BI_Handler : MonoBehaviour
{
    public Mesh go;
    public MeshRenderer mRend;

    public Material bi_Mat;

    // Start is called before the first frame update
    void Start()
    {
        go = gameObject.GetComponent<Mesh>();

        go.subMeshCount = go.subMeshCount++;
        go.SetTriangles(go.triangles, go.subMeshCount - 1);

        mRend.sharedMaterials[0] = bi_Mat;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
