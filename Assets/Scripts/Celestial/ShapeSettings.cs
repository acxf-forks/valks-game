using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ShapeSettings : ScriptableObject
{
    public float renderRadius = 10;

    public bool generateNoise = false;

    [Range(1, 20)]
    public float radius = 10;

    [Tooltip("The number of chunk recursions per base face.")]
    [Range(1, 3)]
    public int chunks = 1;

    [Tooltip("The number of triangle recursions per chunk.")]
    [Range(1, 3)]
    public int chunkTriangles = 1;

    [Tooltip("The material of the planet.")]
    public Material material;

    [Range(0, 1)]
    public float amplitude = 0.5f;
}
