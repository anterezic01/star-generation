using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ringMiniScript : MonoBehaviour
{

    void Update()
    {
        if(transform.parent.gameObject.GetComponent<ringControl>().viewToggle == 1)
        {
            gameObject.GetComponent<Renderer>().enabled = true;
        }
        else
        {
            gameObject.GetComponent<Renderer>().enabled = false;
        }
    }
}
