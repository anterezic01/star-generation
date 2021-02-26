using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraControl : MonoBehaviour
{
    private float position = -10f;
    public GameObject centerOfFocus;

    void Start()
    {

    }
   
    void Update()
    {
        Camera.main.transform.position = centerOfFocus.transform.position + new Vector3(0,0,position);
        position += Input.GetAxis("Mouse ScrollWheel")*Camera.main.transform.position.z/-2f;
    }
}
