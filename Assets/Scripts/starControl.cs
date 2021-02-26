using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class starControl : MonoBehaviour
{      
    //Star Properties
    public static float mass;
    public int nOfPlanets;
    private float minSafeOrbit;
    public float relativeLuminosity;
    private int denotation;
    private Color starsColor;

    //GameObjects
    [HideInInspector] public GameObject planetPrefab;
    [HideInInspector] public GameObject star;
    private GameObject[] listToDestroy;

    //Scripts
    private planetControl scriptPlanet;
    private cameraControl cameraControlScript;
    private generateSystem systemGenerator;

    //Star Colors 
    public Color MStarColor;
    public Color KStarColor;
    public Color GStarColor;
    public Color FStarColor;
    public Color AStarColor;
    public Color BStarColor;
    public Color OStarColor;

    //Visuals
    [HideInInspector] public GameObject stellarClassObject;

    //Other Components
    Renderer rend;
    RectTransform rectTransform;

    void Start()
    {
        cameraControlScript = Camera.main.GetComponent<cameraControl>();
        cameraControlScript.centerOfFocus = gameObject; //Focus on star at the start of new system
        systemGenerator = Camera.main.GetComponent<generateSystem>(); //Call the generate system function that is for some reason not in this script

        rend = GetComponent<Renderer>();
        rectTransform = GetComponent<RectTransform>();

        mass = StellarMassGenerator(); //Generate mass
        CreateStellarDenotationDecagons(); //Visualize stellar class

        gameObject.name += " Star"; //Finish the naming of star
        nOfPlanets = (int)Random.Range(5,12); //Generate the number of planets. May need review?

        rectTransform.localScale = new Vector2(mass/12.4f,mass/12.4f); //Rescale the stars prefab
        
        if (mass <= 0.50f) //Create minimal safe orbit;
        {
            minSafeOrbit = (mass + Random.Range(0.5f,1.2f)*mass);
        }
        else
        {
            minSafeOrbit = mass + Random.Range(0.5f,1.2f);
        }

        for (int i = 1; i <= nOfPlanets; i++) //Generate planets
        {
            GeneratePlanet();
        }
        
    }

    void GeneratePlanet()
    {
            GameObject planet = Instantiate(planetPrefab, new Vector2(0,0), Quaternion.identity) as GameObject;
            scriptPlanet = planet.GetComponent<planetControl>();
            scriptPlanet.starsLuminosity = relativeLuminosity * 3.846f * Mathf.Pow(10,26);//Converts relative luminosity to actual luminosity for calculating temperature
            scriptPlanet.parentMass = mass;
            scriptPlanet.SMA = Random.Range(minSafeOrbit*0.95f,minSafeOrbit*1.05f);
            minSafeOrbit *= Random.Range(1.4f,2f); //Increases so that plants dont overlap
            scriptPlanet.star = gameObject;

            if(mass <= 0.45) //If star too small on screen, reduce the size of planets so it doesnt look weird
            {
                scriptPlanet.smallStarTax = 2;
            }       
    }

    void CreateStellarDenotationDecagons() //Creates classification
    {
        float step = 180f/(9-denotation);
        float angle = 0f;
        for (int i = 0; i < 9-denotation; i++)
        {
            GameObject a = Instantiate(stellarClassObject, gameObject.transform);
            a.transform.Rotate(0, 0, angle);
            a.GetComponent<SpriteRenderer>().color = rend.material.color;
            a.hideFlags = HideFlags.HideInHierarchy;
            angle += step;
        }
    }

    void OnMouseOver()
    {

        if(Input.GetMouseButtonDown(1))//Destroy everything and generate new system
        {
            listToDestroy = GameObject.FindGameObjectsWithTag ("moon");
            for (int i = 0; i < listToDestroy.Length; i++)
            {
                Destroy(listToDestroy[i]);
            }
            listToDestroy = GameObject.FindGameObjectsWithTag ("planet");
            for (int i = 0; i < listToDestroy.Length; i++)
            {
                Destroy(listToDestroy[i]);
            }
            listToDestroy = GameObject.FindGameObjectsWithTag ("decoration");
            for (int i = 0; i < listToDestroy.Length; i++)
            {
                Destroy(listToDestroy[i]);
            }
            Destroy(gameObject);
            listToDestroy = GameObject.FindGameObjectsWithTag ("ring");
            for (int i = 0; i < listToDestroy.Length; i++)
            {
                Destroy(listToDestroy[i]);
            }
            Destroy(gameObject);
            systemGenerator.GenerateSystem();
        }
    }

    float StellarMassGenerator() //Generate mass, return it and begin naming star
    {
        float seed = Random.Range(0f,100f);
        float massGenerated;
        float lowerBoundMass, upperBoundMass, lowerBoundRelativeLuminosity, upperBoundRelativeLuminosity;
        if (seed <= 50f)
        {
            gameObject.transform.name = "M";
            lowerBoundMass = 0.08f;
            upperBoundMass = 0.45f;

            massGenerated = Random.Range(lowerBoundMass,upperBoundMass);
            denotation = 9 - (int)(10*massGenerated/upperBoundMass);
            gameObject.name += denotation.ToString();
            rend.material.color = Color.Lerp(Color.red, MStarColor, massGenerated/upperBoundMass);
            relativeLuminosity = (massGenerated/upperBoundMass) * lowerBoundMass;
            return massGenerated;
        }
        else if (seed <= 75f)
        {
            gameObject.name = "K";
            lowerBoundMass = 0.45f;
            upperBoundMass = 0.8f;
            lowerBoundRelativeLuminosity = 0.08f;
            upperBoundRelativeLuminosity = 0.6f;

            massGenerated =  Random.Range(lowerBoundMass,upperBoundMass);
            rend.material.color = Color.Lerp(MStarColor, KStarColor, massGenerated/upperBoundMass);
            relativeLuminosity = (((massGenerated-lowerBoundMass)/(upperBoundMass-lowerBoundMass))*(upperBoundRelativeLuminosity-lowerBoundRelativeLuminosity))+lowerBoundRelativeLuminosity;
            denotation = 9 - (int)((relativeLuminosity-lowerBoundRelativeLuminosity)/(upperBoundRelativeLuminosity-lowerBoundRelativeLuminosity)*10f);
            gameObject.name += denotation.ToString();
            return massGenerated;
            
        }
        else if (seed <= 87f)
        {
            gameObject.name = "G";
            lowerBoundMass = 0.8f;
            upperBoundMass = 1.04f;
            lowerBoundRelativeLuminosity = 0.6f;
            upperBoundRelativeLuminosity = 1.5f;

            massGenerated =  Random.Range(lowerBoundMass,upperBoundMass);
            rend.material.color = Color.Lerp(KStarColor, GStarColor, massGenerated/upperBoundMass);
            relativeLuminosity = (((massGenerated-lowerBoundMass)/(upperBoundMass-lowerBoundMass))*(upperBoundRelativeLuminosity-lowerBoundRelativeLuminosity))+lowerBoundRelativeLuminosity;
            denotation = 9 - (int)((relativeLuminosity-lowerBoundRelativeLuminosity)/(upperBoundRelativeLuminosity-lowerBoundRelativeLuminosity)*10f);
            gameObject.name += denotation.ToString();
            return massGenerated;
        }
        else if (seed <= 95f)
        {
            gameObject.name = "F";
            lowerBoundMass = 1.04f;
            upperBoundMass = 1.4f;
            lowerBoundRelativeLuminosity = 1.5f;
            upperBoundRelativeLuminosity = 5f;

            massGenerated =  Random.Range(lowerBoundMass,upperBoundMass);
            rend.material.color = Color.Lerp(GStarColor, FStarColor, massGenerated/upperBoundMass);
            relativeLuminosity = (((massGenerated-lowerBoundMass)/(upperBoundMass-lowerBoundMass))*(upperBoundRelativeLuminosity-lowerBoundRelativeLuminosity))+lowerBoundRelativeLuminosity;
            denotation = 9 - (int)((relativeLuminosity-lowerBoundRelativeLuminosity)/(upperBoundRelativeLuminosity-lowerBoundRelativeLuminosity)*10f);
            gameObject.name += denotation.ToString();
            return massGenerated;
        }
        else if (seed <= 99f)
        {
            gameObject.name = "A";
            lowerBoundMass = 1.4f;
            upperBoundMass = 2.1f;
            lowerBoundRelativeLuminosity = 5f;
            upperBoundRelativeLuminosity = 25f;

            massGenerated =  Random.Range(lowerBoundMass,upperBoundMass);
            rend.material.color = Color.Lerp(FStarColor, AStarColor, massGenerated/upperBoundMass);
            relativeLuminosity = (((massGenerated-lowerBoundMass)/(upperBoundMass-lowerBoundMass))*(upperBoundRelativeLuminosity-lowerBoundRelativeLuminosity))+lowerBoundRelativeLuminosity;
            denotation = 9 - (int)((relativeLuminosity-lowerBoundRelativeLuminosity)/(upperBoundRelativeLuminosity-lowerBoundRelativeLuminosity)*10f);
            gameObject.name += denotation.ToString();
            return massGenerated;
        }
        else if (seed <= 99.9f)
        {
            gameObject.name = "B";
            lowerBoundMass = 2.1f;
            upperBoundMass = 16f;
            lowerBoundRelativeLuminosity = 25f;
            upperBoundRelativeLuminosity = 30000f;

            massGenerated =  Random.Range(lowerBoundMass,upperBoundMass);
            rend.material.color = Color.Lerp(AStarColor, BStarColor, massGenerated/upperBoundMass);
            relativeLuminosity = (((massGenerated-lowerBoundMass)/(upperBoundMass-lowerBoundMass))*(upperBoundRelativeLuminosity-lowerBoundRelativeLuminosity))+lowerBoundRelativeLuminosity;
            denotation = 9 - (int)((relativeLuminosity-lowerBoundRelativeLuminosity)/(upperBoundRelativeLuminosity-lowerBoundRelativeLuminosity)*10f);
            gameObject.name += denotation.ToString();
            return massGenerated;
        }
        else
        {
            gameObject.name = "O";
            lowerBoundMass = 16f;
            upperBoundMass = 90f;
            lowerBoundRelativeLuminosity = 30000f;
            upperBoundRelativeLuminosity = 1000000f;

            massGenerated =  Random.Range(lowerBoundMass,upperBoundMass);
            rend.material.color = Color.Lerp(BStarColor, OStarColor, massGenerated/upperBoundMass);
            relativeLuminosity = (((massGenerated-lowerBoundMass)/(upperBoundMass-lowerBoundMass))*(upperBoundRelativeLuminosity-lowerBoundRelativeLuminosity))+lowerBoundRelativeLuminosity;
            denotation = 9 - (int)((relativeLuminosity-lowerBoundRelativeLuminosity)/(upperBoundRelativeLuminosity-lowerBoundRelativeLuminosity)*10f);
            gameObject.name += denotation.ToString();
            return massGenerated;
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))//Focuses on star when esc is clicked
        {
            cameraControlScript.centerOfFocus = gameObject;
        }
    }
}
