using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class oscillation : MonoBehaviour
{
    void Update()
    {
        transform.localScale = new Vector2(Mathf.PingPong(Time.time/30f, 0.03f)+1.03f,Mathf.PingPong(Time.time/30f, 0.03f)+1.03f);
    }
}
