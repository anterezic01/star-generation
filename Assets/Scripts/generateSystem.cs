using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class generateSystem : MonoBehaviour
{
    public GameObject starPrefab;
    
    void Start()
    {
        GenerateSystem();
    }

    // Update is called once per frame
    public void GenerateSystem()
    {
        Instantiate(starPrefab, new Vector2(0, 0), Quaternion.identity);
    }
}
