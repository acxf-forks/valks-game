using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class Planet : MonoBehaviour
{
    public PlanetSettings planetSettings;
    public ShapeSettings shapeSettings;
    public ColourSettings colourSettings;

    public bool autoUpdate = true;

    private PlanetMeshChunkRenderer terrain;
    private PlanetMeshChunkRenderer ocean;
    private static int planetCount = 0;

    private Transform parentTerrainChunks;
    private Transform parentOceanChunks;

    [HideInInspector]
    public bool planetSettingsFoldout;
    [HideInInspector]
    public bool shapeSettingsFoldout;
    [HideInInspector]
    public bool colourSettingsFoldout;

    public void GeneratePlanet()
    {
        if (CheckSettingsInvalid()) 
        {
            Debug.LogWarning("One or more settings for a planet is undefined. Please define the appropriate settings.");
            return;
        }
        GeneratePlanetSettings();
        GenerateTerrainMesh();
        GenerateOceanMesh();
        GenerateColours();
    }

    public void OnPlanetSettingsUpdated()
    {
        if (autoUpdate) 
        {
            if (CheckSettingsInvalid())
            {
                Debug.LogWarning("One or more settings for a planet is undefined. Please define the appropriate settings.");
                return;
            }
            GeneratePlanetSettings();
        }
    }

    public void OnShapeSettingsUpdated()
    {
        if (autoUpdate)
        {
            if (CheckSettingsInvalid())
            {
                Debug.LogWarning("One or more settings for a planet is undefined. Please define the appropriate settings.");
                return;
            }
            GenerateTerrainMesh();
            GenerateOceanMesh();
        }
    }

    public void OnColourSettingsUpdated()
    {
        if (autoUpdate)
        {
            if (CheckSettingsInvalid())
            {
                Debug.LogWarning("One or more settings for a planet is undefined. Please define the appropriate settings.");
                return;
            }
            GenerateColours();
        }
    }

    private void GeneratePlanetSettings() 
    {
        gameObject.name = $"({planetCount++}) Planet - " + planetSettings.name;
    }

    private void GenerateTerrainMesh()
    {
        // Remove old chunks
        if (parentTerrainChunks)
            DestroyImmediate(parentTerrainChunks.gameObject);

        if (!parentTerrainChunks)
        {
            parentTerrainChunks = new GameObject("Terrain Chunks").transform;
            parentTerrainChunks.parent = transform;
        }

        terrain = new PlanetMeshChunkRenderer(parentTerrainChunks, shapeSettings, PlanetMeshChunkRenderer.ShapeType.Terrain);
    }

    private void GenerateOceanMesh() 
    {
        // Remove old chunks
        if (parentOceanChunks)
            DestroyImmediate(parentOceanChunks.gameObject);

        if (!shapeSettings.ocean)
            return;

        if (!parentOceanChunks)
        {
            parentOceanChunks = new GameObject("Ocean Chunks").transform;
            parentOceanChunks.parent = transform;
        }

        ocean = new PlanetMeshChunkRenderer(parentOceanChunks, shapeSettings, PlanetMeshChunkRenderer.ShapeType.Ocean);
    }

    private void GenerateColours()
    {
        foreach (var chunk in terrain.chunks) 
        {
            chunk.meshRenderer.sharedMaterial.color = colourSettings.terrainColour;
        }
    }

    private bool CheckSettingsInvalid() => !planetSettings || !shapeSettings || !colourSettings;

    private void Update()
    {
        if (terrain != null)
            terrain.RenderNearbyChunks(shapeSettings.renderRadius);
        if (ocean != null)
            if (shapeSettings.ocean)
                ocean.RenderNearbyChunks(shapeSettings.renderRadius);
    }
}