using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSetup : MonoBehaviour
{
    // Start is called before the first frame update
    public Mesh target;
    void Start()
    {
        Vector3 normal = target.normals[0];
        GetComponent<Transform>().rotation = Quaternion.FromToRotation(Vector3.up, normal);
        GetComponent<Transform>().rotation.z +=180;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
