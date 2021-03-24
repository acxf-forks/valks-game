﻿using System.Collections;
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

    public MinMax elevationMinMax = new MinMax();

    [HideInInspector]
    public bool planetSettingsFoldout;
    [HideInInspector]
    public bool shapeSettingsFoldout;
    [HideInInspector]
    public bool colourSettingsFoldout;

    private ShapeGenerator shapeGenerator = new ShapeGenerator();

	private void AssignParentObjects()
	{
		if(parentTerrainChunks==null)
			parentTerrainChunks = transform.Find("Terrain Chunks");
		if(parentOceanChunks==null)
			parentOceanChunks = transform.Find("Ocean Chunks");
	}
	
	// Planet meshes are generated at the start of the game or when the level is loaded.
	private void Awake()
	{
		AssignParentObjects();
		GeneratePlanet();
	}

	// Destroy procedurally generated meshes for file size reduction. Called in `DestroyOnSave.cs`
	public void Destroy()
	{
		while (parentTerrainChunks.childCount > 0) {
			DestroyImmediate(parentTerrainChunks.GetChild(0).gameObject);
		}
		while (parentOceanChunks.childCount > 0) {
			DestroyImmediate(parentOceanChunks.GetChild(0).gameObject);
		}
	}
	
    public void GeneratePlanet()
    {
        shapeGenerator.UpdateSettings(shapeSettings);

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
            shapeGenerator.UpdateSettings(shapeSettings);
            GenerateTerrainMesh();
            GenerateOceanMesh();
            GenerateColours();
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
        gameObject.name = $"({planetCount++}) Planet - " + planetSettings.planetName;
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

        terrain = new PlanetMeshChunkRenderer(parentTerrainChunks, shapeGenerator, PlanetMeshChunkRenderer.ShapeType.Noise);
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

        ocean = new PlanetMeshChunkRenderer(parentOceanChunks, shapeGenerator, PlanetMeshChunkRenderer.ShapeType.Sphere);
    }

    Texture2D texture;
    const int textureResolution = 50;

    private void GenerateColours()
    {
        if (texture == null)
        {
            texture = new Texture2D(textureResolution, 1);
        }

        Color[] colours = new Color[textureResolution];
        for (int i = 0; i < textureResolution; i++)
        {
            colours[i] = colourSettings.terrainGradient.Evaluate(i / (textureResolution - 1f));
        }
        texture.SetPixels(colours);
        texture.Apply();

        if (terrain == null) 
        {
            GenerateTerrainMesh();
        }

        foreach (var chunk in terrain.chunks)
        {
            chunk.meshRenderer.sharedMaterial.SetVector("_elevationMinMax", new Vector4(shapeGenerator.elevationMinMax.Min, shapeGenerator.elevationMinMax.Max));
            chunk.meshRenderer.sharedMaterial.SetTexture("_texture", texture);
        }

        if (ocean == null) 
        {
            GenerateOceanMesh();
        }

        if (!shapeSettings.ocean)
            return;

        foreach (var chunk in ocean.chunks) 
        {
            chunk.meshRenderer.sharedMaterial.SetColor("_deepOceanColor", colourSettings.deepOceanColour);
            chunk.meshRenderer.sharedMaterial.SetColor("_shallowOceanColor", colourSettings.shallowOceanColour);
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
