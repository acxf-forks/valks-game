using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public bool autoUpdate = true;

    public ShapeSettings shapeSettings;
    public ColourSettings colourSettings;

    private ShapeGenerator shapeGenerator = new ShapeGenerator();
    private ColourGenerator colourGenerator = new ColourGenerator();

    [HideInInspector]
    public bool shapeSettingsFoldout;
    [HideInInspector]
    public bool colourSettingsFoldout;

    [HideInInspector]
    private MeshFilter[] meshFilters;
    private TerrainFace[] terrainFaces;

    private void Initialize()
    {
        shapeGenerator.UpdateSettings(shapeSettings);
        colourGenerator.UpdateSettings(colourSettings);

        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[6];
        }
        terrainFaces = new TerrainFace[6];

        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };
        //var material = Resources.Load<Material>("Materials/Planet Shader");

        for (int i = 0; i < directions.Length; i++)
        {
            if (meshFilters[i] == null)
            {
                var meshObj = new GameObject($"Face {i}");
                meshObj.transform.parent = transform;

                var renderer = meshObj.AddComponent<MeshRenderer>();
                renderer.material = colourSettings.planetMaterial;

                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                meshFilters[i].mesh = new Mesh();
            }

            terrainFaces[i] = new TerrainFace(shapeGenerator, meshFilters[i].sharedMesh, directions[i]);
        }
    }

    public void GeneratePlanet()
    {
        Initialize();
        GenerateMesh();
        GenerateColours();
    }

    public void OnShapeSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateMesh();
        }
    }

    public void OnColourSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateColours();
        }
    }

    void GenerateMesh()
    {
        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i].gameObject.activeSelf)
            {
                terrainFaces[i].ConstructMesh();
            }
        }

        colourGenerator.UpdateElevation(shapeGenerator.elevationMinMax);
    }

    void GenerateColours()
    {
        colourGenerator.UpdateColours();
    }
}

[System.Serializable]
public class NoiseSettings 
{
    public enum FilterType { Simple, Ridgid }
    public FilterType filterType;

    [ConditionalHide("filterType", 0)]
    public SimpleNoiseSettings simpleNoiseSettings;
    [ConditionalHide("filterType", 1)]
    public RidgidNoiseSettings ridgidNoiseSettings;

    [System.Serializable]
    public class SimpleNoiseSettings 
    {
        public bool enabled = true;
        [Range(0, 2)]
        public float strength;
        [Range(1, 8)]
        public int numLayers;
        [Range(0, 5)]
        public float baseRoughness;
        [Range(0, 5)]
        public float roughness;
        [Range(0, 1)]
        public float persistence;
        [Range(0, 2)]
        public float minValue;
        public Vector3 centre;
    }

    [System.Serializable]
    public class RidgidNoiseSettings : SimpleNoiseSettings
    {
        public float weightMultiplier = .8f;
    }
}