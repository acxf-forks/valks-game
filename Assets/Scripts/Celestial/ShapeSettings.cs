using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ShapeSettings : ScriptableObject
{
    public float renderRadius = 10;
    public bool renderEverything = false;

    [Tooltip("The number of chunk recursions per base face.")]
    public int chunks = 1;

    [Tooltip("The number of triangle recursions per chunk.")]
    public int chunkTriangles = 1;

    [Tooltip("The terrain material of the planet.")]
    public Material terrainMaterial;
    [Tooltip("The ocean material of the planet.")]
    public Material oceanMaterial;

    public float radius = 10;

    public bool ocean = true;

    [Range(0, 1)]
    public float oceanDepth = 0f;

    [Range(0, 1f)]
    public float frequency = 1f;

    public float minValue;

    public Vector3 center;
}
