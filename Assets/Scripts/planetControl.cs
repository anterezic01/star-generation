using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class planetControl : MonoBehaviour
{
    private int seedOfCreation;
    [HideInInspector] public float defaultZoom;
    
    //Planet Properties
    public float atmPressure;
    public float greenhouseEffect;
    public float earthMasses;
    [HideInInspector] public float SMA;
    [HideInInspector] public float smallStarTax = 1;
    public float distanceFromStar;
    private float averageTemperatureKelvin;
    public float averageTemperatureCelsius;
    public float starsLuminosity;
    public float albedo;
    private float velocity;
    private float angle;
    private float distanceCoeff = 4f;
    [HideInInspector] public float parentMass;

    //Planet Naming
    [HideInInspector] public string temperatureString;
    private string compositionString;

    //GameObjects
    [HideInInspector] public GameObject star;
    [HideInInspector] public GameObject moonPrefab;
    private GameObject[] objectList;
    private GameObject[] listToHide;
    [HideInInspector] public GameObject indicator;
    [HideInInspector] public GameObject atmospherePrefab;

    //Visuals
    private int vertexCount = 120;
    private float lineWidth; 
    private int selection = 0; 
    private int hiddenEveryone = 0; 
    private LineRenderer lineRenderer;
    [HideInInspector] public Material materialToUse;
    [HideInInspector] public Sprite LargerSprite;
    [HideInInspector] public Sprite UsualSprite;
    private Vector2 positionMemory;
    private cameraControl cameraControlScript;
    private float zoomFactor;
    private GameObject atmos;

    //Moon Data
    private int maxMoonsPossible = 0;
    private int guaranteedMoons = 0;
    private float minMoonMass = 0.001f;
    private float maxMoonMass = 0.03f;
    public float nOfMoons;
    private moonControl scriptMoon;
    [HideInInspector] public float moonATModds;
    [HideInInspector] public float moonMedianATM;

    //Ring Data
    [HideInInspector] public GameObject ringPrefab;
    public GameObject ring; 
    [HideInInspector] public bool hasRings;
    private GameObject ringSystem;
    [HideInInspector] public GameObject smallRingPrefab;


    //Other Components
    Renderer rend;
    RectTransform rectTransform;
    private CircleCollider2D colliderThis;
    private followPlanetScript scriptIndicator;
    private float minSafeOrbit;

    void Start()
    {
        rend = GetComponent<Renderer>();
        rectTransform = GetComponent<RectTransform>();
        cameraControlScript = Camera.main.GetComponent<cameraControl>();
        colliderThis = GetComponent<CircleCollider2D>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.material = materialToUse;

        GenerateProperties();
        NamePlanet();
        SetupCircle(); //Create orbit visual
    
        SetRandomPosition(); //Asign random position in orbit at start
        defaultZoom /= smallStarTax;//Reduce size if star <0.5 solar masses
        Zoom(defaultZoom);//resize to system view size
        SetIndicator();//Create visual aid to see planets when far away
        
        GenerateMoons();
    }

    public void GenerateMoons()
    {
        nOfMoons = (int)Random.Range(guaranteedMoons,maxMoonsPossible+1);//Generate number of moons
        minSafeOrbit = defaultZoom*20f;//Create minimal orbit outside roshe limit

        for (int i = 0; i < nOfMoons; i++)
        {
            GameObject moon = Instantiate(moonPrefab, new Vector2(0,0), Quaternion.identity) as GameObject;
            scriptMoon = moon.GetComponent<moonControl>();

            scriptMoon.mass = Random.Range(minMoonMass,maxMoonMass);
            scriptMoon.SMA = Random.Range(minSafeOrbit,minSafeOrbit*1.4f);
            scriptMoon.nOfTheMoon = i+1;
            scriptMoon.nOfParentMoons = nOfMoons;
            scriptMoon.parentsMass = earthMasses;
            scriptMoon.parentDefaultZoom = defaultZoom;
            scriptMoon.starsLuminosity = starsLuminosity;
            scriptMoon.SMAofParent = SMA;
            scriptMoon.moonMedianATM = moonMedianATM;
            scriptMoon.moonATModds = moonATModds;
            minSafeOrbit *= Random.Range(1.4f,2f);
            scriptMoon.parentPlanet = gameObject;
            
        }
    }

    public void GenerateRing(float odds)
    {
        if(Random.Range(0f,100f) <= odds) //Create ring if odds
        {
            ring = Instantiate(ringPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            // ring.GetComponent<ringControl>().minOrbit = Random.Range(defaultZoom*5.2f,defaultZoom*12.5f);
            // ring.GetComponent<ringControl>().maxOrbit = Random.Range(defaultZoom*5.2f,defaultZoom*12.5f);
            //ring.GetComponent<ringControl>().minOrbit = minSafeOrbit/8f;
            //ring.GetComponent<ringControl>().maxOrbit = minSafeOrbit;
            ring.GetComponent<ringControl>().parentPlanet = gameObject;
            hasRings = true;
            gameObject.name += " with rings";
            ringSystem = Instantiate(smallRingPrefab, gameObject.transform);
            ringSystem.hideFlags = HideFlags.HideInHierarchy;
        }
    }

    public void SetRandomPosition()//Asign random position in orbit at start
    {
        angle = Random.Range(0f,360f);
        rectTransform.localPosition = new Vector2(SMA * Mathf.Cos(angle), SMA * Mathf.Sin(angle));
    }
    public void NamePlanet()//Sum the strings to name planet
    {
        gameObject.name = "";
        gameObject.name += temperatureString;
        gameObject.name += " ";
        gameObject.name += compositionString;

    }

    public void CalculateGreenhouseeffect()//Calculate the increase in atmosphere thanks to various gasses
    {
        if(atmPressure > 0)
        {
            float seed = Random.Range(0f,100f);
            if(seed <= 75f)
            {
                greenhouseEffect = Random.Range(0.1f,2f);
                
            }
            else if (seed <= 95f)
            {
                greenhouseEffect = Random.Range(2f,10f);
            }
            else
            {
                greenhouseEffect = Random.Range(10f,100f);
            }

            if(atmPressure >= 10f)
            {
                greenhouseEffect*= (atmPressure/100f)*10f;
            }

            if(greenhouseEffect <= 1)
            {
                averageTemperatureKelvin *= 1.095f;
            }
            else
            {
                averageTemperatureKelvin *= 1.095f;
                averageTemperatureKelvin += (greenhouseEffect-1)*15;
            }
        }
    }

    public void CalculateTemperature()
    {
        averageTemperatureKelvin = Mathf.Pow(((starsLuminosity*(1f-albedo))/(16*Mathf.PI*Mathf.Pow(SMA*100000000000/distanceCoeff,2)*5.67f*Mathf.Pow(10f,-8))),0.25f);
        CalculateGreenhouseeffect();//Add Greenhouse Effect to temperature
        averageTemperatureCelsius = averageTemperatureKelvin - 273.15f;//Show temp in celsius
        if (averageTemperatureCelsius < -260)
        {
            temperatureString = "Zero";
        }
        else if (averageTemperatureCelsius < -100)
        {
            temperatureString = "Frigid";
        }
        else if (averageTemperatureCelsius < -30)
        {
            temperatureString = "Frozen";
        }
        else if (averageTemperatureCelsius < 0)
        {
            temperatureString = "Cool";
        }
        else if (averageTemperatureCelsius < 30)
        {
            temperatureString = "Temperate";
        }
        else if (averageTemperatureCelsius < 60)
        {
            temperatureString = "Warm";
        }
        else if (averageTemperatureCelsius < 90)
        {
            temperatureString = "Hot";
        }
        else if (averageTemperatureCelsius < 120)
        {
            temperatureString = "Boiling";
        }
        else if (averageTemperatureCelsius < 600)
        {
            temperatureString = "Burning";
        }
        else if (averageTemperatureCelsius < 2000)
        {
            temperatureString = "Molten";
        }
        else
        {
            temperatureString = "Vaporizing";
        }

        if (averageTemperatureCelsius <= -100)
        {
            lineRenderer.material.SetColor("_EmissionColor", Color.Lerp(Color.white,Color.cyan,averageTemperatureKelvin/173.15f));
        }
        else if (averageTemperatureCelsius <= -30)
        {
            lineRenderer.material.SetColor("_EmissionColor", Color.Lerp(Color.cyan,Color.blue,averageTemperatureKelvin/243.15f));
        }
        else if (averageTemperatureCelsius <= 0)
        {
            lineRenderer.material.SetColor("_EmissionColor", Color.Lerp(Color.blue,Color.green,averageTemperatureKelvin/273.15f));
        }
        else if (averageTemperatureCelsius <= 60)
        {
            lineRenderer.material.SetColor("_EmissionColor", Color.green);
        }
        else if (averageTemperatureCelsius <= 100)
        {
            lineRenderer.material.SetColor("_EmissionColor", Color.Lerp(Color.green,Color.red,averageTemperatureKelvin/373.15f));
        }
        else if (averageTemperatureCelsius <= 2000)  
        {
            lineRenderer.material.SetColor("_EmissionColor", Color.Lerp(Color.red,Color.magenta,averageTemperatureKelvin/2273.15f));
        }
        else
        {
            lineRenderer.material.SetColor("_EmissionColor", Color.blue);
        }
    }

    public void CalculatePlanetType()
    {
        if(SMA*100000000000/distanceCoeff > 77000000000)//Ensure no gass collossi appear below the water limit. Dunno why.
        {
            seedOfCreation = Random.Range(0,6);
        }
        else
        {
            seedOfCreation = Random.Range(0,5);
        }


        if (seedOfCreation == 0)
        {
            earthMasses = Random.Range(0.1f,0.7f);
            compositionString = "Sub-Terra";
            defaultZoom = ((earthMasses-0.1f)/0.6f)*0.01f + 0.03f;

            maxMoonsPossible = 1;
            minMoonMass = 0.001f;
            maxMoonMass = 0.01f;
            moonATModds = 1f;
            moonMedianATM = 0.1f;

            GenerateAtmosphere(50f, 0.1f);
            GenerateRing(20f);    
        }
        else if (seedOfCreation == 1)
        {
            earthMasses = Random.Range(0.7f,2f);
            compositionString = "Terra";
            defaultZoom = ((earthMasses-0.7f)/1.3f)*0.02f + 0.04f;

            maxMoonsPossible = 2;
            minMoonMass = 0.004f;
            maxMoonMass = 0.015f;
            moonATModds = 10f;
            moonMedianATM = 0.5f;

            GenerateAtmosphere(90f, 1f);
            GenerateRing(15f);    
        }
        else if (seedOfCreation == 2)
        {
            earthMasses = Random.Range(2f,10f);
            compositionString = "Mega-Terra";
            defaultZoom = ((earthMasses-2f)/8f)*0.02f + 0.06f;

            maxMoonsPossible = 3;
            minMoonMass = 0.004f;
            maxMoonMass = 0.03f;
            moonATModds = 30f;
            moonMedianATM = 1f;

            GenerateAtmosphere(99f, 3f);
            GenerateRing(20f);    

        }
        else if (seedOfCreation == 3)
        {
            earthMasses = Random.Range(5f,10f);
            compositionString = "Gas Dwarf";
            defaultZoom = ((earthMasses-5f)/5f)*0.04f + 0.07f;

            maxMoonsPossible = 3;
            minMoonMass = 0.006f;
            maxMoonMass = 0.03f;
            moonATModds = 30f;
            moonMedianATM = 0.5f;

            GenerateRing(30f);    
        }
        else if (seedOfCreation == 4)
        {
            earthMasses = Random.Range(10f,100f);
            compositionString = "Gas Giant";
            defaultZoom = ((earthMasses-10f)/90f)*0.06f + 0.11f;

            guaranteedMoons = 1;
            maxMoonsPossible = 4;
            minMoonMass = 0.01f;
            maxMoonMass = 0.04f;
            moonATModds = 30f;
            moonMedianATM = 0.75f;

            GenerateRing(50f);    
        }
        else if (seedOfCreation == 5)
        {
            earthMasses = Random.Range(100f,600f);
            compositionString = "Gas Colossus";
            defaultZoom = ((earthMasses-100f)/500f)*0.08f + 0.17f;

            guaranteedMoons = 3;
            maxMoonsPossible = 8;
            minMoonMass = 0.01f;
            maxMoonMass = 0.08f;
            moonATModds = 45f;
            moonMedianATM = 0.9f;

            GenerateRing(80f);    
        }

    }

    public void GenerateAtmosphere(float odds, float medianATM)
    {
        float seed = Random.Range(0f,100f);
        float leastATM = medianATM * 0.1f;
        float mostATM = medianATM * 2f;
        if(seed <= odds)
        {
            if(Random.Range(0f,100f) <= 90f)
            {
                atmPressure = Random.Range(leastATM,mostATM);
            }
            else
            {
                atmPressure = Random.Range(mostATM,mostATM*100f);
            }

            atmos = Instantiate(atmospherePrefab, gameObject.transform);
            atmos.GetComponent<Renderer>().enabled = false;
            atmos.transform.localScale = new Vector2(1.02f, 1.02f);
            atmos.GetComponent<Renderer>().sortingOrder = -1;
            atmos.GetComponent<SpriteRenderer>().color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

            if(atmPressure <= 1f)
            {
                Color temp = atmos.GetComponent<SpriteRenderer>().color;
                temp.a = atmPressure*0.6f + 0.4f;
                atmos.GetComponent<SpriteRenderer>().color = temp;
            }
            else
            {
                float skejl = 1.02f + atmPressure / 5000f;
                atmos.transform.localScale = new Vector2(skejl,skejl);
            }

        }

    }

    public void GenerateProperties()
    {
        distanceFromStar = SMA/4f;
        albedo = Random.Range(0.1f,0.5f);
        CalculatePlanetType();
        CalculateTemperature();
        CalculateVelocity();
    }

    public void CalculateVelocity()
    {
        velocity = Mathf.Sqrt(6.6743f * Mathf.Pow(10,-11) * 1f/SMA);
    }

    void SetIndicator()
    {
        GameObject indicatorObject = Instantiate(indicator);
        scriptIndicator = indicatorObject.GetComponent<followPlanetScript>();
        scriptIndicator.planet = gameObject;
    }
     
 
 
     private void SetupCircle()
     {
        lineRenderer.widthMultiplier = 0.005f;
        lineWidth = lineRenderer.widthMultiplier;
 
        float deltaTheta = (2f * Mathf.PI) / vertexCount;
        float theta = 0f;
 
        lineRenderer.positionCount = vertexCount+2;
        for (int i=0; i<=lineRenderer.positionCount; i++)
        {
            Vector3 pos = new Vector3(SMA * Mathf.Cos(theta), SMA * Mathf.Sin(theta), 0f);
            lineRenderer.SetPosition(i, pos);
            theta += deltaTheta;
        }
     }


    // Update is called once per frame
    void Update()
    {
        if (hiddenEveryone == 0)
        {
            gameObject.transform.RotateAround(star.transform.position, new Vector3(0,0,1), velocity*1000000 * Time.deltaTime);
            lineRenderer.widthMultiplier = 0.005f * (Camera.main.transform.position.z/-1.62f);
        }
        else
        {
            gameObject.transform.position = new Vector2(0, 0);
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (hiddenEveryone == 1)
            {
                selection = 0;
                hiddenEveryone=0;
                rectTransform.localPosition = positionMemory;
                ShowOtherObjects();
                setRingVisibility(0);
                FocusAway();
            }
            
        }
        colliderThis.radius = 5*(1f+0.5f/(earthMasses*10))*(Camera.main.transform.position.z/-10);
    }

    public void setRingVisibility(int visi)
    {
        if(ring)
        {
            ring.GetComponent<ringControl>().viewToggle = visi;
        }
    }

    
    void OnMouseOver()
    {

        if(Input.GetMouseButtonDown(0))
        {
            cameraControlScript.centerOfFocus = gameObject;
            selection++;
            if (selection >= 2 && hiddenEveryone != 1)
            {
                selection = 0;
                hiddenEveryone = 1;
                GetComponent<CircleCollider2D>().radius = 10;
                HideOtherObjects();
                setRingVisibility(1);
                FocusOnObject();
            }
        }
    }

    void FocusOnObject()
    {
        positionMemory = transform.position;
        gameObject.GetComponent<SpriteRenderer>().sprite = LargerSprite;
        listToHide = GameObject.FindGameObjectsWithTag ("moon");
        foreach  (GameObject obj in listToHide)
        {
            scriptMoon = obj.GetComponent<moonControl>();
            if (scriptMoon.parentPlanet == gameObject)
            {
                scriptMoon.viewToggle = 0;
            }
        }
        atmos.GetComponent<Renderer>().enabled = true;
        Zoom(defaultZoom);
    }

    void Zoom(float factor)
    {
        rectTransform.localScale = new Vector2(factor,factor);
    }

    void FocusAway()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = UsualSprite;
        listToHide = GameObject.FindGameObjectsWithTag ("moon");
        foreach  (GameObject obj in listToHide)
        {
            scriptMoon = obj.GetComponent<moonControl>();
            if (scriptMoon.parentPlanet == gameObject)
            {
                scriptMoon.viewToggle = 1;
            }
        }
        atmos.GetComponent<Renderer>().enabled = false;
        Zoom(defaultZoom);
    }

    void HideOtherObjects()
    {
        lineRenderer.enabled = false;
        DisableReality(star, true);
        listToHide = GameObject.FindGameObjectsWithTag ("moon");
        foreach  (GameObject obj in listToHide)
        {
            scriptMoon = obj.GetComponent<moonControl>();
            if (scriptMoon.parentPlanet != gameObject)
            {
                DisableReality(obj, false);
            }
        }
        listToHide = GameObject.FindGameObjectsWithTag ("planet");
        foreach  (GameObject obj in listToHide)
        {
            if (obj != gameObject)
            {
                DisableReality(obj, false);
            }
        }
        listToHide = GameObject.FindGameObjectsWithTag ("decoration");
        foreach  (GameObject obj in listToHide)
        {
            Renderer rendererTemp;
            rendererTemp = obj.GetComponent<Renderer>();
            rendererTemp.enabled = false;
        }
    }

    void ShowOtherObjects()
    {
        lineRenderer.enabled = true;
        EnableReality(star, true);
        listToHide = GameObject.FindGameObjectsWithTag ("moon");
        foreach  (GameObject obj in listToHide)
        {
            EnableReality(obj, false);
        }
        listToHide = GameObject.FindGameObjectsWithTag ("planet");
        foreach  (GameObject obj in listToHide)
        {
            EnableReality(obj, false);
        }
        listToHide = GameObject.FindGameObjectsWithTag ("decoration");
        foreach  (GameObject obj in listToHide)
        {
            Renderer rendererTemp;
            rendererTemp = obj.GetComponent<Renderer>();
            rendererTemp.enabled = true;
        }
    }


    void DisableReality(GameObject obj, bool starIsIt)
    {
        if (starIsIt)
        {
            Renderer rendererTemp;
            CircleCollider2D colliderTemp;
            rendererTemp = obj.GetComponent<Renderer>();
            colliderTemp = obj.GetComponent<CircleCollider2D>();
            rendererTemp.enabled = false;
            colliderTemp.enabled = false;
        }
        else
        {
        Renderer rendererTemp;
        CircleCollider2D colliderTemp;
        LineRenderer lineRendererTemp;
        rendererTemp = obj.GetComponent<Renderer>();
        colliderTemp = obj.GetComponent<CircleCollider2D>();
        lineRendererTemp = obj.GetComponent<LineRenderer>();
        rendererTemp.enabled = false;
        colliderTemp.enabled = false;
        lineRendererTemp.enabled = false;
        }

    }

    void EnableReality(GameObject obj, bool starIsIt)
    {
        if (starIsIt)
        {
        Renderer rendererTemp;
        CircleCollider2D colliderTemp;
        rendererTemp = obj.GetComponent<Renderer>();
        colliderTemp = obj.GetComponent<CircleCollider2D>();
        rendererTemp.enabled = true;
        colliderTemp.enabled = true;
            
        }
        else
        {
        Renderer rendererTemp;
        CircleCollider2D colliderTemp;
        LineRenderer lineRendererTemp;
        rendererTemp = obj.GetComponent<Renderer>();
        colliderTemp = obj.GetComponent<CircleCollider2D>();
        lineRendererTemp = obj.GetComponent<LineRenderer>();
        rendererTemp.enabled = true;
        colliderTemp.enabled = true;
        lineRendererTemp.enabled = true;
        }

    }
    
}
