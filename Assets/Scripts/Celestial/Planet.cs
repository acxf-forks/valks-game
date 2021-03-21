using System.Collections;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public PlanetSettings planetSettings;
    public ShapeSettings terrainShapeSettings;
    public ShapeSettings oceanShapeSettings;
    public ColourSettings colourSettings;

    private static int planetCount = 0;
    private PlanetMeshChunkRenderer terrain;
    private PlanetMeshChunkRenderer ocean;

    public bool autoUpdate = true;

    private Transform parentTerrainChunks;
    private Transform parentOceanChunks;

    [HideInInspector]
    public bool planetSettingsFoldout;
    [HideInInspector]
    public bool terrainShapeSettingsFoldout;
    [HideInInspector]
    public bool oceanShapeSettingsFoldout;
    [HideInInspector]
    public bool colourSettingsFoldout;

    public void GeneratePlanet()
    {
        gameObject.name = $"({planetCount++}) Planet - " + planetSettings.name;

        GenerateTerrainMesh();
        GenerateOceanMesh();
        GenerateColours();
    }

    public void OnPlanetSettingsUpdated()
    {
        if (autoUpdate) 
        {
            
        }
    }

    public void OnTerrainShapeSettingsUpdated()
    {
        if (autoUpdate)
        {
            GenerateTerrainMesh();
        }
    }

    public void OnOceanShapeSettingsUpdated() 
    {
        if (autoUpdate) 
        {
            GenerateOceanMesh();
        }
    }

    public void OnColourSettingsUpdated()
    {
        if (autoUpdate)
        {
            GenerateColours();
        }
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

        terrain = new PlanetMeshChunkRenderer(parentTerrainChunks, terrainShapeSettings);
    }

    private void GenerateOceanMesh() 
    {
        // Remove old chunks
        if (parentOceanChunks)
            DestroyImmediate(parentOceanChunks.gameObject);

        if (!parentOceanChunks)
        {
            parentOceanChunks = new GameObject("Ocean Chunks").transform;
            parentOceanChunks.parent = transform;
        }

        ocean = new PlanetMeshChunkRenderer(parentOceanChunks, oceanShapeSettings);
    }

    private void GenerateColours()
    {
        
    }

    private void Update()
    {
        terrain.RenderNearbyChunks(terrainShapeSettings.renderRadius);
        ocean.RenderNearbyChunks(oceanShapeSettings.renderRadius);
    }
}