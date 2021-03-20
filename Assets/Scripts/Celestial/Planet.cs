using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public PlanetSettings planetSettings;
    private PlanetMeshChunkRenderer planetChunkRenderer;
    private static int planetCount = 0;

    private void Awake()
    {
        planetCount++;
    }

    private void Start()
    {
        GeneratePlanet();
    }

    private void Update()
    {
        planetChunkRenderer.RenderNearbyChunks(planetSettings.renderRadius);
    }

    private void GeneratePlanet() 
    {
        gameObject.name = $"({planetCount}) Planet - " + planetSettings.name;
        planetChunkRenderer = new PlanetMeshChunkRenderer(this, planetSettings);
    }
}
