using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ringControl : MonoBehaviour
{

    public float minOrbit;
    public float maxOrbit;

    public int viewToggle;

    public GameObject ring;
    public GameObject parentPlanet;
    public Color baseColor;

    void Start()
    {
        viewToggle = 0;
        gameObject.transform.localScale = parentPlanet.transform.localScale;
        minOrbit = gameObject.transform.localScale.x;
        maxOrbit = 12 * minOrbit;
        minOrbit = Random.Range(minOrbit,maxOrbit);
        maxOrbit = Random.Range(gameObject.transform.localScale.x,maxOrbit);
        gameObject.hideFlags = HideFlags.HideInHierarchy;

        if(maxOrbit < minOrbit)
        {
            float temp;
            temp = minOrbit;
            minOrbit = maxOrbit;
            maxOrbit = temp;
        }
        baseColor.r = Random.Range(0f,1f);
        baseColor.g = Random.Range(0f,1f);
        baseColor.b = Random.Range(0f,1f);
        baseColor.a = 0.85f;

        while(minOrbit <= maxOrbit)
        {
            if(Random.Range(0f,100f) <= 97f)
            {
                GameObject ringObject = Instantiate(ring);
                ringObject.transform.localScale = new Vector2(minOrbit,minOrbit);
                ringObject.transform.parent = gameObject.transform;
                ringObject.GetComponent<Renderer>().material.color = genColor(); //ADD COLORS LATER
                ringObject.GetComponent<Renderer>().enabled = false;
                minOrbit *= 1.003f;
            }
            else
            {
                if(Random.Range(0f,100f) <= 85f)
                {
                    baseColor.r *= Random.Range(0.8f,1.2f);
                    baseColor.g *= Random.Range(0.8f,1.2f);
                    baseColor.b *= Random.Range(0.8f,1.2f);
                    minOrbit *= Random.Range(1.005f,1.009f);
                }
                else
                {
                    baseColor.r *= Random.Range(0.5f,1.7f);
                    baseColor.g *= Random.Range(0.5f,1.7f);
                    baseColor.b *= Random.Range(0.5f,1.7f);
                    minOrbit *= Random.Range(1.02f,1.04f); 
                }
                
            }
        }


    }

    public Color genColor()
    {
        Color temp;
        temp.r = baseColor.r * Random.Range(0.95f,1.05f);
        temp.g = baseColor.g * Random.Range(0.95f,1.05f);
        temp.b = baseColor.b * Random.Range(0.95f,1.05f);
        temp.a = 1f;

        return temp;
    }


    void Update()
    {
        if(parentPlanet.GetComponent<SpriteRenderer>().enabled == false)
        {
            viewToggle = 0;
        }
        
    }
}
