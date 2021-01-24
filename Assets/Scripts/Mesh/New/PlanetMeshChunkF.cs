using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PlanetMeshChunkF : MonoBehaviour
{
    private List<Vector3> vertices;
    private List<int> triangles;

    private Mesh mesh;

    private static int count = 0;

    public void Create(int a, int b, int c) 
    {
        count++;
        gameObject.name = $"Chunk {count}";
        var baseFormVertices = PlanetMeshF.baseFormVertices;
        vertices = new List<Vector3> { baseFormVertices[a], baseFormVertices[b], baseFormVertices[c] };
        triangles = new List<int>();
        triangles.AddRange(new List<int> { 0, 1, 2 });

        mesh = GetComponent<MeshFilter>().mesh;
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }
}
