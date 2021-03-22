using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ShapeSettings : ScriptableObject
{
    [Header("Terrain")]
    public float renderRadius = 10;
    public bool renderEverything = false;
    public Material terrainMaterial;
    [Tooltip("The number of chunk recursions per base face.")]
    public int chunks = 1;
    [Tooltip("The number of triangle recursions per chunk.")]
    public int chunkTriangles = 1;
    [Tooltip("The terrain material of the planet.")]
    public float radius = 10;

    [Header("Ocean")]
    public bool ocean = true;
    [Tooltip("The ocean material of the planet.")]
    public Material oceanMaterial;
    [Range(0, 1)]
    public float oceanDepth = 0f;
    [Range(1, 2)]
    public int oceanChunks = 1;
    [Range(1, 2)]
    public int oceanTriangles = 1;

    public NoiseLayer[] noiseLayers;

    [System.Serializable]
    public class NoiseLayer
    {
        public bool enabled = true;
        public bool useFirstLayerAsMask;
        public NoiseSettings noiseSettings;
    }
}
