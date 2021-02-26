using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followPlanetScript : MonoBehaviour
{
    public GameObject planet;
    public Color temp;
    public float tempVal;

    void Start()
    {
        gameObject.transform.localScale = new Vector2(0.1f,0.1f);
        gameObject.hideFlags = HideFlags.HideInHierarchy;
    }

    void Update()
    {
        transform.position = planet.transform.position;
        if(Camera.main.transform.position.z < -10f)
        {
            GetComponent<SpriteRenderer>().color = Color.white;
            temp = GetComponent<SpriteRenderer>().color;
            tempVal = (Camera.main.transform.position.z+10)/-4;
            if (tempVal < 1)
            {
                temp.a = tempVal;
            }
            else
            {
                temp.a = 1;
            }
            GetComponent<SpriteRenderer>().color = temp;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = Color.clear;
        }
        gameObject.transform.localScale = new Vector2(0.1f*(Camera.main.transform.position.z/-6),0.1f*(Camera.main.transform.position.z/-6));
    }
}
