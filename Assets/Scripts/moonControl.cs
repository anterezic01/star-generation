using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moonControl : MonoBehaviour
{
    //Moon Properties
    [HideInInspector] public float mass;
    public float moonMasses;
    public float atmPressure;
    public float greenhouseEffect;
    public float SMA;
    [HideInInspector] public float velocity;
    [HideInInspector] public float averageTemperatureKelvin;
    public float averageTemperatureCelsius;
    public float albedo;
    public float starsLuminosity;
    public float SMAofParent;
    [HideInInspector] public GameObject atmospherePrefab;
    private GameObject atmos;


    //Parent Properties
    public float nOfTheMoon;
    public float nOfParentMoons;
    [HideInInspector] public float parentsMass;
    [HideInInspector] public float distanceFromMoonSystemView;
    [HideInInspector] public float parentDefaultZoom;
    [HideInInspector] public float moonATModds;
    [HideInInspector] public float moonMedianATM;

    //Other Components
    Renderer rend;
    RectTransform rectTransform;

    //GameObjects
    public GameObject parentPlanet;
    private GameObject[] objectList;
    
    //Various temps and visual aids
    private float angle;
    private float angleTwo;
    public int viewToggle;
    public int temp;
    private float ratio;

    //Orbit Marker
    [HideInInspector] public Sprite visualOrbit;
    private LineRenderer lineRenderer;
    [HideInInspector] public Material materialToUse;
    [HideInInspector] public GameObject orbitMarkerObject;
    [HideInInspector] public GameObject prefabOrbitMarker;
    public Color orbitColor;
    
    [HideInInspector] public string temperatureString;

    void Start()
    {
        temp = 1;
        viewToggle = 1;
        rend = GetComponent<Renderer>();
        rectTransform = GetComponent<RectTransform>();

        ratio = (moonMasses/parentsMass) * 12.42f;
        

        Zoom(0.01f);
        velocity = Mathf.Sqrt(6.6743f * Mathf.Pow(10,-11) * 1f/SMA);
        
        angle = 360f/nOfParentMoons * (nOfTheMoon + 1) * Mathf.Deg2Rad;

        moonMasses = mass * 81.28f;
        albedo = Random.Range(0.1f,0.5f);
        GenerateMoon();
        distanceFromMoonSystemView = 1.1f;
        if(parentPlanet.GetComponent<planetControl>().hasRings)
        {
            distanceFromMoonSystemView = 1.4f;
        }
        SetupCircle();
        
        
    }

    void OnMouseOver()
    {

        if(Input.GetMouseButtonDown(0))
        {
            Camera.main.GetComponent<cameraControl>().centerOfFocus = gameObject;
        }
    }


    void Update()
    {
        if (viewToggle == 1)
        {
            transform.localPosition = parentPlanet.transform.position + new Vector3(parentDefaultZoom * distanceFromMoonSystemView * Mathf.Cos(angle), parentDefaultZoom * distanceFromMoonSystemView * Mathf.Sin(angle), 0f);
            angle += 0.015f;
            temp = 1;
            GetComponent<CircleCollider2D>().enabled = false;
            atmos.GetComponent<Renderer>().enabled = false;

            if(parentPlanet.transform.localScale.x>0.031f)
            {
                Zoom(0.01f);
            }
            else
            {
                Zoom(parentPlanet.transform.localScale.x*0.322f);
            }


        }
        else
        {
            if (temp == 1)
            {
                angleTwo = Random.Range(0f,360f);
                rectTransform.localPosition = new Vector2(SMA* 8 * Mathf.Cos(angleTwo), SMA * 8 * Mathf.Sin(angleTwo));
                temp = 0;
                GetComponent<CircleCollider2D>().enabled = true;
                atmos.GetComponent<Renderer>().enabled = true;
                Zoom(parentPlanet.transform.localScale.x * 12.42f);

            }
            transform.RotateAround(parentPlanet.transform.position, new Vector3(0,0,1), velocity * 3400000 * Time.deltaTime);
            Zoom(parentPlanet.transform.localScale.x * ratio);
        }
        
    }

    void Zoom(float factor)
    {
        rectTransform.localScale = new Vector2(factor,factor);
    }

    public void CalculateGreenhouseeffect()
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

    public void GenerateAtmosphere()
    {
        float seed = Random.Range(0f,100f);
        float leastATM = moonMedianATM * 0.1f;
        float mostATM = moonMedianATM * 2f;
        if(seed <= moonATModds)
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
            atmos.transform.localScale = new Vector2(1.01f*0.0805f, 1.01f*0.0805f);
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
                float skejl = 1.01f + atmPressure / 5000f;
                atmos.transform.localScale = new Vector2(skejl*0.0805f,skejl*0.0805f);
            }

        }

    }

    void GenerateMoon()
    {
        GenerateAtmosphere();
        TemperatureString();
        gameObject.name = temperatureString;
        gameObject.name += " Satellite";
    }

    public void TemperatureString()
    {
        averageTemperatureKelvin = Mathf.Pow(((starsLuminosity*(1f-albedo))/(16*Mathf.PI*Mathf.Pow(SMAofParent*100000000000/4f,2)*5.67f*Mathf.Pow(10f,-8))),0.25f);
        CalculateGreenhouseeffect();
        averageTemperatureCelsius = averageTemperatureKelvin - 273.15f;
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
            orbitColor = Color.Lerp(Color.white,Color.cyan,averageTemperatureKelvin/173.15f);
        }
        else if (averageTemperatureCelsius <= -30)
        {
            orbitColor = Color.Lerp(Color.cyan,Color.blue,averageTemperatureKelvin/243.15f);
        }
        else if (averageTemperatureCelsius <= 0)
        {
            orbitColor = Color.Lerp(Color.blue,Color.green,averageTemperatureKelvin/273.15f);
        }
        else if (averageTemperatureCelsius <= 60)
        {
            orbitColor = Color.green;
        }
        else if (averageTemperatureCelsius <= 100)
        {
            orbitColor = Color.Lerp(Color.green,Color.red,averageTemperatureKelvin/373.15f);
        }
        else if (averageTemperatureCelsius <= 2000) 
        {
            orbitColor = Color.Lerp(Color.red,Color.magenta,averageTemperatureKelvin/2273.15f);
        }
        else
        {
            orbitColor = Color.blue;
        }
    }

    private void SetupCircle()
    {
        orbitMarkerControl orbitControl;

        GameObject orbitObject = Instantiate(prefabOrbitMarker);
        orbitMarkerObject = orbitObject;
        orbitControl = orbitObject.GetComponent<orbitMarkerControl>();
        orbitControl.parent = gameObject;
        orbitControl.SMA = SMA;
        orbitControl.orbitColor = orbitColor;
        lineRenderer.material = materialToUse;       
    }
}
