using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PlanetMeshChunk : MonoBehaviour
{
    private List<Vector3> vertices;
    private List<int> triangles;

    private Mesh mesh;

    private static int count = 0;

    public void Create(PlanetSettings settings, List<Vector3> vertices) 
    {
        this.vertices = vertices;

        count++;
        gameObject.name = $"Chunk {count}";
        GetComponent<MeshRenderer>().material = settings.material;
        vertices = new List<Vector3> { vertices[0], vertices[1], vertices[2] };
        triangles = new List<int>();
        triangles.AddRange(new List<int> { 0, 1, 2 });

        mesh = GetComponent<MeshFilter>().mesh;
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }

    public Vector3 GetCenterPoint() => PlanetUtils.GetCenterPoint(vertices[0], vertices[1], vertices[2]);
}
