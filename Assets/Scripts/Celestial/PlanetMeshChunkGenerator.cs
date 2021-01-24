using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetMeshChunkGenerator
{
    public static List<Vector3> baseFormVertices;

    private PlanetSettings settings;
    private Planet planet;

    public void Create(Planet planet, PlanetSettings settings)
    {
        this.planet = planet;
        this.settings = settings;

        var radius = settings.radius;
        var t = (1.0f + Mathf.Sqrt(5.0f)) / 2.0f;

        // The base vertices that make up a base form icosahedron
        baseFormVertices = new List<Vector3>()
        {
            new Vector3(-1, t, 0).normalized * radius,
            new Vector3(1, t, 0).normalized * radius ,
            new Vector3(-1, -t, 0).normalized * radius,
            new Vector3(1, -t, 0).normalized * radius,
            new Vector3(0, -1, t).normalized * radius,
            new Vector3(0, 1, t).normalized * radius ,
            new Vector3(0, -1, -t).normalized * radius,
            new Vector3(0, 1, -t).normalized * radius,
            new Vector3(t, 0, -1).normalized * radius,
            new Vector3(t, 0, 1).normalized * radius ,
            new Vector3(-t, 0, -1).normalized * radius,
            new Vector3(-t, 0, 1).normalized * radius
        };

        HandleBaseFormFaces();
    }

    private void HandleBaseFormFaces()
    {
        GenerateChunks(0, 11, 5);
        GenerateChunks(0, 5, 1);
        GenerateChunks(0, 1, 7);
        GenerateChunks(0, 7, 10);
        GenerateChunks(0, 10, 11);
        GenerateChunks(1, 5, 9);
        GenerateChunks(5, 11, 4);
        GenerateChunks(11, 10, 2);
        GenerateChunks(10, 7, 6);
        GenerateChunks(7, 1, 8);
        GenerateChunks(3, 9, 4);
        GenerateChunks(3, 4, 2);
        GenerateChunks(3, 2, 6);
        GenerateChunks(3, 6, 8);
        GenerateChunks(3, 8, 9);
        GenerateChunks(4, 9, 5);
        GenerateChunks(2, 4, 11);
        GenerateChunks(6, 2, 10);
        GenerateChunks(8, 6, 7);
        GenerateChunks(9, 8, 1);
    }

    private void GenerateChunks(int a, int b, int c, int n = 0)
    {
        var numChunks = settings.chunks;
        GenerateChunk(a, b, c);

    }

    private void GenerateChunk(int a, int b, int c) 
    {
        // Create chunk gameObject
        var chunkObj = new GameObject();

        // Set parent
        chunkObj.transform.parent = planet.transform;

        // Add PlanetMeshChunk script to chunk gameObject
        var chunkScript = chunkObj.AddComponent<PlanetMeshChunk>();
        chunkScript.Create(settings, a, b, c);
    }
}
