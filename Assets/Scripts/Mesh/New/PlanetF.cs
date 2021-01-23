using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetF : MonoBehaviour
{
    public PlanetSettingsF planetSettings;
    private static int planetCount = 0;

    private void Awake()
    {
        planetCount++;
    }

    private void OnValidate()
    {
        GeneratePlanet();
    }

    private void GeneratePlanet() 
    {
        gameObject.name = $"({planetCount}) Planet - " + planetSettings.name;
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/Planet");

        new PlanetMeshF().Create(planetSettings);
    }
}
