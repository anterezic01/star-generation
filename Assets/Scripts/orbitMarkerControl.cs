using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class orbitMarkerControl : MonoBehaviour
{

    public GameObject parent;
    public GameObject indicator;
    private GameObject indicatorMoon;
    public float SMA;
    public Color orbitColor;

    private float lineWidth;
    private int vertexCount = 120;

    private LineRenderer lineRenderer;

    
    void Start()
    {
        gameObject.hideFlags = HideFlags.HideInHierarchy;
        indicatorMoon = Instantiate(indicator);
        indicatorMoon.hideFlags = HideFlags.HideInHierarchy;

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        lineRenderer.material.SetColor("_EmissionColor", orbitColor);

        float deltaTheta = (2f * Mathf.PI) / vertexCount;
        float theta = 0f;

        lineRenderer.widthMultiplier = 0.005f;
        lineWidth = lineRenderer.widthMultiplier;
 
        lineRenderer.positionCount = vertexCount+2;
        for (int i=0; i<=lineRenderer.positionCount; i++)
        {
            Vector3 pos = new Vector2(0,0) + new Vector2(SMA * 8 * Mathf.Cos(theta), SMA * 8 * Mathf.Sin(theta));
            lineRenderer.SetPosition(i, pos);
            theta += deltaTheta;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!parent)
        {
            Destroy(gameObject);
        }

        if(parent.GetComponent<moonControl>().viewToggle != 1)
        {
            lineRenderer.enabled = true;
            lineRenderer.widthMultiplier = 0.003f * (Camera.main.transform.position.z/-1.62f);

            indicatorMoon.GetComponent<SpriteRenderer>().enabled = true;
            indicatorMoon.transform.position = parent.transform.position;
                if(Camera.main.transform.position.z < -24f)
                {
                    indicatorMoon.GetComponent<SpriteRenderer>().color = Color.white;
                    Color temp = indicatorMoon.GetComponent<SpriteRenderer>().color;
                    float tempVal = (Camera.main.transform.position.z+24)/-4;
                    if (tempVal < 1)
                    {
                        temp.a = tempVal;
                    }
                    else
                    {
                        temp.a = 1;
                    }
                    indicatorMoon.GetComponent<SpriteRenderer>().color = temp;
                }
                else
                {
                    indicatorMoon.GetComponent<SpriteRenderer>().color = Color.clear;
                }
            indicatorMoon.transform.localScale = new Vector2(0.1f*(Camera.main.transform.position.z/-6),0.1f*(Camera.main.transform.position.z/-6));
            }
        else
        {
            lineRenderer.enabled = false;
            indicatorMoon.GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}
