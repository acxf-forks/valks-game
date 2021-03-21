using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class Planet : MonoBehaviour
{
    public PlanetSettings planetSettings;
    public ShapeSettings terrainShapeSettings;
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

        terrain = new PlanetMeshChunkRenderer(parentTerrainChunks, terrainShapeSettings, PlanetMeshChunkRenderer.ShapeType.Terrain);

        GenerateOceanMesh();
    }

    private void GenerateOceanMesh() 
    {
        // Remove old chunks
        if (parentOceanChunks)
            DestroyImmediate(parentOceanChunks.gameObject);

        if (!terrainShapeSettings.ocean)
            return;

        if (!parentOceanChunks)
        {
            parentOceanChunks = new GameObject("Ocean Chunks").transform;
            parentOceanChunks.parent = transform;
        }

        ocean = new PlanetMeshChunkRenderer(parentOceanChunks, terrainShapeSettings, PlanetMeshChunkRenderer.ShapeType.Ocean);
    }

    private void GenerateColours()
    {
        foreach (var chunk in terrain.chunks) 
        {
            chunk.meshRenderer.sharedMaterial.color = colourSettings.terrainColour;
        }
    }

    private void Update()
    {
        if (terrain != null)
            terrain.RenderNearbyChunks(terrainShapeSettings.renderRadius);
        if (ocean != null)
            if (terrainShapeSettings.ocean)
                ocean.RenderNearbyChunks(terrainShapeSettings.renderRadius);
    }
}